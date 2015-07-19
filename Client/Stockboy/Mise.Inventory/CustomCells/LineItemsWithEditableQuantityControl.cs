using System;
using Xamarin.Forms;
using Mise.Core.Entities.Inventory;
using System.Collections.Generic;


namespace Mise.Inventory.CustomCells
{
	public class LineItemsWithEditableQuantityControl : Grid
	{
		public delegate void QuantityChangedEventHandler(IBaseBeverageLineItem li, int newQuantity);
		public event QuantityChangedEventHandler QuantityUpdated;

		private int Quantity{get;set;}
		private int _prevQuantity;
		public LineItemsWithEditableQuantityControl (IBaseBeverageLineItem li, int quantity)
		{
			Padding = new Thickness (5, 10, 0, 0);
			ColumnDefinitions = new ColumnDefinitionCollection
			{
				new ColumnDefinition { Width = new GridLength(20, GridUnitType.Star) },
				new ColumnDefinition { Width = new GridLength(10, GridUnitType.Star) },
				new ColumnDefinition{ Width = new GridLength(10, GridUnitType.Star)}, 
				new ColumnDefinition{ Width = new GridLength(10, GridUnitType.Star)}, 
			};

			var name = new Label{ Text = li.DisplayName };
			Children.Add (name, 0, 0);

			var container = new Label{ Text = li.Container.DisplayName };
			Children.Add (container, 1, 0);

			var quantEntry = new LiQuantityEntry(li) {
				Text = quantity.ToString ()
			};

			quantEntry.Completed += (sender, e) => {
				int newQuant;
				var realEntry = sender as LiQuantityEntry;
				if(realEntry != null)
				{
					if(int.TryParse (realEntry.Text, out newQuant)){
						if(newQuant != _prevQuantity)
						{
							quantEntry.Text = newQuant.ToString ();
							Quantity = newQuant;
							_prevQuantity = newQuant;
							if(QuantityUpdated != null){
								QuantityUpdated(realEntry.LineItem, newQuant);
							}
						}
					} else {
						quantEntry.Text = string.Empty;
					}
				}
			};
			Children.Add (quantEntry, 2, 0);

			_prevQuantity = quantity;
		}

		public class LiQuantityEntry : Entry
		{
			public IBaseBeverageLineItem LineItem{get;private set;}
			public LiQuantityEntry(IBaseBeverageLineItem li){
				LineItem = li;
				Keyboard = Keyboard.Numeric;
			}
		}
	}
}

