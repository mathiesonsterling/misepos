using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Services.UtilityServices;
using Mise.Database.AzureDefinitions.Context;
using Mise.Database.AzureDefinitions.Entities.People;
using InventoryCategory = Mise.Database.AzureDefinitions.Entities.Categories.InventoryCategory;
using RestaurantInventorySection = Mise.Database.AzureDefinitions.Entities.Inventory.RestaurantInventorySection;

namespace TransferMiseEntitesTool.Consumers
{ 
    class InventoriesConsumer : BaseConsumer<Inventory, Mise.Database.AzureDefinitions.Entities.Inventory.Inventory>
    {
        public InventoriesConsumer(IJSONSerializer jsonSerializer) : base(jsonSerializer)
        {
        }

        public override string EntityName => "Inventories";

        protected override int BatchSize => 5;

        private IEnumerable<InventoryCategory> _categories;
        private IList<Employee> _employees;
        private IList<RestaurantInventorySection> _restaurantInventorySections;
         
        protected override async Task<Mise.Database.AzureDefinitions.Entities.Inventory.Inventory> SaveEntity(StockboyMobileAppServiceContext db, Inventory entity)
        {
            if (_employees == null)
            {
                _employees = await db.Employees.Where(e => !e.Deleted).ToListAsync();
            }

            if (_categories == null)
            {
                _categories = await db.InventoryCategories.Where(c => !c.Deleted).ToListAsync();
            }

            if (_restaurantInventorySections == null)
            {
                _restaurantInventorySections = await db.RestaurantInventorySections.Where(rs => !rs.Deleted).ToListAsync();
            }
            var rest = await db.Restaurants.FirstOrDefaultAsync(r => r.RestaurantID == entity.RestaurantID);

            var vendorIds =
                entity.Sections.SelectMany(s => s.GetInventoryBeverageLineItemsInSection())
                    .Select(li => li.VendorBoughtFrom)
                    .Where(v => v != null)
                    .Distinct()
                    .ToList();
            var vendors = await db.Vendors.Where(v => vendorIds.Contains(v.EntityId)).ToListAsync();

            var dbInv = new Mise.Database.AzureDefinitions.Entities.Inventory.Inventory(entity, rest, 
                _employees, _restaurantInventorySections, _categories, vendors);
            db.Inventories.Add(dbInv);

            return dbInv;
        }

        protected override Task<Mise.Database.AzureDefinitions.Entities.Inventory.Inventory> GetSavedEntity(StockboyMobileAppServiceContext db, Guid id)
        {
            return db.Inventories.FirstOrDefaultAsync(i => i.EntityId == id);
        }
    }
}
