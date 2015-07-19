using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Mise.Inventory.ViewModels;
using Mise.Core.Entities.Inventory;

namespace Mise.Inventory.Pages
{
	public partial class PurchaseOrderSelectPage : ContentPage
	{
		public PurchaseOrderSelectPage ()
		{
			var vm = App.PurchaseOrderSelectViewModel;
			BindingContext = vm;
			vm.LoadItemsOnView = LoadItems;
			InitializeComponent ();
		}

		protected override async void OnAppearing ()
		{
			Xamarin.Insights.Track("ScreenLoaded", new Dictionary<string, string>{{"ScreenName", "PurchaseOrderSelectPage"}});
			var vm = BindingContext as PurchaseOrderSelectViewModel;
			if (vm != null) {
				await vm.OnAppearing ();
			}
		}

		public void LoadItems(){
			var vm = BindingContext as PurchaseOrderSelectViewModel;
			stckPOs.Children.Clear ();
			var template = new DataTemplate (typeof(TextCell));
			template.SetBinding (TextCell.TextProperty, "DisplayName");
			template.SetBinding (TextCell.DetailProperty, "DetailDisplay");
			var lv = new ListView {
				ItemsSource = vm.LineItems,
				ItemTemplate = template,
			};

			lv.ItemTapped += async (sender, e) => {
				var selectedVendor = e.Item as IPurchaseOrder;
				((ListView)sender).SelectedItem = null;
				if(selectedVendor != null){
					await vm.SelectLineItem (selectedVendor);
				}
			};
			stckPOs.Children.Add (lv);
		}
	}
}

