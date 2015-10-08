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

        public Guid Id { get; set; }
        public Guid RestaurantId { get; set; }
        public EventID EventOrder { get; set; }
        public Guid CausedById { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
		public string DeviceId{get;set;}
    }
}
