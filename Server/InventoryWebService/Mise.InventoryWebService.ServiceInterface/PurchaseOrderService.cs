using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using Mise.Core.Repositories;
using Mise.InventoryWebService.ServiceModelPortable.Responses;
using ServiceStack;
using PurchaseOrder = Mise.InventoryWebService.ServiceModelPortable.Responses.PurchaseOrder;

namespace Mise.InventoryWebService.ServiceInterface
{
    public class PurchaseOrderService : Service
    {
        private readonly IPurchaseOrderRepository _purchaseOrderRepository;

        public PurchaseOrderService(IPurchaseOrderRepository repository)
        {
            _purchaseOrderRepository = repository;
        }

        public PurchaseOrderResponse Get(PurchaseOrder request)
        {
            if (_purchaseOrderRepository.Loading)
            {
                Thread.Sleep(100);
                if (_purchaseOrderRepository.Loading)
                {
                    throw new HttpError(HttpStatusCode.ServiceUnavailable, "Server has not yet loaded");
                }
            }
            var items = request.PurchaseOrderID.HasValue 
                ? new List<Core.Common.Entities.Inventory.PurchaseOrder> {_purchaseOrderRepository.GetByID(request.PurchaseOrderID.Value) as Core.Common.Entities.Inventory.PurchaseOrder} 
                : _purchaseOrderRepository.GetAll()
                    .Where(po => po.RestaurantID == request.RestaurantID)
                    .Cast<Core.Common.Entities.Inventory.PurchaseOrder>();

            return new PurchaseOrderResponse
            {
                Results = items
            };
        }
    }
}
