using System;
using Mise.Core.Common.Events.Employee;
using Mise.Core.Entities;
namespace Mise.Core.Common.Events.Inventory
{
	/// <summary>
	/// Fired when an employee begins to work on an inventory section
	/// </summary>
	public class InventorySectionStartedByEmployeeEvent : BaseInventoryEvent
	{
		#region implemented abstract members of BaseEmployeeEvent

		public override MiseEventTypes EventType {
			get {
				return MiseEventTypes.InventorySectionStartedByEmployee;
			}
		}

		#endregion

		public Guid InventorySectionId{get;set;}
	}
}

