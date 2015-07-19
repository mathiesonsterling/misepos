using System;
using Mise.Core.Entities;
namespace Mise.Core.Common.Events.Inventory
{
	public class ReceivingOrderLineItemZeroedOutEvent : BaseReceivingOrderEvent
	{
		#region implemented abstract members of BaseReceivingOrderEvent
		public override MiseEventTypes EventType {
			get {
				return MiseEventTypes.ReceivingOrderLineItemZeroedOut;
			}
		}
		#endregion

		public Guid LineItemID{get;set;}
	}
}

