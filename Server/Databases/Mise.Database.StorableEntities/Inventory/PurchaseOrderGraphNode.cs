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
    public class PurchaseOrderGraphNode :IStorableEntityGraphNode
    {
        public PurchaseOrderGraphNode()
        {
            
        }

        public PurchaseOrderGraphNode(IPurchaseOrder source)
        {
            ID = source.ID;
            RestaurantID = source.RestaurantID;
            CreatedDate = source.CreatedDate;
            LastUpdatedDate = source.LastUpdatedDate;
            Revision = source.Revision.ToDatabaseString();
            CreatedByEmployeeID = source.CreatedByEmployeeID;
            CreatedByName = source.CreatedByName;
        }

        public IPurchaseOrder Rehydrate(IEnumerable<PurchaseOrderPerVendor> perVendors)
        {
            return new PurchaseOrder
            {
                CreatedByEmployeeID = CreatedByEmployeeID,
                RestaurantID = RestaurantID,
                CreatedDate = CreatedDate,
                ID = ID,
                LastUpdatedDate = LastUpdatedDate,
                Revision = new EventID(Revision),
                CreatedByName = CreatedByName,
                PurchaseOrdersPerVendor = perVendors.ToList()
            };
        }

        public string CreatedByName { get; set; }

        public PurchaseOrderStatus Status { get; set; }

        public DateTimeOffset LastUpdatedDate { get; set; }

        public DateTimeOffset CreatedDate { get; set; }

        public Guid RestaurantID { get; set; }

        public Guid CreatedByEmployeeID { get; set; }

        public Guid ID { get; set; }
        public string Revision { get; set; }
    }
}
