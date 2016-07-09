using System;
using System.Collections.Generic;
using System.Linq;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Entities.Base;
using Mise.Core.Entities.Vendors;
using Mise.Core.ValueItems;

namespace Mise.Core.Common.Entities.Vendors
{
    public class VendorBeverageLineItem : BaseBeverageLineItem, IVendorBeverageLineItem
    {
        public VendorBeverageLineItem()
        {
            Categories = new List<InventoryCategory>();
            PricePerUnitForRestaurant = new Dictionary<Guid, Money>();
        }

        public Guid VendorID
        {
            get;
            set;
        }

        public string DetailDisplay
        {
            get
            {
                var res = string.Empty;
                if (Categories.Any())
                {
                    res += Categories.First().Name;
                }
                if (Container != null)
                {
                    res += "  " + Container.DisplayName;
                }
                return res;
            }
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

        //last price given to this restaurant
        public Dictionary<Guid, Money> PricePerUnitForRestaurant { get; set; }

        public Money GetLastPricePaidByRestaurantPerUnit(Guid restaurantID)
        {
            return PricePerUnitForRestaurant.ContainsKey(restaurantID)
                ? PricePerUnitForRestaurant[restaurantID]
                : null;
        }

        public Dictionary<Guid, Money> GetPricesForRestaurants()
        {
            return PricePerUnitForRestaurant;
        }

        public DateTimeOffset? LastTimePriceSet
        {
            get;
            set;
        }

        public ICloneableEntity Clone()
        {
            var newItem = CloneEntityBase(new VendorBeverageLineItem());
            newItem.MiseName = MiseName;
            newItem.VendorID = VendorID;
            newItem.Container = Container;
            newItem.UPC = UPC;
            newItem.PublicPricePerUnit = PublicPricePerUnit;
            newItem.PricePerUnitForRestaurant = new Dictionary<Guid, Money>(PricePerUnitForRestaurant);
            newItem.NameInVendor = NameInVendor;
            newItem.Categories = Categories.Select(c => c).ToList();
            return newItem;
        }

        public override decimal Quantity
        {
            get { return decimal.MaxValue; }
            set { }
        }
    }
}

