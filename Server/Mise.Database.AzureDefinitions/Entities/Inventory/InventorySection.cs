using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities.Inventory;

namespace Mise.Database.AzureDefinitions.Entities.Inventory
{
    public class InventorySection : BaseDbEntity<IInventorySection, Core.Common.Entities.Inventory.InventorySection>
    {
        protected override Core.Common.Entities.Inventory.InventorySection CreateConcreteSubclass()
        {
            return new Core.Common.Entities.Inventory.InventorySection
            {
                InventoryID = InventoryID,
                Name = Name,
                LastCompletedBy = LastCompletedBy,
                CurrentlyInUseBy = CurrentlyInUseBy,
                TimeCountStarted = TimeCountStarted,
                RestaurantInventorySectionID = RestaurantInventorySectionID,
                LineItems = LineItems.Select(li => li.ToBusinessEntity()).ToList()
            };
        }

        public Guid InventoryID
        {
            get;
            set;
        }

        public string Name { get; set; }

        public Guid? LastCompletedBy { get; set; }

        public Guid? CurrentlyInUseBy
        {
            get;
            set;
        }

        public DateTimeOffset? TimeCountStarted { get; set; }

        public Guid RestaurantInventorySectionID { get; set; }

        public List<InventoryBeverageLineItem> LineItems { get; set; }
    }
}
