using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities;
using Mise.Core.ValueItems;

namespace Mise.Core.Common.Events.Vendors
{
    public class VendorCreatedEvent : BaseVendorEvent
    {
        public override MiseEventTypes EventType
        {
            get { return MiseEventTypes.VendorCreatedEvent; }
        }

        public override bool IsEntityCreation
        {
            get { return true; }
        }

        public override bool IsAggregateRootCreation
        {
            get { return true; }
        }

        public string Name{ get; set;}
		public StreetAddress Address{get;set;}
		public PhoneNumber PhoneNumber{get;set;}

        public EmailAddress Email { get; set; }
    }
}
