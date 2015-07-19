using System;
using System.Collections.Generic;
using System.Linq;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Entities.Inventory;
using Mise.Core.ValueItems;
using Mise.Core.ValueItems.Inventory;

namespace Mise.Database.StorableEntities.Inventory
{
    public class PARLineItemGraphNode : IStorableEntityGraphNode
    {
        public PARLineItemGraphNode()
        {
            
        }

        public PARLineItemGraphNode(IPARBeverageLineItem source)
        {
            ID = source.ID;
            RestaurantID = source.RestaurantID;
            CreatedDate = source.CreatedDate;
            LastUpdatedDate = source.LastUpdatedDate;
            Revision = source.Revision.ToDatabaseString();

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
                Revision = new EventID(Revision),
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

        public int Quantity { get; set; }

        public string MiseName { get; set; }

        public DateTimeOffset LastUpdatedDate { get; set; }

        public DateTimeOffset CreatedDate { get; set; }

        public Guid ID { get; set; }
        public string Revision { get; set; }
    }
}
