﻿using System;
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
using Mise.Core.Entities.People;

namespace Mise.Core.Client.Repositories
{
    /// <summary>
    /// Implementation of the restaurant repository for the client.  Only stores and retrieves here!
    /// </summary>
	public class ClientRestaurantRepository : BaseEventSourcedClientRepository<IRestaurant, IRestaurantEvent, Restaurant>, 
		IRestaurantRepository
	{
		readonly IInventoryApplicationWebService _webService;
        private readonly IDeviceLocationService _locationService;
        public ClientRestaurantRepository(ILogger logger, IInventoryApplicationWebService webService, IDeviceLocationService locationService)
            : base(logger, webService)
        {
			_webService = webService;
            _locationService = locationService;
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

        public async Task<IEnumerable<IRestaurant>> GetRestaurantsEmployeeWorksAt(IEmployee emp)
        {
            var res = new List<IRestaurant>();

            foreach (var restId in emp.GetRestaurantIDs())
            {
                var local = GetByID(restId);
                if (local == null)
                {
                    local = await _webService.GetRestaurant(restId);
                }

                if (local != null)
                {
                    res.Add(local);
                }
            }

            if (res.Any())
            {
                Cache.UpdateCache(res);
            }
            return res;
        }

        public override Guid GetEntityID(IRestaurantEvent ev)
        {
            return ev.RestaurantId;
        }

        protected override async Task<IEnumerable<Restaurant>> LoadFromWebservice(Guid? restaurantID)
        {
            if (restaurantID.HasValue)
            {
                await _webService.SetRestaurantId(restaurantID.Value);
                var rest = await _webService.GetRestaurant(restaurantID.Value);
				return rest != null ? new List<Restaurant> {rest} : new List<Restaurant>();
            }

            var location = await _locationService.GetDeviceLocation();
			var items = await _webService.GetRestaurants(location, new Distance{Kilometers = 100});
            return items;
        }
	}
}

