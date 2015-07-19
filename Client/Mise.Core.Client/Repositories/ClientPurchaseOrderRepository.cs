using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
		private IPurchaseOrderWebService _webService;
        public ClientPurchaseOrderRepository(ILogger logger, IClientDAL dal, IPurchaseOrderWebService webService) : base(logger, dal, webService)
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

        public override Task Load(Guid? restaurantID)
        {
            //we don't currently load POS to the client right now
            return Task.FromResult(false);
        }
    }
}
