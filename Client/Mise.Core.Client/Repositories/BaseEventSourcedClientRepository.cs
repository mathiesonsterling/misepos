using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Common.Repositories.Base;
using Mise.Core.Common.Services;
using Mise.Core.Entities.Base;
using Mise.Core.Services;
using Mise.Core.Services.WebServices;
using Mise.Core.ValueItems;

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
 
        protected BaseEventSourcedClientRepository(ILogger logger, 
            IClientDAL dal,
            IEventStoreWebService<TEntity, TEventType> webService 
            ) : base(logger)
        {
            DAL = dal;
            _webService = webService;
        }

        /// <summary>
        /// Each client repository needs to be able to load, both when attached to a restaurant and when not
        /// </summary>
        /// <param name="restaurantID"></param>
        /// <returns></returns>
        public abstract Task Load(Guid? restaurantID);


        /// <summary>
        /// Whether or not the item is currently loading
        /// </summary>
        public bool Loading { get; protected set; }

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
            Cache.UpdateCache(bundle.NewVersion, ItemCacheStatus.TerminalMemory);

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
                Cache.UpdateCache(bundle.NewVersion, ItemCacheStatus.TerminalDB);
            }

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
            Cache.UpdateCache(bundle.NewVersion, ItemCacheStatus.TerminalMemory);

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
                var storeRes = await DAL.StoreEventsAsync(toSend).ConfigureAwait(false);
                if (storeRes)
                {
                    commitRes = CommitResult.StoredInDB;
                }
            }

            var entRes = await DAL.UpsertEntitiesAsync(new[] {bundle.NewVersion});

              
            if (entRes)
            {
                Cache.UpdateCache(bundle.NewVersion, ItemCacheStatus.TerminalDB);
            }

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

        public async Task<ResendResult> AttemptToResendOfflineEvents()
        {
            if (HasEventsToSend == false)
            {
                return ResendResult.NoItemsToSend;
            }

            //get our events
            var events = await GetOfflineEvents();

            return ResendResult.Error;
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
