using System;
using System.Collections.Generic;
using System.Linq;
using Mise.Core.Entities.Vendors;
using Mise.Core.ValueItems;
using Mise.Core.Client.Entities.Categories;
using Mise.Core.Client.Entities.Inventory.LineItems;
using Mise.Core.Client.ValueItems;

namespace Mise.Core.Client.Entities.Vendor
{
    public class VendorBeverageLineItem : BaseLiquidLineItemEntity<IVendorBeverageLineItem, Core.Common.Entities.Vendors.VendorBeverageLineItem>
    {
        public VendorBeverageLineItem()
        {
            PublicPricePerUnit = new MoneyDb();
        }

	    public VendorBeverageLineItem(IVendorBeverageLineItem source, Vendor vendor,
		    IEnumerable<InventoryCategory> cats)
		    : base(source, cats)
	    {
		    Vendor = vendor;
		    NameOfItemInVendor = source.NameInVendor;
		    PublicPricePerUnit = source.PublicPricePerUnit != null 
                ? new MoneyDb(source.PublicPricePerUnit) 
                : new MoneyDb();
           
	    }

        public Vendor Vendor
        {
            get;
            set;
        }

        public string NameOfItemInVendor
        {
            get;
            set;
        }


        //the price published per unit
        public MoneyDb PublicPricePerUnit
        {
            get;
            set;
        }

        public List<VendorPrivateRestaurantPrice> VendorPrivateRestaurantPrices { get; set; }

        protected override Core.Common.Entities.Vendors.VendorBeverageLineItem CreateConcreteLineItemClass()
        {
           var priceDic = VendorPrivateRestaurantPrices?.ToDictionary(kv => kv.Restaurant.RestaurantID,
              kv => kv.PriceCharged.ToValueItem()) ?? new Dictionary<Guid, Money>();
            return new Core.Common.Entities.Vendors.VendorBeverageLineItem
            {
                VendorID = Vendor.EntityId,
                NameInVendor = NameOfItemInVendor,
                PublicPricePerUnit = PublicPricePerUnit.ToValueItem(),
                LastTimePriceSet = VendorPrivateRestaurantPrices?.Where(p => p.UpdatedAt.HasValue).Max(p => p.UpdatedAt),
                    
                PricePerUnitForRestaurant = priceDic
            };
        }
    }
}
