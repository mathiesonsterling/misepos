using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities.Inventory;

namespace MiseReporting.Repository
{
    public interface IInventoryRepository
    {
        Task<IEnumerable<IInventory>> GetAllForRestaurant(Guid restaurantId);
        Task<IInventory> GetById(Guid inventoryId);
    }
}
