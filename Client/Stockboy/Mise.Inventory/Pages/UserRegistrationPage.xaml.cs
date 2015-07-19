using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace Mise.Inventory
{
	public partial class UserRegistrationPage : ContentPage
	{
		public UserRegistrationPage ()
		{
			var vm = App.UserRegistrationViewModel;
			BindingContext = vm;
			InitializeComponent ();
		}

		protected override void OnAppearing(){
			Xamarin.Insights.Track("ScreenLoaded", new Dictionary<string, string>{{"ScreenName", "UserRegistrationPage"}});
			var vm = App.UserRegistrationViewModel;
			vm.OnAppearing();
		}
	}
}

