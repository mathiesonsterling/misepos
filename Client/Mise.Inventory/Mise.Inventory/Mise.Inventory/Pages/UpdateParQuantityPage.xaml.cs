using System;
using System.Collections.Generic;

using Xamarin.Forms;

using Mise.Inventory.ViewModels;
namespace Mise.Inventory
{
	public partial class UpdateParQuantityPage : ContentPage
	{
		public UpdateParQuantityPage ()
		{
			var vm = App.UpdateParQuantityViewModel;
			BindingContext = vm;
			InitializeComponent ();
		}

		protected override async void OnAppearing ()
		{
			Xamarin.Insights.Track("ScreenLoaded", new Dictionary<string, string>{{"ScreenName", "UpdateParQuantityPage"}});
			var vm = BindingContext as UpdateParQuantityViewModel;
			if (vm != null) {
				await vm.OnAppearing ();

				btnUpdateQuantity.IsEnabled = vm.UpdateQuantityCommand.CanExecute (null);
			}
		}
	}
}

