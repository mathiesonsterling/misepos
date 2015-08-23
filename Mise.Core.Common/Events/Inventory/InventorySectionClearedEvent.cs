using System;
using Mise.Core.Common.Events.Inventory;
using Mise.Core.Entities;
namespace Mise.Core.Common.Events.Inventory
{
	/// <summary>
	/// Remove all line items from an inventory section
	/// </summary>
	public class InventorySectionClearedEvent : BaseInventoryEvent
	{
		#region implemented abstract members of BaseInventoryEvent

		public override MiseEventTypes EventType {
			get {
				return MiseEventTypes.InventorySectionCleared;
			}
		}

		#endregion

		public Guid SectionId{get;set;}
	}
}

