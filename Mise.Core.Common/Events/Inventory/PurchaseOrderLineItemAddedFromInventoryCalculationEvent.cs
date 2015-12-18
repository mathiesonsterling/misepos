using System;
using System.Collections.Generic;
using Mise.Core.ValueItems.Inventory;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Entities;
namespace Mise.Core.Common.Events.Inventory
{
	public class PurchaseOrderLineItemAddedFromInventoryCalculationEvent : BasePurchaseOrderEvent
	{
		public override MiseEventTypes EventType {
			get {
				return MiseEventTypes.PurchaseOrderLineItemAddedFromInventoryCalculation;
			}
		}

	    public override bool IsEntityCreation
	    {
	        get { return true; }
	    }

	    public Guid LineItemID { get; set; }

		/// <summary>
		/// The PAR item we're filling for
		/// </summary>
		/// <value>The PAR line item.</value>
		public ParBeverageLineItem PARLineItem{get;set;}
		public int? NumBottlesNeeded{get;set;}
		public LiquidAmount AmountNeeded{ get; set; }

		/// <summary>
		/// If set, we've found our vendor that has the lowest price
		/// </summary>
		/// <value>The vendor with best price I.</value>
		public Guid? VendorWithBestPriceID{get;set;}
        public string VendorName{get;set;}

		public IEnumerable<ItemCategory> Categories{ get; set;}
	}
}

