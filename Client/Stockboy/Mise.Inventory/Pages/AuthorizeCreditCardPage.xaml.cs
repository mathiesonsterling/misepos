using System;
using System.Collections.Generic;
using Mise.Inventory.ViewModels;
using Xamarin.Forms;

namespace Mise.Inventory.Pages
{
	public partial class AuthorizeCreditCardPage : BasePage
	{
		public AuthorizeCreditCardPage ()
		{
			InitializeComponent ();
			var vm = ViewModel as AuthorizeCreditCardViewModel;
			wvMain.Navigated += async (sender, e) => await vm.UrlHasChanged (e.Url);
		}
			
	    protected override void OnAppearing()
	    {
			base.OnAppearing ();
	        var vm = ViewModel as AuthorizeCreditCardViewModel;
	        if (vm != null)
	        {
				//we need the source after processed
				wvMain.Source = vm.StartUrl;
	        }
	    }

		#region implemented abstract members of BasePage

		public override BaseViewModel ViewModel {
			get {
				return App.AuthorizeCreditCardViewModel;
			}
		}

		public override string PageName {
			get {
				return "AuthorizeCreditCardPage";
			}
		}

		#endregion
	}
}

