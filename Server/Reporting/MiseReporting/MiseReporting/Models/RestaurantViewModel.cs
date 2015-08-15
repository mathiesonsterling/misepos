using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities.Restaurant;

namespace MiseReporting.Models
{
    public class RestaurantViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public string Address { get; set; }

        public RestaurantViewModel()
        {
        }

        public RestaurantViewModel(IRestaurant rest)
        {
            Id = rest.ID;

            if (rest.StreetAddress != null && rest.StreetAddress.City != null && rest.StreetAddress.State != null)
            {
                Address = rest.StreetAddress.City.Name + ", " + rest.StreetAddress.State.Name;
            }


            Name = rest.Name.FullName;
        }
    }
}
