using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mise.Core.Common.Entities;
using Mise.Core.Entities.Restaurant;
using Mise.Core.Entities.Restaurant.Events;
using Mise.Core.ValueItems;

namespace Mise.Core.Common.Services.WebServices
{
    public interface IInventoryRestaurantWebService : IEventStoreWebService<Restaurant, IRestaurantEvent>
    {
		Task<IEnumerable<Restaurant>> GetRestaurants (Location deviceLocation, Distance maxDistance);

		Task<Restaurant> GetRestaurant (Guid restaurantID);
    }
}
