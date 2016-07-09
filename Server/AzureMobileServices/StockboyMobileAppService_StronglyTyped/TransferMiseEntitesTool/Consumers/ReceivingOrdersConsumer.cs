using System;
using System.Data.Entity;
using System.Threading.Tasks;
using Mise.Core.Entities.Inventory;
using Mise.Core.Services.UtilityServices;
using Mise.Database.AzureDefinitions.Context;
using dbRO = Mise.Database.AzureDefinitions.Entities.Inventory.ReceivingOrder;
namespace TransferMiseEntitesTool.Consumers
{
    class ReceivingOrdersConsumer : BaseConsumer<Mise.Core.Common.Entities.Inventory.ReceivingOrder, dbRO>
    {
        public ReceivingOrdersConsumer(IJSONSerializer jsonSerializer) : base(jsonSerializer)
        {
        }

        public override string EntityName => "ReceivingOrder";

        protected override async Task<dbRO> SaveEntity(StockboyMobileAppServiceContext db, Mise.Core.Common.Entities.Inventory.ReceivingOrder entity)
        {
            var cats = await db.InventoryCategories.ToListAsync();
            var rest = await db.Restaurants.FirstOrDefaultAsync(r => entity.RestaurantID == r.EntityId);

            var emp = await db.Employees.FirstOrDefaultAsync(e => e.EntityId == entity.ReceivedByEmployeeID);

            var po = await db.PurchaseOrders.FirstOrDefaultAsync(p => p.EntityId == entity.PurchaseOrderID);
            var vendor = await db.Vendors.FirstOrDefaultAsync(v => v.EntityId == entity.VendorID);
            var dbEnt = new dbRO(entity, rest, emp, po, vendor, cats);
            db.ReceivingOrders.Add(dbEnt);

            return dbEnt;
        }

        protected override Task<dbRO> GetSavedEntity(StockboyMobileAppServiceContext db, Guid id)
        {
            return db.ReceivingOrders.FirstOrDefaultAsync(ro => ro.EntityId == id);
        }
    }
}
