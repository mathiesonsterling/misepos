using System;
using System.Linq;
using System.Net;
using System.Threading;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Repositories;
using Mise.InventoryWebService.ServiceModelPortable.Responses;
using ServiceStack;
using ReceivingOrder = Mise.InventoryWebService.ServiceModelPortable.Responses.ReceivingOrder;

namespace Mise.InventoryWebService.ServiceInterface
{
    public class ReceivingOrderService : Service
    {
        private readonly IReceivingOrderRepository _receivingOrderRepository;

        public ReceivingOrderService(IReceivingOrderRepository repos)
        {
            _receivingOrderRepository = repos;
        }

        public ReceivingOrderResponse Get(ReceivingOrder request)
        {
            if (_receivingOrderRepository.Loading)
            {
                Thread.Sleep(100);
                if (_receivingOrderRepository.Loading)
                {
                    throw new HttpError(HttpStatusCode.ServiceUnavailable, "Server has not yet loaded");
                }
            }

            if (request.ReceivingOrderID.HasValue)
            {
                var ro = _receivingOrderRepository.GetByID(request.ReceivingOrderID.Value);
                return new ReceivingOrderResponse
                {
                    Results = new[] {ro as Core.Common.Entities.Inventory.ReceivingOrder}
                };
            }

            var res = _receivingOrderRepository.GetAll()
                .Where(ro => ro.RestaurantID == request.RestaurantID)
                .Cast<Core.Common.Entities.Inventory.ReceivingOrder>();
            return new ReceivingOrderResponse
            {
                Results = res
            };
        }
    }
}
