using System;
using Mise.Core.Entities;
namespace Mise.Core.Common.Events.Inventory
{
	public class InventoryLineItemDeletedEvent : BaseInventoryEvent
	{
		#region implemented abstract members of BaseInventoryEvent
		public override MiseEventTypes EventType {
			get {
				return MiseEventTypes.InventoryLineItemDeleted;
			}
		}

		public Guid InventoryLineItemID{get;set;}
		public Guid InventorySectionID{get;set;}
		#endregion
	}
}

