using System;
using Mise.Core.Entities;
using Mise.Core.Entities.People.Events;

namespace Mise.Core.Common.Events.ApplicationInvitations
{
	public class EmployeeRejectsInvitationEvent : BaseInvitationEvent, IEmployeeEvent
	{
		#region implemented abstract members of BaseRestaurantEvent
		public override MiseEventTypes EventType {
			get {
				return MiseEventTypes.EmployeeRejectsInvitation;
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

