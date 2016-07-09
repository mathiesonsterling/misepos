using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using Mise.Core.Entities.People;
using Mise.Core.Entities.Inventory;
using Mise.Core.Entities.Vendors;
using System.Collections;
using Mise.Core.ValueItems.Inventory;

namespace Mise.Inventory.Services
{
	public interface IPurchaseOrderService
	{
		/// <summary>
		/// Creates a purchase order using our current PAR and Inventory
		/// </summary>
		/// <returns>The purchase order.</returns>
		Task<IPurchaseOrder> CreatePurchaseOrder();

		/// <summary>
		/// Sends an approved PO to the server for processing
		/// </summary>
		/// <returns>The P.</returns>
		/// <param name="order">Order.</param>
		Task SubmitPO(IPurchaseOrder order);

		Task<IEnumerable<IPurchaseOrder>> GetPurchaseOrdersWaitingForVendor (IVendor vendor);

		/// <summary>
		/// Tell us if all items in the PO have been delivered
		/// </summary>
		/// <returns>true if all the items requested are there, false otherwise</returns>
		Task<bool> IsPurchaseOrderTotallyFufilledByReceivingOrder (IReceivingOrder ro);

		/// <summary>
		/// Marks the items given as received, and gives the vendor portion the correct status
		/// </summary>
		Task MarkItemsAsReceivedForPurchaseOrder (IReceivingOrder ro, PurchaseOrderStatus status);
	}
}

