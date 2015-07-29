using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Common.Events.Inventory;
using Mise.Core.Entities;
using Mise.Core.Entities.Base;
using Mise.Core.Entities.Inventory;
using Mise.Core.Entities.Inventory.Events;
using Mise.Core.Repositories;
using Mise.Core.Server.Services;
using Mise.Core.Server.Services.DAL;
using Mise.Core.Services;
using Mise.Core.ValueItems;

namespace MiseInventoryService.Repositories
{
    public class InventoryServerRepository : BaseAdminServiceRepository<IInventory, IInventoryEvent>, IInventoryRepository
    {
        public InventoryServerRepository(ILogger logger, IEntityDAL entityDAL, IWebHostingEnvironment host) : base(logger, entityDAL, host)
        {
        }

        public override IInventory ApplyEvents(IEnumerable<IInventoryEvent> events)
        {
            var allEvents = events.ToList();
            var hasChangedCurrent = allEvents.FirstOrDefault(e => e.EventType == MiseEventTypes.InventoryMadeCurrent);
       
            var res = base.ApplyEvents(allEvents);

            if (hasChangedCurrent != null)
            {
                var restID = allEvents.First().RestaurantID;
                var others =
                    Cache.GetAll()
                        .Where(inv =>
                                inv.IsCurrent && inv.RestaurantID == restID && inv.ID != hasChangedCurrent.InventoryID)
                                .Cast<Inventory>();

                var updated = new List<IInventory>();
                foreach (var other in others)
                {
                    other.IsCurrent = false;
                    updated.Add(other);
                }
                Cache.UpdateCache(updated);
            }

            return res;
        }

        public override Task<CommitResult> Commit(Guid entityID)
        {
            return CommitBase(entityID, EntityDAL.UpdateInventoryAsync, EntityDAL.AddInventoryAsync);
        }

        public Task<IInventory> GetCurrentInventory(Guid restaurantID)
        {
            return Task.Run(() => GetAll().FirstOrDefault(i => i.IsCurrent && i.RestaurantID == restaurantID));
        }

        public Task<IInventory> GetCurrentInventory()
        {
            return Task.Run(() => GetAll().FirstOrDefault(i => i.IsCurrent));
        }

        protected override IInventory CreateNewEntity()
        {
            return new Inventory();
        }

        protected override bool IsEventACreation(IEntityEventBase ev)
        {
            return ev is InventoryCreatedEvent;
        }

        public override Guid GetEntityID(IInventoryEvent ev)
        {
            return ev.InventoryID;
        }

        protected override async Task LoadFromDB()
        {
            var items = await EntityDAL.GetInventoriesAsync(DefaultCacheTime);
            Cache.UpdateCache(items);
        }
    }
}
