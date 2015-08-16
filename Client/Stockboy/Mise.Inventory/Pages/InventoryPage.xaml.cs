using System;
using System.Collections.Generic;
using System.Linq;

using Xamarin.Forms;
using Mise.Inventory.ViewModels;
using Mise.Inventory.CustomCells;
namespace Mise.Inventory.Pages
{
	public partial class InventoryPage : ContentPage
	{
		ListView _customVL;
		public InventoryPage()
		{
			InitializeComponent();
			var vm = BindingContext as InventoryViewModel;
			vm.LoadItemsOnView = LoadItems;

		}

        protected override async void OnAppearing()
        {
            Xamarin.Insights.Track("ScreenLoaded", new Dictionary<string, string> { { "ScreenName", "InventoryPage" } });
            var vm = BindingContext as InventoryViewModel;
            if (vm != null)
            {
                //this will be reset
                var cameFromAdd = vm.CameFromAdd;
                await vm.OnAppearing();

				if (vm.FocusedItem != null)
				{
					_customVL.ScrollTo(vm.FocusedItem, ScrollToPosition.MakeVisible, false);
				}
            }
        }

		void LineItemDeleted(object item){
			var realItem = item as InventoryViewModel.InventoryLineItemDisplayLine;
			var vm = BindingContext as InventoryViewModel;
			if(realItem != null && vm != null){
				if (vm.DeleteLineItemCommand.CanExecute (realItem)) {
					vm.DeleteLineItemCommand.Execute (realItem);
				}
			}
		}
			
		void LoadItems ()
		{
			listItems.Children.Clear ();

			//TODO arrange line items by their display order field
			var vm = BindingContext as InventoryViewModel;
			if (vm != null) {

				var dataTemplate = new DataTemplate (() => new InventoryLineItemCell (LineItemDeleted));
				_customVL = new ListView {
					ItemsSource = vm.LineItems,
					ItemTemplate = dataTemplate,
					RowHeight = 50,
					HasUnevenRows = true,
					HorizontalOptions = LayoutOptions.FillAndExpand
				};
				_customVL.ItemTapped += async (sender, e) =>  {
					//mark it as the item being measured
					var lineItem = e.Item as InventoryViewModel.InventoryLineItemDisplayLine;
					if (lineItem != null) {
						await vm.SelectLineItem (lineItem);
					}
					((ListView)sender).SelectedItem = null;
				};
				listItems.Children.Add (_customVL);
			    if (vm.FocusedItem != null)
			    {
			        _customVL.ScrollTo(vm.FocusedItem, ScrollToPosition.MakeVisible, false);
			    }
			}
		}
	}
}

