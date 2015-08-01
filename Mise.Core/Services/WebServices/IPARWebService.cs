using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities.Inventory;
using Mise.Core.Entities.Inventory.Events;

namespace Mise.Core.Services.WebServices
{
    public interface IPARWebService : IEventStoreWebService<IPar, IPAREvent>
    {
        /// <summary>
        /// Get the current PAR for this restaurant
        /// </summary>
        /// <returns></returns>
		Task<IPar> GetCurrentPAR(Guid restaurantID);
		Task<IEnumerable<IPar>> GetPARsForRestaurant(Guid restaurantID);
    }
}
