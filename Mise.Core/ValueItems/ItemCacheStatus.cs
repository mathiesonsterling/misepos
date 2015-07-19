using System;

namespace Mise.Core.ValueItems
{
	/// <summary>
	/// The current status of each cached item across the service layers
	/// </summary>
	public enum ItemCacheStatus
	{
		/// <summary>
		/// Changes exist in the local memory only.  
		/// </summary>
		TerminalMemory =10,

		/// <summary>
		/// Changes have been stored in our local db, so if there's a power item or crash we're ok
		/// </summary>
		TerminalDB = 20,

		/// <summary>
		/// Events and changes have been sent to the server, but not verified yet
		/// </summary>
		BeginSendToRestaurantServer = 30,

		/// <summary>
		/// Has been sent, and returned from service.  For entites, this means our snapshot is integrated
		/// </summary>
		SentToRestaurantServer = 40,

		/// <summary>
		/// We've started to send from the Restaurant service to the admin service
		/// </summary>
		BeginSendToAdminServer = 50,

		/// <summary>
		/// Sent from restaurant service to admin
		/// </summary>
		SentToAdminServer = 60,

        InMiseDB = 65,

		/// <summary>
		/// Item is fully updated across all layers
		/// </summary>
		Clean = 70
	}
}

