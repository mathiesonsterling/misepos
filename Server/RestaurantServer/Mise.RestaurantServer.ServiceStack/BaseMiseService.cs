using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Mise.Core.Entities.Restaurant;
using Mise.Core.Repositories;
using Mise.Core.Services;
using ServiceStack;

namespace Mise.RestaurantServer.ServiceStack
{
    public abstract class BaseMiseService : Service
    {
        protected IRestaurantRepository RestaurantRepository { get; private set; }
        protected ILogger Logger { get; private set; }
        protected BaseMiseService(IRestaurantRepository restaurantRepository, ILogger logger)
        {
            RestaurantRepository = restaurantRepository;
            Logger = logger;
        }

        /// <summary>
        /// Gets our restaurant, or throws an argument exception (400) error if not found
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected IRestaurant GetRestaurantByFriendlyName(string name)
        {
            var restaurant = RestaurantRepository.GetByFriendlyID(name);
            if (restaurant == null)
            {
                throw new ArgumentException("No restaurant found for " + name);
            }

            return restaurant;
        }
    }
}