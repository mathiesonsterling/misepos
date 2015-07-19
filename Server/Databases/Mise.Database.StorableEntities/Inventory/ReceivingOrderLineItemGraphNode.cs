using System;
using System.Collections.Generic;
using System.Linq;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Entities.Inventory;
using Mise.Core.ValueItems;
using Mise.Core.ValueItems.Inventory;

namespace Mise.Database.StorableEntities.Inventory
{
    public class ReceivingOrderLineItemGraphNode : IStorableEntityGraphNode
    {
        public ReceivingOrderLineItemGraphNode()
        {
            
        }

        public ReceivingOrderLineItemGraphNode(IReceivingOrderLineItem source)
        {
            ID = source.ID;
            Revision = source.Revision.ToDatabaseString();
            CreatedDate = source.CreatedDate;
            LastUpdatedDate = source.LastUpdatedDate;
            MiseName = source.MiseName;
            UPC = source.UPC;
            RestaurantID = source.RestaurantID;
            Quantity = source.Quantity;
            CaseSize = source.CaseSize;
            LineItemPrice = source.LineItemPrice != null ? source.LineItemPrice.Dollars : 0;
            UnitPrice = source.UnitPrice != null ? source.UnitPrice.Dollars : 0;
            DisplayName = source.DisplayName;
            ZeroedOut = source.ZeroedOut;
        }


        public ReceivingOrderLineItem Rehydrate(LiquidContainer container, IEnumerable<ItemCategory> categories)
        {
            return new ReceivingOrderLineItem
            {
                ID = ID,
                CreatedDate = CreatedDate,
                LastUpdatedDate = LastUpdatedDate,
                MiseName = MiseName,
                UPC = UPC,
                LineItemPrice = new Money(LineItemPrice),
                UnitPrice = new Money(UnitPrice),
                Quantity = Quantity,
                RestaurantID = RestaurantID,
                Revision = new EventID(Revision),
                DisplayName = DisplayName,
                CaseSize = CaseSize,
                Container = container,
                ZeroedOut = ZeroedOut,
                Categories = categories.ToList()
            };
        }

        public int? CaseSize { get; set; }

        public string DisplayName { get; set; }

        public int Quantity { get; set; }

        public decimal LineItemPrice { get; set; }
        public decimal UnitPrice { get; set; }

        public Guid RestaurantID { get; set; }

        public string UPC { get; set; }

        public string MiseName { get; set; }

        public DateTimeOffset LastUpdatedDate { get; set; }

        public DateTimeOffset CreatedDate { get; set; }

        public Guid ID { get; set; }
        public string Revision { get; set; }

        public bool ZeroedOut { get; set; }
    }
}
