using Mise.Core.Entities;
using Mise.Core.ValueItems;

namespace Mise.Core.Common.Events.Employee
{
	/// <summary>
	/// Fired to show an employee has logged into the app
	/// </summary>
	public class EmployeeLoggedIntoInventoryAppEvent : BaseEmployeeEvent
	{
		#region implemented abstract members of BaseEmployeeEvent
		public override MiseEventTypes EventType {
			get {
				return MiseEventTypes.EmployeeLoggedIntoInventoryAppEvent;
			}
		}
		#endregion
	}
}

