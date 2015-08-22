using System;
using System.Collections.Generic;
using System.Linq;

using Xamarin.Forms;
using Mise.Inventory.ViewModels;
using Mise.Inventory.CustomControls;
namespace Mise.Inventory.Pages
{
	public partial class InventoryPage : ContentPage
	{
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

            }
        }

		async void LineItemDeleted(object item){
			var realItem = item as InventoryViewModel.InventoryLineItemDisplayLine;
			var vm = BindingContext as InventoryViewModel;
			if(realItem != null && vm != null){
				if (vm.CanDeleteLI (realItem)) {
					await vm.DeleteLineItem (realItem);
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
					var _customVL = new DragAndDropListView {
						ItemsSource = vm.LineItems,
						ItemTemplate = dataTemplate,
						RowHeight = 50,
						HasUnevenRows = true,
						HorizontalOptions = LayoutOptions.FillAndExpand
					};
					_customVL.ItemTapped += async (sender, e) => {
						//mark it as the item being measured
						var lineItem = e.Item as InventoryViewModel.InventoryLineItemDisplayLine;
						if (lineItem != null) {
							await vm.SelectLineItem (lineItem);
						}
						((ListView)sender).SelectedItem = null;
					};
					listItems.Children.Add (_customVL);
					if (vm.FocusedItem != null) {
						_customVL.ScrollTo (vm.FocusedItem, ScrollToPosition.MakeVisible, false);
					}

			}
		}
	}
}

