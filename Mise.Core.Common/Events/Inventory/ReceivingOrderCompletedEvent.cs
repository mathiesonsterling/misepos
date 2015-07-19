using System;
using Mise.Core.Entities;

namespace Mise.Core.Common.Events.Inventory
{
	/// <summary>
	/// Event to mark that a user has marked a receiving order as taken in
	/// </summary>
	public class ReceivingOrderCompletedEvent : BaseReceivingOrderEvent
	{
		public override MiseEventTypes EventType {
			get {
				return MiseEventTypes.ReceivingOrderCompleted;
			}
		}

		public string Notes{get;set;}
		public string InvoiceID{get;set;}
	}
}

