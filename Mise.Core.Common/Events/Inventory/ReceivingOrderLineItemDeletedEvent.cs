using System;
using Mise.Core.Entities;

namespace Mise.Core.Common.Events.Inventory
{
	public class ReceivingOrderLineItemDeletedEvent : BaseReceivingOrderEvent
	{
		#region implemented abstract members of BaseReceivingOrderEvent

		public override MiseEventTypes EventType {
			get {
				return MiseEventTypes.ReceivingOrderLineItemDeleted;
			}
		}

		#endregion

		public Guid LineItemId{get;set;}
	}
}

