using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;

namespace Mise.Inventory.Droid
{
	[Activity (ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, 
               Label = "Mise Stockboy", MainLauncher = true, NoHistory = true, Icon = "@drawable/icon")]
	public class SplashActivity : Activity
	{
		protected override void OnCreate (Bundle savedInstanceState)
		{
            global::Xamarin.Forms.Forms.Init (this, savedInstanceState);
            Xamarin.Insights.Initialize("ed66b318e3febcdfc08ca11d6c20e33c79f2f434", this);
			base.OnCreate (savedInstanceState);
			var intent = new Intent (this, typeof(MainActivity));
			StartActivity (intent);
		}
	}
}
