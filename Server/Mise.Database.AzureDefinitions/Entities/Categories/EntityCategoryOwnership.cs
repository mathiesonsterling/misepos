using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public InventoryCategory InventoryCategory { get; set; }
    }
}
