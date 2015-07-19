using Mise.Core.Entities.Check;
using Mise.Core.Common.Entities;

namespace Mise.Core.Common.Events
{
	public class CheckCreatedEvent : BaseCheckEvent
	{
		public override string EventType {
			get {
				return "CheckCreated";
			}
		}

		public override ICheck ApplyEvent(ICheck check)
		{
		    ICheck retCheck;
			if (check == null) {
				//TODO determine the subclass of check here!
				var newCheck = new BarTab
				{
					CreatedDate = CreatedDate,
					ID = CheckID,
                    CreatedByServerID = EmployeeID,
					LastTouchedServerID = EmployeeID,
				};
				retCheck = newCheck;
			} else {
				retCheck = check;
			}

		    return base.ApplyEvent(retCheck);
		}
	}
}

