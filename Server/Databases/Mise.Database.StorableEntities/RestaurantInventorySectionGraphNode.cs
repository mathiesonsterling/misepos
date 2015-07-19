using System;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Entities.Inventory;
using Mise.Core.ValueItems;

namespace Mise.Database.StorableEntities
{
    public class RestaurantInventorySectionGraphNode : IStorableEntityGraphNode
    {
        public RestaurantInventorySectionGraphNode() { }

        public RestaurantInventorySectionGraphNode(IRestaurantInventorySection source)
        {
            CreatedDate = source.CreatedDate;
            ID = source.ID;
            LastUpdatedDate = source.LastUpdatedDate;
            Name = source.Name;
            RestaurantID = source.RestaurantID;
            Revision = source.Revision.ToDatabaseString();
            AllowsPartialBottles = source.AllowsPartialBottles;
        }
        public RestaurantInventorySection Rehydrate(Beacon beacon)
        {
            return new RestaurantInventorySection
            {
                Beacon = beacon,
                CreatedDate = CreatedDate,
                ID = ID,
                LastUpdatedDate = LastUpdatedDate,
                Name = Name,
                RestaurantID = RestaurantID,
                AllowsPartialBottles = AllowsPartialBottles,
                Revision = new EventID(Revision)
            };
        }

        public bool AllowsPartialBottles { get; set; }

        public string Revision { get; set; }

        public Guid RestaurantID { get; set; }

        public string Name { get; set; }

        public DateTimeOffset LastUpdatedDate { get; set; }

        public Guid ID { get; set; }

        public DateTimeOffset CreatedDate { get; set; }
    }
}
