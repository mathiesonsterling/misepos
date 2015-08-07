using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Client.Services;
using Mise.Core.Common.Entities;
using Mise.Core.Common.Events.Restaurant;
using Mise.Core.Common.Services;
using Mise.Core.Common.Services.WebServices;
using Mise.Core.Entities.Base;
using Mise.Core.Entities.Restaurant;
using Mise.Core.Entities.Restaurant.Events;
using Mise.Core.Repositories;
using Mise.Core.Services.UtilityServices;
using Mise.Core.ValueItems;

namespace Mise.Core.Client.Repositories
{
    /// <summary>
    /// Implementation of the restaurant repository for the client.  Only stores and retrieves here!
    /// </summary>
	public class ClientRestaurantRepository : BaseEventSourcedClientRepository<IRestaurant, IRestaurantEvent, Restaurant>, 
		IRestaurantRepository
	{
		readonly IInventoryRestaurantWebService _webService;
        private readonly IDeviceLocationService _locationService;
        public ClientRestaurantRepository(ILogger logger, IClientDAL dal, IInventoryRestaurantWebService webService, IResendEventsWebService resend, IDeviceLocationService locationService)
            : base(logger, dal, webService, resend)
        {
			_webService = webService;
            _locationService = locationService;
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


        public override Guid GetEntityID(IRestaurantEvent ev)
        {
            return ev.RestaurantID;
        }

        protected override async Task<IEnumerable<Restaurant>> LoadFromWebservice(Guid? restaurantID)
        {
            if (restaurantID.HasValue)
            {
                var rest = await _webService.GetRestaurant(restaurantID.Value);
				return rest != null ? new List<Restaurant> {rest} : new List<Restaurant>();
            }

            var location = await _locationService.GetDeviceLocation();
			var items = await _webService.GetRestaurants(location, new Distance{Kilometers = 100});
            return items;
        }

        protected override async Task<IEnumerable<Restaurant>> LoadFromDB(Guid? restaurantID)
        {
            var items = await DAL.GetEntitiesAsync<Restaurant>();
            if (restaurantID.HasValue)
            {
                items = items.Where(r => r.ID == restaurantID);
            }

            return items;
        }
	}
}

