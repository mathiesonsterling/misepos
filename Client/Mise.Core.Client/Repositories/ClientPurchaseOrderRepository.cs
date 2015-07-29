using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Common.Events.Inventory;
using Mise.Core.Common.Services;
using Mise.Core.Entities.Base;
using Mise.Core.Entities.Inventory;
using Mise.Core.Entities.Inventory.Events;
using Mise.Core.Repositories;
using Mise.Core.Services;
using Mise.Core.Services.WebServices;

namespace Mise.Core.Client.Repositories
{
    public class ClientPurchaseOrderRepository : BaseEventSourcedClientRepository<IPurchaseOrder, IPurchaseOrderEvent>, IPurchaseOrderRepository
    {
        private readonly IPurchaseOrderWebService _webService;
        public ClientPurchaseOrderRepository(ILogger logger, IClientDAL dal, IPurchaseOrderWebService webService, IResendEventsWebService resend)
            : base(logger, dal, webService, resend)
        {
            _webService = webService;
        }

        protected override IPurchaseOrder CreateNewEntity()
        {
            return new PurchaseOrder();
        }

        protected override bool IsEventACreation(IEntityEventBase ev)
        {
            return ev is PurchaseOrderCreatedEvent;
        }

        public override Guid GetEntityID(IPurchaseOrderEvent ev)
        {
            return ev.PurchaseOrderID;
        }

        protected override async Task<IEnumerable<IPurchaseOrder>> LoadFromDB(Guid? restaurantID)
        {
            var items = await DAL.GetEntitiesAsync<PurchaseOrder>();
            if (restaurantID.HasValue)
            {
                items = items.Where(po => po.RestaurantID == restaurantID);
            }
            return items;
        }

        protected override Task<IEnumerable<IPurchaseOrder>> LoadFromWebservice(Guid? restaurantID)
        {
            //TODO enable returning POs at some point
            return Task.FromResult(new List<IPurchaseOrder>().AsEnumerable());
        }

    }
}
