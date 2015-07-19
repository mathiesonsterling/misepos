using System;
using System.Linq;
using Mise.Core.Common.Events.Checks;
using Mise.Core.Entities;
using Mise.Core.ValueItems;
using Mise.Core.Entities.Check;
namespace Mise.Core.Common.Events
{
	public class OrderItemVoidedEvent : BaseCheckEvent
	{
		#region implemented abstract members of BaseCheckEvent

		public override MiseEventTypes EventType {
			get {
				return MiseEventTypes.OrderItemVoided;
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

	}
}

