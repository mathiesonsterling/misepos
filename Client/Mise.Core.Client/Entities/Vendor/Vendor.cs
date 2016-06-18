using System;
using System.Collections.Generic;
using System.Linq;
using Mise.Core.Client.Entities.Categories;
using Mise.Core.Client.Entities.People;
using Mise.Core.Entities.Vendors;
using Mise.Core.ValueItems;

namespace Mise.Core.Client.Entities.Vendor
{
    public class Vendor : BaseDbEntity<IVendor, Core.Common.Entities.Vendors.Vendor>
    {
        public Vendor()
        {
            VendorBeverageLineItems = new List<VendorBeverageLineItem>();
        }

	    public Vendor(IVendor source, Employee createdBy, 
            IEnumerable<Restaurant.Restaurant> rests, IEnumerable<InventoryCategory> cats)
	    	:base(source)
	    {
	        EmailToOrderFrom = source.EmailToOrderFrom?.Value;
		    Website = source.Website?.AbsoluteUri;
		    VendorPhoneNumber = source.PhoneNumber?.Number;
		    VendorPhoneNumberAreaCode = source.PhoneNumber?.AreaCode;
	        FullName = source.Name;
		    CreatedBy = createdBy;

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

            RestaurantsAssociatedWith = rests.Select(r => new VendorRestaurantRelationships(this, r)).ToList();

	        VendorBeverageLineItems = source.GetItemsVendorSells()
	            .Select(li => new VendorBeverageLineItem(li, this, cats)).ToList();
	    }

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

        public string EmailToOrderFrom { get; set; }

        public string Website { get; set; }

        public string VendorPhoneNumberAreaCode { get; set; }

        public string VendorPhoneNumber { get; set; }

        public Employee CreatedBy { get; set; }
        public string CreatedById { get; set; }
        public List<VendorRestaurantRelationships> RestaurantsAssociatedWith { get; set; } 

        public List<VendorBeverageLineItem> VendorBeverageLineItems { get; set; }

        public string FullName { get; set; }
        public string ShortName { get; set; }

        protected override Core.Common.Entities.Vendors.Vendor CreateConcreteSubclass()
        {
            return new Core.Common.Entities.Vendors.Vendor
            {
                StreetAddress = new StreetAddress(StreetNumber, StreetDirection,
                    StreetName, City, State, Country, Zip, Latitude, Longitude),
                EmailToOrderFrom = new EmailAddress(EmailToOrderFrom),
                Website = new Uri(Website),
                PhoneNumber = new PhoneNumber(VendorPhoneNumberAreaCode, VendorPhoneNumber),
                CreatedByEmployeeID = CreatedBy?.EntityId,
                RestaurantsAssociatedIDs = RestaurantsAssociatedWith?.Select(r => r.Restaurant.RestaurantID).ToList(),
                VendorBeverageLineItems = VendorBeverageLineItems.Select(li => li.ToBusinessEntity()).ToList(),
                Name = FullName
            };
        }
    }
}
