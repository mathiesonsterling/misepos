using System;
using System.Linq;
using Mise.Core.Entities.Check;
namespace Mise.Core.Common.Events
{
	public class OrderItemSetMemoEvent : BaseCheckEvent
	{
		public override string EventType
		{
			get{ return "OrderItemSetMemoEvent";}
		}
			
		public string Memo {
			get;
			set;
		}

		public Guid OrderItemID{get;set;}

		public override ICheck ApplyEvent (ICheck check)
		{
            check = base.ApplyEvent(check);
			var orderItem = check.OrderItems.FirstOrDefault(oi => oi.ID == OrderItemID);
			if (orderItem != null) {
				orderItem.Memo = Memo;
			}

			return check;
		}

	}
}

