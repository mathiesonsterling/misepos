using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Mise.Inventory.ViewModels;
using Mise.Core.Entities.Inventory;

namespace Mise.Inventory.Pages
{
	public partial class ReceivingOrderPage : ContentPage
	{
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

		void LoadItems ()
		{
			var vm = BindingContext as ReceivingOrderViewModel;
			if (vm != null) {
				lineItems.Children.Clear ();
				var customVL = new ListView {
					ItemsSource = vm.LineItems,
					ItemTemplate = new DataTemplate (typeof(LineItemWithQuantityCell)),
					RowHeight = 50,
					HasUnevenRows = true,
					HorizontalOptions = LayoutOptions.FillAndExpand
				};
				customVL.ItemTapped += async (sender, e) =>  {
					//mark it as the item being measured
					var lineItem = e.Item as ReceivingOrderDisplayLine;
					if (lineItem != null) {
						await vm.SelectLineItem (lineItem);
					}
					((ListView)sender).SelectedItem = null;
				};
				lineItems.Children.Add (customVL);

				if(vm.FocusOnLineItem != null){
					customVL.ScrollTo(vm.FocusOnLineItem, ScrollToPosition.MakeVisible, false);
				}
			}
		}
	}
}

