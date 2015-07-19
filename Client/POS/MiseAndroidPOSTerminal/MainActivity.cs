using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace MiseAndroidPOSTerminal.AndroidViews
{
	[Activity(Label = "MiseAndroidPOSTerminal")]
	public class MainActivity : Activity
	{
		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.Main);


			//move activity to our first screen
			StartActivity(typeof(ViewChecks));
		}
	}
}


