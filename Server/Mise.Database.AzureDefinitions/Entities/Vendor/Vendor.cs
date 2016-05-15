using System;
using System.Collections.Generic;
using System.Linq;
using Mise.Core.Entities.Vendors;
using Mise.Core.ValueItems;
using Mise.Database.AzureDefinitions.Entities.Categories;
using Mise.Database.AzureDefinitions.Entities.People;
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

	    public Vendor(IVendor source, Employee createdBy, ICollection<Restaurant.Restaurant> rests, ICollection<InventoryCategory> cats)
	    	:base(source)
	    {
		    StreetAddress = new StreetAddress(source.StreetAddress);
	        EmailToOrderFrom = source.EmailToOrderFrom?.Value;
		    Website = source.Website?.AbsoluteUri;
		    VendorPhoneNumber = source.PhoneNumber?.Number;
		    VendorPhoneNumberAreaCode = source.PhoneNumber?.AreaCode;
		    Name = new BusinessName(source.Name);
		    CreatedBy = createdBy;
	        RestaurantsAssociatedWith = rests.Select(r => new VendorRestaurantRelationships(this, r)).ToList();

	        LineItems = GetLineItems(source.GetItemsVendorSells(), rests, cats).ToList();
	    }

	    IEnumerable<VendorBeverageLineItem> GetLineItems(IEnumerable<IVendorBeverageLineItem> source,
		    ICollection<Restaurant.Restaurant> rests, ICollection<InventoryCategory> cats)
	    {
		    foreach (var sourceLI in source)
		    {
			    var thisRestIds = sourceLI.GetPricesForRestaurants().Keys.Distinct();
			    var thisRests = rests.Where(r => thisRestIds.Contains(r.RestaurantID)).ToList();

			    yield return new VendorBeverageLineItem(sourceLI, this, cats, thisRests);
		    }
	    }

        public StreetAddress StreetAddress { get; set; }

        public string EmailToOrderFrom { get; set; }

        public string Website { get; set; }

        public string VendorPhoneNumberAreaCode { get; set; }

        public string VendorPhoneNumber { get; set; }

        public Employee CreatedBy { get; set; }

        public List<VendorRestaurantRelationships> RestaurantsAssociatedWith { get; set; } 

        public List<VendorBeverageLineItem> LineItems { get; set; }

        public BusinessName Name { get; set; }

        protected override Core.Common.Entities.Vendors.Vendor CreateConcreteSubclass()
        {
            return new Core.Common.Entities.Vendors.Vendor
            {
                StreetAddress = StreetAddress.ToValueItem(),
                EmailToOrderFrom = new EmailAddress(EmailToOrderFrom),
                Website = new Uri(Website),
                PhoneNumber = new PhoneNumber(VendorPhoneNumberAreaCode, VendorPhoneNumber),
                CreatedByEmployeeID = CreatedBy?.EntityId,
                RestaurantsAssociatedIDs = RestaurantsAssociatedWith?.Select(r => r.Restaurant.RestaurantID).ToList(),
                VendorBeverageLineItems = LineItems.Select(li => li.ToBusinessEntity()).ToList(),
                VendorName = Name.ToValueItem()
            };
        }
    }
}
