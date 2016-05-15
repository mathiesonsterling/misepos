using System;
using System.Collections.Generic;
using System.Linq;
using Mise.Core.Entities.Vendors;
using Mise.Core.ValueItems;
using Mise.Database.AzureDefinitions.Entities.Categories;
using Mise.Database.AzureDefinitions.Entities.Inventory.LineItems;
using Mise.Database.AzureDefinitions.ValueItems;

namespace Mise.Database.AzureDefinitions.Entities.Vendor
{
    public class VendorBeverageLineItem : BaseLiquidLineItemEntity<IVendorBeverageLineItem, Core.Common.Entities.Vendors.VendorBeverageLineItem>
    {
        public VendorBeverageLineItem()
        {
            PublicPricePerUnit = new MoneyDb();
        }

	    public VendorBeverageLineItem(IVendorBeverageLineItem source, Vendor vendor,
		    IEnumerable<InventoryCategory> cats, IEnumerable<Restaurant.Restaurant> rests)
		    : base(source, cats)
	    {
		    Vendor = vendor;
		    NameInVendor = source.NameInVendor;
		    PublicPricePerUnit = source.PublicPricePerUnit != null 
                ? new MoneyDb(source.PublicPricePerUnit) 
                : new MoneyDb();
		    LastTimePriceSet = source.LastTimePriceSet;

		    var prices = (
                from kv in source.GetPricesForRestaurants()
                let rest = rests.FirstOrDefault(r => r.RestaurantID == kv.Key)
                where rest != null
                select new VendorPrivateRestaurantPrice(this, rest, new MoneyDb(kv.Value))
            ).ToList();

	        PrivateRestaurantPrices = prices;
	    }

        public Vendor Vendor
        {
            get;
            set;
        }

        public string NameInVendor
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

        public DateTimeOffset? LastTimePriceSet
        {
            get;
            set;
        }

        public List<VendorPrivateRestaurantPrice> PrivateRestaurantPrices { get; set; }

        protected override Core.Common.Entities.Vendors.VendorBeverageLineItem CreateConcreteLineItemClass()
        {
          var priceDic = PrivateRestaurantPrices?.ToDictionary(kv => kv.Restaurant.RestaurantID,
              kv => kv.PriceCharged.ToValueItem()) ?? new Dictionary<Guid, Money>();
            return new Core.Common.Entities.Vendors.VendorBeverageLineItem
            {
                VendorID = Vendor.EntityId,
                NameInVendor = NameInVendor,
                PublicPricePerUnit = PublicPricePerUnit.ToValueItem(),
                LastTimePriceSet = LastTimePriceSet,
                PricePerUnitForRestaurant = priceDic
            };
        }
    }
}
