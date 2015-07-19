using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using Mise.Core.Repositories;
using Mise.InventoryWebService.ServiceModelPortable.Responses;
using ServiceStack;
using Restaurant = Mise.InventoryWebService.ServiceModelPortable.Responses.Restaurant;

namespace Mise.InventoryWebService.ServiceInterface
{
    public class RestaurantService : Service
    {
        private readonly IRestaurantRepository _restaurantRepository;

        public RestaurantService(IRestaurantRepository restaurantRepository)
        {
            _restaurantRepository = restaurantRepository;
        }

        public RestaurantResponse Get(Restaurant request)
        {
            if (_restaurantRepository.Loading)
            {
                Thread.Sleep(100);
                if (_restaurantRepository.Loading)
                {
                    throw new HttpError(HttpStatusCode.ServiceUnavailable, "Server has not yet loaded");
                }
            }
            if (request.RestaurantID.HasValue == false)
            {
                var rests = _restaurantRepository.GetAll().Cast<Core.Common.Entities.Restaurant>().ToList();
                //give them all - is this a privacy concern?
                return new RestaurantResponse
                {
                    Results = rests
                };
            }

            var rest =  _restaurantRepository.GetByID(request.RestaurantID.Value);
            if (rest == null)
            {
                throw HttpError.NotFound("No restaurant with ID " + request.RestaurantID.Value);
            }
            return new RestaurantResponse
            {
                Results = new List<Core.Common.Entities.Restaurant> {rest as Core.Common.Entities.Restaurant}
            };
        }
    }
}
