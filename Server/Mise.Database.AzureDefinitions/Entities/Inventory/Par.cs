﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Mise.Core.Entities.Inventory;
using Mise.Database.AzureDefinitions.Entities.Inventory.LineItems;
using Mise.Database.AzureDefinitions.Entities.People;

namespace Mise.Database.AzureDefinitions.Entities.Inventory
{
    public class Par : BaseDbEntity<IPar, Core.Common.Entities.Inventory.Par>
    {
        public Par() { }

        public Par(IPar source, Restaurant.Restaurant restaurant, Employee createdByEmployee, IEnumerable<Categories.InventoryCategory> categories) : base(source)
        {
            Restaurant = restaurant;
            RestaurantId = restaurant.Id;
            CreatedByEmployee = createdByEmployee;
            CreatedByEmployeeId = createdByEmployee?.Id;
            IsCurrent = source.IsCurrent;
            ParLineItems = source.GetBeverageLineItems().Select(li => new ParBeverageLineItem(li, this, categories)).ToList();
        }

        public Restaurant.Restaurant Restaurant { get; set; }
        [ForeignKey("Restaurant")]
        public string RestaurantId { get; set; }

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
        [ForeignKey("CreatedByEmployee")]
        public string CreatedByEmployeeId { get; set; }
        public bool IsCurrent { get; set; }

        public List<ParBeverageLineItem> ParLineItems { get; set; }
    }
}
