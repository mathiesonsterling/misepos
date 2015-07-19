using System;
using System.Collections.Generic;

using Xamarin.Forms;

using Mise.Inventory.ViewModels;
using Mise.Core.ValueItems;
namespace Mise.Inventory.Pages
{
	public partial class LoginPage : ContentPage
	{
		public LoginPage()
		{
			InitializeComponent();

			entEmail.Completed += (sender, e) => {
				var vm = BindingContext as LoginViewModel;
				if(vm != null){
					if(string.IsNullOrEmpty (vm.Username) == false){
						if(EmailAddress.IsValid (vm.Username))
						{
							if(string.IsNullOrEmpty (vm.Password) == false){
								vm.LoginCommand.Execute (null);
							}
							else {
								entPassword.Focus ();
							}
						} else {
							vm.Username = string.Empty;
							entEmail.Focus ();
						}
					}
				}
			};

			entPassword.Completed += (sender, e) => {
				var vm = BindingContext as LoginViewModel;
				if(vm != null){
					if(string.IsNullOrEmpty (vm.Password) == false){
						if(string.IsNullOrEmpty (vm.Username) == false){
							vm.LoginCommand.Execute (null);
						}
						else {
							entEmail.Focus ();
						}
					}
				}
			};
		}

		protected override async void OnAppearing(){
			Xamarin.Insights.Track("ScreenLoaded", new Dictionary<string, string>{{"ScreenName", "LoginPage"}});

			var vm = BindingContext as LoginViewModel;
			if (vm != null) {
				await vm.OnAppearing ();
			}
		}
	}
}

