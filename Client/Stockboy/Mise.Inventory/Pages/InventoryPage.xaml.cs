using System;
using System.Collections.Generic;
using System.Linq;

using Xamarin.Forms;
using Mise.Inventory.ViewModels;
using Mise.Core.Entities.Inventory;
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

				if (cameFromAdd && _customVL != null && vm.LineItems != null && vm.LineItems.Any())
                {
					_customVL.ScrollTo (vm.LineItems.Last(), ScrollToPosition.End, false);
                }
            }
        }

		void LoadItems ()
		{
			listItems.Children.Clear ();

			//TODO arrange line items by their display order field
			var vm = BindingContext as InventoryViewModel;
			if (vm != null) {
				_customVL = new ListView {
					ItemsSource = vm.LineItems,
					ItemTemplate = new DataTemplate (typeof(LineItemWithQuantityCell)),
					RowHeight = 50,
					HasUnevenRows = true
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
			    if (vm.FirstUnmeasuredItem != null)
			    {
			        _customVL.ScrollTo(vm.FirstUnmeasuredItem, ScrollToPosition.MakeVisible, false);
			    }
			}
		}
	}
}

