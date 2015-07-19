using System;
using Mise.Core.Entities;

namespace Mise.Core.Common.Events.Checks
{
	public class OrderItemDeletedEvent : BaseCheckEvent
	{

		public Guid OrderItemID {
			get;
			set;
		}

		#region implemented abstract members of BaseCheckEvent

		public override MiseEventTypes EventType {
			get {
				return MiseEventTypes.OrderItemDeleted;
			}
		}
			
		#endregion
	}
}

