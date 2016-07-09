using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Mise.Core.Entities.Restaurant;
using Mise.Core.ValueItems;
using Mise.Database.AzureDefinitions.Entities.Accounts;
using Mise.Database.AzureDefinitions.Entities.Inventory;

namespace Mise.Database.AzureDefinitions.Entities.Restaurant
{
    public class Restaurant : BaseDbEntity<IRestaurant, Core.Common.Entities.Restaurant>
    {
        public Restaurant()
        {
            InventorySections = new List<RestaurantInventorySection>();
        }

        public Restaurant(IRestaurant source, RestaurantAccount acct) : base(source)
        {
            RestaurantID = source.RestaurantID != Guid.Empty ? source.RestaurantID : source.Id;
            RestaurantAccount = acct;
            RestaurantAccountId = acct?.Id;
            FullName = source.Name.FullName;
            ShortName = source.Name.ShortName;
            PhoneNumberAreaCode = source.PhoneNumber?.AreaCode;
            PhoneNumber = source.PhoneNumber?.Number;
            IsPlaceholder = source.IsPlaceholder;

            var emails = source.GetEmailsToSendInventoryReportsTo().Select(e => e.Value);
            EmailsToSendReportsTo = string.Join(",", emails);

            var sections = source.GetInventorySections().Select(invS => new RestaurantInventorySection(invS)).ToList();
            InventorySections = sections;

            if (source.StreetAddress != null)
            {
                StreetNumber = source.StreetAddress.StreetAddressNumber.Number;
                StreetDirection = source.StreetAddress.StreetAddressNumber.Direction;
                ApartmentNumber = source.StreetAddress.StreetAddressNumber.ApartmentNumber;
                Latitude = source.StreetAddress.StreetAddressNumber.Latitude;
                Longitude = source.StreetAddress.StreetAddressNumber.Longitude;

                StreetName = source.StreetAddress.Street.Name;
                City = source.StreetAddress.City.Name;
                State = source.StreetAddress.State.Name;
                Country = source.StreetAddress.Country.Name;
                Zip = source.StreetAddress.Zip.Value;
            }
        }

        public Guid RestaurantID { get; set; }

        public RestaurantAccount RestaurantAccount { get; set; }
        [ForeignKey("RestaurantAccount")]
        public string RestaurantAccountId { get; set; }

        public string FullName { get; set; }
        public string ShortName { get; set; }

        public string StreetNumber { get; set; }
        public string StreetDirection { get; set; }

        public string ApartmentNumber { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }

        public string StreetName { get; set; }

        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string Zip { get; set; }

        public string PhoneNumberAreaCode { get; set; }
        public string PhoneNumber { get; set; }

        public bool IsPlaceholder { get; set; }

        public string EmailsToSendReportsTo { get; set; }

        public List<RestaurantInventorySection> InventorySections { get; set; }

        public List<RestaurantApplicationUse> RestaurantApplicationUses { get; set; }
         
        protected override Core.Common.Entities.Restaurant CreateConcreteSubclass()
        {
            var emails = EmailsToSendReportsTo.Split(',').Select(e => new EmailAddress(e)).ToList();
            return new Core.Common.Entities.Restaurant 
            {
                EmailsToSendInventoryReportsTo = emails,
                StreetAddress = new StreetAddress(StreetNumber, StreetDirection,
                    StreetName, City, State, Country, Zip, Latitude, Longitude),
                InventorySections = InventorySections.Select(s => s.ToBusinessEntity()).ToList(),
                Name = new BusinessName(FullName, ShortName),
                AccountID = RestaurantAccount?.EntityId,
                PhoneNumber = new PhoneNumber(PhoneNumberAreaCode, PhoneNumber),
                RestaurantID = EntityId,
            };
        }
    }
}
