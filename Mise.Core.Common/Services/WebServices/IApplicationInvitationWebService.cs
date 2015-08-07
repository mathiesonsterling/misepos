using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mise.Core.Common.Entities;
using Mise.Core.Entities.People;
using Mise.Core.Entities.Restaurant.Events;
using Mise.Core.ValueItems;

namespace Mise.Core.Common.Services.WebServices
{
	public interface IApplicationInvitationWebService : IEventStoreWebService<ApplicationInvitation, IApplicationInvitationEvent>
	{
		Task<IEnumerable<ApplicationInvitation>> GetInvitationsForRestaurant (Guid restaurantID);
	    Task<IEnumerable<ApplicationInvitation>> GetInvitationsForEmail(EmailAddress email);
	}
}

