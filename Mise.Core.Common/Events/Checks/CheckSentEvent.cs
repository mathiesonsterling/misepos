using Mise.Core.Entities;

namespace Mise.Core.Common.Events.Checks
{
	public class CheckSentEvent : BaseCheckEvent
	{
		public override MiseEventTypes EventType {
			get {
				return MiseEventTypes.CheckSent;
			}
		}
	}
}

