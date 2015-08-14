using System;
using Mise.Core.Common.Events.Inventory;

namespace Mise.Core.Common.Events.Inventory
{
	public class InventorySectionCompletedEvent : BaseInventoryEvent
	{
        public Guid InventorySectionID { get; set; }
		public override Core.Entities.MiseEventTypes EventType {
			get {
				return Core.Entities.MiseEventTypes.InventorySectionCompleted;
			}
		}
	}
}

