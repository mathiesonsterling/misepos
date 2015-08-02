using System;
using System.Collections.Generic;
using System.Linq;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Entities.Inventory;
using Mise.Core.ValueItems;
using Mise.Core.ValueItems.Inventory;

namespace Mise.Database.StorableEntities.Inventory
{
    public class ParLineItemGraphNode : IStorableEntityGraphNode
    {
        public ParLineItemGraphNode()
        {
            
        }

        public ParLineItemGraphNode(IParBeverageLineItem source)
        {
            ID = source.ID;
            RestaurantID = source.RestaurantID;
            CreatedDate = source.CreatedDate;
            LastUpdatedDate = source.LastUpdatedDate;
            Revision = source.Revision != null? source.Revision.ToDatabaseString() : string.Empty;

            MiseName = source.MiseName;
            DisplayName = source.DisplayName;
            Quantity = source.Quantity;
            UPC = source.UPC;
            CaseSize = source.CaseSize;
        }

        public int? CaseSize { get; set; }

        public PARBeverageLineItem Rehydrate(LiquidContainer container, IEnumerable<ItemCategory> categories)
        {
            return new PARBeverageLineItem
            {
                ID = ID,
                Revision = string.IsNullOrEmpty(Revision) == false ? new EventID(Revision) : null,
                CreatedDate = CreatedDate,
                DisplayName = DisplayName,
                LastUpdatedDate = LastUpdatedDate,
                MiseName = MiseName,
                Quantity = Quantity,
                RestaurantID = RestaurantID,
                UPC = UPC,
                Container = container,
                CaseSize = CaseSize,
                Categories = categories.ToList()
            };
        }

        public string DisplayName { get; set; }

        public string UPC { get; set; }

        public Guid RestaurantID { get; set; }

        public decimal Quantity { get; set; }

        public string MiseName { get; set; }

        public DateTimeOffset LastUpdatedDate { get; set; }

        public DateTimeOffset CreatedDate { get; set; }

        public Guid ID { get; set; }
        public string Revision { get; set; }
    }
}
