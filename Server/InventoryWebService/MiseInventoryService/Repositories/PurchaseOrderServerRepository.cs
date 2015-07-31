using System;
using System.Threading.Tasks;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Common.Events.Inventory;
using Mise.Core.Entities.Base;
using Mise.Core.Entities.Inventory;
using Mise.Core.Entities.Inventory.Events;
using Mise.Core.Repositories;
using Mise.Core.Server.Repositories;
using Mise.Core.Server.Services;
using Mise.Core.Server.Services.DAL;
using Mise.Core.Services;
using Mise.Core.Services.UtilityServices;
using Mise.Core.ValueItems;

namespace MiseInventoryService.Repositories
{
    public class PurchaseOrderServerRepository : BaseAdminServiceRepository<IPurchaseOrder, IPurchaseOrderEvent>, IPurchaseOrderRepository
    {
        public PurchaseOrderServerRepository(ILogger logger, IEntityDAL entityDAL, IWebHostingEnvironment host) : base(logger, entityDAL, host)
        {
        }

        public override Task<CommitResult> Commit(Guid entityID)
        {
            return CommitBase(entityID, EntityDAL.UpdatePurchaseOrderAsync, EntityDAL.AddPurchaseOrderAsync);
        }

        protected override IPurchaseOrder CreateNewEntity()
        {
            return new PurchaseOrder();
        }

        public override Guid GetEntityID(IPurchaseOrderEvent ev)
        {
            return ev.PurchaseOrderID;
        }

        protected override async Task LoadFromDB()
        {
            var items = await EntityDAL.GetPurchaseOrdersAsync(DefaultCacheTime);
            Cache.UpdateCache(items);
        }

    }
}
