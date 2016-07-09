using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Mise.Inventory.CustomControls
{ 
	public class InventoryLineItemCell : LineItemWithQuantityCell
	{
		public delegate Task InventoryLineItemHandler(object item);
		public InventoryLineItemCell (InventoryLineItemHandler deleted, InventoryLineItemHandler moveUp, 
			InventoryLineItemHandler moveDown)
		{
			var moveUpAction = new MenuItem{ Text = "Up" };
			moveUpAction.SetBinding (MenuItem.CommandParameterProperty, new Binding("."));
			moveUpAction.Clicked += async (sender, e) => {
				var mi = ((MenuItem)sender);
				await moveUp(mi.CommandParameter);
			};
			ContextActions.Add (moveUpAction);

			var moveDownAction = new MenuItem { Text = "Down" };
			moveDownAction.SetBinding (MenuItem.CommandParameterProperty, new Binding("."));
			moveDownAction.Clicked += async (sender, e) => {
				var mi =((MenuItem)sender);
				await moveDown(mi.CommandParameter);
			};
			ContextActions.Add (moveDownAction);

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

