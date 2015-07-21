using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Mise.Core.ValueItems.Inventory;
using Mise.Core.ValueItems;
using Mise.Core.Entities.Inventory;
using Mise.Core.Entities.Vendors;

namespace Mise.Inventory.Services{
	public interface IReceivingOrderService{

		Task<IReceivingOrder> StartReceivingOrderForSelectedVendor();
		Task<IReceivingOrder> StartReceivingOrder(IPurchaseOrder po);
		Task<IReceivingOrder> GetCurrentReceivingOrder();
		Task<bool> CompleteReceivingOrderForSelectedVendor(string notes, string invoiceID);
		/// <summary>
		/// Final commit to mark a RO as done
		/// </summary>
		/// <returns>The completed order.</returns>
		/// <param name="status">The status to give any associated POs</param>
		Task CommitCompletedOrder(PurchaseOrderStatus status);

		Task<IReceivingOrderLineItem> AddLineItemToCurrentReceivingOrder (IBaseBeverageLineItem sourceItem, int quantity);
		Task<IReceivingOrderLineItem> AddLineItemToCurrentReceivingOrder(string name, ICategory category, string upc, 
			int quantity, int caseSize, LiquidContainer container);
		Task UpdateQuantityOfLineItem(IReceivingOrderLineItem li, int newQuantity, Money price);
		Task ZeroOutLineItem (IReceivingOrderLineItem li);

	    Task SetCurrentLineItem(IReceivingOrderLineItem lineItem);
	    Task<IReceivingOrderLineItem> GetCurrentLineItem();
	}
}
