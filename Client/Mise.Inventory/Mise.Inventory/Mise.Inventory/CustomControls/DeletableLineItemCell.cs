using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Mise.Inventory.CustomControls
{
	public class DeletableLineItemCell : LineItemWithQuantityCell
	{
		public delegate Task InventoryLineItemHandler(object item);
		public DeletableLineItemCell (InventoryLineItemHandler deleted)
		{
			var deleteAction = new MenuItem { Text = "Delete", IsDestructive = true }; // red background
			deleteAction.SetBinding (MenuItem.CommandParameterProperty, new Binding ("."));
			deleteAction.Clicked += async (sender, e) => {
				var mi = ((MenuItem)sender);
				await deleted(mi.CommandParameter);
			};
			ContextActions.Add (deleteAction);
		}
	}
}

