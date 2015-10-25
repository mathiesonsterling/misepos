using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;

using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

using XLabs.Forms;

using Mise.Core.Common;
namespace Mise.Inventory.iOS
{
	[Register("AppDelegate")]
	public partial class AppDelegate : XFormsApplicationDelegate
	{

		public override bool FinishedLaunching(UIApplication app, NSDictionary options)
		{
			Forms.Init();
			MR.Gestures.iOS.Settings.LicenseKey = "8TJV-AFFS-72EV-SF4E-8BGG-S5YP-J9X4-CQQU-N9AY-YTBZ-GF8F-C3ED-GTWE";

			LoadApplication(new App(new DependencySetup()));

			return base.FinishedLaunching(app, options);
		}

		private void SetPaymentProvider(){
			var settings = StripePaymentProviderSettingsFactory.GetSettings ();
			Stripe.StripeClient.DefaultPublishableKey = settings.PublishableKey;
		}
	}
}

