using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Common.Repositories.Base;
using Mise.Core.Entities.Base;
using Mise.Core.Server.Services;
using Mise.Core.Server.Services.DAL;
using Mise.Core.Services;
using Mise.Core.ValueItems;

namespace MiseInventoryService.Repositories
{
    public abstract class BaseAdminServiceRepository<TEntity, TEventType> : BaseEventSourcedRepository<TEntity, TEventType> 
        where TEntity : class, IEventStoreEntityBase<TEventType>, ICloneableEntity 
        where TEventType : class, IEntityEventBase
    {
        public async Task Load(Guid? restID)
        {
            Loading = true;
            await LoadFromDB();
            Loading = false;
        }

        public bool Loading { get; protected set; }
        /// <summary>
        /// Tells our repository to start loading its data from the DAL
        /// </summary>
        /// <returns></returns>
        protected abstract Task LoadFromDB();

        public IEntityDAL EntityDAL;

        protected IWebHostingEnvironment HostingEnvironment { get; private set; }

        protected BaseAdminServiceRepository(ILogger logger, IEntityDAL entityDAL, IWebHostingEnvironment hostEnvironment) : base(logger)
        {
            EntityDAL = entityDAL;
            DefaultCacheTime = new DateTimeOffset(DateTime.Now.AddYears(-5));
            Cache = new RepositoryCache<TEntity>();
            HostingEnvironment = hostEnvironment;
        }

        public override TEntity ApplyEvents(IEnumerable<TEventType> events)
        {
            TEntity ent = null;
            //group our events, then do a trans for each.  Events are stored by the service separately
            var groups = events.GroupBy(GetEntityID);
            foreach (var g in groups)
            {
                StartTransaction(g.Key);
                ent = base.ApplyEvents(g.AsEnumerable());
            }

            return ent;
        }

        public Task<CommitResult> CommitOnlyImmediately(Guid entityID)
        {
            throw new NotImplementedException();
        }

        public async Task<CommitResult> CommitBase(Guid entityID, Func<TEntity, Task> updateFunc, Func<TEntity, Task> addFunc)
        {
            var bundle = UnderTransaction[entityID];
            var itemExistsAlready = Cache.ContainsItem(entityID);

            //update us in memory first
            Cache.UpdateCache(bundle.NewVersion, ItemCacheStatus.Clean);
            //todo - put the cache in first, then commit to DB via an update function
            HostingEnvironment.QueueBackgroundWorkItem(async token =>
            {
                try
                {
                    var upsert = itemExistsAlready
                        ? updateFunc(bundle.NewVersion)
                        : addFunc(bundle.NewVersion);
                    await upsert.ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    Logger.HandleException(e);
                }
            });

            return CommitResult.StoredInDB;
        }

        public DateTimeOffset DefaultCacheTime;
    }
}
