using System;
using ServiceStack;

namespace Mise.InventoryWebService.ServiceModelPortable.Responses
{
    [Route("/restaurants/{RestaurantID}/receivingorders")]
    [Route("/restaurants/{RestaurantID}/receivingorders/{ReceivingOrderID}")]
    public class ReceivingOrder : IReturn<ReceivingOrderResponse>
    {
        public Guid RestaurantID { get; set; }
        public Guid? ReceivingOrderID { get; set; }
    }
}