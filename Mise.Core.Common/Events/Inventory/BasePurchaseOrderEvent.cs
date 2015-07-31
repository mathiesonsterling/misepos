using System;
using Mise.Core.Entities;
using Mise.Core.Entities.Inventory.Events;
using Mise.Core.ValueItems;

namespace Mise.Core.Common.Events.Inventory
{
    public abstract class BasePurchaseOrderEvent : IPurchaseOrderEvent
    {
        public Guid PurchaseOrderID { get; set; }
        public abstract MiseEventTypes EventType { get; }

        public virtual bool IsEntityCreation { get { return false; } }
        public virtual bool IsAggregateRootCreation { get { return false; } }

        public Guid ID { get; set; }
        public Guid RestaurantID { get; set; }
        public EventID EventOrderingID { get; set; }
        public Guid CausedByID { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
		public string DeviceID{get;set;}
    }
}
