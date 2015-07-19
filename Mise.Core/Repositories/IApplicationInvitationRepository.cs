using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mise.Core.Entities.People;
using Mise.Core.Entities.Restaurant.Events;
using Mise.Core.ValueItems;

namespace Mise.Core.Repositories
{
	public interface IApplicationInvitationRepository : 
		IEventSourcedEntityRepository<IApplicationInvitation, IApplicationInvitationEvent>
	{
	    Task<IEnumerable<IApplicationInvitation>> GetOpenInvitesForEmail(EmailAddress address);
	    Task<IEnumerable<IApplicationInvitation>> GetInvitesForRestaurant(Guid restaurantID);

		/// <summary>
		/// Overload of load to get information based upon emails
		/// </summary>
		/// <param name="email">Email.</param>
		Task Load (EmailAddress email);
	}
}

