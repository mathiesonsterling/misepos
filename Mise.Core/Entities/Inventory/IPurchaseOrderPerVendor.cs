using System;
using Mise.Core.Entities.Base;
using System.Collections.Generic;
using Mise.Core.Entities.Inventory;
using Mise.Core.ValueItems.Inventory;
using Mise.Core.ValueItems;


namespace Mise.Core.Entities.Inventory
{
	public interface IPurchaseOrderPerVendor : ICloneableEntity, IRestaurantEntityBase, ITextSearchable
	{
		Guid? VendorID{ get;}
        string VendorName{ get; }

		IEnumerable<IPurchaseOrderLineItem> GetLineItems();

		void AddLineItem (IPurchaseOrderLineItem li);

		bool IsExpectingShipments ();

		PurchaseOrderStatus Status { get;}

		Money GetTotal();
	}
}

