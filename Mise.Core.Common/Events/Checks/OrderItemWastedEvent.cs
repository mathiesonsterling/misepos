using Mise.Core.Entities;

namespace Mise.Core.Common.Events
{
	public class OrderItemWastedEvent : OrderItemVoidedEvent
	{
		public override MiseEventTypes EventType {
			get {
				return MiseEventTypes.OrderItemWasted;
			}
		}
	}
}

