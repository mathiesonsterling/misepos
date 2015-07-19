using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities;
using Mise.Core.ValueItems;

namespace Mise.Core.Common.Events.Vendors
{
    public class VendorPhoneNumberUpdatedEvent : BaseVendorEvent
    {
        public override MiseEventTypes EventType
        {
            get { return MiseEventTypes.VendorPhoneNumberUpdated; }
        }

        public PhoneNumber PhoneNumber { get; set; }
    }

}
