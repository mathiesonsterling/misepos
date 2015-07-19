using Mise.Core.Common.Events.Employee;
using Mise.Core.Entities;
using Mise.Core.Entities.People;
namespace Mise.Core.Common.Events
{
	public class EmployeeClockedOutEvent : BaseEmployeeEvent
	{
		public override MiseEventTypes EventType {
			get {
				return MiseEventTypes.EmployeeClockedOut;
			}
		}
	}
}

