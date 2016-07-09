using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Mise.Inventory.ViewModels;
using Mise.Core.Entities.Vendors;
namespace Mise.Inventory.Pages
{
	public partial class VendorFindPage : BasePage
	{
		private ListView _lv;
		public VendorFindPage()
		{
			InitializeComponent();
			var vm = ViewModel as VendorFindViewModel;
			vm.LoadItemsOnView = LoadItems;
			searchEntry.SearchButtonPressed += (sender, e) => {
				//TODO we might need to look up additional vendors we don't have
				vm.SearchString = searchEntry.Text;
			};
		}

		#region implemented abstract members of BasePage

		public override BaseViewModel ViewModel {
			get {
				return App.VendorFindViewModel;
			}
		}

		public override string PageName {
			get {
				return "VendorFindPage";
			}
		}

		#endregion

		public void LoadItems(){
			var vm = ViewModel as VendorFindViewModel;
			var template = new DataTemplate (typeof(TextCell));
			template.SetBinding (TextCell.TextProperty, "Name");
			template.SetBinding (TextCell.DetailProperty, "Detail");
			if (_lv == null) {
				_lv = new ListView {
					ItemTemplate = template,
					HorizontalOptions = LayoutOptions.FillAndExpand
				};

				_lv.ItemTapped += async (sender, e) => {
					var selectedVendor = e.Item as IVendor;
					((ListView)sender).SelectedItem = null;
					if (selectedVendor != null) {
						await vm.SelectLineItem (selectedVendor);
					}
				};
				stckVendors.Children.Add (_lv);
			}
			_lv.ItemsSource = vm.LineItems;
		}
	}
}

