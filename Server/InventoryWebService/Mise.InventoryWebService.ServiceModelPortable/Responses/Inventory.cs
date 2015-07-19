using System;
using ServiceStack;

namespace Mise.InventoryWebService.ServiceModelPortable.Responses
{
    [Route("/restaurants/{RestaurantID}/inventories")]
    [Route("/restaurants/{RestaurantID}/inventories/{InventoryID}")]
    public class Inventory : IReturn<InventoryResponse>
    {
        /// <summary>
        /// ID of the restaurant this inventory belongs to
        /// </summary> 
        public Guid RestaurantID { get; set; }

        public Guid? InventoryID { get; set; }

    }
}