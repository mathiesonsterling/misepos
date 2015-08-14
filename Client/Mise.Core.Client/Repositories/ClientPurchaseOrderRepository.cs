using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Common.Events.Inventory;
using Mise.Core.Common.Services;
using Mise.Core.Common.Services.WebServices;
using Mise.Core.Entities.Base;
using Mise.Core.Entities.Inventory;
using Mise.Core.Entities.Inventory.Events;
using Mise.Core.Repositories;
using Mise.Core.Services.UtilityServices;

namespace Mise.Core.Client.Repositories
{
    public class ClientPurchaseOrderRepository 
        : BaseEventSourcedClientRepository<IPurchaseOrder, IPurchaseOrderEvent, PurchaseOrder>, 
        IPurchaseOrderRepository
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


        public override Guid GetEntityID(IPurchaseOrderEvent ev)
        {
            return ev.PurchaseOrderID;
        }

        protected override async Task<IEnumerable<PurchaseOrder>> LoadFromDB(Guid? restaurantID)
        {
            var items = await DAL.GetEntitiesAsync<PurchaseOrder>();
            if (restaurantID.HasValue)
            {
                items = items.Where(po => po.RestaurantID == restaurantID);
            }
            return items;
        }

        protected override Task<IEnumerable<PurchaseOrder>> LoadFromWebservice(Guid? restaurantID)
        {
            //TODO enable returning POs at some point
            return Task.FromResult(new List<PurchaseOrder>().AsEnumerable());
        }

    }
}
