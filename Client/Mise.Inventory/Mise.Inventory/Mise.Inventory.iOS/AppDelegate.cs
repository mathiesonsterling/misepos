using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;

namespace Mise.Inventory.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();
            MR.Gestures.iOS.Settings.LicenseKey = "8TJV-AFFS-72EV-SF4E-8BGG-S5YP-J9X4-CQQU-N9AY-YTBZ-GF8F-C3ED-GTWE";
            LoadApplication(new App(new DependencySetup()));

            return base.FinishedLaunching(app, options);
        }
    }
}
