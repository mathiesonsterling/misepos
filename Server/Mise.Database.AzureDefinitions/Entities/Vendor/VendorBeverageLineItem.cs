using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
		    IEnumerable<InventoryCategory> cats, ICollection<Restaurant.Restaurant> rests)
		    : base(source, cats)
	    {
		    Vendor = vendor;
		    NameInVendor = source.NameInVendor;
		    PublicPricePerUnit = new MoneyDb(source.PublicPricePerUnit);
		    LastTimePriceSet = source.LastTimePriceSet;

		    var prices = new List<VendorPrivateRestaurantPrice>();
		    foreach (var restList in source.GetPricesForRestaurants())
		    {
			    var rest = rests.FirstOrDefault(r => r.RestaurantID == restList.Key);
			    if (rest != null)
			    {
				    var allMoneys = restList.Value.Select(
					    m => new VendorPrivateRestaurantPrice(this, rest, new MoneyDb(m)));

				    foreach (var pPrice in allMoneys)
				    {
					    prices.Add(pPrice);
				    }
			    }
		    }

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
          var priceDic = PrivateRestaurantPrices != null
            ? PrivateRestaurantPrices.ToDictionary(kv => kv.Restaurant.RestaurantID,
              kv => kv.PriceCharged.ToValueItem())
            : new Dictionary<Guid, Money>();
            return new Core.Common.Entities.Vendors.VendorBeverageLineItem
            {
                VendorID = VendorID,
                NameInVendor = NameInVendor,
                PublicPricePerUnit = PublicPricePerUnit.ToValueItem(),
                LastTimePriceSet = LastTimePriceSet,
                PricePerUnitForRestaurant = priceDic
            };
        }
    }
}
