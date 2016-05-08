using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities.Vendors;
using Mise.Database.AzureDefinitions.Entities.Inventory.LineItems;
using Mise.Database.AzureDefinitions.ValueItems;


namespace Mise.Database.AzureDefinitions.Entities.Vendor
{
    public class VendorBeverageLineItem : BaseLiquidLineItemEntity<IVendorBeverageLineItem, Core.Common.Entities.Vendors.VendorBeverageLineItem>
    {
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
        public Money PublicPricePerUnit
        {
            get;
            set;
        }

        public DateTimeOffset? LastTimePriceSet
        {
            get;
            set;
        }

        public Dictionary<Guid, decimal> PricePerUnitForRestaurantInDollars { get; set; }

        protected override Core.Common.Entities.Vendors.VendorBeverageLineItem CreateConcreteLineItemClass()
        {
            var priceDic = PricePerUnitForRestaurantInDollars?.ToDictionary(kv => kv.Key,kv => new Core.ValueItems.Money(kv.Value)) 
                ?? new Dictionary<Guid, Core.ValueItems.Money>();
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
