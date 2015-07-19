using Mise.Core.Entities;

namespace Mise.Core.Common.Events.Checks
{
	public class CheckReopenedEvent : BaseCheckEvent
	{
		public override MiseEventTypes EventType {
		    get { return MiseEventTypes.CheckReopened; }
		}
	}
}

