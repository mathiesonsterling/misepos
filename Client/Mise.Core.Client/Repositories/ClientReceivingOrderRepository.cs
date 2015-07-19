using System;
using System.Threading.Tasks;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Common.Services;
using Mise.Core.Entities.Base;
using Mise.Core.Entities.Inventory;
using Mise.Core.Entities.Vendors.Events;
using Mise.Core.Services;
using Mise.Core.Services.WebServices;
using Mise.Core.Common.Events.Inventory;
using Mise.Core.Repositories;


namespace Mise.Core.Client.Repositories
{
    public class ClientReceivingOrderRepository : BaseEventSourcedClientRepository<IReceivingOrder, IReceivingOrderEvent>, IReceivingOrderRepository
    {
        readonly IReceivingOrderWebService _webService;
        public ClientReceivingOrderRepository(ILogger logger, IClientDAL dal, IReceivingOrderWebService webService) : base(logger, dal, webService)
        {
            _webService = webService;
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

        public override async Task Load(Guid? restaurantID)
        {
            Loading = true;
            if (restaurantID.HasValue)
            {
                var items = await _webService.GetReceivingOrdersForRestaurant(restaurantID.Value);

			    Cache.UpdateCache (items);
            }
            Loading = false;
        }
    }
}
