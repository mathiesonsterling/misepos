using System;

using Mise.Core.Client.ApplicationModel;
using MiseAndroidPOSTerminal.Themes;
using Mise.Core.Services;
using Mise.AndroidCommon.Services;
using MisePOSTerminal.ViewModels;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Mise.Core.Client.Services;
using Mise.AndroidCommon;


namespace MiseAndroidPOSTerminal.AndroidViews
{		
	public abstract class BasePOSView : Activity
	{
		protected IMiseAndroidTheme POSTheme;
		protected ITerminalApplicationModel Model;
		public ILogger Logger{get;private set;}
		protected override void OnCreate (Android.OS.Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			Logger = new AndroidClientLogger ();
			POSTheme = GetTheme ();
			Model = GetApplicationModel ();

			Window.RequestFeature (WindowFeatures.NoTitle);
			Window.SetFlags (WindowManagerFlags.Fullscreen, WindowManagerFlags.Fullscreen);
			Window.SetFlags (WindowManagerFlags.HardwareAccelerated, WindowManagerFlags.HardwareAccelerated);

			Window.DecorView.SetBackgroundColor (POSTheme.WindowBackground);
		}

		protected string GivenCode{ get;private set;}
		protected BaseViewModel ViewModel{ get; set;}
		public override bool OnKeyDown (Keycode keyCode, KeyEvent e)
		{
				if (ViewModel != null && ViewModel.EnteringText == false) {
					ViewModel.AddChars ((char)e.UnicodeChar);
				} 
				
			return base.OnKeyDown (keyCode, e);

		}
			

		/// <summary>
		/// Force the soft keyboard to show.  Need this because the swiper fools the system into thinking there's a keyboard already
		/// </summary>
		protected void ShowKeyboard(){
			Window.SetSoftInputMode (SoftInput.StateVisible);
		}

		protected void HideKeyboard ()
		{
			Window.SetSoftInputMode (SoftInput.StateHidden);
		}
			

		protected override void OnResume(){
			base.OnResume ();
			//turn off the keyboard if it's on!
			//getWindow().setSoftInputMode(WindowManager.LayoutParams.SOFT_INPUT_STATE_HIDDEN);
			HideKeyboard ();

			//if we have the credit IO reader, it will need to do some android stuff to switch activities.  Downcast it
			var ccReader = GetCreditCardReaderService() as CreditIOReaderService;
			if(ccReader != null){
				ccReader.SetStartActivityFunction (this, StartActivityForResult);
			}
		}

		/// <summary>
		/// The time when this activity was last updated
		/// </summary>
		protected DateTime LastUpdated{ get; set; }

		/// <summary>
		/// Retrieve the view model from our application
		/// </summary>
		/// <returns></returns>
		ITerminalApplicationModel GetApplicationModel ()
		{
			var app = Application as POSTerminalApplication;
		    return app != null ? app.ApplicationModel : null;
		}

		ICreditCardReaderService GetCreditCardReaderService(){
			var app = Application as POSTerminalApplication;
			if(app != null){
				if(app.ApplicationModel != null){
					return app.ApplicationModel.CreditCardReaderService;
				}
			}
			return null;
		}

		IPrinterService GetPrinterService(){
			var app = Application as POSTerminalApplication;
			if(app != null && app.ApplicationModel != null){
				var appModel = app.ApplicationModel;
				return appModel.LocalPrinterService;
			}
			return null;
		}

		IMiseAndroidTheme GetTheme() {
			var app = Application as POSTerminalApplication;
			if(app == null){
				Logger.Log("Unable to retrieve application!", LogLevel.Fatal);
				return null;
			}
			//get our screen dimensions to adjust all others based on it
			var d = WindowManager.DefaultDisplay;
			var m = new Android.Util.DisplayMetrics();
			d.GetMetrics( m );

			var theme = app.POSTheme;
			theme.SetScreenDimensions (m.WidthPixels, m.HeightPixels);

			return theme;
		}

		public void MoveToView(TerminalViewTypes view){
			//determine which view to move to and do so
			switch (view) {
			case TerminalViewTypes.AdvertisementScreen:
				break;
			case TerminalViewTypes.OrderOnCheck:
				StartActivity (typeof(OrderOnTabNew));
				break;
			case TerminalViewTypes.PaymentScreen:
			case TerminalViewTypes.AddTips:
				StartActivity(typeof(PaymentsScreen));
				break;
			case TerminalViewTypes.DisplayChange:
			case TerminalViewTypes.NoSale:
				Model.SetCreditCardProcessedCallback(null);
				StartActivity(typeof(DrawerOpen));
				break;
			case TerminalViewTypes.ViewChecks:
				StartActivity (typeof(ViewChecks));
				//StartActivity (typeof(ViewTabsXF));
				break;
			case TerminalViewTypes.ClosedChecks:
				StartActivity (typeof(ClosedTabs));
				break;
			}
		}

		protected void MoveToCurrentView()
		{
			try{
				MoveToView(Model.CurrentTerminalViewTypeToDisplay);
			}
			catch(Exception e){
				try{
					Logger.Log ("Error switching to " + Model.CurrentTerminalViewTypeToDisplay);
					Logger.HandleException (e, LogLevel.Fatal);
					StartActivity (typeof(ViewChecks));
				}
				catch(Exception ex2){
					Logger.HandleException(ex2, LogLevel.Fatal);
					//throw ex2;
				}
			}

			try{
				Finish ();
			}
			catch(Exception ef){
				Logger.HandleException (ef);
			}
		}
		
	}
}

