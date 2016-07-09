using System.Collections.Generic;
using Mise.Core.Client.Entities.Inventory;
using Mise.Core.Entities.Inventory;

namespace Mise.Core.Client.Entities.Categories
{
    public class InventoryCategory : BaseDbEntity<IInventoryCategory, Core.Common.Entities.Inventory.InventoryCategory>
    {
        public InventoryCategory() { }

        public InventoryCategory(IInventoryCategory source, LiquidContainer container) : base(source)
        {
           // ParentCategory = parent;
            Name = source.Name;
            IsCustomCategory = source.IsCustomCategory;
            IsAssignable = source.IsAssignable;
            
            PreferredContainer = container;
        }

        protected override Core.Common.Entities.Inventory.InventoryCategory CreateConcreteSubclass()
        {
            var prefContainers = new List<Core.ValueItems.Inventory.LiquidContainer>();

            if (PreferredContainer != null)
            {
                prefContainers =
                    new List<Core.ValueItems.Inventory.LiquidContainer> {new Core.ValueItems.Inventory.LiquidContainer(PreferredContainer.ToBusinessEntity())};
            }
            return new Core.Common.Entities.Inventory.InventoryCategory
            {
                ParentCategoryID = ParentCategory.EntityId,
                Name = Name,
                IsCustomCategory = IsCustomCategory,
                IsAssignable = IsAssignable,
                PreferredContainers = prefContainers
            };
        }

	    public string ParentCategoryId { get; set; }

        public InventoryCategory ParentCategory
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

        public string PreferredContainerId { get; set; }
    }
}
