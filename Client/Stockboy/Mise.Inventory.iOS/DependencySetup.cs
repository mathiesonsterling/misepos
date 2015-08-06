using System;
using Autofac;
using Mise.Core.Services;
using XLabs.Platform.Device;
using Mise.Inventory.iOS.Services;
using Mise.Inventory.Services;
using Mise.Core.Services.UtilityServices;

using Mise.Inventory.Services.Implementation.WebServiceClients.Azure;
using Mise.Core.Common.Services.Implementation.Serialization;
using Microsoft.WindowsAzure.MobileServices;
namespace Mise.Inventory.iOS
{
	public class DependencySetup : Mise.Inventory.DependencySetup
	{
		protected override void RegisterDepenencies(ContainerBuilder cb)
		{
			Logger = new IOSLogger ();
			cb.RegisterInstance<IDevice> (AppleDevice.CurrentDevice).SingleInstance ();
			var processor = new MercuryPaymentProcessorService (Logger);
			cb.RegisterInstance<ICreditCardProcessorService>(processor).SingleInstance ();

			var wsLocation = GetWebServiceLocation ();
			if (wsLocation != null) {
				var mobileService = new MobileServiceClient (
					wsLocation.ToString (),
					"vvECpsmISLzAxntFjNgSxiZEPmQLLG42"
				);
				CurrentPlatform.Init ();
				var webService = new AzureWeakTypeSharedClient (Logger, new JsonNetSerializer (), mobileService);

				RegisterWebService (cb, webService);
			} 

			var raygun = new RaygunErrorTracking ();
			cb.RegisterInstance<IErrorTrackingService>(raygun).SingleInstance ();

			base.RegisterDepenencies (cb);
		}
	}
}

