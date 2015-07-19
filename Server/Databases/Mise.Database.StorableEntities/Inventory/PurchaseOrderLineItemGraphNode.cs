using System;
using System.Collections.Generic;
using System.Linq;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Entities.Inventory;
using Mise.Core.ValueItems;
using Mise.Core.ValueItems.Inventory;

namespace Mise.Database.StorableEntities.Inventory
{
    public class PurchaseOrderLineItemGraphNode : IStorableEntityGraphNode
    {
        public PurchaseOrderLineItemGraphNode()
        {
            
        }

        public PurchaseOrderLineItemGraphNode(IPurchaseOrderLineItem source)
        {
            ID = source.ID;
            RestaurantID = source.RestaurantID;
            CreatedDate = source.CreatedDate;
            LastUpdatedDate = source.LastUpdatedDate;
            Revision = source.Revision.ToDatabaseString();

            UPC = source.UPC;
            DisplayName = source.DisplayName;
            MiseName = source.MiseName;
            Quantity = source.Quantity;
            CaseSize = source.CaseSize;
            VendorID = source.VendorID;
        }

        public PurchaseOrderLineItem Rehydrate(LiquidContainer container, IEnumerable<ItemCategory> categories)
        {
            return new PurchaseOrderLineItem
            {
                ID = ID,
                RestaurantID = RestaurantID,
                CreatedDate = CreatedDate,
                LastUpdatedDate = LastUpdatedDate,
                Revision = new EventID(Revision),

                Container = container,
                MiseName = MiseName,
                DisplayName = DisplayName,
                UPC = UPC,
                Quantity = Quantity,
                CaseSize = CaseSize,
                VendorID = VendorID,
                Categories = categories.ToList()
            };
        }

        public int? CaseSize { get; set; }

        public string DisplayName { get; set; }

        public int Quantity { get; set; }

        public Guid? VendorID { get; set; }
        public string UPC { get; set; }

        public DateTimeOffset LastUpdatedDate { get; set; }

        public DateTimeOffset CreatedDate { get; set; }

        public Guid RestaurantID { get; set; }

        public string MiseName { get; set; }

        public Guid ID { get; set; }
        public string Revision { get; set; }
    }
}
