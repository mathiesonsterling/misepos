using System;

using Mise.Core.Entities.Check.Events;
using Mise.Core.Entities.Check;
namespace Mise.Core.Common.Events
{
	public class OrderItemDeletedEvent : BaseCheckEvent
	{

		public Guid MenuItemID {
			get;
			set;
		}

		#region implemented abstract members of BaseCheckEvent

		public override ICheck ApplyEvent (ICheck check)
		{
            check = base.ApplyEvent(check);
			return check;
		}

		public override string EventType {
			get {
				return "OrderItemDeletedEvent";
			}
		}
			
		#endregion
	}
}

