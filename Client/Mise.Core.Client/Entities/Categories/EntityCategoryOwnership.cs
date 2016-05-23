using System;
using Mise.Core.Client.Entities.Categories;
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

        public InventoryCategory InventoryCategory { get; set; }
    }
}
