using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Mise.Inventory.ViewModels;
namespace Mise.Inventory.Pages
{
	public partial class RestaurantLoadingPage : BasePage
	{
		public RestaurantLoadingPage ()
		{
			InitializeComponent ();
		}

		#region implemented abstract members of BasePage

		public override BaseViewModel ViewModel {
			get {
				return App.RestaurantLoadingViewModel;
			}
		}

		public override string PageName {
			get {
				return "RestaurantLoadingPage";
			}
		}

		#endregion
	}
}

