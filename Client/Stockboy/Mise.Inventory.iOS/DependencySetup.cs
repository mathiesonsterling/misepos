using Autofac;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Mise.Core.Client.Services;
using Mise.Core.Common.Services.Implementation;
using Mise.Core.Common.Services.Implementation.Serialization;
using Mise.Core.Services;
using Mise.Core.Services.UtilityServices;
using Mise.Inventory.iOS.Services;
using Mise.Inventory.Services;
using Mise.Inventory.Services.Implementation;
using Mise.Inventory.Services.Implementation.WebServiceClients.Azure;
//using ModernHttpClient;
using SQLitePCL;
using System.Threading.Tasks;
using Mise.Inventory.Services.Implementation.WebServiceClients.Azure.AzureStrongTypedClient;

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
            try{
                var initTask = Task.Run(async () => await InitWebService (cb));

                initTask.Wait();
            } catch(System.Exception e){
                Logger.HandleException(e);
            }

			base.RegisterDepenencies (cb);
		}

	    static async Task InitWebService (ContainerBuilder cb)
		{
            //var wsLocation = AzureServiceLocator.GetAzureMobileServiceLocation (GetBuildLevel (), true);
            var wsLocation = AzureServiceLocator.GetAzureMobileServiceLocation(GetBuildLevel(), false);
			if (wsLocation != null) {
				Microsoft.WindowsAzure.MobileServices.CurrentPlatform.Init ();
                var mobileService = new MobileServiceClient (wsLocation.Uri.ToString ());
				//create the SQL store for offline
				var dbService = new iOSSQLite ();

				cb.RegisterInstance<ISQLite> (dbService);
                SQLitePCL.CurrentPlatform.Init ();
				var store = new MobileServiceSQLiteStore (dbService.GetLocalFilename ());

                store = AzureWeakTypeSharedClient.DefineTables(store);

                try{
				    //await mobileService.SyncContext.InitializeAsync (store, new AzureConflictHandler(Logger));
                    await mobileService.SyncContext.InitializeAsync(store).ConfigureAwait(false);
                }catch(System.Exception e){
                    var msg = e.Message;
                    throw;
                }

				var deviceConnection = new DeviceConnectionService ();
                //var webService = new AzureStrongTypedClient(Logger, mobileService, deviceConnection);

                var webService = new AzureWeakTypeSharedClient(Logger, new JsonNetSerializer(), mobileService, deviceConnection);
                try{
                    await webService.SynchWithServer().ConfigureAwait(false);
                } catch(System.Exception e)
                {
                    //we've got to do something here!
                    var msg = e.Message;
                }
				//var webService = new AzureWeakTypeSharedClient (Logger, new JsonNetSerializer (), mobileService, deviceConnection);
				RegisterWebService (cb, webService);
			}
		}
	}
}

