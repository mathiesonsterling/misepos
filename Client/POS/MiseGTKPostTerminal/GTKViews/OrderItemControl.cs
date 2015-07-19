using System;

using Gtk;
using Mise.Core.Entities;
using Mise.Core.Entities.Menu;
using Mise.Core.Entities.Check;
namespace MiseGTKPostTerminal
{
	public class OrderItemControl : Label
	{
		/// <summary>
		/// The orderitem this displays
		/// </summary>
		/// <value>The order item I.</value>
		public IOrderItem OrderItem{ get; private set;}
		public OrderItemControl (IOrderItem orderItem)
		{
			OrderItem = orderItem;
			if (orderItem.MenuItem != null) {
				this.Text = orderItem.Name + "     $" + orderItem.MenuItem.Price.Dollars;
			} else {
				this.Text = orderItem.Name;
			}
		}
	}
}

