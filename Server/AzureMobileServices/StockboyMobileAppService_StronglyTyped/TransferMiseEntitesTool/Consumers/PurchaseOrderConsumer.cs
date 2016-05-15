using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities.Inventory;
using Mise.Core.Services.UtilityServices;
using Mise.Database.AzureDefinitions.Context;
using Mise.Database.AzureDefinitions.Entities.Inventory;

namespace TransferMiseEntitesTool.Consumers
{
    class PurchaseOrderConsumer : BaseConsumer<IPurchaseOrder>
    {
        public PurchaseOrderConsumer(IJSONSerializer jsonSerializer) : base(jsonSerializer)
        {
        }

        protected override async Task SaveEntity(StockboyMobileAppServiceContext db, IPurchaseOrder entity)
        {
            var cats = await db.InventoryCategories.ToListAsync();

            var vendorIds = entity.GetPurchaseOrderPerVendors().Select(pov => pov.VendorID).Distinct().ToList();
            var vendors = await db.Vendors.Where(v => vendorIds.Contains(v.EntityId)).ToListAsync();
            var rest = await db.Restaurants.FirstOrDefaultAsync(r => entity.RestaurantID == r.EntityId);

            var createdEmp = await db.Employees.FirstOrDefaultAsync(e => e.EntityId == entity.CreatedByEmployeeID);
            var dbEnt = new PurchaseOrder(entity, cats, vendors, rest, createdEmp);
            db.PurchaseOrders.Add(dbEnt);
        }
    }
}
