using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities.Inventory;
using Mise.Core.Services.UtilityServices;
using Mise.Database.AzureDefinitions.Context;
using Mise.Database.AzureDefinitions.Entities.Categories;

namespace TransferMiseEntitesTool.Consumers
{
    class InventoryCategoriesConsumer : BaseConsumer<IInventoryCategory>
    {
        public InventoryCategoriesConsumer(IJSONSerializer jsonSerializer) : base(jsonSerializer)
        {
        }

        protected override Task SaveEntity(StockboyMobileAppServiceContext db, IInventoryCategory entity)
        {
            var dbEnt = new InventoryCategory(entity);
            db.InventoryCategories.Add(dbEnt);

            return Task.FromResult(true);
        }
    }
}
