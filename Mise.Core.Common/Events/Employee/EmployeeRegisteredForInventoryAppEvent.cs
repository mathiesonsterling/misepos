using System;
using Mise.Core.Common.Events;
using Mise.Core.Common.Events.Employee;
using Mise.Core.Entities;
using Mise.Core.ValueItems;


namespace Mise.Core.Common
{
	public class EmployeeRegisteredForInventoryAppEvent : BaseEmployeeEvent
	{
		#region implemented abstract members of BaseEmployeeEvent
		public override MiseEventTypes EventType {
			get {
				return MiseEventTypes.EmployeeRegisteredForInventoryAppEvent;
			}
		}

		public EmailAddress EmailAddress{get;set;}
		public Password Password{get;set;}
		#endregion
	}
}

