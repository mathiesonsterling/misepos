using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Entities.Inventory;
using Mise.Core.Entities.Inventory.Events;

namespace Mise.Core.Common.Services.WebServices
{
	public interface IPurchaseOrderWebService : IEventStoreWebService<PurchaseOrder, IPurchaseOrderEvent>
	{
	}
}


