
using Xamarin.Forms;
using Mise.Inventory.ViewModels.Reports;
using Mise.Inventory.CustomControls;
using Mise.Inventory.ViewModels;
namespace StockboyForms.Pages.Reports
{
	public partial class ReportResultsPage : BasePage
	{
		private ListView customVl;
		public ReportResultsPage ()
		{
			InitializeComponent ();
		}

		#region implemented abstract members of BasePage

		public override BaseViewModel ViewModel {
			get {
				return App.ReportResultsViewModel;
			}
		}

		public override string PageName {
			get {
				return "ReportResultsPage";
			}
		}
			
		#endregion
		protected override async void OnAppearing ()
		{
			var vm = ViewModel as ReportResultsViewModel;
			await vm.OnAppearing ();
			LoadItems ();
		}
		private void LoadItems(){
			var vm = ViewModel as ReportResultsViewModel;
			if (vm != null) {
				if (customVl == null) {
					customVl = new ListView {
						ItemTemplate = new DataTemplate (typeof(LineItemWithQuantityCell)),
						RowHeight = 50,
						HasUnevenRows = true,
						HorizontalOptions = LayoutOptions.FillAndExpand
					};
					customVl.ItemTapped += (sender, e) => {
						//just disable highlights for now
						((ListView)sender).SelectedItem = null;
					};
					stckItems.Children.Add (customVl);
				}
				customVl.ItemsSource = vm.LineItems;
			}
		}
	}
}

