using System;
using System.Collections.Generic;
using System.Linq;
using Mise.Core.Entities.Inventory;
using Mise.Database.AzureDefinitions.Entities.Inventory.LineItems;

namespace Mise.Database.AzureDefinitions.Entities.Inventory
{
    public class Par : BaseDbEntity<IPar, Core.Common.Entities.Inventory.Par>
    {
        public Restaurant.Restaurant Restaurant { get; set; }

        protected override Core.Common.Entities.Inventory.Par CreateConcreteSubclass()
        {
            return new Core.Common.Entities.Inventory.Par
            {
                CreatedByEmployeeID = CreatedByEmployeeID,
                IsCurrent = IsCurrent,
                ParLineItems = ParLineItems.Select(pl => pl.ToBusinessEntity()).ToList(),
                RestaurantID = Restaurant.RestaurantID
            };
        }

        public Guid CreatedByEmployeeID { get; set; }
        public bool IsCurrent { get; set; }

        public List<ParBeverageLineItem> ParLineItems { get; set; }
    }
}
