using System;
using System.Collections.Generic;
using System.Linq;
using Mise.Core.Entities.Vendors;
using Mise.Core.ValueItems;
using Mise.Database.AzureDefinitions.Entities.People;
using Mise.Database.AzureDefinitions.ValueItems;
using BusinessName = Mise.Database.AzureDefinitions.ValueItems.BusinessName;
using StreetAddress = Mise.Database.AzureDefinitions.ValueItems.StreetAddress;

namespace Mise.Database.AzureDefinitions.Entities.Vendor
{
    public class Vendor : BaseDbEntity<IVendor, Core.Common.Entities.Vendors.Vendor>
    {
        public Vendor()
        {
            StreetAddress = new StreetAddress();
            Name = new BusinessName();
        }
        public StreetAddress StreetAddress { get; set; }

        public EmailAddressDb EmailToOrderFrom { get; set; }

        public string Website { get; set; }

        public string VendorPhoneNumberAreaCode { get; set; }
        public string VendorPhoneNumber { get; set; }

        public Employee CreatedBy { get; set; }

        public List<Restaurant.Restaurant> RestaurantsAssociatedWith { get; set; } 

        public List<VendorBeverageLineItem> LineItems { get; set; }

        public BusinessName Name { get; set; }

        protected override Core.Common.Entities.Vendors.Vendor CreateConcreteSubclass()
        {
            return new Core.Common.Entities.Vendors.Vendor
            {
                StreetAddress = StreetAddress.ToValueItem(),
                EmailToOrderFrom = EmailToOrderFrom.ToValueItem(),
                Website = new Uri(Website),
                PhoneNumber = new PhoneNumber(VendorPhoneNumberAreaCode, VendorPhoneNumber),
                CreatedByEmployeeID = CreatedBy?.EntityId,
                RestaurantsAssociatedIDs = RestaurantsAssociatedWith?.Select(r => r.RestaurantID).ToList(),
                VendorBeverageLineItems = LineItems.Select(li => li.ToBusinessEntity()).ToList(),
                VendorName = Name.ToValueItem()
            };
        }
    }
}
