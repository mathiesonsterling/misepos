using Autofac;

using Xamarin.Forms;
using Mise.Core.Services;
using Mise.Inventory.Services;
using XLabs.Platform;
using XLabs.Platform.Device;
using Mise.Inventory.Android.Services;

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

			var dbConn = new AndroidSQLite ();
			SqlLiteConnection = dbConn;
			cb.RegisterInstance<ISQLite> (dbConn).SingleInstance ();

			base.RegisterDepenencies(cb);
		}
	}
}