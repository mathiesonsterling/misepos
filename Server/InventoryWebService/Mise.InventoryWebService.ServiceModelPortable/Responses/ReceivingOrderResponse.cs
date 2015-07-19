using System.Collections.Generic;
using Mise.Core.Common.Entities.Inventory;

namespace Mise.InventoryWebService.ServiceModelPortable.Responses
{
    public class ReceivingOrderResponse
    {
        public IEnumerable<Core.Common.Entities.Inventory.ReceivingOrder> Results { get; set; } 
    }
}
