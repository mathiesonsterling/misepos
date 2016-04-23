using System;
using Plugin.Connectivity;
using Mise.Core.Client.Services;


namespace Mise.Inventory.Services.Implementation
{
	public class DeviceConnectionService : IDeviceConnectionService
	{
		public DeviceConnectionService ()
		{
			CrossConnectivity.Current.ConnectivityChanged += (sender, e) => {
				//TODO implement reaching mise

				if(ConnectionStateChanged != null){
					var myArgs = new ConnectionStateChangedEventArgs {
						IsConnected = e.IsConnected,
						CanReachMiseServer = e.IsConnected
					};

					ConnectionStateChanged(sender, myArgs);

				}
			};
		}

		#region IDeviceConnectionService implementation

		public event ConnectionStateChangedEventHandler ConnectionStateChanged;

		public bool IsConnected {
			get {
				return CrossConnectivity.Current.IsConnected;
			}
		}

		public bool CanReachMiseServer {
			get {
				//todo make this actually store connection to mise server
				return CrossConnectivity.Current.IsConnected;
			}
		}

		#endregion
	}
}

