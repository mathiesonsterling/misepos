using System;
using System.Collections.Generic;
using Mise.Core.Entities;
using Mise.Core.Entities.People;
using Mise.Core.Entities.Restaurant;
using Mise.Core.Entities.Restaurant.Events;

namespace Mise.Core.Repositories
{
    public interface IRestaurantRepository : IEventSourcedEntityRepository<IRestaurant, IRestaurantEvent>
    {
        IRestaurant GetByFriendlyID(string friendlyID);

		/// <summary>
		/// Gets a selection of possible restaurants by name
		/// </summary>
		/// <returns>The by name.</returns>
		/// <param name="name">Name.</param>
		IEnumerable<IRestaurant> GetByName (string name);

        /// <summary>
        /// Restaurants associated with an account id
        /// </summary>
        /// <param name="accountID"></param>
        /// <returns></returns>
        IEnumerable<IRestaurant> GetRestaurantsForAccount(Guid accountID);
    }
}
