using System.Collections.Generic;
using Mise.Core.Common.Entities.Inventory;

namespace Mise.InventoryWebService.ServiceModelPortable.Responses
{
    public class PurchaseOrderResponse
    {
        public IEnumerable<Core.Common.Entities.Inventory.PurchaseOrder> Results { get; set; } 
    }
}
