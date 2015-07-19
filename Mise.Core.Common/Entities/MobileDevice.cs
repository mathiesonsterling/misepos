using System;

using Mise.Core.ValueItems;
namespace Mise.Core.Common.Entities
{
	/// <summary>
	/// 
	/// </summary>
	public class MobileDevice : IMobileDevice
	{
		#region IMobileDevice implementation
		public MobileDeviceTypes DeviceType {
			get;
			set;
		}

		public string DeviceID {
			get;
			set;
		}
		#endregion
	}
}

