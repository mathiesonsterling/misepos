using System;
using System.Collections.Generic;
using Mise.Inventory.ViewModels;
using Xamarin.Forms;

namespace StockboyForms.Pages
{
	public partial class UpdateReceivingOrderLineItemPage : BasePage
	{
		public UpdateReceivingOrderLineItemPage ()
		{
			InitializeComponent ();
		}

		#region implemented abstract members of BasePage

		public override BaseViewModel ViewModel {
			get {
				return App.UpdateReceivingOrderLineItemViewModel;
			}
		}

		public override string PageName {
			get {
				return "UpdateReceivingOrderLineItemPage";
			}
		}

		#endregion
	}
}

