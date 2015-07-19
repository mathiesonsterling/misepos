using System.Collections.Generic;
using Mise.Core.Common.Entities.Inventory;
namespace Mise.InventoryWebService.ServiceModelPortable.Responses
{
    public class InventoryResponse
    {
        public IEnumerable<Core.Common.Entities.Inventory.Inventory> Results { get; set; }
    }
}
