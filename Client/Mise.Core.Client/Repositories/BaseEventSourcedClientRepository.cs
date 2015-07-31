using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Common.Repositories.Base;
using Mise.Core.Common.Services;
using Mise.Core.Entities.Base;
using Mise.Core.ExtensionMethods;
using Mise.Core.Services.UtilityServices;
using Mise.Core.Services;
using Mise.Core.Services.WebServices;
using Mise.Core.ValueItems;
using System.Linq.Expressions;

namespace Mise.Core.Client.Repositories
{
    /// <summary>
    /// A repository which deals with event sourced entities on the client, sending their changes back to the server
    /// when online, or storing in the database when not
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TEventType"></typeparam>
    public abstract class BaseEventSourcedClientRepository<TEntity, TEventType> : BaseEventSourcedRepository<TEntity, TEventType> 
        where TEntity : class, IEventStoreEntityBase<TEventType>, ICloneableEntity where TEventType : class, IEntityEventBase
    {
        protected readonly IClientDAL DAL;
        private readonly IEventStoreWebService<TEntity, TEventType> _webService;
        private readonly IResendEventsWebService _resendEventsWebService;

        protected BaseEventSourcedClientRepository(ILogger logger, 
            IClientDAL dal,
            IEventStoreWebService<TEntity, TEventType> webService,
            IResendEventsWebService resendService
            ) : base(logger)
        {
            DAL = dal;
            _webService = webService;
            _resendEventsWebService = resendService;
        }

        protected abstract Task<IEnumerable<TEntity>> LoadFromWebservice(Guid? restaurantID);
        protected abstract Task<IEnumerable<TEntity>> LoadFromDB(Guid? restaurantID);

        protected virtual TimeSpan DelayToReload { get { return new TimeSpan(0, 0, 0, 1); } }

        /// <summary>
        /// The last restaurantID we got loaded with
        /// </summary>
        protected Guid? RestaurantID { get; private set; }

        /// <summary>
        /// What to do when we've go online when previously we weren't
        /// </summary>
        protected virtual async void OnWentOnline()
        {
            try
            {
                //we want to reload, but do so after a delay
                await Task.Delay(DelayToReload).ConfigureAwait(false);
                await Load(RestaurantID).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Logger.HandleException(e);
            }
        }

        /// <summary>
        /// Each client repository needs to be able to load, both when attached to a restaurant and when not
        /// </summary>
        /// <param name="restaurantID"></param>
        /// <returns></returns>
        public async Task Load(Guid? restaurantID)
        {
            Loading = true;
            RestaurantID = restaurantID;
            var needsDBLoad = false;
            try
            {
                var items = (await LoadFromWebservice(restaurantID)).ToList();
                Cache.UpdateCache(items);

                //update our database with these
                await DAL.UpsertEntitiesAsync(items);
                Offline = false;
            }
            catch (Exception e)
            {
                Logger.HandleException(e, LogLevel.Warn);
                needsDBLoad = true;
            }

            if (needsDBLoad)
            {
                var dbItems = await LoadFromDB(restaurantID);
                Cache.UpdateCache(dbItems, ItemCacheStatus.ClientDB);
                Offline = true;
            }
            Loading = false;
        }



        /// <summary>
        /// Whether or not the item is currently loading
        /// </summary>
        public bool Loading { get; protected set; }

        /// <summary>
        /// If the repository loaded from the DB last pass
        /// </summary>
        public bool Offline { get; private set; }

        /// <summary>
        /// Commit directly to the server, not allowing any caching of items or events.  Usually used when we need an immediate server verificatoin, like in registration
        /// </summary>
        /// <param name="entityID"></param>
        /// <returns></returns>
        public virtual async Task<CommitResult> CommitOnlyImmediately(Guid entityID)
        {
            if (UnderTransaction.ContainsKey(entityID) == false)
            {
                var msg = "Error - repository " + GetType() + " has no transaction for entity " + entityID + " to commit";
                Logger.Log(msg);
                throw new ArgumentException(msg);
            }
            var commitRes = CommitResult.NothingToCommit;
            var bundle = UnderTransaction[entityID];
            if (bundle.Events == null) return commitRes;

            //update our cache, mark us as in memory
            Cache.UpdateCache(bundle.NewVersion, ItemCacheStatus.ClientMemory);

            //clear this trans from the queue
            Logger.Log("Ending transaction for " + entityID, LogLevel.Debug);
            UnderTransaction.Remove(entityID);

            var toSend = bundle.Events;
            Logger.Log("Sending events to service", LogLevel.Debug);

            commitRes = CommitResult.Error;
            try
            {
				var sendRes = await _webService.SendEventsAsync(bundle.NewVersion, toSend).ConfigureAwait(false);
                if (sendRes)
                {
                    commitRes = CommitResult.SentToServer;
                    if (Offline)
                    {
                        OnWentOnline();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.HandleException(ex);
                throw;
            }

            var entRes = await DAL.UpsertEntitiesAsync(new[] { bundle.NewVersion });


            if (entRes)
            {
                var status = commitRes == CommitResult.SentToServer ? ItemCacheStatus.Clean : ItemCacheStatus.ClientDB;
                Cache.UpdateCache(bundle.NewVersion, status);
            }

            Dirty = false;

            return commitRes;
        }

        public override async Task<CommitResult> Commit(Guid entityID)
        {
			Task startResend = null;
			try{
            	startResend = CheckAndSendResends();
			} catch(Exception e){
				Logger.HandleException (e, LogLevel.Error);
			}

            if (UnderTransaction.ContainsKey(entityID) == false)
            {
                var msg = "Error - recieved message to commit entity " + entityID + " but no transaction exists for that";
                Logger.Log(msg);
                throw new ArgumentException (msg);
            }
            var commitRes = CommitResult.NothingToCommit;
            var bundle = UnderTransaction[entityID];
            if (bundle.Events == null) return commitRes;
            //update our cache, mark us as in memory
            Cache.UpdateCache(bundle.NewVersion, ItemCacheStatus.ClientMemory);

            //clear this trans from the queue
            Logger.Log("Ending transaction for " + entityID, LogLevel.Debug);
            UnderTransaction.Remove(entityID);

            var toSend = bundle.Events;
            Logger.Log("Sending events to service", LogLevel.Debug);

            commitRes = CommitResult.Error;
            var needsDBStore = false;
            try{
				var sendRes = await _webService.SendEventsAsync (bundle.NewVersion, toSend).ConfigureAwait (false);
                if(sendRes){
                    commitRes = CommitResult.SentToServer;
                    if (Offline)
                    {
                        OnWentOnline();
                    }
                }
                else
                {
                    needsDBStore = true;
                }
            } catch(Exception ex){
                Logger.HandleException (ex);
                needsDBStore = true;
            }

            if (needsDBStore)
            {
				try{
                await DAL.AddEventsThatFailedToSend(toSend);
                commitRes = CommitResult.StoredInDB;
				} catch(Exception e){
					Logger.HandleException (e, LogLevel.Fatal);
					throw;
				}
            }

            var entRes = await DAL.UpsertEntitiesAsync(new[] {bundle.NewVersion});

              
            if (entRes)
            {
                var status = commitRes == CommitResult.SentToServer ? ItemCacheStatus.Clean : ItemCacheStatus.ClientDB;
                Cache.UpdateCache(bundle.NewVersion, status);
            }

            Dirty = false;

			if (startResend != null) {
				try{
					await startResend;
				} catch(Exception e){
					Logger.HandleException (e);
				}
			}

            return commitRes;
        }

        public const int RESEND_CHUNK_SIZE = 10;
        private async Task CheckAndSendResends()
        {
            var toResend = (await DAL.GetUnsentEvents()).ToList();

			var allResends = toResend.Where (ev => ev != null);

            var chunks = allResends.Chunk(RESEND_CHUNK_SIZE);
			foreach (var chunk in chunks) {
				if (chunk != null && chunk.Any (ev => ev != null)) {
					bool sent;
					try {
						var castItems = chunk.Cast<IEntityEventBase> ().ToList ();
						sent = await _resendEventsWebService.ResendEvents (castItems);
					} catch (Exception e) {
						sent = false;
						Logger.HandleException (e);
					}

					if (sent == false) {
						await DAL.ReAddFailedSendEvents (chunk);
					} else {
						await DAL.MarkEventsAsSent (chunk);
					}
				}
			}
        }

        /// <summary>
        /// If true, we have events we stored in the DB that we need to send again
        /// </summary>
        public bool HasEventsToSend { get; protected set; }

        public enum ResendResult
        {
            NoItemsToSend,
            StillOffline,
            SentSome,
            SentAll,
            Error
        }


        /// <summary>
        /// Get the events of this type that are waiting to send
        /// </summary>
        protected Task<IEnumerable<TEventType>> GetOfflineEvents()
        {
            throw new NotImplementedException();
            //TODO get them from teh DAL
        }
    }
}
