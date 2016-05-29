using System;
using System.Linq;
using System.Collections.Generic;
using Mise.Core.Entities;
using Mise.Core.Entities.Base;
using Mise.Core.Entities.Inventory;
using Mise.Core.ValueItems;
using Mise.Core.ValueItems.Inventory;

namespace Mise.Core.Common.Entities.Inventory
{
	public class ReceivingOrderLineItem : BaseBeverageLineItem, IReceivingOrderLineItem
    {
		public ReceivingOrderLineItem(){
			Categories = new List<InventoryCategory> ();
			LineItemPrice = Money.None;
			UnitPrice = Money.None;
		}

        /// <summary>
        /// How much we paid, per bottle
        /// </summary>
        public Money LineItemPrice { get; set; }
		public Money UnitPrice{get;set;}

		public bool ZeroedOut { get; set;}

        public ICloneableEntity Clone()
        {
            var newItem = CloneRestaurantBase(new ReceivingOrderLineItem());
            newItem.MiseName = MiseName;
            newItem.UPC = UPC;
            newItem.Container = Container;
            newItem.Quantity = Quantity;
            newItem.UnitPrice = UnitPrice;
			newItem.LineItemPrice = LineItemPrice;
			newItem.Categories = Categories.Select (c => c).ToList ();
            return newItem;
        }
    }
}
