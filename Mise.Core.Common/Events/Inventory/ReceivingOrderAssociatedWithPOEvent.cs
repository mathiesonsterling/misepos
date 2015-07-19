using System;
using Mise.Core.Common.Events.Inventory;
using Mise.Core.Entities;

namespace Mise.Core.Common.Events.Inventory
{
	public class ReceivingOrderAssociatedWithPOEvent : BaseReceivingOrderEvent
	{
		#region implemented abstract members of BaseReceivingOrderEvent
		public override MiseEventTypes EventType {
			get {
				return MiseEventTypes.ReceivingOrderAssociatedWithPO;
			}
		}
		#endregion

		public Guid PurchaseOrderID{get;set;}
		public DateTimeOffset PurchaseOrderSentDate{get;set;}
	}
}

