namespace Mise.Core.ValueItems
{
    public enum PaymentType
    {
		/// <summary>
		/// Credit card that is processed through mise
		/// </summary>
        InternalCreditCard,
		/// <summary>
		/// Credit card processed through an external terminal
		/// </summary>
		ExternalCreditCard,
		/// <summary>
		/// CASH MONEY
		/// </summary>
        Cash,

        GiftCertificate,
        /// <summary>
		/// Comped an item, not a dollar amount.  Doesn't include sales tax on this
        /// </summary>
        CompItem,
        /// <summary>
		/// Comped an amount - has sales tax, just reduces the amount of money we take in
        /// </summary>
        CompAmount,

        /// <summary>
        /// Credit given by Mise for accounts and other things
        /// </summary>
        MiseCredit
    }
}
