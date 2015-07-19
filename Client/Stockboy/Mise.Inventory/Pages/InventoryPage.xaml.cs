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
		public InventoryPage()
		{
			InitializeComponent();
			var vm = BindingContext as InventoryViewModel;
			vm.LoadItemsOnView = LoadItems;

		}

		void LoadItems ()
		{
			listItems.Children.Clear ();

			//TODO arrange line items by their display order field
			var vm = BindingContext as InventoryViewModel;
			if (vm != null) {
				var customVL = new ListView {
					ItemsSource = vm.LineItems,
					ItemTemplate = new DataTemplate (typeof(LineItemWithQuantityCell)),
					RowHeight = 50,
					HasUnevenRows = true
				};
				customVL.ItemTapped += async (sender, e) =>  {
					//mark it as the item being measured
					var lineItem = e.Item as InventoryViewModel.InventoryLineItemDisplayLine;
					if (lineItem != null) {
						await vm.SelectLineItem (lineItem);
					}
					((ListView)sender).SelectedItem = null;
				};
				listItems.Children.Add (customVL);
			    if (vm.FirstUnmeasuredItem != null)
			    {
			        customVL.ScrollTo(vm.FirstUnmeasuredItem, ScrollToPosition.MakeVisible, false);

			    }
			}
		}

		protected override async void OnAppearing ()
		{
			Xamarin.Insights.Track("ScreenLoaded", new Dictionary<string, string>{{"ScreenName", "InventoryPage"}});
			var vm = BindingContext as InventoryViewModel;
			if(vm != null){
				await vm.OnAppearing ();
			}
		}
	}
}

