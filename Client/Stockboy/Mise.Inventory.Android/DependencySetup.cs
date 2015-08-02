using Autofac;

using Xamarin.Forms;
using Mise.Core.Services;
using Mise.Inventory.Services;
using XLabs.Platform;
using XLabs.Platform.Device;
using Mise.Inventory.Android.Services;
using Mise.Core.Services.UtilityServices;
using Mise.Inventory.Services.Implementation.WebServiceClients.Azure;
using Mise.Core.Common.Services.Implementation.Serialization;
using Microsoft.WindowsAzure.MobileServices;
using Mise.Core.Services.WebServices;
namespace Mise.Inventory.Android
{
	public class DependencySetup : Mise.Inventory.DependencySetup
	{
		protected override void RegisterDepenencies(ContainerBuilder cb)
		{
			Logger = new AndroidLogger ();
			var device = AndroidDevice.CurrentDevice;
			cb.RegisterInstance<IDevice> (device).SingleInstance ();

			var processor = new MercuryPaymentProcessorService (Logger);
			cb.RegisterInstance<ICreditCardProcessorService>(processor).SingleInstance();

			/*var dbConn = new AndroidSQLite ();
			SqlLiteConnection = dbConn;
			cb.RegisterInstance<ISQLite> (dbConn).SingleInstance ();*/

			//make the web service
			var wsLocation = GetWebServiceLocation ();
			if (wsLocation != null) {
				var mobileService = new MobileServiceClient (
					                   wsLocation.ToString (),
					                   "vvECpsmISLzAxntFjNgSxiZEPmQLLG42"
				                   );
				CurrentPlatform.Init ();
				var webService = new AzureWeakTypeSharedClient (Logger, new JsonNetSerializer (), mobileService, GetBuildLevel ());
	
				RegisterWebService (cb, webService);
			} 
			var errorService = new AndroidRaygun ();
			cb.RegisterInstance<IErrorTrackingService> (errorService).SingleInstance ();
			base.RegisterDepenencies(cb);
		}
			
	}
}