using System;
using System.Collections.Generic;
using System.Linq;
using Mise.Core.Entities.Restaurant;
using Mise.Core.ValueItems;
using Mise.Database.AzureDefinitions.Entities.Inventory;
using BusinessName = Mise.Database.AzureDefinitions.ValueItems.BusinessName;
using StreetAddress = Mise.Database.AzureDefinitions.ValueItems.StreetAddress;

namespace Mise.Database.AzureDefinitions.Entities.Restaurant
{
    public class Restaurant : BaseDbEntity<IRestaurant, Core.Common.Entities.Restaurant>
    {
        public Restaurant()
        {
            Name = new BusinessName();
            StreetAddress = new StreetAddress();
            InventorySections = new List<RestaurantInventorySection>();
        }

        public Restaurant(IRestaurant source) : base(source)
        {
            RestaurantID = source.RestaurantID != Guid.Empty ? source.RestaurantID : source.Id;
            AccountID = source.AccountID;
            Name = new BusinessName(source.Name);
            StreetAddress = new StreetAddress(source.StreetAddress);
            PhoneNumberAreaCode = source.PhoneNumber?.AreaCode;
            PhoneNumber = source.PhoneNumber?.Number;
            IsPlaceholder = source.IsPlaceholder;

            var emails = source.GetEmailsToSendInventoryReportsTo().Select(e => e.Value);
            EmailsToSendReportsTo = string.Join(",", emails);

            var sections = source.GetInventorySections().Select(invS => new RestaurantInventorySection(invS)).ToList();
            InventorySections = sections;
        }

        public Guid RestaurantID { get; set; }

        public Guid? AccountID { get; set; }

        public BusinessName Name { get; set; }

        public StreetAddress StreetAddress { get; set; }

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
                StreetAddress = StreetAddress.ToValueItem(),
                InventorySections = InventorySections.Select(s => s.ToBusinessEntity()).ToList(),
                Name = Name.ToValueItem(),
                AccountID = AccountID,
                PhoneNumber = new PhoneNumber(PhoneNumberAreaCode, PhoneNumber),
                RestaurantID = EntityId,
            };
        }
    }
}
