using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Mise.Inventory.ViewModels;
namespace Mise.Inventory
{
	public partial class AccountRegistrationPage : ContentPage
	{
		public AccountRegistrationPage ()
		{
			BindingContext = App.AccountRegistrationViewModel;
			InitializeComponent ();
		}

		protected override async void OnAppearing ()
		{
			var vm = BindingContext as AccountRegistrationViewModel;
			if (vm != null) {
				await vm.OnAppearing ();
			}
		}
	}
}

