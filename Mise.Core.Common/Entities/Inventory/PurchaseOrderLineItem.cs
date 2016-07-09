using System;
using System.Linq;
using System.Collections.Generic;
using Mise.Core.Entities.Base;
using Mise.Core.Entities.Inventory;
using Mise.Core.ValueItems.Inventory;

namespace Mise.Core.Common.Entities.Inventory
{
	public class PurchaseOrderLineItem : BaseBeverageLineItem, IPurchaseOrderLineItem
    {
		public PurchaseOrderLineItem(){
			Categories = new List<InventoryCategory> ();
		}

		public Guid? VendorID{get;set;}

        public ICloneableEntity Clone()
        {
            var newItem = CloneRestaurantBase(new PurchaseOrderLineItem());
            newItem.Quantity = Quantity;
            newItem.MiseName = MiseName;
            newItem.UPC = UPC;
            newItem.DisplayName = DisplayName;
            newItem.Container = Container;
			newItem.Categories = Categories.Select (c => c).ToList ();
            return newItem;
        }
    }
}
