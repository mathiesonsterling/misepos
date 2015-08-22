using System;
using Mise.Core.Common.Events.Checks;


using Mise.Core.Entities;
namespace Mise.Core.Common.Events.Inventory
{
	public class InventoryLineItemMovedToNewPositionEvent : BaseInventoryEvent
	{
		public override MiseEventTypes EventType {
			get {
				return MiseEventTypes.InventoryLineItemMovedToNewPosition;
			}
		}
		public Guid LineItemID{ get; set;}

		public Guid InventorySectionID{get;set;}

		public int NewPositionWanted{get;set;}
	}
}

