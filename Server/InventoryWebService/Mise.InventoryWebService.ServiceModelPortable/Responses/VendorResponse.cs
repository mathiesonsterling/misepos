using System.Collections.Generic;
using Mise.Core.Common.Entities.Vendors;

namespace Mise.InventoryWebService.ServiceModelPortable.Responses
{
    public class VendorResponse
    {
        public IEnumerable<Core.Common.Entities.Vendors.Vendor> Results { get; set; } 
    }
}
