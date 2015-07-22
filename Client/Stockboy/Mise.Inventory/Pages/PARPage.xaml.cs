using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Mise.Inventory.ViewModels;
using Mise.Inventory.CustomCells;
using Mise.Core.Entities.Inventory;
namespace Mise.Inventory.Pages
{
	public partial class PARPage : ContentPage
	{
		public PARPage()
		{
			InitializeComponent();

			var vm = BindingContext as ParViewModel;
			vm.LoadItemsOnView = LoadItems;

		}

		protected override async void OnAppearing ()
		{
			Xamarin.Insights.Track("ScreenLoaded", new Dictionary<string, string>{{"ScreenName", "ParPage"}});
			var vm = BindingContext as ParViewModel;
		    if (vm != null)
		    {
		        await vm.OnAppearing();
		    }
		}

		void LoadItems ()
		{
			var vm = BindingContext as ParViewModel;
			if (vm != null) {
				lineItems.Children.Clear ();
				var customVL = new ListView {
					ItemsSource = vm.LineItems,
					ItemTemplate = new DataTemplate (typeof(LineItemWithQuantityCell)),
					RowHeight = 50,
					HasUnevenRows = true
				};
				customVL.ItemTapped += async (sender, e) =>  {
					//mark it as the item being measured
					var lineItem = e.Item as PARLineItemDisplay;
					if (lineItem != null) {
						await vm.SelectLineItem (lineItem);
					}
					((ListView)sender).SelectedItem = null;
				};
				lineItems.Children.Add (customVL);
			}
		}
	}
}

