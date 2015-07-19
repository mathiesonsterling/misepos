using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;

using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

using XLabs.Forms;
namespace Mise.Inventory.iOS
{
	[Register("AppDelegate")]
	public partial class AppDelegate : XFormsApplicationDelegate
	{

		public override bool FinishedLaunching(UIApplication app, NSDictionary options)
		{
			Forms.Init();
			LoadApplication(new App(new DependencySetup()));

			return base.FinishedLaunching(app, options);
		}
	}
}

