using System;
using System.Threading.Tasks;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Common.Events.Inventory;
using Mise.Core.Entities.Base;
using Mise.Core.Entities.Inventory;
using Mise.Core.Entities.Vendors.Events;
using Mise.Core.Repositories;
using Mise.Core.Server.Repositories;
using Mise.Core.Server.Services;
using Mise.Core.Server.Services.DAL;
using Mise.Core.Services;
using Mise.Core.ValueItems;

namespace MiseInventoryService.Repositories
{
    public class ReceivingOrderRepository : BaseAdminServiceRepository<IReceivingOrder, IReceivingOrderEvent>, IReceivingOrderRepository
    {
        public ReceivingOrderRepository(ILogger logger, IEntityDAL entityDAL, IWebHostingEnvironment host) : base(logger, entityDAL, host)
        {
        }

        public override Task<CommitResult> Commit(Guid entityID)
        {
            return CommitBase(entityID, EntityDAL.UpdateReceivingOrderAsync, EntityDAL.AddReceivingOrderAsync);
        }

        protected override IReceivingOrder CreateNewEntity()
        {
            return new ReceivingOrder();
        }

        protected override bool IsEventACreation(IEntityEventBase ev)
        {
            return ev is ReceivingOrderCreatedEvent;
        }

        public override Guid GetEntityID(IReceivingOrderEvent ev)
        {
            return ev.ReceivingOrderID;
        }

        protected override async Task LoadFromDB()
        {
            var items= await EntityDAL.GetReceivingOrdersAsync(DefaultCacheTime);
            Cache.UpdateCache(items);
        }
    }
}
