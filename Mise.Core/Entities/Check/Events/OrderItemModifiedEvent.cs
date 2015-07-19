using System;
using System.Linq;
using System.Collections.Generic;

using Mise.Core.Entities.Check;
using Mise.Core.Entities.Check.Events;
using Mise.Core.Entities.Menu;
using Mise.Core.Common.Entities.MenuItems;

namespace Mise.Core.Common.Events
{
	public class OrderItemModifiedEvent : BaseCheckEvent
	{
		#region ICheckEvent implementation

		public override ICheck ApplyEvent (ICheck check)
		{
            check = base.ApplyEvent(check);
		    //get the OI
		    var orderItem = check.OrderItems.FirstOrDefault (oi => oi.ID == OrderItemID);
		    if (orderItem == null)
		    {
		        return check;
		    }
				

		    orderItem.ClearModifiers();
		    foreach (var m in Modifiers)
		    {
		        orderItem.AddModifier(m);
		    }
		    return check;

		}


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

		public override string EventType {
			get {
				return "OrderItemModifiedEvent";
			}
		}

		#endregion
	}
}

