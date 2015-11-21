using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Entities.Inventory;
using Mise.Core.Entities.Inventory.Events;

namespace Mise.Core.Common.Services.WebServices
{
	public interface IPurchaseOrderWebService : IEventStoreWebService<PurchaseOrder, IPurchaseOrderEvent>
	{
        Task<IEnumerable<IPurchaseOrder>> GetPurchaseOrdersForRestaurant(Guid restaurantId);
	}
}


