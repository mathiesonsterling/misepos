using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
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
        public InventorySection() { }

        public InventorySection(IInventorySection source, Inventory inv, Employee lastCompletedBy, RestaurantInventorySection rSec, Employee inUseBy, 
            IEnumerable<Vendor.Vendor> vendors, IEnumerable<Categories.InventoryCategory> categories) 
            : base(source)
        {
            Inventory = inv;
            InventoryId = inv.Id;
            Name = source.Name;
            LastCompletedBy = lastCompletedBy;
            LastCompletedById = lastCompletedBy?.Id;

            RestaurantInventorySection = rSec;
            RestaurantInventorySectionId = rSec?.Id;

            CurrentlyInUseBy = inUseBy;
            CurrentlyInUseById = inUseBy?.Id;

            TimeCountStarted = source.TimeCountStarted;

            LineItems = source.GetInventoryBeverageLineItemsInSection().Select(li => new InventoryBeverageLineItem(li, this, vendors, categories)).ToList();
        }
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

	    [ForeignKey("Inventory")]
        public string InventoryId { get; set; }

        public string Name { get; set; }

        public Employee LastCompletedBy { get; set; }
        [ForeignKey("LastCompletedBy")]
        public string LastCompletedById { get; set; }

        public Employee CurrentlyInUseBy
        {
            get;
            set;
        }
        [ForeignKey("CurrentlyInUseBy")]
        public string CurrentlyInUseById { get; set; }

        public DateTimeOffset? TimeCountStarted { get; set; }

        public RestaurantInventorySection RestaurantInventorySection { get; set; }
        [ForeignKey("RestaurantInventorySection")]
        public string RestaurantInventorySectionId { get; set; }

        public List<InventoryBeverageLineItem> LineItems { get; set; }
    }
}
