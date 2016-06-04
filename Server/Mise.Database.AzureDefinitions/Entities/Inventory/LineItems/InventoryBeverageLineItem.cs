using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using Mise.Core.Entities.Inventory;
using Mise.Core.ValueItems.Inventory;
using Mise.Database.AzureDefinitions.Entities.Categories;
using Mise.Database.AzureDefinitions.ValueItems;
using LiquidAmount = Mise.Database.AzureDefinitions.ValueItems.Inventory.LiquidAmount;

namespace Mise.Database.AzureDefinitions.Entities.Inventory.LineItems
{
    public class InventoryBeverageLineItem : BaseLiquidLineItemEntity<IInventoryBeverageLineItem, Core.Common.Entities.Inventory.InventoryBeverageLineItem>
    {
        public InventoryBeverageLineItem()
        {
            CurrentAmount = new LiquidAmount();
            PricePaid = new MoneyDb();
        }

        public InventoryBeverageLineItem(IInventoryBeverageLineItem source, InventorySection section, 
            IEnumerable<Vendor.Vendor> vendors, IEnumerable<InventoryCategory> categories) 
            :base(source, categories)
        {
            VendorBoughtFrom = source.VendorBoughtFrom.HasValue
                ? vendors.FirstOrDefault(v => v.EntityId == source.VendorBoughtFrom)
                : null;

            CurrentAmount = new LiquidAmount(source.CurrentAmount);
            PricePaid = new MoneyDb(source.PricePaid);
            PartialBottleListing = ConcatePartialBottleListing(source.GetPartialBottlePercentages());
            NumFullBottles = source.NumFullBottles;
            MethodsMeasuredLast = source.MethodsMeasuredLast;
            InventoryPosition = source.InventoryPosition;
        }

        public Vendor.Vendor VendorBoughtFrom { get; set; }

        public LiquidAmount CurrentAmount { get; set; }

        public MoneyDb PricePaid { get; set; }

        public string PartialBottleListing { get; set; }
        public int NumFullBottles { get; set; }
        public MeasurementMethods MethodsMeasuredLast { get; set; }
        public int InventoryPosition { get; set; }

        public InventorySection InventorySection { get; set; }

	    [ForeignKey("InventorySection")]
        public string InventorySectionId { get; set; }

        protected override Core.Common.Entities.Inventory.InventoryBeverageLineItem CreateConcreteLineItemClass()
        {
            return new Core.Common.Entities.Inventory.InventoryBeverageLineItem
            {
                CurrentAmount = CurrentAmount.ToValueItem(),
                VendorBoughtFrom = VendorBoughtFrom?.EntityId,
                PricePaid = PricePaid.ToValueItem(),
                NumFullBottles = NumFullBottles,
                MethodsMeasuredLast = MethodsMeasuredLast,
                InventoryPosition = InventoryPosition,
                PartialBottleListing = GetPartialBottleListing(PartialBottleListing)
            };
        }

        private static List<decimal> GetPartialBottleListing(string source)
        {
            var items = source.Split(',');
            return items.Select(decimal.Parse).ToList();
        }

        private static string ConcatePartialBottleListing(IEnumerable<decimal> arr)
        {
            var strings = arr.Select(n => n.ToString(CultureInfo.InvariantCulture));
            return string.Join(",", strings);
        }
    }
}
