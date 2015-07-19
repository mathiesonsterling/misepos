using System;
using Mise.Core.Entities;
using Mise.Core.Entities.People.Events;

namespace Mise.Core.Common.Events.ApplicationInvitations
{
	public class EmployeeAcceptsInvitationEvent : BaseInvitationEvent, IEmployeeEvent
	{
		#region implemented abstract members of BaseInventoryEvent

		public override MiseEventTypes EventType {
			get {
				return MiseEventTypes.EmployeeAcceptsInvitation;
			}
		}

		#endregion

		public Guid EmployeeID {
			get;
			set;
		}

		public MiseAppTypes Application{get;set;}
	}
}

