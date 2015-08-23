using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

using Xamarin.Forms;
using Mise.Inventory.ViewModels;
using Mise.Inventory.CustomControls;
using System.Threading;


using System.Runtime.InteropServices;


namespace Mise.Inventory.Pages
{
	public partial class InventoryPage : ContentPage
	{
		private DragAndDropListView _customVL;
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
                await vm.OnAppearing();

            }
        }

		async Task LineItemDeleted(object item){
			var realItem = item as InventoryViewModel.InventoryLineItemDisplayLine;
			var vm = BindingContext as InventoryViewModel;
			if(realItem != null && vm != null){
				if (vm.CanDeleteLI (realItem)) {
					await vm.DeleteLineItem (realItem);
				}
			}
		}

		async Task LineItemMovedUp(object item){
			var realItem = item as InventoryViewModel.InventoryLineItemDisplayLine;
			var vm = BindingContext as InventoryViewModel;
			if(realItem != null && vm != null){
				await vm.MoveLineItemUp (realItem);
			}
		}	

		async Task LineItemMovedDown(object item){
			var realItem = item as InventoryViewModel.InventoryLineItemDisplayLine;
			var vm = BindingContext as InventoryViewModel;
			if(realItem != null && vm != null){
				await vm.MoveLineItemDown (realItem);
			}
		}

		void LoadItems ()
		{
			var vm = BindingContext as InventoryViewModel;
			if (vm != null) {

					var dataTemplate = new DataTemplate (() => new InventoryLineItemCell (LineItemDeleted, LineItemMovedUp, LineItemMovedDown));
				if (_customVL == null) {
					
					_customVL = new DragAndDropListView {
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

					_customVL.ItemDraggedToNewPosition += (sender, args) => {
						
					};

					_customVL.SwipedLeftOnItem += async (sender, args) => {
						var item = vm.LineItems.FirstOrDefault(li => li.ID == args.ItemId);
						if(item != null){
							vm.DeleteLineItem (item);
						}
					};

					listItems.Children.Add (_customVL);
				}
				_customVL.ItemsSource = vm.LineItems;
					if (vm.FocusedItem != null) {
						_customVL.ScrollTo (vm.FocusedItem, ScrollToPosition.MakeVisible, false);
					}

			}
		}
	}
}

