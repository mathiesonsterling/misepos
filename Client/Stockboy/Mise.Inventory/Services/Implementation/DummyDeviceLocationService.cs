using System;
using System.Threading.Tasks;

using Mise.Core.ValueItems;
using Mise.Core.Client.Services;
namespace Mise.Inventory
{
	/// <summary>
	/// Fakes out the location service - that is device dependent
	/// </summary>
	public class DummyDeviceLocationService : IDeviceLocationService
	{
		#region IDeviceLocationService implementation

		public Task<Location> GetDeviceLocation ()
		{
			return Task.FromResult(new Location ());
		}

		#endregion
	}
}

