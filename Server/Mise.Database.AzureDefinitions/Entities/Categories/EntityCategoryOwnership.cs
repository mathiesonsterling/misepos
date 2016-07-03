using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Azure.Mobile.Server;

namespace Mise.Database.AzureDefinitions.Entities.Categories
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

	    [ForeignKey("InventoryCategory")]
	    public string InventoryCategoryId { get; set; }

        public InventoryCategory InventoryCategory { get; set; }
    }
}
