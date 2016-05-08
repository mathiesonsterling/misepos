using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities.Vendors;
using Mise.Database.AzureDefinitions.Entities.People;
using Mise.Database.AzureDefinitions.ValueItems;

namespace Mise.Database.AzureDefinitions.Entities.Vendor
{
    public class Vendor : BaseDbEntity<IVendor, Core.Common.Entities.Vendors.Vendor>
    {
        public StreetAddress StreetAddress { get; set; }

        public EmailAddress EmailToOrderFrom { get; set; }

        public string Website { get; set; }

        public PhoneNumber PhoneNumber { get; set; }

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
                PhoneNumber = PhoneNumber.ToValueItem(),
                CreatedByEmployeeID = CreatedBy?.EntityId,
                RestaurantsAssociatedIDs = RestaurantsAssociatedWith?.Select(r => r.RestaurantID).ToList(),
                VendorBeverageLineItems = LineItems.Select(li => li.ToBusinessEntity()).ToList(),
                Name = Name.ToValueItem()
            };
        }
    }
}
