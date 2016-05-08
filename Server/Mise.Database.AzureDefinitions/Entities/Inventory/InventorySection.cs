using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities.Inventory;
using Mise.Database.AzureDefinitions.Entities.Inventory.LineItems;
using Mise.Database.AzureDefinitions.Entities.People;

namespace Mise.Database.AzureDefinitions.Entities.Inventory
{
    public class InventorySection : BaseDbEntity<IInventorySection, Core.Common.Entities.Inventory.InventorySection>
    {
        protected override Core.Common.Entities.Inventory.InventorySection CreateConcreteSubclass()
        {
            return new Core.Common.Entities.Inventory.InventorySection
            {
                InventoryID = Inventory.EntityId,
                Name = Name,
                LastCompletedBy = LastCompletedBy?.EntityId,
                CurrentlyInUseBy = CurrentlyInUseBy?.EntityId,
                TimeCountStarted = TimeCountStarted,
                RestaurantInventorySectionID = RestaurantInventorySection.EntityId,
                LineItems = LineItems.Select(li => li.ToBusinessEntity()).ToList()
            };
        }

        public Inventory Inventory
        {
            get;
            set;
        }

        public string Name { get; set; }

        public Employee LastCompletedBy { get; set; }

        public Employee CurrentlyInUseBy
        {
            get;
            set;
        }

        public DateTimeOffset? TimeCountStarted { get; set; }

        public RestaurantInventorySection RestaurantInventorySection { get; set; }

        public List<InventoryBeverageLineItem> LineItems { get; set; }
    }
}
