﻿using System;
using Autofac;
using Mise.Core.Services;
using XLabs.Platform.Device;
using Mise.Inventory.iOS.Services;
using Mise.Inventory.Services;
using Mise.Core.Services.UtilityServices;

using Mise.Inventory.Services.Implementation.WebServiceClients.Azure;
using Mise.Core.Common.Services.Implementation.Serialization;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Mise.Inventory.Services.Implementation;
using ModernHttpClient;

namespace Mise.Inventory.iOS
{
	public class DependencySetup : Mise.Inventory.DependencySetup
	{
		
		protected override void RegisterDepenencies(ContainerBuilder cb)
		{
			var raygun = new RaygunErrorTracking ();
			cb.RegisterInstance<IErrorTrackingService>(raygun).SingleInstance ();

			Logger = new IOSLogger (raygun);
			cb.RegisterInstance<IDevice> (AppleDevice.CurrentDevice).SingleInstance ();
			var processor = new MercuryPaymentProcessorService (Logger);
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
				SQLitePCL.CurrentPlatform.Init ();
				var store = new MobileServiceSQLiteStore (dbService.GetLocalFilename ());

				store.DefineTable<AzureEntityStorage>();
				store.DefineTable<AzureEventStorage>();

				await mobileService.SyncContext.InitializeAsync (store);

				var deviceConnection = new DeviceConnectionService ();
				var webService = new AzureWeakTypeSharedClient (Logger, new JsonNetSerializer (), mobileService, deviceConnection);
				RegisterWebService (cb, webService);
			}
		}
	}
}

