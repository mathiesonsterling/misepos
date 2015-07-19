using System;
using Mise.Core.Services.WebServices;
using Mise.Core.Entities.Inventory.Events;
using Mise.Core.Entities.Inventory;

namespace Mise.Core.Services.WebServices
{
	public interface IPurchaseOrderWebService : IEventStoreWebService<IPurchaseOrder, IPurchaseOrderEvent>
	{
	}
}


