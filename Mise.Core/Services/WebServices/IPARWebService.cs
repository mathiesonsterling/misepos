using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities.Inventory;
using Mise.Core.Entities.Inventory.Events;

namespace Mise.Core.Services.WebServices
{
    public interface IPARWebService : IEventStoreWebService<IPAR, IPAREvent>
    {
        /// <summary>
        /// Get the current PAR for this restaurant
        /// </summary>
        /// <returns></returns>
		Task<IPAR> GetCurrentPAR(Guid restaurantID);
		Task<IEnumerable<IPAR>> GetPARsForRestaurant(Guid restaurantID);
    }
}
