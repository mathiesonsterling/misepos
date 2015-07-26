using Android.App;
using Android.Content.PM;
using Android.OS;

using Xamarin;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

using Mise.Inventory;

using XLabs.Forms;

namespace Mise.Inventory.Android
{
	[Activity(
		ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation,
		Label = "Mise Stockboy",
		MainLauncher = false,
		Theme = @"@style/App.Theme"
	)]
	public class MainActivity : XFormsApplicationDroid
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			Forms.Init(this, savedInstanceState);
			MR.Gestures.Android.Settings.LicenseKey = "8TJV-AFFS-72EV-SF4E-8BGG-S5YP-J9X4-CQQU-N9AY-YTBZ-GF8F-C3ED-GTWE";
			LoadApplication(new App(new DependencySetup()));
		}
	}
}
