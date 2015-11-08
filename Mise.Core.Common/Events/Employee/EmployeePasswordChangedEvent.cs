using System;
using Mise.Core.ValueItems;
using Mise.Core.Entities;
namespace Mise.Core.Common.Events.Employee
{
	public class EmployeePasswordChangedEvent : BaseEmployeeEvent
	{
		#region implemented abstract members of BaseEmployeeEvent

		public override Mise.Core.Entities.MiseEventTypes EventType {
			get {
				return MiseEventTypes.EmployeePasswordChanged;
			}
		}

		#endregion

		public Password NewPassword{get;set;}
	}
}

