using System;
using Mise.Core.Entities;
using Mise.Core.ValueItems;
namespace Mise.Core.Common.Events.Vendors
{
	/// <summary>
	/// Event marking that a restaurant got an item from a vendor, and gave a price they paid
	/// </summary>
	public class VendorRestaurantSetsPriceForReceivedItemEvent : BaseVendorEvent
	{
		#region implemented abstract members of BaseVendorEvent
		public override MiseEventTypes EventType {
			get {
				return MiseEventTypes.VendorRestaurantSetsPriceForReceivedItem;
			}
		}
		#endregion

		public Guid VendorLineItemID{ get; set; }
		public Money PricePerUnit{get;set;}
	}
}

