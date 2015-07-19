using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace Mise.Inventory
{
	public partial class AccountRegistrationPage : ContentPage
	{
		public AccountRegistrationPage ()
		{
			BindingContext = App.AccountRegistrationViewModel;
			InitializeComponent ();
		}
	}
}

