using System;

namespace Mise.Core.ValueItems
{
	/// <summary>
	/// Represents the current status of the item - ordered, prepped, awaiting pickup, etc
	/// </summary>
	public enum OrderItemStatus
	{
		/// <summary>
		/// Used to represent an item that has been created, but is in the middle of ordering
		/// </summary>
		Ordering,

		/// <summary>
		/// Added to the check, but not yet fired.  Can be canceled
		/// </summary>
		Added,

		/// <summary>
		/// Fired, and on check
		/// </summary>
		Sent,

		/// <summary>
		/// Completed
		/// </summary>
		Made,

        /// <summary>
        /// Item was removed after being sent
        /// </summary>
		Voided
	}
}

