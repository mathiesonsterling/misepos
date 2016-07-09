using System;
using System.Collections.Generic;
using System.Linq;
using Mise.Core.Client.Entities.Categories;
using Mise.Core.Client.Entities.Inventory.LineItems;
using Mise.Core.Entities.Vendors;
using Mise.Core.ValueItems;

namespace Mise.Core.Client.Entities.Vendor
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

        public Core.Client.Entities.Vendor.Vendor Vendor
        {
            get;
            set;
        }

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
