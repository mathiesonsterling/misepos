using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Mise.Inventory.ViewModels;
namespace Mise.Inventory.Pages
{
	public partial class SettingsPage : BasePage
	{
		public SettingsPage ()
		{
			InitializeComponent ();
		}

		#region implemented abstract members of BasePage

		public override BaseViewModel ViewModel {
			get {
				return App.SettingsViewModel;
			}
		}

		public override string PageName {
			get {
				return "Settings";
			}
		}

		#endregion
	}
}

