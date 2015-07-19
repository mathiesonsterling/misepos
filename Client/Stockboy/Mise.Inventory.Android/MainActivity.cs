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
		Label = "Stockboy",
		MainLauncher = false,
		Theme = @"@style/App.Theme"
	)]
	public class MainActivity : XFormsApplicationDroid
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			Forms.Init(this, savedInstanceState);
			LoadApplication(new App(new DependencySetup()));
		}
	}
}
