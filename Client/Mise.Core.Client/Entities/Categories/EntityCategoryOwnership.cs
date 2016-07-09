using System;

namespace Mise.Core.Client.Entities.Categories
{
    public class EntityCategoryOwnership : EntityData
    {
        public EntityCategoryOwnership() { }

        public EntityCategoryOwnership(Guid entityId, InventoryCategory category)
        {
            Entity = entityId;
            InventoryCategory = category;
            Id = Entity + ":" + InventoryCategory.EntityId;
        }

        public Guid Entity { get; set; }

	    public string InventoryCategoryId { get; set; }

        public InventoryCategory InventoryCategory { get; set; }
    }
}
