using System;
using Mise.Core.Common.Events.ApplicationInvitations;
using Mise.Core.Entities.Base;
using Mise.Core.Entities;
using Mise.Core.Entities.People;
using Mise.Core.ValueItems;
using Mise.Core.Entities.Restaurant.Events;
namespace Mise.Core.Common.Entities
{
	public class ApplicationInvitation : RestaurantEntityBase, IApplicationInvitation
	{
		public ICloneableEntity Clone ()
		{
			return new ApplicationInvitation {
				ID = ID,
				Revision = Revision,
				CreatedDate = CreatedDate,
				LastUpdatedDate = LastUpdatedDate,
				RestaurantID = RestaurantID,

				Application = Application,
				Status = Status,
				DestinationEmail = DestinationEmail,
				DestinationEmployeeID = DestinationEmployeeID,
				InvitingEmployeeID = InvitingEmployeeID
			};
		}

		public MiseAppTypes Application {
			get;
			set;
		}
		public InvitationStatus Status {
			get;
			set;
		}
		public EmailAddress DestinationEmail {
			get;
			set;
		}
		public Guid? DestinationEmployeeID {
			get;
			set;
		}
		public Guid InvitingEmployeeID {
			get;
			set;
		}

		public PersonName InvitingEmployeeName {
			get;
			set;
		}

		public RestaurantName RestaurantName {
			get;
			set;
		}

		public void When (IApplicationInvitationEvent entityEvent)
		{
			switch (entityEvent.EventType) {
			case MiseEventTypes.EmployeeInvitedToApplication:
				WhenInvitationMade ((EmployeeInvitedToApplicationEvent)entityEvent);
				break;
			case MiseEventTypes.EmployeeAcceptsInvitation:
				WhenInvitationAccepted ((EmployeeAcceptsInvitationEvent)entityEvent);
				break;
			case MiseEventTypes.EmployeeRejectsInvitation:
				WhenInvitationRejected ((EmployeeRejectsInvitationEvent)entityEvent);
				break;
			default:
				throw new InvalidOperationException ("Don't know how to handle event of type " + entityEvent.EventType);
			}

			Revision = entityEvent.EventOrderingID;
			LastUpdatedDate = entityEvent.CreatedDate;

		}

		void WhenInvitationMade (EmployeeInvitedToApplicationEvent ev)
		{
			ID = ev.InvitationID;
			CreatedDate = ev.CreatedDate;
			Application = ev.Application;
			RestaurantID = ev.RestaurantID;
		    RestaurantName = ev.RestaurantName;
			DestinationEmail = ev.EmailToInvite;
			InvitingEmployeeID = ev.CausedByID;
			Status = InvitationStatus.Created;
		}

		void WhenInvitationAccepted (EmployeeAcceptsInvitationEvent ev)
		{
			Status = InvitationStatus.Accepted;
		}

		void WhenInvitationRejected (EmployeeRejectsInvitationEvent employeeRejectsInvitationEvent)
		{
			Status = InvitationStatus.Rejected;
		}
	}
}

