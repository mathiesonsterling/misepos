using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Entities.Inventory;
using Mise.Core.Entities.Vendors.Events;

namespace Mise.Core.Common.Services.WebServices
{
    public interface IReceivingOrderWebService : IEventStoreWebService<ReceivingOrder, IReceivingOrderEvent>
    {
        Task<IEnumerable<ReceivingOrder>> GetReceivingOrdersForRestaurant(Guid restaurantID);
    }
}
