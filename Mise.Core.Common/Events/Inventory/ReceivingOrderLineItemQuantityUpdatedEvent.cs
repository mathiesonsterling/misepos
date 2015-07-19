using System;
using Mise.Core.Common.Events.Inventory;
using Mise.Core.Entities;
using Mise.Core.ValueItems;
namespace Mise.Core.Common.Events.Inventory
{
	public class ReceivingOrderLineItemQuantityUpdatedEvent : BaseReceivingOrderEvent
	{
		#region implemented abstract members of BaseReceivingOrderEvent
		public override MiseEventTypes EventType {
			get {
				return MiseEventTypes.ReceivingOrderLineItemQuantityUpdated;
			}
		}
		#endregion

		public Guid LineItemID{get;set;}
		public int UpdatedQuantity{get;set;}
		public Money LineItemPrice{get;set;}
	}
}

