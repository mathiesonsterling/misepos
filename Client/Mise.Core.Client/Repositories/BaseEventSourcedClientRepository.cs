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
using Mise.Core.ValueItems;
using System.Linq.Expressions;
using Mise.Core.Common.Services.WebServices;
using Mise.Core.Common.Services.WebServices.Exceptions;

namespace Mise.Core.Client.Repositories
{
    /// <summary>
    /// A repository which deals with event sourced entities on the client, sending their changes back to the server
    /// when online, or storing in the database when not
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TEventType"></typeparam>
    public abstract class BaseEventSourcedClientRepository<TEntity, TEventType, TConcreteStorageType> 
		: BaseEventSourcedRepository<TEntity, TEventType> 
        where TEntity : class, IEventStoreEntityBase<TEventType>, ICloneableEntity 
        where TEventType : class, IEntityEventBase
        where TConcreteStorageType : class, IEntityBase, TEntity, new()
    {
        private readonly IEventStoreWebService<TConcreteStorageType, TEventType> _webService;
        private readonly IResendEventsWebService _resendEventsWebService;

        protected BaseEventSourcedClientRepository(ILogger logger,
            IEventStoreWebService<TConcreteStorageType, TEventType> webService,
            IResendEventsWebService resendService
            ) : base(logger)
        {
            _webService = webService;
            _resendEventsWebService = resendService;
        }

        protected abstract Task<IEnumerable<TConcreteStorageType>> LoadFromWebservice(Guid? restaurantID);

        protected virtual TimeSpan DelayToReload { get { return new TimeSpan(0, 0, 0, 1); } }

        /// <summary>
        /// The last restaurantID we got loaded with
        /// </summary>
        protected Guid? RestaurantID { get; private set; }


        /// <summary>
        /// Each client repository needs to be able to load, both when attached to a restaurant and when not
        /// </summary>
        /// <param name="restaurantID"></param>
        /// <returns></returns>
        public async Task Load(Guid? restaurantID)
        {
            Loading = true;
            RestaurantID = restaurantID;
            try
            {
				var items = (await LoadFromWebservice(restaurantID)).ToList();
                Cache.UpdateCache(items);

                Offline = false;
            }
            catch (Exception e)
            {
                Logger.HandleException(e, LogLevel.Warn);
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
				var sendRes = await _webService.SendEventsAsync(bundle.NewVersion as TConcreteStorageType, toSend).ConfigureAwait(false);
                if (sendRes)
                {
                    commitRes = CommitResult.SentToServer;
				} else{
					throw new DataNotSavedOnServerException();
				}
            }
            catch (Exception ex)
            {
                Logger.HandleException(ex);
				throw new DataNotSavedOnServerException (ex);
            }
				

            var status = commitRes == CommitResult.SentToServer ? ItemCacheStatus.Clean : ItemCacheStatus.ClientDB;
            Cache.UpdateCache(bundle.NewVersion, status);

            Dirty = false;

            return commitRes;
        }

        public override async Task<CommitResult> Commit(Guid entityID)
        {
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


            try{
				var sendRes = await _webService.SendEventsAsync (bundle.NewVersion as TConcreteStorageType, toSend).ConfigureAwait (false);
                if(sendRes){
                    commitRes = CommitResult.SentToServer;
                }
                else
                {
					throw new DataNotSavedOnServerException ();
                }
            } catch(Exception ex){
                Logger.HandleException (ex);
				throw new DataNotSavedOnServerException (ex);
            }


            Cache.UpdateCache(bundle.NewVersion, ItemCacheStatus.Clean);

            Dirty = false;

            return commitRes;
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
