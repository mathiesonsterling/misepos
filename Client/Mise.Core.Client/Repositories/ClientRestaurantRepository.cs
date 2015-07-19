using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Common.Entities;
using Mise.Core.Common.Events.Restaurant;
using Mise.Core.Common.Services;
using Mise.Core.Entities.Base;
using Mise.Core.Entities.Restaurant;
using Mise.Core.Entities.Restaurant.Events;
using Mise.Core.Repositories;
using Mise.Core.Services;
using Mise.Core.Services.WebServices;
using Mise.Core.ValueItems;
using System.Threading;

namespace Mise.Core.Client.Repositories
{
    /// <summary>
    /// Implementation of the restaurant repository for the client.  Only stores and retrieves here!
    /// </summary>
	public class ClientRestaurantRepository : BaseEventSourcedClientRepository<IRestaurant, IRestaurantEvent>, 
		IRestaurantRepository
	{
		readonly IInventoryRestaurantWebService _webService;
        public ClientRestaurantRepository(ILogger logger, IClientDAL dal, IInventoryRestaurantWebService webService) : base(logger, dal, webService)
        {
			_webService = webService;
        }

			
	    public IRestaurant GetByFriendlyID(string friendlyID)
	    {
	        return Cache.FirstOrDefault(r => r.FriendlyID == friendlyID);
	    }

        public IEnumerable<IRestaurant> GetByName(string name)
        {
            return Cache.GetAll().Where(r => r.Name.ContainsSearchString(name));
        }

        public IEnumerable<IRestaurant> GetRestaurantsForAccount(Guid accountID)
        {
            return GetAll().Where(r => r.AccountID.HasValue && r.AccountID.Value == accountID);
        }

        protected override IRestaurant CreateNewEntity()
        {
            return new Restaurant();
        }

        protected override bool IsEventACreation(IEntityEventBase ev)
        {
			return ev is PlaceholderRestaurantCreatedEvent || ev is NewRestaurantRegisteredOnAppEvent;
        }

        public override Guid GetEntityID(IRestaurantEvent ev)
        {
            return ev.RestaurantID;
        }

        public override async Task Load(Guid? restaurantID)
        {
			try
			{
			    Loading = true;
				if(restaurantID.HasValue){
					var rest = await _webService.GetRestaurant (restaurantID.Value);
					Cache.UpdateCache (rest, ItemCacheStatus.Clean);
					return;
				}
					
				var allRests = await _webService.GetRestaurants (new Location ());
				Cache.UpdateCache (allRests);
			    Loading = false;
			} catch(Exception e){
				Logger.HandleException (e);
			}
        }
	}
}

