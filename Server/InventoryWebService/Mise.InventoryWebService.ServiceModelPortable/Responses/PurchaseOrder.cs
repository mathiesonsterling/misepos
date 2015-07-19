using System;
using ServiceStack;

namespace Mise.InventoryWebService.ServiceModelPortable.Responses
{
    [Route("/restaurants/{RestaurantID}/purchaseorders")]
    [Route("/restaurants/{RestaurantID}/purchaseorders/{PurchaseOrderID}")]
    public class PurchaseOrder : IReturn<PurchaseOrderResponse>
    {
        public Guid RestaurantID { get; set; }
        public Guid? PurchaseOrderID { get; set; }
    }
}