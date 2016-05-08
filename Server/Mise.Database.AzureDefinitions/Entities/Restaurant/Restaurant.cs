using System;
using System.Collections.Generic;
using System.Linq;
using Mise.Core.Entities.Restaurant;
using Mise.Core.ValueItems;
using Mise.Database.AzureDefinitions.Entities.Inventory;
using Mise.Database.AzureDefinitions.ValueItems;
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
            EmailsToSendReportsTo = new List<EmailAddressDb>();
            InventorySections = new List<RestaurantInventorySection>();
        }

	  public Restaurant(IRestaurant source, List<EmailAddressDb> emails) : base(source)
	  {
		RestaurantID = source.RestaurantID != Guid.Empty ? source.RestaurantID : source.Id;
		AccountID = source.AccountID;
		Name = source.Name;
		StreetAddress = new StreetAddress(source.StreetAddress);
		  PhoneNumberAreaCode = source.PhoneNumber?.AreaCode;
		  PhoneNumber = source.PhoneNumber?.Number;
		  IsPlaceholder = source.IsPlaceholder;
		  EmailsToSendReportsTo = emails;

		  var sections = source.GetInventorySections().Select(invS => new RestaurantInventorySection(invS));
		  InventorySections = sections;
	  }
        public Guid RestaurantID { get; set; }

        public Guid? AccountID { get; set; }

        public BusinessName Name { get; set; }

        public StreetAddress StreetAddress { get; set; }

        public string PhoneNumberAreaCode { get; set; }
        public string PhoneNumber { get; set; }

        public bool IsPlaceholder { get; set; }

        public List<EmailAddressDb> EmailsToSendReportsTo { get; set; }

        public List<RestaurantInventorySection> InventorySections { get; set; }
         
        protected override Core.Common.Entities.Restaurant CreateConcreteSubclass()
        {
            return new Core.Common.Entities.Restaurant
            {
                EmailsToSendInventoryReportsTo = EmailsToSendReportsTo.Select(e => e.ToValueItem()).ToList(),
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
