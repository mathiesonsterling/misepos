using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities;
using Mise.Core.Entities.Vendors.Events;
using Mise.Core.ValueItems;

namespace Mise.Core.Common.Events.Vendors
{
    public abstract class BaseVendorEvent : IVendorEvent
    {
        public abstract MiseEventTypes EventType { get; }
        public Guid ID { get; set; }
        public Guid RestaurantID { get; set; }
		public string DeviceID{get;set;}
        public EventID EventOrderingID { get; set; }
        public Guid CausedByID { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public Guid VendorID { get; set; }
    }
}
