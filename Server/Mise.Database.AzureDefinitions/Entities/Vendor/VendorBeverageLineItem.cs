using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities.Vendors;
using Mise.Core.ValueItems;
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

        public Guid VendorID
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
