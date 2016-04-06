using Autofac;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Mise.Core.Common.Services.Implementation;
using Mise.Core.Common.Services.Implementation.Serialization;
using Mise.Core.Services;
using Mise.Core.Services.UtilityServices;
using Mise.Inventory.iOS.Services;
using Mise.Inventory.Services;
using Mise.Inventory.Services.Implementation;
using Mise.Inventory.Services.Implementation.WebServiceClients.Azure;
using ModernHttpClient;
using SQLitePCL;

namespace Mise.Inventory.iOS
{
	public class DependencySetup : Inventory.DependencySetup
	{
		
		protected override void RegisterDepenencies(ContainerBuilder cb)
		{
			var raygun = new RaygunErrorTracking ();
			cb.RegisterInstance<IErrorTrackingService>(raygun).SingleInstance ();

			Logger = new IOSLogger (raygun);
			//cb.RegisterInstance<IDevice> (AppleDevice.CurrentDevice).SingleInstance ();

            var stripeClient = new ClientStripeFacade();
            var processor = new StripePaymentProcessorService(Logger, stripeClient);
			cb.RegisterInstance<ICreditCardProcessorService>(processor).SingleInstance ();

			InitWebService (cb);

			base.RegisterDepenencies (cb);
		}

	    static async void InitWebService (ContainerBuilder cb)
		{
			var wsLocation = GetWebServiceLocation ();
			if (wsLocation != null) {
				CurrentPlatform.Init ();
				var mobileService = new MobileServiceClient (wsLocation.Uri.ToString (), wsLocation.AppKey, 
					new NativeMessageHandler());
				//create the SQL store for offline
				var dbService = new iOSSQLite ();

				cb.RegisterInstance<ISQLite> (dbService);
				CurrentPlatform.Init ();
				var store = new MobileServiceSQLiteStore (dbService.GetLocalFilename ());

				store.DefineTable<AzureEntityStorage>();
				store.DefineTable<AzureEventStorage>();

				await mobileService.SyncContext.InitializeAsync (store, new AzureConflictHandler(Logger));

				var deviceConnection = new DeviceConnectionService ();
				var webService = new AzureWeakTypeSharedClient (Logger, new JsonNetSerializer (), mobileService, deviceConnection);
				RegisterWebService (cb, webService);
			}
		}
	}
}

