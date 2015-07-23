using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Mise.Inventory.ViewModels;
using Mise.Core.Entities.Vendors;
namespace Mise.Inventory.Pages
{
	public partial class VendorFindPage : ContentPage
	{
		public VendorFindPage()
		{
			InitializeComponent();
			var vm = BindingContext as VendorFindViewModel;
			vm.LoadItemsOnView = LoadItems;
			searchEntry.SearchButtonPressed += (sender, e) => {
				//TODO we might need to look up additional vendors we don't have
				vm.SearchString = searchEntry.Text;
			};
		}

		protected override async void OnAppearing ()
		{
			Xamarin.Insights.Track("ScreenLoaded", new Dictionary<string, string>{{"ScreenName", "VendorFindPage"}});
			var vm = BindingContext as VendorFindViewModel;
			if (vm != null) {
				await vm.OnAppearing ();
			}
		}

		public void LoadItems(){
			var vm = BindingContext as VendorFindViewModel;
			stckVendors.Children.Clear ();
			var template = new DataTemplate (typeof(TextCell));
			template.SetBinding (TextCell.TextProperty, "Name");
			template.SetBinding (TextCell.DetailProperty, "Detail");
			var lv = new ListView {
				ItemsSource = vm.LineItems,
				ItemTemplate = template,
				HorizontalOptions = LayoutOptions.FillAndExpand
			};

			lv.ItemTapped += async (sender, e) => {
				var selectedVendor = e.Item as IVendor;
				((ListView)sender).SelectedItem = null;
				if(selectedVendor != null){
					await vm.SelectLineItem (selectedVendor);
				}
			};
			stckVendors.Children.Add (lv);
		}
	}
}

