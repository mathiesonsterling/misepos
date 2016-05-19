using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Services.UtilityServices;
using Mise.Database.AzureDefinitions.Context;

namespace TransferMiseEntitesTool.Consumers
{ 
    class InventoriesConsumer : BaseConsumer<Inventory, Mise.Database.AzureDefinitions.Entities.Inventory.Inventory>
    {
        public InventoriesConsumer(IJSONSerializer jsonSerializer) : base(jsonSerializer)
        {
        }

        protected override async Task<Mise.Database.AzureDefinitions.Entities.Inventory.Inventory> SaveEntity(StockboyMobileAppServiceContext db, Inventory entity)
        {
            var rest = await db.Restaurants.FirstOrDefaultAsync(r => r.RestaurantID == entity.RestaurantID);

            var vendorIds =
                entity.Sections.SelectMany(s => s.GetInventoryBeverageLineItemsInSection())
                    .Select(li => li.VendorBoughtFrom)
                    .Where(v => v != null)
                    .Distinct()
                    .ToList();
            var vendors = await db.Vendors.Where(v => vendorIds.Contains(v.EntityId)).ToListAsync();

            var emps = db.Employees.Where(e => !e.Deleted).ToList();
            var rSecs = db.RestaurantInventorySections.Where(rs => !rs.Deleted).ToList();
            var cats = db.InventoryCategories.Where(c => !c.Deleted);
            var dbInv = new Mise.Database.AzureDefinitions.Entities.Inventory.Inventory(entity, rest, emps, rSecs, cats, vendors);
            db.Inventories.Add(dbInv);

            return dbInv;
        }

        protected override Task<Mise.Database.AzureDefinitions.Entities.Inventory.Inventory> GetSavedEntity(StockboyMobileAppServiceContext db, Guid id)
        {
            return db.Inventories.FirstOrDefaultAsync(i => i.EntityId == id);
        }
    }
}
