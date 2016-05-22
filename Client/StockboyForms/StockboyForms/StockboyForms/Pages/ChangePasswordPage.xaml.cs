using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Mise.Inventory.ViewModels;
namespace StockboyForms.Pages
{
	public partial class ChangePasswordPage : BasePage
	{
		public ChangePasswordPage ()
		{
			InitializeComponent ();
		}

		public override BaseViewModel ViewModel {
			get {
				return App.ChangePasswordViewModel;
			}
		}

		public override string PageName {
			get {
				return "Change Password";
			}
		}
	}
}

