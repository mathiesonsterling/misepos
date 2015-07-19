namespace Mise.Core.Common.Events
{
	public class OrderItemWastedEvent : OrderItemVoidedEvent
	{
		public override string EventType {
			get {
				return "OrderItemWastedEvent";
			}
		}
	}
}

