using System;
using Mise.Core.Entities.Check.Events;
using Mise.Core.Entities.Check;
using Mise.Core.ValueItems;
namespace Mise.Core.Common.Events
{
	public class CheckReopenedEvent : BaseCheckEvent
	{
		#region implemented abstract members of BaseCheckEvent
		public override ICheck ApplyEvent (ICheck check)
		{
            check = base.ApplyEvent(check);
			check.PaymentStatus = CheckPaymentStatus.Open;
			check.LastTouchedServerID = EmployeeID;
			return check;
		}
		public override string EventType {
			get {
				return "CheckReopenedEvent";
			}
		}
		#endregion
	}
}

