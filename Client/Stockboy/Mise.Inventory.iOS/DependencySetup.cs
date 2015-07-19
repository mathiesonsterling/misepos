using System;
using Autofac;
using Mise.Core.Services;
using XLabs.Platform.Device;

namespace Mise.Inventory.iOS
{
	public class DependencySetup : Mise.Inventory.DependencySetup
	{
		protected override void RegisterDepenencies(ContainerBuilder cb)
		{
			Logger = new IOSLogger ();
			cb.RegisterInstance<IDevice> (AppleDevice.CurrentDevice).SingleInstance ();
			base.RegisterDepenencies (cb);
		}
	}
}

