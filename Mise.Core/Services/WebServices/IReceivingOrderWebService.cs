using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities.Inventory;
using Mise.Core.Entities.Vendors.Events;

namespace Mise.Core.Services.WebServices
{
    public interface IReceivingOrderWebService : IEventStoreWebService<IReceivingOrder, IReceivingOrderEvent>
    {
        Task<IEnumerable<IReceivingOrder>> GetReceivingOrdersForRestaurant(Guid restaurantID);
    }
}
