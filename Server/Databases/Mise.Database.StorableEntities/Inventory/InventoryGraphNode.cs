using System;
using System.Collections.Generic;
using System.Linq;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Entities.Inventory;
using Mise.Core.ValueItems;

namespace Mise.Database.StorableEntities.Inventory
{
    public class InventoryGraphNode : IStorableEntityGraphNode
    {
        public InventoryGraphNode(IInventory source)
        {

            ID = source.ID;
            RestaurantID = source.RestaurantID;
            CreatedDate = source.CreatedDate;
            LastUpdatedDate = source.LastUpdatedDate;
            Revision = source.Revision.ToDatabaseString();

            CreatedByEmployeeID = source.CreatedByEmployeeID;
            IsCurrent = source.IsCurrent;
            DateCompleted = source.DateCompleted;
        }

        public DateTimeOffset? DateCompleted { get; set; }

        public InventoryGraphNode()
        {
            
        }

        public IInventory Rehydrate(IEnumerable<InventorySection> sections)
        {
            return new Core.Common.Entities.Inventory.Inventory
            {
                CreatedByEmployeeID = CreatedByEmployeeID,
                RestaurantID = RestaurantID,
                CreatedDate = CreatedDate,
                ID = ID,
                LastUpdatedDate = LastUpdatedDate,
                Revision = new EventID(Revision),
                DateCompleted = DateCompleted,
                Sections = sections.ToList(),
                IsCurrent = IsCurrent
            };
        }

        public bool IsCurrent { get; set; }

        public DateTimeOffset? TimePhysicallyMeasured { get; set; }

        public Guid CreatedByEmployeeID { get; set; }

        public Guid ID { get; set; }
        public string Revision { get; set; }

        public Guid RestaurantID { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset LastUpdatedDate { get; set; }
    }
}
