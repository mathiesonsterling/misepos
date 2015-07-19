namespace Mise.Core.ValueItems
{
	/// <summary>
	/// Authorization code given back from our payment provider
	/// </summary>
	public class CreditCardAuthorizationCode
	{
		/// <summary>
		/// If true, what we tried to do/charge is permitted
		/// </summary>
		/// <value><c>true</c> if this instance is authorized; otherwise, <c>false</c>.</value>
		public bool IsAuthorized{ get; set;}

		/// <summary>
		/// Name of the payment network we used.
		/// </summary>
		/// <value>The name of the payment provider.</value>
		public string PaymentProviderName{get;set;}

		/// <summary>
		/// Unique ID given by the vendor
		/// </summary>
		/// <value>The autorization key.</value>
		public string AuthorizationKey{get;set;}
	}
}

