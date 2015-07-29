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
		ClientMemory =10,

		/// <summary>
		/// Changes have been stored in our local db, so if there's a power item or crash we're ok
		/// </summary>
		ClientDB = 20,

		/// <summary>
		/// Item is fully updated across all layers
		/// </summary>
		Clean = 70
	}
}

