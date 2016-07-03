using System;
using System.Collections.Generic;
using System.Linq;
using Mise.Core.Client.Entities.Categories;
using Mise.Core.Client.Entities.Inventory.LineItems;
using Mise.Core.Client.Entities.People;
using Mise.Core.Entities.Inventory;

namespace Mise.Core.Client.Entities.Inventory
{
    public class InventorySection : BaseDbEntity<IInventorySection, Core.Common.Entities.Inventory.InventorySection>
    {
        public InventorySection() { }

        public InventorySection(IInventorySection source, Inventory inv, Employee lastCompletedBy, RestaurantInventorySection rSec, Employee inUseBy, 
            IEnumerable<Vendor.Vendor> vendors, IEnumerable<InventoryCategory> categories) 
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

        public string InventoryId { get; set; }

        public string Name { get; set; }

        public Employee LastCompletedBy { get; set; }
        public string LastCompletedById { get; set; }

        public Employee CurrentlyInUseBy
        {
            get;
            set;
        }
        public string CurrentlyInUseById { get; set; }

        public DateTimeOffset? TimeCountStarted { get; set; }

        public RestaurantInventorySection RestaurantInventorySection { get; set; }
        public string RestaurantInventorySectionId { get; set; }

        public List<InventoryBeverageLineItem> LineItems { get; set; }
    }
}
