using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Xamarin.Forms;
using Mise.Inventory.ViewModels;
using Mise.Inventory.CustomControls;

namespace Mise.Inventory.Pages
{
	public partial class ReceivingOrderPage : ContentPage
	{
		ListView _customVL;
		public ReceivingOrderPage()
		{
			InitializeComponent();

			var vm = BindingContext as ReceivingOrderViewModel;
			vm.LoadItemsOnView = LoadItems;
		}

		protected override async void OnAppearing ()
		{
			Xamarin.Insights.Track("ScreenLoaded", new Dictionary<string, string>{{"ScreenName", "ReceivingOrderPage"}});
			stckNotes.IsVisible = Device.Idiom != TargetIdiom.Phone;
			var vm = BindingContext as ReceivingOrderViewModel;
		    if (vm != null)
		    {
		        await vm.OnAppearing();
		    }
		}

		async Task LineItemDeleted(object item){
			var realItem = item as ReceivingOrderViewModel.ReceivingOrderDisplayLine;
			var vm = BindingContext as ReceivingOrderViewModel;
			if(realItem != null && vm != null){
				await vm.DeleteLineItem(realItem);
			}
		}

		void LoadItems ()
		{
			var vm = BindingContext as ReceivingOrderViewModel;
			if (vm != null) {
				if (_customVL == null) {
					_customVL = new ListView {
						ItemTemplate = new DataTemplate (() => new DeletableLineItemCell (LineItemDeleted)),
						RowHeight = 50,
						HasUnevenRows = true,
						HorizontalOptions = LayoutOptions.FillAndExpand
					};
					_customVL.ItemTapped += async (sender, e) => {
						//mark it as the item being measured
						var lineItem = e.Item as ReceivingOrderViewModel.ReceivingOrderDisplayLine;
						if (lineItem != null) {
							await vm.SelectLineItem (lineItem);
						}
						((ListView)sender).SelectedItem = null;
					};
					lineItems.Children.Add (_customVL);
				}
				_customVL.ItemsSource = vm.LineItems;
				if(vm.FocusedItem != null){
					_customVL.ScrollTo(vm.FocusedItem, ScrollToPosition.MakeVisible, false);
				}
			}
		}
	}
}

