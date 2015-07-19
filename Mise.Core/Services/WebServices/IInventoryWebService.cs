using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities.Inventory;
using Mise.Core.Entities.Inventory.Events;

namespace Mise.Core.Services.WebServices
{
    public interface IInventoryWebService : IEventStoreWebService<IInventory, IInventoryEvent>
    {
        Task<IEnumerable<IInventory>> GetInventoriesForRestaurant(Guid restaurantID);
    }
}
