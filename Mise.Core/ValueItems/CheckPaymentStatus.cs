namespace Mise.Core.ValueItems
{
	public enum CheckPaymentStatus
	{
		/// <summary>
		/// Check is open 
		/// </summary>
		Open,

		/// <summary>
		/// Currently closing.  Means check dropped, but not yet out
		/// </summary>
		Closing,

		/// <summary>
		/// Check has been paid in full
		/// </summary>
		Closed,

        /// <summary>
        /// Check has been paid and THEN z'd out.  
        /// </summary>
        ClosedAndZed,

		/// <summary>
		/// Check has been submitted to payment provider, but not yet sent
		/// </summary>
		PaymentPending,

		/// <summary>
		/// Check has been submitted to payment provider, but not yet had the tip added
		/// </summary>
		PaymentApprovedWithoutTip,

		/// <summary>
		/// Payment was rejected by provider
		/// </summary>
		PaymentRejected,
	}
}

