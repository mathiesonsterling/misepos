using System;
using Mise.Core.Entities.Check.Events;

namespace Mise.Core.ValueItems
{
	/// <summary>
	/// Represents that status a payment that authorizes, like a credit card
	/// or cryptocurrency
	/// </summary>
	public enum PaymentProcessingStatus
	{
        /// <summary>
        /// No payment has yet been attempted, we have work to do!
        /// </summary>
        Empty,

		/// <summary>
		/// Created by us, not yet sent
		/// </summary>
		Created,

		/// <summary>
		/// We've sent it for the base authorization.  The check amount, but not the tip.
		/// </summary>
		SentForBaseAuthorization,

		/// <summary>
		/// Base payment says go ahead.  
		/// </summary>
		BaseAuthorized,

		/// <summary>
		/// Base payment was refused, not a valid payment
		/// </summary>
		BaseRejected,

		/// <summary>
		/// Tip added, but we're waiting for a batch to be sent
		/// </summary>
		WaitingToClose,

		/// <summary>
		/// We sent our full amount over, including tip
		/// </summary>
		SentForFullAuthorization,

		/// <summary>
		/// We authorized, then cancelled
		/// </summary>
		Cancelled,

		/// <summary>
		/// Full amount approved, all good
		/// </summary>
		Complete,

		/// <summary>
		/// Full amount was rejected.  What to do depends
		/// </summary>
		FullAmountRejected,

        /// <summary>
        /// A credit has been awarded but not applied yet
        /// </summary>
        CreditNeedsProcessing,
        /// <summary>
        /// Credit has been applied and used
        /// </summary>
        CreditProcessed,
	}
}

