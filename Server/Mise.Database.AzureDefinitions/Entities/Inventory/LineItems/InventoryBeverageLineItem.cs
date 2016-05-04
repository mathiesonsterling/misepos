using System;
using System.Collections.Generic;
using Mise.Core.Entities.Inventory;
using Mise.Core.ValueItems.Inventory;
using Mise.Database.AzureDefinitions.ValueItems;
using LiquidAmount = Mise.Database.AzureDefinitions.ValueItems.Inventory.LiquidAmount;

namespace Mise.Database.AzureDefinitions.Entities.Inventory.LineItems
{
    public class InventoryBeverageLineItem : BaseLiquidLineItemEntity<IInventoryBeverageLineItem, Core.Common.Entities.Inventory.InventoryBeverageLineItem>
    {
        public InventoryBeverageLineItem()
        {
            CurrentAmount = new LiquidAmount();
            PricePaid = new Money();
        }

        public Guid? VendorBoughtFrom { get; set; }

        public LiquidAmount CurrentAmount { get; set; }

        public Money PricePaid { get; set; }

        public List<decimal> PartialBottleListing { get; set; }
        public int NumFullBottles { get; set; }
        public MeasurementMethods MethodsMeasuredLast { get; set; }
        public int InventoryPosition { get; set; }

        protected override Core.Common.Entities.Inventory.InventoryBeverageLineItem CreateConcreteLineItemClass()
        {
            return new Core.Common.Entities.Inventory.InventoryBeverageLineItem
            {
                CurrentAmount = CurrentAmount.ToValueItem(),
                VendorBoughtFrom = VendorBoughtFrom,
                PricePaid = PricePaid.ToValueItem(),
                NumFullBottles = NumFullBottles,
                MethodsMeasuredLast = MethodsMeasuredLast,
                InventoryPosition = InventoryPosition,
                PartialBottleListing = PartialBottleListing
            };
        }
    }
}
