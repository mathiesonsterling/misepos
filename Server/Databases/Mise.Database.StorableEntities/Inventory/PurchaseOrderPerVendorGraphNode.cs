using System;
using System.Collections.Generic;
using System.Linq;
using Mise.Core.Common;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Entities.Inventory;
using Mise.Core.ValueItems;
using Mise.Core.ValueItems.Inventory;

namespace Mise.Database.StorableEntities.Inventory
{
    public class PurchaseOrderPerVendorGraphNode : IStorableEntityGraphNode
    {
        public PurchaseOrderPerVendorGraphNode() { }

        public PurchaseOrderPerVendorGraphNode(IPurchaseOrderPerVendor source)
        {
            ID = source.ID;
            CreatedDate = source.CreatedDate;
            LastUpdatedDate = source.LastUpdatedDate;
            RestaurantID = source.RestaurantID;
            Revision = source.Revision.ToDatabaseString();

            Status = source.Status.ToString();
            VendorID = source.VendorID;
        }

        public PurchaseOrderPerVendor Rehydrate(IEnumerable<PurchaseOrderLineItem> lineItems)
        {
            return new PurchaseOrderPerVendor
            {
                CreatedDate = CreatedDate,
                ID = ID,
                LastUpdatedDate = LastUpdatedDate,
                RestaurantID = RestaurantID,
                Revision = new EventID(Revision),
                Status = (PurchaseOrderStatus) Enum.Parse(typeof (PurchaseOrderStatus), Status, true),
                LineItems = lineItems.ToList(),
                VendorID = VendorID
            };
        }
        public string Status { get; set; }
        public Guid? VendorID { get; set; }

        public DateTimeOffset LastUpdatedDate { get; set; }

        public DateTimeOffset CreatedDate { get; set; }

        public Guid RestaurantID { get; set; }

        public Guid CreatedByEmployeeID { get; set; }

        public Guid ID { get; set; }
        public string Revision { get; set; }
    }
}
