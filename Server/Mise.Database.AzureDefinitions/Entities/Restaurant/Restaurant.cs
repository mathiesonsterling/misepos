using System;
using System.Collections.Generic;
using System.Linq;
using Mise.Core.Entities.Restaurant;
using Mise.Database.AzureDefinitions.Entities.Inventory;
using Mise.Database.AzureDefinitions.ValueItems;

namespace Mise.Database.AzureDefinitions.Entities.Restaurant
{
    public class Restaurant : BaseDbEntity<IRestaurant, Core.Common.Entities.Restaurant>
    {
        public Restaurant()
        {
            Name = new BusinessName();
            StreetAddress = new StreetAddress();
            PhoneNumber = new PhoneNumber();
        }

        public Guid RestaurantID { get; set; }

        public Guid? AccountID { get; set; }

        public BusinessName Name { get; set; }

        public StreetAddress StreetAddress { get; set; }

        public PhoneNumber PhoneNumber { get; set; }

        public bool IsPlaceholder { get; set; }

        public List<EmailAddress> EmailsToSendReportsTo { get; set; }

        public List<RestaurantInventorySection> InventorySections { get; set; }
         
        protected override Core.Common.Entities.Restaurant CreateConcreteSubclass()
        {
            return new Core.Common.Entities.Restaurant
            {
                EmailsToSendInventoryReportsTo = EmailsToSendReportsTo.Select(e => e.ToValueItem()).ToList(),
                StreetAddress = StreetAddress.ToValueItem(),
                InventorySections = InventorySections.Select(s => s.ToBusinessEntity()).ToList(),
                Name = Name,
                AccountID = AccountID,
                PhoneNumber = PhoneNumber,
                RestaurantID = EntityId,
            };
        }
    }
}
