using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using Mise.Core.Client.ApplicationModel;
using MiseAndroidPOSTerminal.Themes;
using Mise.Core.Services;
using MiseAndroidPOSTerminal.Services;

using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using MisePOSTerminal;

namespace MiseAndroidPOSTerminal.AndroidViews
{
	/// <summary>
	/// A page that just hosts a Xamarin forms page, till we move over completely
	/// </summary>
	public abstract class BaseXamarinFormsHostPage : AndroidActivity
	{
		protected abstract TerminalViewTypes Type{ get; }

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			var androidApp = Application as POSTerminalApplication;
			//TODO we might need to check if this has already initted!
			if (App.XamarinFormsInitted == false) {
				Forms.Init(this, bundle);

				//set our view model

				App.AppModel = androidApp.ApplicationModel;
				App.XamarinFormsInitted = true;
			}

			App.MoveToPageFunc = MoveToView;
			//hack right now to go between the two systems
			//get our android theme
			if (androidApp != null) {
				var exported = androidApp.POSTheme.ExportEmployeeColors();
				App.LoadEmployeeColors(exported);
			}

			var page = App.GetPageForType(Type);
			SetPage(page);
		}

		public void MoveToView(TerminalViewTypes view)
		{
			//determine which view to move to and do so
			switch (view) {
			case TerminalViewTypes.AdvertisementScreen:
				break;
			case TerminalViewTypes.OrderOnCheck:
				StartActivity (typeof(OrderOnTabNew));
				break;
			case TerminalViewTypes.ClockInPage:
				StartActivity(typeof(ClockInPage));
				break;
			case TerminalViewTypes.NoSale:
			case TerminalViewTypes.DisplayChange:
				StartActivity(typeof(DrawerOpen));
				break;
			case TerminalViewTypes.PaymentScreen:
			case TerminalViewTypes.AddTips:
				StartActivity(typeof(PaymentsScreen));
				break;
			case TerminalViewTypes.ViewChecks:
				StartActivity (typeof(ViewChecks));
				break;
			case TerminalViewTypes.ClosedChecks:
				StartActivity (typeof(ClosedTabs));
				break;
			default:
				throw new ArgumentException("Type " + view + " does not exist for Xamarin forms");
			}
		}
	}
}

