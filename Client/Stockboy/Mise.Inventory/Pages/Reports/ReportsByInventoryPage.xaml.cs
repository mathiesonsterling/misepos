using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Mise.Inventory.ViewModels;
namespace Mise.Inventory.Pages.Reports
{
	public partial class ReportsByInventoryPage : BasePage
	{
		public ReportsByInventoryPage ()
		{
			InitializeComponent ();
		}

		public override BaseViewModel ViewModel {
			get {
				return App.ReportsByInventoryViewModel;
			}
		}

		public override string PageName {
			get {
				return "ReportsByInventoryPage";
			}
		}
	}
}

