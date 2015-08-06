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


			InitWebService (cb);

			var raygun = new RaygunErrorTracking ();
			cb.RegisterInstance<IErrorTrackingService>(raygun).SingleInstance ();

			base.RegisterDepenencies (cb);
		}

		static async void InitWebService (ContainerBuilder cb)
		{
			var wsLocation = GetWebServiceLocation ();
			if (wsLocation != null) {
				var mobileService = new MobileServiceClient (wsLocation.Uri.ToString (), wsLocation.AppKey);
				CurrentPlatform.Init ();
				//create the SQL store for offline
				var dbServ = new iOSSQLLite ();
				var fileName = dbServ.GetLocalFilename ();
				var store = new MobileServiceSQLiteStore (fileName);
				await mobileService.SyncContext.InitializeAsync (store);
				var webService = new AzureWeakTypeSharedClient (Logger, new JsonNetSerializer (), mobileService);
				RegisterWebService (cb, webService);
			}
		}
	}
}

