using System;
using System.Collections.Generic;
using System.Linq;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Entities.Inventory;
using Mise.Core.ValueItems;
using Mise.Core.ValueItems.Inventory;

namespace Mise.Database.StorableEntities.Inventory
{
    public class InventoryBeverageLineItemGraphNode : IStorableEntityGraphNode
    {
        public InventoryBeverageLineItemGraphNode(IInventoryBeverageLineItem source)
        {
            ID = source.ID;
            RestaurantID = source.RestaurantID;
            CreatedDate = source.CreatedDate;
            LastUpdatedDate = source.LastUpdatedDate;
            Revision = source.Revision.ToDatabaseString();

            UPC = source.UPC;
            MeasurementMethods = (int) source.MethodsMeasuredLast;
            MiseName = source.MiseName;
            CurrentAmountMilliliters = source.CurrentAmount.Milliliters;
            SpecificGravity = source.CurrentAmount.SpecificGravity;
            DisplayName = source.DisplayName;
            VendorBoughtFrom = source.VendorBoughtFrom;
            PricePaid = source.PricePaid.Dollars;
            NumFullBottles = source.NumFullBottles;
            PartialBottlePercentages = source.PartialBottlePercentages.ToList();
            CaseSize = source.CaseSize;
            InventoryPosition = source.InventoryPosition;
        }


        public InventoryBeverageLineItemGraphNode()
        {
            
        }

        public InventoryBeverageLineItem Rehydrate(LiquidContainer container, IEnumerable<ItemCategory> categories)
        {
            return new InventoryBeverageLineItem
            {
                ID = ID,
                RestaurantID = RestaurantID,
                CreatedDate = CreatedDate,
                LastUpdatedDate = LastUpdatedDate,
                Revision = new EventID(Revision),

                Container = container,
                UPC = UPC,
                MethodsMeasuredLast = (MeasurementMethods)MeasurementMethods,
                MiseName = MiseName,
                CurrentAmount = new LiquidAmount { Milliliters = CurrentAmountMilliliters, SpecificGravity = SpecificGravity},
                DisplayName = DisplayName,
                VendorBoughtFrom = VendorBoughtFrom,
                PricePaid = new Money { Dollars = PricePaid},
                NumFullBottles = NumFullBottles,
                PartialBottleListing = PartialBottlePercentages,
                CaseSize = CaseSize,
                InventoryPosition = InventoryPosition,
                Categories = categories.ToList()
            };
        }

        public int? CaseSize { get; set; }

        public int NumFullBottles { get; set; }
        public int InventoryPosition { get; set; }

        public List<decimal> PartialBottlePercentages { get; set; } 

        public decimal PricePaid { get; set; }

        public Guid? VendorBoughtFrom { get; set; }

        public string DisplayName { get; set; }

        public string MiseName { get; set; }

        public decimal? SpecificGravity { get; set; }

        public decimal CurrentAmountMilliliters { get; set; }

        public Guid ID { get; set; }
        public string Revision { get; set; }

        public string UPC { get; set; }

        public DateTimeOffset LastUpdatedDate { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public Guid RestaurantID { get; set; }
        public int MeasurementMethods { get; set; }

    }
}
