using Mise.Core.Entities.Check;
using Mise.Core.Entities.Check.Events;

namespace Mise.Core.Common.Events
{
	public class OrderedOnCheckEvent : BaseCheckEvent
	{
		public override string EventType{ get { return "OrderedOnCheck"; } }


		public OrderItem OrderItem{ get; set; }

		public override ICheck ApplyEvent (ICheck check)
		{
            check = base.ApplyEvent(check);
			check.AddOrderItem(OrderItem);
			check.LastUpdatedDate = CreatedDate;
			return check;
		}
	}
}
