using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities.Inventory;

namespace Mise.Database.AzureDefinitions.Entities.Inventory
{
    public class Inventory : BaseDbEntity<IInventory, Core.Common.Entities.Inventory.Inventory>
    {
        public Restaurant.Restaurant Restaurant { get; set; }

        protected override Core.Common.Entities.Inventory.Inventory CreateConcreteSubclass()
        {
            return new Core.Common.Entities.Inventory.Inventory
            {
                RestaurantID = Restaurant.RestaurantID,
                CreatedByEmployeeID = CreatedByEmployeeID,
                IsCurrent = IsCurrent,
                DateCompleted = DateCompleted,
                Sections = Sections.Select(s => s.ToBusinessEntity()).ToList()
            };
        }

        public List<InventorySection> Sections { get; set; }

        public DateTimeOffset? DateCompleted { get; set; }

        public Guid CreatedByEmployeeID { get; set; }


        public bool IsCurrent { get; set; }

    }
}
