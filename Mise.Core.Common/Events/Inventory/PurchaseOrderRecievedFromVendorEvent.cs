using System;
using Mise.Core.Common.Events.Inventory;
using Mise.Core.Entities;
using Mise.Core.ValueItems.Inventory;


namespace Mise.Core.Common.Events.Inventory
{
	public class PurchaseOrderRecievedFromVendorEvent : BasePurchaseOrderEvent
	{
		#region implemented abstract members of BasePurchaseOrderEvent
		public override MiseEventTypes EventType {
			get {
				return MiseEventTypes.PurchaseOrderReceivedFromVendor;
			}
		}
		#endregion

		public Guid VendorID{get;set;}

		/// <summary>
		/// The status we want to give the purchase order from the vendor
		/// </summary>
		/// <value>The status.</value>
		public PurchaseOrderStatus Status{get;set;}

		public Guid ReceivingOrderID{ get; set;}
	}
}

