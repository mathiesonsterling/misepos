using System;
using System.Collections.Generic;
using System.Linq;
using Mise.Core.Entities.Inventory;
using Mise.Core.Client.Entities.Categories;
using Mise.Core.Client.Entities.People;

namespace Mise.Core.Client.Entities.Inventory
{
    public class Inventory : BaseDbEntity<IInventory, Core.Common.Entities.Inventory.Inventory>
    {
        public Inventory() { }

        public Inventory(IInventory source, Restaurant.Restaurant rest, ICollection<Employee> emps, IEnumerable<RestaurantInventorySection> rSecs,
            IEnumerable<InventoryCategory> cats, IEnumerable<Vendor.Vendor> vendors) : base(source)
        {
            Restaurant = rest;
            Sections = source.GetSections().Select(s => CreateSection(s, emps, this, rSecs, cats, vendors)).ToList();
            DateCompleted = source.DateCompleted;
            CreatedByEmployee = emps.FirstOrDefault(e => e.EntityId == source.CreatedByEmployeeID);
            IsCurrent = source.IsCurrent;
        }

        private static InventorySection CreateSection(IInventorySection source, ICollection<Employee> emps, Inventory inv, IEnumerable<RestaurantInventorySection> rSecs,
            IEnumerable<InventoryCategory> cats, IEnumerable<Vendor.Vendor> vendors)
        {
            var usingEmp = source.CurrentlyInUseBy.HasValue
                ? emps.FirstOrDefault(e => e.EntityId == source.CurrentlyInUseBy.Value)
                : null;

            var lastCompletedBy = source.LastCompletedBy.HasValue
                ? emps.FirstOrDefault(e => e.EntityId == source.LastCompletedBy.Value)
                : null;

            var rSec = rSecs.FirstOrDefault(rs => rs.EntityId == source.RestaurantInventorySectionID);
            return new InventorySection(source, inv, lastCompletedBy, rSec, usingEmp, vendors, cats);
        }

        public Restaurant.Restaurant Restaurant { get; set; }

        public List<InventorySection> Sections { get; set; }

        public DateTimeOffset? DateCompleted { get; set; }

        public Employee CreatedByEmployee { get; set; }


        public bool IsCurrent { get; set; }

        protected override Core.Common.Entities.Inventory.Inventory CreateConcreteSubclass()
        {
            return new Core.Common.Entities.Inventory.Inventory
            {
                RestaurantID = Restaurant.RestaurantID,
                CreatedByEmployeeID = CreatedByEmployee.EntityId,
                IsCurrent = IsCurrent,
                DateCompleted = DateCompleted,
                Sections = Sections.Select(s => s.ToBusinessEntity()).ToList()
            };
        }

    }
}
