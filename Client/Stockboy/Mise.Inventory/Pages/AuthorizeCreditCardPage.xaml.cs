using System;
using System.Collections.Generic;
using Mise.Inventory.ViewModels;
using Xamarin.Forms;

namespace Mise.Inventory
{
	public partial class AuthorizeCreditCardPage : ContentPage
	{
		public AuthorizeCreditCardPage ()
		{
		    var vm = App.AuthorizeCreditCardViewModel;
		    BindingContext = vm;
			InitializeComponent ();
			wvMain.Navigated += async (sender, e) => await vm.UrlHasChanged (e.Url);
		}
			
	    protected override async void OnAppearing()
	    {
	        var vm = BindingContext as AuthorizeCreditCardViewModel;
	        if (vm != null)
	        {
	            await vm.OnAppearing();

				//we need the source after processed
				wvMain.Source = vm.StartUrl;
	        }
	    }
	}
}

