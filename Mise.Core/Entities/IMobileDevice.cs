using System;

using Mise.Core.ValueItems;
namespace Mise.Core
{
	/// <summary>
	/// Represents a mobile device running our code
	/// </summary>
	public interface IMobileDevice
	{
		MobileDeviceTypes DeviceType{ get;}

		/// <summary>
		/// A unique(ish) ID for the device
		/// </summary>
		/// <value>The device ID.</value>
		string DeviceID{get;}
	}
}

