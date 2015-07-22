using System;
using System.Collections.Generic;
using Mise.Inventory.ViewModels;
using Xamarin.Forms;

namespace Mise.Inventory
{
	public partial class UpdateParLineItemPage : ContentPage
	{
		public UpdateParLineItemPage ()
		{
			var vm = App.UpdateParLineItemViewModel;
			BindingContext = vm;
			InitializeComponent ();
		}

		protected override async void OnAppearing ()
		{
			base.OnAppearing ();
			var vm = BindingContext as UpdateParLineItemViewModel;
			if (vm != null) {
				await vm.OnAppearing();

				btnNext.IsEnabled = vm.MoveNextCommand.CanExecute (null);
			}
		}
	}
}

