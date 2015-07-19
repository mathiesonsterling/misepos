using System;
using System.Linq;

using Mise.Core.Entities.Check.Events;
using Mise.Core.Entities.Check;
using Mise.Core.ValueItems;
namespace Mise.Core.Common.Events
{
	public class CheckSentEvent : BaseCheckEvent
	{
		public override ICheck ApplyEvent (ICheck check)
		{
            check = base.ApplyEvent(check);

			//mark all order items in the tab as sent
			var unsaved = check.OrderItems.Where (oi => oi.Status == OrderItemStatus.Added);
			foreach (var oi in unsaved) {
				oi.Status = OrderItemStatus.Sent;
			}

			return check;
		}
		public override string EventType {
			get {
				return "CheckSentEvent";
			}
		}
	}
}

