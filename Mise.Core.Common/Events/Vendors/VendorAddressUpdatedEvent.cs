using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities;
using Mise.Core.ValueItems;

namespace Mise.Core.Common.Events.Vendors
{
    public class VendorAddressUpdatedEvent : BaseVendorEvent
    {
        public override MiseEventTypes EventType
        {
            get { return MiseEventTypes.VendorAddressUpdated; }
        }

        public StreetAddress StreetAddress { get; set; }
    }
}
