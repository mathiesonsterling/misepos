using System;
using System.Threading.Tasks;
using Autofac;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Mise.Core.Client.Services;
using Mise.Core.Services;
using Mise.Inventory.iOS.Services;
using Mise.Inventory.Services.Implementation;
using Mise.Inventory.Services.Implementation.WebServiceClients.Azure;
using Mise.Inventory.Services.Implementation.WebServiceClients.Azure.AzureStrongTypedClient;
//using ModernHttpClient;

namespace Mise.Inventory.iOS
{
	public class DependencySetup : Inventory.DependencySetup
	{
		
		protected override void RegisterDepenencies(ContainerBuilder cb)
		{

			Logger = new IOSLogger (null);
            //cb.RegisterInstance<IDevice> (AppleDevice.CurrentDevice).SingleInstance ();

            /*
            var stripeClient = new ClientStripeFacade();
            var processor = new StripePaymentProcessorService(Logger, stripeClient);
			cb.RegisterInstance<ICreditCardProcessorService>(processor).SingleInstance ();*/

            cb.RegisterType<NoProcessorService> ().As<ICreditCardProcessorService> ().SingleInstance ();
            try{
                var initTask = Task.Run(async () => await InitWebService (cb));

                initTask.Wait();
            } catch(Exception e){
                Logger.HandleException(e);
            }

			base.RegisterDepenencies (cb);
		}

	    static async Task InitWebService (ContainerBuilder cb)
		{
            var wsLocation = AzureServiceLocator.GetAzureMobileServiceLocation (GetBuildLevel (), true);
            //var wsLocation = AzureServiceLocator.GetAzureMobileServiceLocation(GetBuildLevel(), false);
			if (wsLocation != null) {
				CurrentPlatform.Init ();

                IMobileServiceClient mobileService;
                try {
                    mobileService = new MobileServiceClient (wsLocation.Uri.ToString ());
                } catch (Exception e) {
                    throw;
                }
				//create the SQL store for offline
				var dbService = new iOSSQLite ();

				cb.RegisterInstance<ISQLite> (dbService);
                SQLitePCL.CurrentPlatform.Init ();
				var store = new MobileServiceSQLiteStore (dbService.GetLocalFilename ());

                store = AzureStrongTypedClient.DefineTables(store);
                //store = AzureWeakTypeSharedClient.DefineTables(store);
                try{
				    await mobileService.SyncContext.InitializeAsync (store, new AzureConflictHandler(Logger)).ConfigureAwait (false);
                    //await mobileService.SyncContext.InitializeAsync(store).ConfigureAwait(false);
                }catch(Exception e){
                    var msg = e.Message;
                    throw;
                }

				var deviceConnection = new DeviceConnectionService ();
                var webService = new AzureStrongTypedClient(Logger, mobileService, deviceConnection);
                try{
                    await webService.SynchWithServer().ConfigureAwait(false);
                } catch(Exception e)
                {
                    //we've got to do something here!
                    var msg = e.Message;
                }
				RegisterWebService (cb, webService);
			}
		}
	}
}

