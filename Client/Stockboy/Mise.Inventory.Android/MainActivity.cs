using System;

using Android.App;
using Android.Content.PM;
using Android.OS;

using Xamarin;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

using Mise.Inventory;

using Mise.Core.Common;
using Mise.Core.Common.Services.Implementation;
namespace Mise.Inventory.Android
{
	[Activity(
		ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation,
		Label = "Mise Stockboy",
		MainLauncher = false,
		Theme = @"@style/App.Theme"
	)]
    public class MainActivity : Xamarin.Forms.Platform.Android.FormsApplicationActivity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			try{
				base.OnCreate(savedInstanceState);
				Mindscape.Raygun4Net.RaygunClient.Attach ("2ZV9A+X5sb5dNz4klhTD8A==");
				Forms.Init(this, savedInstanceState);

				MR.Gestures.Android.Settings.LicenseKey = "8TJV-AFFS-72EV-SF4E-8BGG-S5YP-J9X4-CQQU-N9AY-YTBZ-GF8F-C3ED-GTWE";
				LoadApplication(new App(new DependencySetup()));
			} catch(Exception e){
				Insights.Report (e, Insights.Severity.Critical);
			}
		}

		private void SetupPaymentProvider(){
			var settings = StripePaymentProviderSettingsFactory.GetSettings ();
			Stripe.StripeClient.DefaultPublishableKey = settings.PublishableKey;
		}
	}
}
