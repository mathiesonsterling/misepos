using Mise.Core.Entities;
using Mise.Core.Entities.People;

namespace Mise.Core.Common.Events.Checks
{
	public class CustomerAssignedToCheckEvent : BaseCheckEvent
	{
		public override MiseEventTypes EventType {
			get {
				return MiseEventTypes.CustomerAssignedToCheck;
			}
		}

		public Customer Customer{ get; set;}
	}
}

