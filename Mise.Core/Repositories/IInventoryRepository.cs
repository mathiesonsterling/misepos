using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities.Inventory;
using Mise.Core.Entities.Inventory.Events;

namespace Mise.Core.Repositories
{
    public interface IInventoryRepository : IEventSourcedEntityRepository<IInventory, IInventoryEvent>
    {
        /// <summary>
        /// This gets the inventory that keeps a running total of what's on hand
        /// </summary>
        /// <returns></returns>
        Task<IInventory> GetCurrentInventory(Guid restaurantID);
    }
}
