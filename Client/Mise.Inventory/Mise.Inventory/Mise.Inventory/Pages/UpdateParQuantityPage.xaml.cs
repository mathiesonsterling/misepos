using System.Collections.Generic;
using Mise.Inventory.ViewModels;
using Xamarin;

namespace Mise.Inventory.Pages
{
	public partial class UpdateParQuantityPage
	{
		public UpdateParQuantityPage ()
		{
			var vm = App.UpdateParLineItemViewModel;
			BindingContext = vm;
			InitializeComponent ();
		}

	    public override BaseViewModel ViewModel => App.UpdateParLineItemViewModel;
	    public override string PageName => "Update Par";

	    protected override async void OnAppearing ()
		{
			Insights.Track("ScreenLoaded", new Dictionary<string, string>{{"ScreenName", "UpdateParQuantityPage"}});
			var vm = BindingContext as UpdateParQuantityViewModel;
			if (vm != null) {
				await vm.OnAppearing ();

				btnUpdateQuantity.IsEnabled = vm.UpdateQuantityCommand.CanExecute (null);
			}
		}
	}
}

