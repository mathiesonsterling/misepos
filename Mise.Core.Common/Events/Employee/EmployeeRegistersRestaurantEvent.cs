using System;
using Mise.Core.Entities;

namespace Mise.Core.Common.Events.Employee
{
	/// <summary>
	/// This needs to be a seperate events, since the other is an aggregate root creator
	/// </summary>
	public class EmployeeRegistersRestaurantEvent : BaseEmployeeEvent
	{
		#region implemented abstract members of BaseEmployeeEvent
		public override Mise.Core.Entities.MiseEventTypes EventType {
			get {
				return MiseEventTypes.EmployeeRegistersRestaurant;
			}
		}
		#endregion

		public MiseAppTypes ApplicationUsed{ get;set;}
	}
}

