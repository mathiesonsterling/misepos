using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Mise.Core.Common.Entities;
using Mise.Core.Entities;
using Mise.Core.Repositories;
using Mise.Core.Services;
using ServiceStack;

namespace Mise.RestaurantServer.ServiceStack
{
    [Route("/api/{RestaurantFriendlyName*}", "GET")]
    
    public class RestaurantRequest : IReturn<Restaurant>
    {
        [ApiMember(IsRequired = true)]
        public string RestaurantFriendlyName { get; set; }

        /// <summary>
        /// If set, the terminal that is requesting this
        /// </summary>
        public string TerminalName { get; set; }
    }

    [Route("/api/{RestaurantFriendlyName}/Terminals", "POST")]
    public class TerminalRequest
    {
        [ApiMember(IsRequired = true)]
         public string RestaurantFriendlyName { get; set; }

         [ApiMember(IsRequired = true)]
         public MiseTerminalDevice Terminal { get; set; }
    }

    public class RestaurantService : BaseMiseService
    {
        public RestaurantService(IRestaurantRepository restaurantRepository, ILogger logger) : base(restaurantRepository, logger)
        {
        }

        /// <summary>
        /// Register the terminal with this restaurant
        /// </summary>
        /// <returns></returns>
        public object Post(TerminalRequest request)
        {
            //do we have the terminal already?
            throw new NotImplementedException();
        }

        public object Get(RestaurantRequest request)
        {
            var restaurant= RestaurantRepository.GetByFriendlyID(request.RestaurantFriendlyName);
            if (restaurant != null && string.IsNullOrEmpty(request.TerminalName) == false)
            {
                var term = restaurant.GetTerminals().FirstOrDefault(t => t.FriendlyID == request.TerminalName);
                if (term != null)
                {
                    term.IsMe = true;
                }
            }

            return restaurant;
        }
    }
}