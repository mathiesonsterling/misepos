using System;

using Mise.Core.ValueItems;
namespace Mise.Core.Entities.Payments
{
	/// <summary>
	/// Represents a payment which takes time to authenticate and process
	/// </summary>
	public interface IProcessingPayment : IPayment
	{
		PaymentProcessingStatus PaymentProcessingStatus{ get; set;}

		/// <summary>
		/// Code sent back from payment provider on our final closing
		/// We use this to track through stuff and for records
		/// </summary>
		/// <value>The authorization code.</value>
		CreditCardAuthorizationCode FinalAuthorizationCode{get;}
	}
}

