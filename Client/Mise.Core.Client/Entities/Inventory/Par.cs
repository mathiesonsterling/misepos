using System.Collections.Generic;
using System.Linq;
using Mise.Core.Client.Entities.Categories;
using Mise.Core.Client.Entities.Inventory.LineItems;
using Mise.Core.Client.Entities.People;
using Mise.Core.Entities.Inventory;

namespace Mise.Core.Client.Entities.Inventory
{
    public class Par : BaseDbRestaurantEntity<IPar, Core.Common.Entities.Inventory.Par>
    {
        public Par() { }

        public Par(IPar source, Restaurant.Restaurant restaurant, Employee createdByEmployee) : base(source)
        {
            Restaurant = restaurant;
            RestaurantId = restaurant.Id;
            CreatedByEmployee = createdByEmployee;
            CreatedByEmployeeId = createdByEmployee?.Id;
            IsCurrent = source.IsCurrent;
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
        public string CreatedByEmployeeId { get; set; }
        public bool IsCurrent { get; set; }

        public List<ParBeverageLineItem> ParLineItems { get; set; }
    }
}
