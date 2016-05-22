using System;
using System.Collections.Generic;
using System.Linq;
using Mise.Core.Entities.Base;
using Mise.Core.Entities.Inventory;
using Mise.Core.ValueItems;
using Mise.Core.ValueItems.Inventory;

namespace Mise.Core.Common.Entities.Inventory
{
	public class PurchaseOrderPerVendor : RestaurantEntityBase, IPurchaseOrderPerVendor
	{
		public Guid? VendorID{ get; set;}
        public BusinessName VendorName{get;set;}

		public List<PurchaseOrderLineItem> LineItems{ get; set;}

		public bool IsExpectingShipments()
		{
			return Status == PurchaseOrderStatus.SentToVendor;
		}

		public PurchaseOrderStatus Status { get; set; }

		public IEnumerable<IPurchaseOrderLineItem> GetLineItems ()
		{
			return LineItems;
		}

		public void AddLineItem (IPurchaseOrderLineItem li)
		{
			var downLI = li as PurchaseOrderLineItem;
			LineItems.Add (downLI);
		}

		public PurchaseOrderPerVendor(){
			LineItems = new List<PurchaseOrderLineItem> ();
		}

		public ICloneableEntity Clone ()
		{
			return new PurchaseOrderPerVendor {
				VendorID = VendorID,
                VendorName = VendorName,
				LineItems = LineItems.Select(l => l.Clone () as PurchaseOrderLineItem).ToList(),
				Status = Status
			};
		}

		public Money GetTotal(){
			throw new NotImplementedException();
		}

		public bool ContainsSearchString(string searchString){
			return Status.ToString().Contains(searchString)
				|| LineItems.Any(li => li.ContainsSearchString(searchString));
		}
	}
}

