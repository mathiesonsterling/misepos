using System;
using Mise.Core.Entities.Base;
using Mise.Core.Entities.Restaurant.Events;
using Mise.Core.ValueItems;

namespace Mise.Core.Entities.People
{
	public enum InvitationStatus{
		Created,
		/// <summary>
		/// The invite was sent, but not yet processed
		/// </summary>
		Sent,
		/// <summary>
		/// User accepted the invitation
		/// </summary>
		Accepted,
		/// <summary>
		/// User rejected the invitation
		/// </summary>
		Rejected,
		/// <summary>
		/// Restaurant no longer wishes to extend the invitation
		/// </summary>
		Withdrawn
	}

	/// <summary>
	/// Represents an invitation to use a MiseApplication for a restaurant
	/// </summary>
	public interface IApplicationInvitation : IEventStoreEntityBase<IApplicationInvitationEvent>, IRestaurantEntityBase, ITextSearchable
	{
		/// <summary>
		/// Which application this is for
		/// </summary>
		/// <value>The application.</value>
		MiseAppTypes Application{get;}

		InvitationStatus Status{get;}

		/// <summary>
		/// Email address of the person we're inviting to use
		/// </summary>
		/// <value>The destination email.</value>
		EmailAddress DestinationEmail{get;}

		/// <summary>
		/// If set, we this is an already existing person we're inviting to a new app or restaurant
		/// </summary>
		/// <value>The destination employee I.</value>
		Guid? DestinationEmployeeID{get;}

		/// <summary>
		/// ID of the employee who made the invitation
		/// </summary>
		/// <value>The inviting employee I.</value>
		Guid InvitingEmployeeID{get;}
		PersonName InvitingEmployeeName{get;}

		BusinessName RestaurantName{get;}
	}
}

