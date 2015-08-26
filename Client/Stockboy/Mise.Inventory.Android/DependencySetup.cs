using Autofac;

using Mise.Core.Services;
using Mise.Inventory.Services;
using XLabs.Platform.Device;
using Mise.Inventory.Android.Services;
using Mise.Core.Services.UtilityServices;
using Mise.Inventory.Services.Implementation.WebServiceClients.Azure;
using Mise.Core.Common.Services.Implementation.Serialization;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Mise.Inventory.Services.Implementation;
using ModernHttpClient;


namespace Mise.Inventory.Android
{
	public class DependencySetup : Mise.Inventory.DependencySetup
	{
		protected override void RegisterDepenencies(ContainerBuilder cb)
		{
			var errorService = new AndroidRaygun ();
			cb.RegisterInstance<IErrorTrackingService> (errorService).SingleInstance ();
			Logger = new AndroidLogger (errorService);

			var device = AndroidDevice.CurrentDevice;
			cb.RegisterInstance<IDevice> (device).SingleInstance ();

			var processor = new MercuryPaymentProcessorService (Logger);
			cb.RegisterInstance<ICreditCardProcessorService>(processor).SingleInstance();

			//make the web service
			InitWebService (cb);
			base.RegisterDepenencies(cb);
		}
			
		static async void InitWebService (ContainerBuilder cb)
		{
			var wsLocation = GetWebServiceLocation ();
			if (wsLocation != null) {
				CurrentPlatform.Init ();
				var mobileService = new MobileServiceClient (wsLocation.Uri.ToString (), wsLocation.AppKey, new NativeMessageHandler());

				var dbService = new AndroidSQLite ();

				cb.RegisterInstance<ISQLite> (dbService);

				//SQLitePCL.CurrentPlatform.Init ();
				var store = new MobileServiceSQLiteStore (dbService.GetLocalFilename ());

				store.DefineTable<AzureEntityStorage>();
				store.DefineTable<AzureEventStorage>();

				await mobileService.SyncContext.InitializeAsync (store);

				var deviceConnection = new DeviceConnectionService ();
				var webService = new AzureWeakTypeSharedClient (Logger, new JsonNetSerializer (), mobileService, 
					deviceConnection);
				RegisterWebService (cb, webService);
			}
		}
	}
}