using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Mise.Core.Entities.Vendors;
using Mise.Core.ValueItems;
using Mise.Database.AzureDefinitions.Entities.Categories;
using Mise.Database.AzureDefinitions.Entities.Inventory.LineItems;

namespace Mise.Database.AzureDefinitions.Entities.Vendor
{
    public class VendorBeverageLineItem : BaseLiquidLineItemEntity<IVendorBeverageLineItem, Core.Common.Entities.Vendors.VendorBeverageLineItem>
    {
        public VendorBeverageLineItem()
        {
        }

	    public VendorBeverageLineItem(IVendorBeverageLineItem source, Vendor vendor,
		    IEnumerable<InventoryCategory> cats)
		    : base(source, cats)
	    {
		    Vendor = vendor;
		    NameOfItemInVendor = source.NameInVendor;
	        PublicPricePerUnit = source.PublicPricePerUnit?.Dollars;

	    }

        public Vendor Vendor
        {
            get;
            set;
        }

	    [ForeignKey("Vendor")]
	    public string VendorId { get; set; }
        public string NameOfItemInVendor
        {
            get;
            set;
        }


        //the price published per unit
        public decimal? PublicPricePerUnit
        {
            get;
            set;
        }

        public List<VendorPrivateRestaurantPrice> VendorPrivateRestaurantPrices { get; set; }

        protected override Core.Common.Entities.Vendors.VendorBeverageLineItem CreateConcreteLineItemClass()
        {
           var priceDic = VendorPrivateRestaurantPrices?.ToDictionary(kv => kv.Restaurant.RestaurantID,
              kv => new Money(kv.PriceCharged)) ?? new Dictionary<Guid, Money>();
            return new Core.Common.Entities.Vendors.VendorBeverageLineItem
            {
                VendorID = Vendor.EntityId,
                NameInVendor = NameOfItemInVendor,
                PublicPricePerUnit = PublicPricePerUnit.HasValue
                    ?new Money(PublicPricePerUnit.Value) 
                    :null ,
                LastTimePriceSet = VendorPrivateRestaurantPrices?.Where(p => p.UpdatedAt.HasValue).Max(p => p.UpdatedAt),
                    
                PricePerUnitForRestaurant = priceDic
            };
        }
    }
}
