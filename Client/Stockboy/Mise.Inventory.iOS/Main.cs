using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;

namespace Mise.Inventory.iOS
{
	public class Application
	{
		// This is the main entry point of the application.
		static void Main(string[] args)
		{
			Xamarin.Insights.Initialize ("ed66b318e3febcdfc08ca11d6c20e33c79f2f434");
			// if you want to use a different Application Delegate class from "AppDelegate"
			// you can specify it here.
			UIApplication.Main(args, null, "AppDelegate");
		}
	}
}

