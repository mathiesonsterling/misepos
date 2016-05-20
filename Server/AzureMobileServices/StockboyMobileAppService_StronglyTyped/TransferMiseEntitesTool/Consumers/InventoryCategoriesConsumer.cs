using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities.Inventory;
using Mise.Core.Services.UtilityServices;
using Mise.Database.AzureDefinitions.Context;
using Mise.Database.AzureDefinitions.Entities.Categories;
using dbCat = Mise.Database.AzureDefinitions.Entities.Categories.InventoryCategory;
namespace TransferMiseEntitesTool.Consumers
{
    class InventoryCategoriesConsumer : BaseConsumer<Mise.Core.Common.Entities.Inventory.InventoryCategory, dbCat>
    {
        public InventoryCategoriesConsumer(IJSONSerializer jsonSerializer) : base(jsonSerializer)
        {
        }

        public override string EntityName => "InventoryCategories";

        protected override Task<dbCat> SaveEntity(StockboyMobileAppServiceContext db, 
            Mise.Core.Common.Entities.Inventory.InventoryCategory entity)
        {
            var dbEnt = new dbCat(entity);
            db.InventoryCategories.Add(dbEnt);

            return Task.FromResult(dbEnt);
        }

        protected override Task<InventoryCategory> GetSavedEntity(StockboyMobileAppServiceContext db, Guid id)
        {
            return db.InventoryCategories.FirstOrDefaultAsync(ic => ic.EntityId == id);
        }
    }
}
