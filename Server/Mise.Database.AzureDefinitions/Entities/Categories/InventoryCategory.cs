using System;
using System.Collections.Generic;
using Mise.Core.Entities.Inventory;
using Mise.Database.AzureDefinitions.ValueItems.Inventory;

namespace Mise.Database.AzureDefinitions.Entities.Categories
{
    public class InventoryCategory : BaseDbEntity<IInventoryCategory, Core.Common.Entities.Inventory.InventoryCategory>
    {
        protected override Core.Common.Entities.Inventory.InventoryCategory CreateConcreteSubclass()
        {
            return new Core.Common.Entities.Inventory.InventoryCategory
            {
                ParentCategoryID = ParentCategoryID,
                Name = Name,
                IsCustomCategory = IsCustomCategory,
                IsAssignable = IsAssignable,
                PreferredContainers =
                    new List<Core.ValueItems.Inventory.LiquidContainer> {PreferredContainer.ToValueItem()}
            };
        }

        public Guid? ParentCategoryID
        {
            get;
            set;
        }
        public string Name
        {
            get;
            set;
        }
        public bool IsCustomCategory
        {
            get;
            set;
        }

        public bool IsAssignable { get; set; }

        public LiquidContainer PreferredContainer { get; set; }
    }
}
