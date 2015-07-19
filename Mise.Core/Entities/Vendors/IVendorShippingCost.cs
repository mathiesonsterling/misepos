using System;

using Mise.Core.ValueItems;
using Mise.Core.Entities.Inventory;


namespace Mise.Core
{
	/// <summary>
	/// Represents the amount a shipment will take.  Can be negative for some orders
	/// </summary>
	public interface IVendorShippingCost
	{
		Money GetShippingAmountForRequisition(IPurchaseOrder purchaseOrder);
	}
}

