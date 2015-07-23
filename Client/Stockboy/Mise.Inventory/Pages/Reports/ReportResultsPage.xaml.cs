using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Mise.Inventory.ViewModels.Reports;
namespace Mise.Inventory
{
	public partial class ReportResultsPage : ContentPage
	{
		public ReportResultsPage ()
		{
			BindingContext = App.ReportResultsViewModel;
			InitializeComponent ();
		}

		protected override void OnAppearing ()
		{
			var vm = BindingContext as ReportResultsViewModel;
			if (vm != null) {
				vm.OnAppearing ();
				LoadItems ();
			}
		}

		private void LoadItems(){
			stckItems.Children.Clear ();
			var vm = BindingContext as ReportResultsViewModel;
			if (vm != null) {
				var _customVL = new ListView {
					ItemsSource = vm.LineItems,
					ItemTemplate = new DataTemplate (typeof(LineItemWithQuantityCell)),
					RowHeight = 50,
					HasUnevenRows = true,
					HorizontalOptions = LayoutOptions.FillAndExpand
				};
				_customVL.ItemTapped += (sender, e) =>  {
					//just disable highlights for now
					((ListView)sender).SelectedItem = null;
				};
				stckItems.Children.Add (_customVL);
			}
		}
	}
}

