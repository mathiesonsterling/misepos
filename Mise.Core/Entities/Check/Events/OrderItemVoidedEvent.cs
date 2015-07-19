using System;
using System.Linq;
using Mise.Core.ValueItems;
using Mise.Core.Entities.Check;
namespace Mise.Core.Common.Events
{
	public class OrderItemVoidedEvent : BaseCheckEvent
	{
		#region implemented abstract members of BaseCheckEvent

		public override string EventType {
			get {
				return "OrderItemVoidedEvent";
			}
		}
			
		#endregion

		public OrderItem OrderItemToVoid {
			get;
			set;
		}
		public Guid ServerPlacedID {
			get;
			set;
		}
		public Guid ManagerApprovedID {
			get;
			set;
		}
		public string Reason {
			get;
			set;
		}
		public OrderItemStatus StatusWhenVoided {
			get;
			set;
		}

		public override ICheck ApplyEvent (ICheck check)
		{
			//find our oi in the check
            check = base.ApplyEvent(check);
			var foundItems = check.OrderItems.Where (oi => oi.ID == OrderItemToVoid.ID).ToList();
			foreach (var oi in foundItems) {
				check.RemoveOrderItem(oi);
			}
			return check;
		}
	}
}

