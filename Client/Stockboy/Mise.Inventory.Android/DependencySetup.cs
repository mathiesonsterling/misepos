using Autofac;

using Xamarin.Forms;
using Mise.Core.Services;

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
			cb.RegisterInstance<ICreditCardProcessorService>(processor);
			base.RegisterDepenencies(cb);
		}
	}
}