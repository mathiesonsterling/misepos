using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Entities.Inventory;
using Mise.Core.Entities.Inventory.Events;

namespace Mise.Core.Common.Services.WebServices
{
    public interface IInventoryWebService : IEventStoreWebService<Inventory, IInventoryEvent>
    {
        Task<IEnumerable<Inventory>> GetInventoriesForRestaurant(Guid restaurantID);
    }
}
