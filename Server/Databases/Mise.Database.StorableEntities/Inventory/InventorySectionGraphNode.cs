using System;
using System.Collections.Generic;
using System.Linq;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Entities.Inventory;
using Mise.Core.ValueItems;

namespace Mise.Database.StorableEntities.Inventory
{
    public class InventorySectionGraphNode :IStorableEntityGraphNode
    {
        public InventorySectionGraphNode()
        {
            
        }

        public InventorySectionGraphNode(IInventorySection source)
        {
            CreatedDate = source.CreatedDate;
            ID = source.ID;
            LastUpdatedDate = source.LastUpdatedDate;
            Name = source.Name;
            RestaurantID = source.RestaurantID;
            RestaurantInventorySectionID = source.RestaurantInventorySectionID;
            Revision = source.Revision.ToDatabaseString();
            LastCompletedBy = source.LastCompletedBy;
        }

        public InventorySection Rehydrate(IEnumerable<InventoryBeverageLineItem> inventoryLineItems, Guid restaurantInventorySectionID)
        {
            return new InventorySection
            {
                CreatedDate = CreatedDate,
                ID = ID,
                LastUpdatedDate = LastUpdatedDate,
                LineItems = inventoryLineItems.ToList(),
                Name = Name,
                RestaurantID = RestaurantID,
                RestaurantInventorySectionID = restaurantInventorySectionID,
                Revision = new EventID(Revision),
                LastCompletedBy = LastCompletedBy
            };
        }

        public Guid RestaurantInventorySectionID { get; set; }

        public string Revision { get; set; }

        public Guid RestaurantID { get; set; }

        public string Name { get; set; }

        public DateTimeOffset LastUpdatedDate { get; set; }

        public Guid ID { get; set; }

        public DateTimeOffset CreatedDate { get; set; }

        public Guid? LastCompletedBy { get; set; }
    }
}
