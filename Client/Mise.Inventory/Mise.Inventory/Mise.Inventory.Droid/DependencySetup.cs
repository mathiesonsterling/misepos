using System;
using System.Threading.Tasks;
using Autofac;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Mise.Core.Client.Services;
using Mise.Core.Common.Services.Implementation;
using Mise.Core.Common.Services.Implementation.Serialization;
using Mise.Core.Services;
using Mise.Inventory.Droid.Services;
using Mise.Inventory.Services.Implementation;
using Mise.Inventory.Services.Implementation.WebServiceClients.Azure;
using Mise.Core.Services.UtilityServices;

namespace Mise.Inventory.Droid
{
	public class DependencySetup : Mise.Inventory.DependencySetup
	{
		protected override void RegisterDepenencies(ContainerBuilder cb)
		{
			Logger = new AndroidLogger ();
            /*
			var device = AndroidDevice.CurrentDevice;
			cb.RegisterInstance<IDevice> (device).SingleInstance ();
            */
            var stripeClient = new ClientStripeFacade();
            var processor = new StripePaymentProcessorService(Logger, stripeClient);
			cb.RegisterInstance<ICreditCardProcessorService>(processor).SingleInstance();
            cb.RegisterType<AndroidCryptography> ().As<ICryptography> ().SingleInstance ();
			//make the web service
            var ws = Task.Run(async () => await InitWebService (cb));
            ws.Wait ();
			base.RegisterDepenencies(cb);
		}
		
        const string applicationURL = @"https://stockboymobile.azure-mobile.net/";
        const string applicationKey = @"zjnThZMLqPYplzheWvyqPaosgWpnrH41";
        const string localDbPath    = "localstore.db";
		async Task InitWebService (ContainerBuilder cb)
		{
            var wsLocation = AzureServiceLocator.GetAzureMobileServiceLocation (GetBuildLevel (), false);
			if (wsLocation != null) {
                try {
                    CurrentPlatform.Init ();
                    var dbService = new AndroidSQLite ();

                    cb.RegisterInstance<ISQLite> (dbService);

                    var mobileService = new MobileServiceClient (applicationURL);

                    /*
                    var mobileService = new MobileServiceClient (wsLocation.Uri.ToString (), wsLocation.AppKey, new NativeMessageHandler());
                    */

                    var store = new MobileServiceSQLiteStore (dbService.GetLocalFilename ());
                    store = AzureWeakTypeSharedClient.DefineTables (store);
                    await mobileService.SyncContext.InitializeAsync (store, new AzureConflictHandler (Logger));
                    //await mobileService.SyncContext.InitializeAsync(store);

                    var deviceConnection = new DeviceConnectionService ();
                    var webService = new AzureWeakTypeSharedClient (Logger, new JsonNetSerializer (), mobileService,
                    deviceConnection);
                    RegisterWebService (cb, webService);
                } catch (Exception e) {
                    var msg = e.Message;
                    throw;
                }
			}
		}
	}
}