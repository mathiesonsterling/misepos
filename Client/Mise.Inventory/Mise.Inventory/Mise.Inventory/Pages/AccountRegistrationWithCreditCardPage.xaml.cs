using System;
using System.Collections.Generic;
using Mise.Inventory.ViewModels;
using Xamarin.Forms;

namespace Mise.Inventory.Pages
{
	public partial class AccountRegistrationWithCreditCardPage : BasePage
	{
		public AccountRegistrationWithCreditCardPage ()
		{
			InitializeComponent ();
		}

		public override BaseViewModel ViewModel {
			get {
				return App.AccountRegistrationWithCreditCardViewModel;
			}
		}

		public override string PageName {
			get {
				return "Account Registration";
			}
		}
	}
}

