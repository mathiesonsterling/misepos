using Android.App;
using Android.Content;
using Android.OS;
using Android.Content.PM;

namespace Mise.POSTerminal.Android
{
	[Activity(
		ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation,
		Label = "Mise POS",
		MainLauncher = true,
		NoHistory = true,
		Theme = @"@style/Theme.Splash"
	)]
	public class SplashActivity : Activity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			var intent = new Intent(this, typeof(MainActivity));
			StartActivity(intent);
		}
	}
}
