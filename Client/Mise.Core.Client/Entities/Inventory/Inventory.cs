using System;
using System.Collections.Generic;
using System.Linq;
using Mise.Core.Client.Entities.Categories;
using Mise.Core.Client.Entities.People;
using Mise.Core.Entities.Inventory;

namespace Mise.Core.Client.Entities.Inventory
{
    public class Inventory : BaseDbRestaurantEntity<IInventory, Core.Common.Entities.Inventory.Inventory>
    {
        public Inventory() { }

        public Inventory(IInventory source, Restaurant.Restaurant rest, ICollection<Employee> emps) : base(source)
        {
            Restaurant = rest;
            RestaurantId = rest.Id;
            DateCompleted = source.DateCompleted;
            CreatedByEmployee = emps.FirstOrDefault(e => e.EntityId == source.CreatedByEmployeeID);
            CreatedByEmployeeId = CreatedByEmployee?.Id;

            IsCurrent = source.IsCurrent;
        }


        public Restaurant.Restaurant Restaurant { get; set; }

        public List<InventorySection> Sections { get; set; }
        public List<Guid> SectionIds { get; set; }

        public DateTimeOffset? DateCompleted { get; set; }

        public string CreatedByEmployeeId { get; set; }
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
