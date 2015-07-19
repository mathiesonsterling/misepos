using Mise.Core.Entities;

namespace Mise.Core.Common.Events.Employee
{
	public class BadLoginAttemptEvent : BaseEmployeeEvent
	{
		#region implemented abstract members of BaseEmployeeEvent

		public override MiseEventTypes EventType {
			get
			{
			    return MiseEventTypes.BadLoginAttempt;
			}
		}

		#endregion

		#region IBadLoginAttemptEvent implementation
		public string PasscodeGiven {
			get;
			set;
		}
		public string FunctionAttempted {
			get;
			set;
		}
		#endregion

	}
}

