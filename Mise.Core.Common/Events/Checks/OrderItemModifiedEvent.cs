using System;
using System.Collections.Generic;
using Mise.Core.Entities;
using Mise.Core.Entities.Menu;

namespace Mise.Core.Common.Events.Checks
{
	public class OrderItemModifiedEvent : BaseCheckEvent
	{
		#region ICheckEvent implementation

		public Guid OrderItemID {
			get;
			set;
		}

		public IEnumerable<MenuItemModifier> Modifiers {
			get;
			set;
		}


		#endregion

		#region IEntityEventBase implementation

		public override MiseEventTypes EventType {
			get
			{
			    return MiseEventTypes.OrderItemModified;
			}
		}

		#endregion
	}
}

