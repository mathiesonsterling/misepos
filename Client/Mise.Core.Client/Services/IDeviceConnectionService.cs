using System;

namespace Mise.Core.Client.Services
{
	public class ConnectionStateChangedEventArgs : EventArgs{
		public bool IsConnected{get;set;}
		public bool CanReachMiseServer{get;set;}
	}

	public delegate void ConnectionStateChangedEventHandler(object sender, ConnectionStateChangedEventArgs args);

	/// <summary>
	/// Lets us know if we've got a connection
	/// </summary>
	public interface IDeviceConnectionService
	{
		/// <summary>
		/// If we're online at all
		/// </summary>
		/// <value><c>true</c> if this instance is online; otherwise, <c>false</c>.</value>
		bool IsConnected{get;}
		bool CanReachMiseServer{get;}

		event ConnectionStateChangedEventHandler ConnectionStateChanged;
	}
}

