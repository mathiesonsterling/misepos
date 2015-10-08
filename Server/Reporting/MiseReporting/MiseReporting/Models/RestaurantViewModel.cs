using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Mise.Core.Common.Entities;
using Mise.Core.Entities;
using Mise.Core.Entities.Restaurant;
using Mise.Core.ValueItems;

namespace MiseReporting.Models
{
    public class RestaurantViewModel
    {
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Address => string.IsNullOrWhiteSpace(City)?"Unknown":City + ", " + State;

        [Required]
        [DisplayName("Street Number")]
        public string StreetAddressNumber { get; set; }

        [DisplayName("Apt / Suite")]
        public string UnitNumber { get; set; }

        [DisplayName("Street Direction (N, S, Etc)")]
        public string StreetDirection { get; set; }

        [Required]
        [DisplayName("Street Name")]
        public string StreetName { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string State { get; set; }

        public string Country { get; set; }
        public string ZipCode { get; set; }

        public IEnumerable<InventoryViewModel> Inventories { get; private set; }

        public IEnumerable<ParViewModel> Pars { get; private set; }

        public IEnumerable<ReceivingOrderViewModel> ReceivingOrders { get; private set; } 

        public IEnumerable<PurchaseOrderViewModel> PurchaseOrders { get; private set; }
         
        public DateTime CurrentTime { get; private set; }

        public RestaurantViewModel()
        {
            CurrentTime = DateTimeOffset.UtcNow.ToLocalTime().DateTime;
        }

        public RestaurantViewModel(IRestaurant rest)
        {
            Id = rest.Id;

            if (rest.StreetAddress != null)
            {
                StreetAddressNumber = rest.StreetAddress.StreetAddressNumber.Number;
                StreetDirection = rest.StreetAddress.StreetAddressNumber.Direction;
                StreetName = rest.StreetAddress.Street.Name;
                City = rest.StreetAddress.City.Name;
                State = rest.StreetAddress.State.Name;
                Country = rest.StreetAddress.Country.Name;
                ZipCode = rest.StreetAddress.Zip.Value;
            }


            Name = rest.Name.FullName;
            CurrentTime = DateTimeOffset.UtcNow.ToLocalTime().DateTime;
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

        public Restaurant ToEntity()
        {
            return new Restaurant
            {
                CreatedDate = DateTime.UtcNow,
                LastUpdatedDate = DateTime.UtcNow,
                Revision = new EventID(MiseAppTypes.ManagementWebsite, 1),
                Id = this.Id,
                RestaurantID = this.Id,
                Name = new RestaurantName(this.Name),
                AccountID = null,
                StreetAddress =
                    new StreetAddress(this.StreetAddressNumber, this.StreetDirection, this.StreetName, this.City,
                        this.State, Mise.Core.ValueItems.Country.UnitedStates.Name, this.ZipCode),

            };
        }
    }
}
