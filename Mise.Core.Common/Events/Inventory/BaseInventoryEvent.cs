﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities;
using Mise.Core.Entities.Inventory.Events;
using Mise.Core.ValueItems;

namespace Mise.Core.Common.Events.Inventory
{
    public abstract class BaseInventoryEvent : IInventoryEvent
    {
        public Guid InventoryID { get; set; }

        public Guid Id { get; set; }
        public Guid RestaurantId { get; set; }
        public EventID EventOrder { get; set; }
        public Guid CausedById { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
		public string DeviceId{get;set;}
        public abstract MiseEventTypes EventType { get; }
        public virtual bool IsEntityCreation { get { return false; } }
        public virtual bool IsAggregateRootCreation { get { return false; } }
    }
}
