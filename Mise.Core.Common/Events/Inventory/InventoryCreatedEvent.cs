using System;
using System.Collections.Generic;
using Mise.Core.Entities;
using Mise.Core.Common.Entities.Inventory;

namespace Mise.Core.Common.Events.Inventory
{
    /// <summary>
    /// Event when we create the inventory, starting it
    /// </summary>
    public class InventoryCreatedEvent : BaseInventoryEvent
    {
        public override MiseEventTypes EventType
        {
            get { return MiseEventTypes.InventoryCreated; }
        }
    }
}
