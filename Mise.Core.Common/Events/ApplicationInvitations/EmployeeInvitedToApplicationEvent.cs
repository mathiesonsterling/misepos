using Mise.Core.Entities;
using Mise.Core.ValueItems;

namespace Mise.Core.Common.Events.ApplicationInvitations
{
	public class EmployeeInvitedToApplicationEvent : BaseInvitationEvent
	{
		#region implemented abstract members of BaseEmployeeEvent
		public override MiseEventTypes EventType {
			get {
				return MiseEventTypes.EmployeeInvitedToApplication;
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

	    #endregion

		/// <summary>
		/// Email address we want to invite
		/// </summary>
		/// <value>The email to invite.</value>
		public EmailAddress EmailToInvite{get;set;}

		/// <summary>
		/// Application this invitation is for
		/// </summary>
		public MiseAppTypes Application{get;set;}

        public RestaurantName RestaurantName { get; set; }
	}
}

