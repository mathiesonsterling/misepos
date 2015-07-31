﻿using Mise.Core.Entities;
using Mise.Core.ValueItems;

namespace Mise.Core.Common.Events.Employee
{
	public class EmployeeRegisteredForInventoryAppEvent : BaseEmployeeEvent
	{
		#region implemented abstract members of BaseEmployeeEvent
		public override MiseEventTypes EventType {
			get {
				return MiseEventTypes.EmployeeRegisteredForInventoryAppEvent;
			}
		}

	    public override bool IsEntityCreation
	    {
	        get { return true; }
	    }

	    public override bool IsAggregateRootCreation
	    {
	        get { return true; }
	    }

	    public EmailAddress EmailAddress{get;set;}
		public Password Password{get;set;}
		#endregion
	}
}

