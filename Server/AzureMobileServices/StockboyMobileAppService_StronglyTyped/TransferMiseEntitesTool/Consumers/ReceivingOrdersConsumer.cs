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
    class ReceivingOrdersConsumer : BaseConsumer<IReceivingOrder>
    {
        public ReceivingOrdersConsumer(IJSONSerializer jsonSerializer) : base(jsonSerializer)
        {
        }

        protected override async Task SaveEntity(StockboyMobileAppServiceContext db, IReceivingOrder entity)
        {
            var cats = await db.InventoryCategories.ToListAsync();
            var rest = await db.Restaurants.FirstOrDefaultAsync(r => entity.RestaurantID == r.EntityId);

            var emp = await db.Employees.FirstOrDefaultAsync(e => e.EntityId == entity.ReceivedByEmployeeID);

            var po = await db.PurchaseOrders.FirstOrDefaultAsync(p => p.EntityId == entity.PurchaseOrderID);
            var vendor = await db.Vendors.FirstOrDefaultAsync(v => v.EntityId == entity.VendorID);
            var dbEnt = new ReceivingOrder(entity, rest, emp, po, vendor, cats);
            db.ReceivingOrders.Add(dbEnt);
        }

        protected override Task<bool> DoesEntityExist(StockboyMobileAppServiceContext db, Guid id)
        {
            return db.ReceivingOrders.AnyAsync(ro => ro.EntityId == id);
        }
    }
}
