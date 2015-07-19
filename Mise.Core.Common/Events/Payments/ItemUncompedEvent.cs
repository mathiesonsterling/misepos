using System;
using Mise.Core.Common.Events.Checks;
using Mise.Core.Entities;
using Mise.Core.ValueItems;

namespace Mise.Core.Common.Events.Payments
{
	public class ItemUncompedEvent : BaseCheckEvent, IItemCompedEvent
	{
		public override MiseEventTypes EventType {
			get {
				return MiseEventTypes.ItemUncomped;
			}
		}

		/// <summary>
		/// The order item the comp is being removed from
		/// </summary>
		/// <value>The order item I.</value>
		public Guid OrderItemID{get;set;}

		public Money Amount{get;set;}
	}
}

