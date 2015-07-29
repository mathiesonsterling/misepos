using System;
using Autofac;
using Mise.Core.Services;
using XLabs.Platform.Device;
using Mise.Inventory.iOS.Services;
using Mise.Inventory.Services;
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

			var dbConn = new iOSSQLLite ();
			SqlLiteConnection = dbConn;
			cb.RegisterInstance<ISQLite> (dbConn).SingleInstance ();

			base.RegisterDepenencies (cb);
		}
	}
}

