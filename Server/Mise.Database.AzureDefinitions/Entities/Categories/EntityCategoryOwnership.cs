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
        public Guid EntityId { get; set; }
        public InventoryCategory InventoryCategory { get; set; }
    }
}
