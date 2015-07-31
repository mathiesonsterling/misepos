﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Entities;
using Mise.Core.Entities.Base;
using Mise.Core.Services.UtilityServices;
using Mise.Core.ValueItems;

namespace Mise.Core.Common.Repositories.Base
{
    public abstract class BaseEventSourcedRepository<TEntity, TEventType> : BaseRepository<TEntity>
        where TEntity : class, IEventStoreEntityBase<TEventType>, ICloneableEntity
        where TEventType : class, IEntityEventBase
    {
        /// <summary>
        /// Holds for each entity their original and modified versions, along with the events that transform them
        /// </summary>
        protected class EntityTransactionBundle
        {
            public Guid EntityID { get; set; }

            public TEntity NewVersion { get; set; }
            public TEntity OriginalVersion { get; set; }
            public List<TEventType> Events { get; set; }

            public EntityTransactionBundle()
            {
                Events = new List<TEventType>();
            }
        }


        protected ILogger Logger { get; private set; }
        protected readonly IDictionary<Guid, EntityTransactionBundle> UnderTransaction;
        protected BaseEventSourcedRepository(ILogger logger)
        {
            Logger = logger;
            UnderTransaction = new Dictionary<Guid, EntityTransactionBundle>();
        }

        public async Task<bool> CommitAll()
        {
            //get the ids
            var idsToCommit = UnderTransaction.Keys;
            var res = new List<CommitResult>();
            foreach (var id in idsToCommit)
            {
                var thisRes = await Commit(id);
                res.Add(thisRes);
            }

            return res.All(r => r == CommitResult.StoredInDB);
        }

        /// <summary>
        /// Create a new instance of our entity
        /// </summary>
        /// <returns></returns>
        protected abstract TEntity CreateNewEntity();
        /// <summary>
        /// Determines if an event should cause us to create a new entity
        /// </summary>
        /// <param name="ev"></param>
        /// <returns></returns>
        protected abstract bool IsEventACreation(IEntityEventBase ev);

        /// <summary>
        /// List out which events we want to see processed before any others.  Allows us to deal with creations of sub types
        /// </summary>
        protected virtual IEnumerable<MiseEventTypes> EventTypesToBeProcessedFirst {
            get { return new List<MiseEventTypes>(); }
        }
 
        public abstract Guid GetEntityID(TEventType ev);

        public bool Dirty { get; protected set; }

        /// <summary>
        /// Get the number of events that are currently under transaction for a given entity ID
        /// </summary>
        /// <param name="entityID"></param>
        /// <returns></returns>
        public int GetNumberOfEventsInTransacitonForEntity(Guid entityID)
        {
            if (UnderTransaction.ContainsKey(entityID))
            {
                return UnderTransaction[entityID].Events.Count;
            }

            return 0;
        }

        public bool StartTransaction(Guid entityID)
        {
            if (UnderTransaction.ContainsKey(entityID) == false)
            {
                var newBundle = new EntityTransactionBundle { EntityID = entityID };
                var thisCheck = GetByID(entityID);
                if (thisCheck != null)
                {
                    newBundle.OriginalVersion = thisCheck.Clone() as TEntity;
                    newBundle.NewVersion = thisCheck;
                }

                UnderTransaction.Add(entityID, newBundle);
                Dirty = true;
                return true;
            }

            return false;
        }

        public void CancelTransaction(Guid entityID)
        {
            if (UnderTransaction.ContainsKey(entityID) == false) return;

            var bundle = UnderTransaction[entityID];
            //get all our item IDs, we'll need to update them
            UnderTransaction.Remove(entityID);
            if (bundle.OriginalVersion != null)
            {
                Cache.UpdateCache(bundle.OriginalVersion, ItemCacheStatus.ClientMemory);
            }
            Dirty = false;
        }

        public virtual TEntity ApplyEvent(TEventType entEvent)
        {
            return ApplyEvents(new[] { entEvent });
        }


        /// <summary>
        /// Class that takes our item, starts a transaction if one doesn't exist, and transforms the entity based upon that
        /// </summary>
        /// <param name="events"></param>
        /// <returns></returns>
        public virtual TEntity ApplyEvents(IEnumerable<TEventType> events)
        {
            var oEvents = OrderEvents(events);

            var firstEv = oEvents.FirstOrDefault();
            if (firstEv == null)
            {
                throw new ArgumentException("Events given to apply are null or empty");
            }
            var entityID = GetEntityID(firstEv);


            if (UnderTransaction.ContainsKey(entityID) == false)
            {
                StartTransaction(entityID);
            }

            var bundle = UnderTransaction[entityID];
            foreach (var cEvent in oEvents)
            {
                if (bundle.NewVersion == null)
                {
                    var entID = GetEntityID(cEvent);
                    bundle.NewVersion = GetByID(entID);
                    //if our event isn't a creation, we might have a problem
                    if (bundle.NewVersion == null && (IsEventACreation(cEvent) == false))
                    {
                        throw new EntityForEventNotFoundException(entID);
                    }
                }
                if (bundle.NewVersion == null)
                {
                    bundle.NewVersion = CreateNewEntity();
                }
                bundle.NewVersion.When(cEvent);
                Logger.Log("Applied event " + cEvent.GetType(), LogLevel.Debug);
            }

            try
            {
                //add them to our staging cache
                if (bundle != null)
                {
                    bundle.Events.AddRange(oEvents);
                }
            }
            catch (Exception e)
            {
                Logger.HandleException(e);
            }

            Dirty = true;
            return bundle == null ? null : bundle.NewVersion;
        }

        public static IEnumerable<TEventType> OrderEventsOld(IEnumerable<TEventType> events)
        {
            var oEvents = events.OrderBy(e => e.CreatedDate)
                .ThenBy(e => e.EventOrderingID);
            return oEvents;
        }

        public ICollection<TEventType> OrderEvents(IEnumerable<TEventType> events)
        {
            //get the events that all came from the same application first
            var appGroups = from ev in events
                            group ev by new { ev.EventOrderingID.AppInstanceCode, ev.DeviceID } into appG
                            select new
                            {
                                AppInstanceCode = appG.Key.AppInstanceCode,
                                DeviceID = appG.Key.DeviceID,
                                Items = appG.AsEnumerable(),
                                FirstDate = appG.AsEnumerable().Min(g => g.CreatedDate)
                            };

            //we can now order these by min date
            //need then by the app type and device ID
            var groupsByDate = appGroups.OrderBy(k => k.AppInstanceCode)
                .ThenBy(k => k.DeviceID)
                .ThenBy(k => k.FirstDate);

            var res = new List<TEventType>();
            foreach (var group in groupsByDate)
            {
                //we want to get first the aggregate root creation, then any other creations, then the rest by order
                var orderedItems = group.Items
                    .OrderByDescending(IsEventACreation)
                    .ThenByDescending(ev => EventTypesToBeProcessedFirst.Contains(ev.EventType))
                    .ThenBy(ev => ev.EventOrderingID.OrderingID);
                res.AddRange(orderedItems);
            }

            return res;
        }
    }
}
