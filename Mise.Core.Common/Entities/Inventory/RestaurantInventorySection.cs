using Mise.Core.Entities;
using Mise.Core.Entities.Base;
using Mise.Core.Entities.Inventory;
using Mise.Core.ValueItems;

namespace Mise.Core.Common.Entities.Inventory
{
    public class RestaurantInventorySection : RestaurantEntityBase, IRestaurantInventorySection
    {
        public string Name { get; set; }
        public bool AllowsPartialBottles { get; set; }

        public bool IsDefaultInventorySection { get; set; }

        public ICloneableEntity Clone()
        {
            return new RestaurantInventorySection
            {
                Id = Id,
                LastUpdatedDate = LastUpdatedDate,
                CreatedDate = CreatedDate,
                RestaurantID = RestaurantID,
                Revision = Revision,

                Name = Name,
                AllowsPartialBottles = AllowsPartialBottles,
                IsDefaultInventorySection = IsDefaultInventorySection
            };
        }
    }
}
