using System.Threading.Tasks;

using Autofac;

using Mise.Core.Services;
using Mise.Inventory.Services;
using Mise.Inventory.Android.Services;
using Mise.Core.Services.UtilityServices;
using Mise.Inventory.Services.Implementation.WebServiceClients.Azure;
using Mise.Core.Common.Services.Implementation.Serialization;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Mise.Core.Client.Services;
using Mise.Inventory.Services.Implementation;
using Mise.Core.Common.Services.Implementation;
using ModernHttpClient;


namespace Mise.Inventory.Android
{
	public class DependencySetup : Mise.Inventory.DependencySetup
	{
		protected override async void RegisterDepenencies(ContainerBuilder cb)
		{
			var errorService = new AndroidRaygun ();
			cb.RegisterInstance<IErrorTrackingService> (errorService).SingleInstance ();
			Logger = new AndroidLogger (errorService);
            /*
			var device = AndroidDevice.CurrentDevice;
			cb.RegisterInstance<IDevice> (device).SingleInstance ();
            */
            var stripeClient = new ClientStripeFacade();
            var processor = new StripePaymentProcessorService(Logger, stripeClient);
			cb.RegisterInstance<ICreditCardProcessorService>(processor).SingleInstance();

			//make the web service
			await InitWebService (cb);
			base.RegisterDepenencies(cb);
		}
		
        const string applicationURL = @"https://stockboymobile.azure-mobile.net/";
        const string applicationKey = @"zjnThZMLqPYplzheWvyqPaosgWpnrH41";
        const string localDbPath    = "localstore.db";
		async Task InitWebService (ContainerBuilder cb)
		{
			var wsLocation = GetWebServiceLocation ();
			if (wsLocation != null) {
				CurrentPlatform.Init ();
                var dbService = new AndroidSQLite ();

                cb.RegisterInstance<ISQLite> (dbService);

                var mobileService = new MobileServiceClient(applicationURL, new NativeMessageHandler());

                /*
				var mobileService = new MobileServiceClient (wsLocation.Uri.ToString (), wsLocation.AppKey, new NativeMessageHandler());
                */

				var store = new MobileServiceSQLiteStore (dbService.GetLocalFilename ());


				store.DefineTable<AzureEntityStorage>();
				store.DefineTable<AzureEventStorage>();

				await mobileService.SyncContext.InitializeAsync (store, new AzureConflictHandler(Logger));
                //await mobileService.SyncContext.InitializeAsync(store);

				var deviceConnection = new DeviceConnectionService ();
				var webService = new AzureWeakTypeSharedClient (Logger, new JsonNetSerializer (), mobileService, 
					deviceConnection);
				RegisterWebService (cb, webService);
			}
		}
	}
}