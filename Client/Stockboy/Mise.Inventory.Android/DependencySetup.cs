﻿using Autofac;

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
using Mise.Inventory.Android.MercuryWebService;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;


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
			InitWebService (cb);
			var errorService = new AndroidRaygun ();
			cb.RegisterInstance<IErrorTrackingService> (errorService).SingleInstance ();
			base.RegisterDepenencies(cb);
		}
			
		static async void InitWebService (ContainerBuilder cb)
		{
			var wsLocation = GetWebServiceLocation ();
			if (wsLocation != null) {
				var mobileService = new MobileServiceClient (wsLocation.Uri.ToString (), wsLocation.AppKey);
				CurrentPlatform.Init ();
				var dbService = new AndroidSQLite ();
				var store = new MobileServiceSQLiteStore (dbService.GetLocalFilename ());
				await mobileService.SyncContext.InitializeAsync (store);
				var webService = new AzureWeakTypeSharedClient (Logger, new JsonNetSerializer (), mobileService);
				RegisterWebService (cb, webService);
			}
		}
	}
}