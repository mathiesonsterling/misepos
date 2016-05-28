using System;
using System.Collections.Generic;
using System.Linq;
using Mise.Core.Entities.Inventory;
using Mise.Core.Client.Entities.Inventory.LineItems;
using Mise.Core.Client.Entities.People;

namespace Mise.Core.Client.Entities.Inventory
{
    public class Par : BaseDbEntity<IPar, Core.Common.Entities.Inventory.Par>
    {
        public Par() { }

        public Par(IPar source, Restaurant.Restaurant restaurant, Employee createdByEmployee, IEnumerable<Categories.InventoryCategory> categories) : base(source)
        {
            Restaurant = restaurant;
            CreatedByEmployee = createdByEmployee;
            IsCurrent = source.IsCurrent;
            ParLineItems = source.GetBeverageLineItems().Select(li => new ParBeverageLineItem(li, categories)).ToList();
        }

        public Restaurant.Restaurant Restaurant { get; set; }

        protected override Core.Common.Entities.Inventory.Par CreateConcreteSubclass()
        {
            return new Core.Common.Entities.Inventory.Par
            {
                CreatedByEmployeeID = CreatedByEmployee.EntityId,
                IsCurrent = IsCurrent,
                ParLineItems = ParLineItems.Select(pl => pl.ToBusinessEntity()).ToList(),
                RestaurantID = Restaurant.RestaurantID
            };
        }

        public Employee CreatedByEmployee { get; set; }

        public bool IsCurrent { get; set; }

        public List<ParBeverageLineItem> ParLineItems { get; set; }
    }
}
