
using System;
using Android.App;
using Android.OS;
using System.Threading.Tasks;

using Xamarin.Forms;
using MisePOSTerminal;

using Mise.AndroidCommon.Services;


namespace MiseAndroidPOSTerminal.AndroidViews
{

	[Activity(Theme = "@style/Theme.Splash", MainLauncher = true, NoHistory = true, Icon = "@drawable/miseLauncher")]
	public class SplashActivity : Activity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			//TODO load our ViewModel here!
			var androidApplication = Application as POSTerminalApplication;
			var logger = new AndroidClientLogger();
			var create = Task.Factory.StartNew(() => {
				androidApplication.CreateApplicationModel();
			}).ContinueWith(task => {
				if (task.IsFaulted) {
					foreach (var ex in task.Exception.Flatten ().InnerExceptions) {
						logger.HandleException(ex);
					}
				}
			});
			Task.WaitAll(new []{ create });

			//setup our stuff for Xamarin forms as well!
			Forms.Init(this, savedInstanceState);

			//set our view model
			App.AppModel = androidApplication.ApplicationModel;
			App.XamarinFormsInitted = true;

			var vm = androidApplication.ApplicationModel;
			if (vm.Setup) {
				logger.Log("ViewModel created and setup, begining ViewTabs", Mise.Core.Services.LogLevel.Debug);
				StartActivity(typeof(ClockInPage));
//				StartActivity(typeof(ViewChecks));
//				StartActivity (typeof(ViewTabsXF));
			} else {
				//need setup!
				logger.Log("Needs setup!");
				throw new NotImplementedException("Setup not ready!");
			}
		}
	}
}

