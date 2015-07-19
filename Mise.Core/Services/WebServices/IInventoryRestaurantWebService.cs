using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities.Restaurant.Events;
using System.Collections;
using Mise.Core.ValueItems;
using Mise.Core.Entities.Restaurant;

namespace Mise.Core.Services.WebServices
{
    public interface IInventoryRestaurantWebService : IEventStoreWebService<IRestaurant, IRestaurantEvent>
    {
		Task<IEnumerable<IRestaurant>> GetRestaurants (Location deviceLocation);

		Task<IRestaurant> GetRestaurant (Guid restaurantID);
    }
}
