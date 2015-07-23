using System;
using System.Collections.Generic;

using Mise.Inventory.ViewModels.Reports;
using Xamarin.Forms;

namespace Mise.Inventory
{
	public partial class SelectCompletedInventoriesPage : ContentPage
	{
		public SelectCompletedInventoriesPage ()
		{
			var vm = App.SelectCompletedInventoryViewModel;
			BindingContext = vm;
			InitializeComponent ();
			vm.LoadItemsOnView = LoadItems;
		}

		protected override async void OnAppearing ()
		{
			var vm = BindingContext as SelectCompletedInventoryViewModel;
			if (vm != null) {
				await vm.OnAppearing ();
			}
		}

		private void LoadItems(){
			stckInventories.Children.Clear ();
			var vm = BindingContext as SelectCompletedInventoryViewModel;
			if (vm != null) {
				var template = new DataTemplate (typeof(TextCell));
				template.SetBinding (TextCell.TextProperty, "DateCompleted");
				template.SetBinding (TextCell.DetailProperty, "EmployeeName");
				var lv = new ListView {
					ItemsSource = vm.LineItems,
					ItemTemplate = template,
					HorizontalOptions = LayoutOptions.FillAndExpand
				};

				lv.ItemTapped += async (sender, e) => {
					var selectedVendor = e.Item as InventoryDisplayLine;
					((ListView)sender).SelectedItem = null;
					if (selectedVendor != null) {
						await vm.SelectLineItem (selectedVendor);
					}
				};
				stckInventories.Children.Add (lv);
			}
		}
	}
}

