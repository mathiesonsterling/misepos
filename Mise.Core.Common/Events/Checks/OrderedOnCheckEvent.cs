using Mise.Core.Entities;
using Mise.Core.Entities.Check;

namespace Mise.Core.Common.Events.Checks
{
	public class OrderedOnCheckEvent : BaseCheckEvent
	{
		public override MiseEventTypes EventType{ get { return MiseEventTypes.OrderOnCheck; } }


		public OrderItem OrderItem{ get; set; }

	}
}
