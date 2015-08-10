using System;
using System.Linq;
using Mise.Core.Common.Events.ApplicationInvitations;
using Mise.Core.Entities.People;
using Mise.Core.Entities.Restaurant.Events;
using Mise.Core.Services;
using Mise.Core.Common.Services;
using Mise.Core.Common.Entities;
using Mise.Core.Common;
using Mise.Core.Entities.Base;
using System.Threading.Tasks;
using System.Collections.Generic;
using Mise.Core.Common.Services.WebServices;
using Mise.Core.Repositories;
using Mise.Core.ValueItems;
using Mise.Core.Services.UtilityServices;
namespace Mise.Core.Client.Repositories
{
	public class ClientApplicationInvitationRepository 
		: BaseEventSourcedClientRepository<IApplicationInvitation, IApplicationInvitationEvent, ApplicationInvitation>, IApplicationInvitationRepository
	{
		readonly IApplicationInvitationWebService _webService;
		public ClientApplicationInvitationRepository(ILogger logger,
            IApplicationInvitationWebService webService, IResendEventsWebService resend)
            : base(logger, webService, resend)
		{
			_webService = webService;
		}

		#region implemented abstract members of BaseEventSourcedRepository

		protected override IApplicationInvitation CreateNewEntity ()
		{
			return new ApplicationInvitation ();
		}


	    public override Guid GetEntityID(IApplicationInvitationEvent ev)
	    {
	        return ev.InvitationID;
	    }

	    #endregion

		#region implemented abstract members of BaseEventSourcedClientRepository

	    protected override async Task<IEnumerable<ApplicationInvitation>> LoadFromWebservice(Guid? restaurantID)
	    {
	        if (restaurantID.HasValue)
	        {
	            var items = await _webService.GetInvitationsForRestaurant(restaurantID.Value);
	            return items.Cast<ApplicationInvitation>();
	        }

            throw new Exception("Cannot load without a restaurant ID");
	    }

	    public async Task Load (EmailAddress email)
		{
			try{
                Loading = true;
				var items = (await _webService.GetInvitationsForEmail (email)).ToList();

				if(items.Any()){
					Cache.UpdateCache (items);
				}
			    Loading = false;
			} catch(Exception e){
				Logger.HandleException (e);
			}
				
		}

		#endregion

	    public Task<IEnumerable<IApplicationInvitation>> GetOpenInvitesForEmail(EmailAddress address)
	    {
	        var items = GetAll().Where(ai => ai.Status == InvitationStatus.Created || ai.Status == InvitationStatus.Sent)
	            .Where(ai => ai.DestinationEmail.Equals(address));

	        return Task.FromResult(items);
	    }

	    public Task<IEnumerable<IApplicationInvitation>> GetInvitesForRestaurant(Guid restaurantID)
	    {
	        var items = GetAll().Where(ai => ai.RestaurantID == restaurantID);
	        return Task.FromResult(items);
	    }
	}
}

