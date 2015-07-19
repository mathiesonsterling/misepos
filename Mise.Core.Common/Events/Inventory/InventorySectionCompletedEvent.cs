using System;
using Mise.Core.Common.Events.Inventory;

namespace Mise.Core.Common.Events.Inventory
{
	public class InventorySectionCompletedEvent : BaseInventoryEvent
	{
		/// <summary>
		/// The restaurant section ID that we're now done with
		/// </summary>
		/// <value>The restaurant section I.</value>
		public Guid RestaurantSectionID{get;set;}

		public override Core.Entities.MiseEventTypes EventType {
			get {
				return Core.Entities.MiseEventTypes.InventorySectionCompleted;
			}
		}
	}
}

