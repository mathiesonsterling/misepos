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
            EntityId = entityId;
            InventoryCategory = category;
            Id = EntityId + ":" + InventoryCategory.EntityId;
        }

        public Guid EntityId { get; set; }
        public InventoryCategory InventoryCategory { get; set; }
    }
}
