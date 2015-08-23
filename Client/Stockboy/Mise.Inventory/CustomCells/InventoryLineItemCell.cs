﻿using System;

using Xamarin.Forms;

namespace Mise.Inventory.CustomCells
{ 
	public class InventoryLineItemCell : LineItemWithQuantityCell
	{
		public delegate void InventoryLineItemDeletedHandler(object item);
		public delegate void InventoryLineItemInsertHandler(object item);
		public InventoryLineItemCell (InventoryLineItemDeletedHandler deleted, InventoryLineItemInsertHandler insert)
		{
			var insertAction = new MenuItem{ Text = "Insert After" };
			insertAction.SetBinding (MenuItem.CommandParameterProperty, new Binding ("."));
			insertAction.Clicked += (sender, e) => {
				var mi = ((MenuItem)sender);
				insert(mi.CommandParameter);
			};
			ContextActions.Add (insertAction);

			var deleteAction = new MenuItem { Text = "Delete", IsDestructive = true }; // red background
			deleteAction.SetBinding (MenuItem.CommandParameterProperty, new Binding ("."));
			deleteAction.Clicked += async (sender, e) => {
				var mi = ((MenuItem)sender);
				deleted(mi.CommandParameter);
			};
			ContextActions.Add (deleteAction);
		}
	}
}
