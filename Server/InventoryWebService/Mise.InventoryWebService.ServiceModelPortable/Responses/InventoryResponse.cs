using System.Collections.Generic;

namespace Mise.InventoryWebService.ServiceModelPortable.Responses
{
    public class InventoryResponse
    {
        public IEnumerable<Core.Common.Entities.Inventory.Inventory> Results { get; set; }
    }
}
