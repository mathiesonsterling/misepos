using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace StockboyForms.Pages
{
	public partial class UserRegistrationPage : BasePage
	{
		public UserRegistrationPage ()
		{
			InitializeComponent ();
		}

		#region implemented abstract members of BasePage

		public override Mise.Inventory.ViewModels.BaseViewModel ViewModel {
			get {
				return App.UserRegistrationViewModel;
			}
		}

		public override string PageName {
			get {
				return "UserRegistrationPage";
			}
		}

		#endregion
	}
}

