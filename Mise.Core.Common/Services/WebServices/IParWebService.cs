using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Entities.Inventory;
using Mise.Core.Entities.Inventory.Events;

namespace Mise.Core.Common.Services.WebServices
{
    public interface IParWebService : IEventStoreWebService<Par, IParEvent>
    {
        /// <summary>
        /// Get the current PAR for this restaurant
        /// </summary>
        /// <returns></returns>
		Task<Par> GetCurrentPAR(Guid restaurantID);
		Task<IEnumerable<Par>> GetPARsForRestaurant(Guid restaurantID);
    }
}
