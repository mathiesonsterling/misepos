using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Mise.Core.Entities.People;
using Mise.Core.Services.WebServices;
using Mise.Core.Entities.Restaurant.Events;
using Mise.Core.Entities.Restaurant;
using Mise.Core.ValueItems;

namespace Mise.Core.Services.WebServices
{
	public interface IApplicationInvitationWebService : IEventStoreWebService<IApplicationInvitation, IApplicationInvitationEvent>
	{
		Task<IEnumerable<IApplicationInvitation>> GetInvitationsForRestaurant (Guid restaurantID);
	    Task<IEnumerable<IApplicationInvitation>> GetInvitationsForEmail(EmailAddress email);
	}
}

