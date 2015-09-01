using System;
using System.Collections.Generic;
using Mise.Core.Entities.Restaurant;

namespace MiseReporting.Models
{
    public class RestaurantViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public string Address { get; set; }

        public IEnumerable<InventoryViewModel> Inventories { get; private set; }

        public IEnumerable<ParViewModel> Pars { get; private set; }

        public IEnumerable<ReceivingOrderViewModel> ReceivingOrders { get; private set; } 

        public IEnumerable<PurchaseOrderViewModel> PurchaseOrders { get; private set; }
         
        public RestaurantViewModel()
        {
        }

        public RestaurantViewModel(IRestaurant rest)
        {
            Id = rest.ID;

            if (rest.StreetAddress?.City != null && rest.StreetAddress.State != null)
            {
                Address = rest.StreetAddress.City.Name + ", " + rest.StreetAddress.State.Name;
            }


            Name = rest.Name.FullName;
        }

        public RestaurantViewModel(IRestaurant rest, IEnumerable<InventoryViewModel> inventories) : this(rest)
        {
            Inventories = inventories;
        }

        public RestaurantViewModel(IRestaurant rest, IEnumerable<ParViewModel> pars) : this(rest)
        {
            Pars = pars;
        }

        public RestaurantViewModel(IRestaurant rest, IEnumerable<ReceivingOrderViewModel> ros) : this(rest)
        {
            ReceivingOrders = ros;
        }

        public RestaurantViewModel(IRestaurant rest, IEnumerable<PurchaseOrderViewModel> pos) : this(rest)
        {
            PurchaseOrders = pos;
        }
    }
}
