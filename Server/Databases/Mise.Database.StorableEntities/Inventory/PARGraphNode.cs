using System;
using System.Collections.Generic;
using System.Linq;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Entities.Inventory;
using Mise.Core.ValueItems;

namespace Mise.Database.StorableEntities.Inventory
{
    public class PARGraphNode : IStorableEntityGraphNode
    {
        public PARGraphNode()
        {
            
        }

        public PARGraphNode(IPar source)
        {
            ID = source.ID;
            RestaurantID = source.RestaurantID;
            CreatedDate = source.CreatedDate;
            LastUpdatedDate = source.LastUpdatedDate;
            Revision = source.Revision.ToDatabaseString();

            IsCurrent = source.IsCurrent;
            CreatedByEmployeeID = source.CreatedByEmployeeID;
        }


        public IPar Rehydrate(IEnumerable<PARBeverageLineItem> beverageLineItems)
        {
            return new Par
            {
                CreatedByEmployeeID = CreatedByEmployeeID,
                CreatedDate = CreatedDate,
                ID = ID,
                Revision = new EventID(Revision),
                IsCurrent = IsCurrent,
                LastUpdatedDate = LastUpdatedDate,
                RestaurantID = RestaurantID,
                ParLineItems = beverageLineItems.ToList()
            };
        }

        public Guid RestaurantID { get; set; }

        public DateTimeOffset LastUpdatedDate { get; set; }

        public bool IsCurrent { get; set; }

        public DateTimeOffset CreatedDate { get; set; }

        public Guid CreatedByEmployeeID { get; set; }

        public Guid ID { get; set; }
        public string Revision { get; set; }
    }
}
