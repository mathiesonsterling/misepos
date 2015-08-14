using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Common.Services;
using Mise.Core.Entities.Base;
using Mise.Core.Entities.Inventory;
using Mise.Core.Entities.Vendors.Events;
using Mise.Core.Services;
using Mise.Core.Common.Events.Inventory;
using Mise.Core.Common.Services.WebServices;
using Mise.Core.Repositories;
using Mise.Core.Services.UtilityServices;

namespace Mise.Core.Client.Repositories
{
    public class ClientReceivingOrderRepository 
        : BaseEventSourcedClientRepository<IReceivingOrder, IReceivingOrderEvent, ReceivingOrder>, 
        IReceivingOrderRepository
    {
        readonly IReceivingOrderWebService _webService;
        public ClientReceivingOrderRepository(ILogger logger, IClientDAL dal, IReceivingOrderWebService webService, IResendEventsWebService resend)
            : base(logger, dal, webService, resend)
        {
            _webService = webService;
        }

        protected override IReceivingOrder CreateNewEntity()
        {
            return new ReceivingOrder();
        }


        public override Guid GetEntityID(IReceivingOrderEvent ev)
        {
            return ev.ReceivingOrderID;
        }

        protected override Task<IEnumerable<ReceivingOrder>> LoadFromWebservice(Guid? restaurantID)
        {
            if (restaurantID.HasValue == false)
            {
                throw new ArgumentException("Cannot load ROs without a restaurant");
            }
            return _webService.GetReceivingOrdersForRestaurant(restaurantID.Value);
        }

        protected override async Task<IEnumerable<ReceivingOrder>> LoadFromDB(Guid? restaurantID)
        {
            var items = await DAL.GetEntitiesAsync<ReceivingOrder>();
            if (restaurantID.HasValue)
            {
                items = items.Where(ro => ro.RestaurantID == restaurantID);
            }
            return items;
        }
    }
}
