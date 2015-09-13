using Mise.Inventory.ViewModels;
using Mise.Inventory.ViewModels.Reports;
using Xamarin.Forms;

namespace Mise.Inventory.Pages.Reports
{
	public partial class SelectCompletedInventoriesPage : BasePage
	{
		private ListView _lv;
		public SelectCompletedInventoriesPage ()
		{
			InitializeComponent ();
			var vm = ViewModel as SelectCompletedInventoryViewModel;
			vm.LoadItemsOnView = LoadItems;
		}

		#region implemented abstract members of BasePage

		public override BaseViewModel ViewModel {
			get {
				return App.SelectCompletedInventoryViewModel;
			}
		}

		public override string PageName {
			get {
				return "SelectCompletedInventoriesPage";
			}
		}

		#endregion

		private void LoadItems(){
			var vm = ViewModel as SelectCompletedInventoryViewModel;
			if (vm != null) {
				if (_lv == null) {
					var template = new DataTemplate (typeof(TextCell));
					template.SetBinding (TextCell.TextProperty, "DateCompleted");
					template.SetBinding (TextCell.DetailProperty, "EmployeeName");
					_lv = new ListView {
						ItemTemplate = template,
						HorizontalOptions = LayoutOptions.FillAndExpand
					};

					_lv.ItemTapped += async (sender, e) => {
						var selectedVendor = e.Item as InventoryDisplayLine;
						((ListView)sender).SelectedItem = null;
						if (selectedVendor != null) {
							await vm.SelectLineItem (selectedVendor);
						}
					};
					stckInventories.Children.Add (_lv);
				}
				_lv.ItemsSource = vm.LineItems;
			}
		}
	}
}

