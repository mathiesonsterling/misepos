using System;
using System.Linq;
using Mise.Core.Common.Events.ApplicationInvitations;
using Mise.Core.Entities.People;
using Mise.Core.Entities.Restaurant.Events;
using Mise.Core.Services;
using Mise.Core.Services.WebServices;
using Mise.Core.Common.Services;
using Mise.Core.Common.Entities;
using Mise.Core.Common;
using Mise.Core.Entities.Base;
using System.Threading.Tasks;
using System.Collections.Generic;
using Mise.Core.Repositories;
using Mise.Core.ValueItems;
using Mise.Core.Services.UtilityServices;
namespace Mise.Core.Client.Repositories
{
	public class ClientApplicationInvitationRepository 
		: BaseEventSourcedClientRepository<IApplicationInvitation, IApplicationInvitationEvent>, IApplicationInvitationRepository
	{
		readonly IApplicationInvitationWebService _webService;
		public ClientApplicationInvitationRepository(ILogger logger, IClientDAL dal,
            IApplicationInvitationWebService webService, IResendEventsWebService resend)
            : base(logger, dal, webService, resend)
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

	    protected override Task<IEnumerable<IApplicationInvitation>> LoadFromWebservice(Guid? restaurantID)
	    {
	        if (restaurantID.HasValue)
	        {
	            return _webService.GetInvitationsForRestaurant(restaurantID.Value);
	        }

            throw new Exception("Cannot load without a restaurant ID");
	    }

	    protected override async Task<IEnumerable<IApplicationInvitation>> LoadFromDB(Guid? restaurantID)
	    {
	        var items = await DAL.GetEntitiesAsync<ApplicationInvitation>();
	        if (restaurantID.HasValue)
	        {
	            items = items.Where(ai => ai.RestaurantID == restaurantID);
	        }
	        return items;
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

