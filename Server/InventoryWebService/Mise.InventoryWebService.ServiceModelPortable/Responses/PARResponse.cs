using System.Collections.Generic;
using Mise.Core.Common.Entities.Inventory;

namespace Mise.InventoryWebService.ServiceModelPortable.Responses
{
    public class PARResponse
    {
        public IEnumerable<Core.Common.Entities.Inventory.PAR> Results { get; set; } 
    }
}
