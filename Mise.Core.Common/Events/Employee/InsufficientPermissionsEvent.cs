using Mise.Core.Common.Events.Employee;
using Mise.Core.Entities;
using Mise.Core.Entities.People.Events;

namespace Mise.Core.Common.Events
{
	public class InsufficientPermissionsEvent : BaseEmployeeEvent
	{
		public override MiseEventTypes EventType {
			get { return MiseEventTypes.InsufficientPermissions; }
		}

		public string FunctionAttempted {
			get;
			set;
		}
	}
}

