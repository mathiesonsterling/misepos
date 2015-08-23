
using Xamarin.Forms;
using Mise.Inventory.ViewModels.Reports;
using Mise.Inventory.CustomControls;
namespace Mise.Inventory
{
	public partial class ReportResultsPage : ContentPage
	{
		public ReportResultsPage ()
		{
			BindingContext = App.ReportResultsViewModel;
			InitializeComponent ();
		}

		protected override async void OnAppearing ()
		{
			var vm = BindingContext as ReportResultsViewModel;
			if (vm != null) {
				await vm.OnAppearing ();
				LoadItems ();
			}
		}

		private void LoadItems(){
			stckItems.Children.Clear ();
			var vm = BindingContext as ReportResultsViewModel;
			if (vm != null) {
				var customVl = new ListView {
					ItemsSource = vm.LineItems,
					ItemTemplate = new DataTemplate (typeof(LineItemWithQuantityCell)),
					RowHeight = 50,
					HasUnevenRows = true,
					HorizontalOptions = LayoutOptions.FillAndExpand
				};
				customVl.ItemTapped += (sender, e) =>  {
					//just disable highlights for now
					((ListView)sender).SelectedItem = null;
				};
				stckItems.Children.Add (customVl);
			}
		}
	}
}

