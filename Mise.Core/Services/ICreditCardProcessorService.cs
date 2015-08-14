using System;
using System.Threading.Tasks;
using Mise.Core.Entities.Payments;
using Mise.Core.ValueItems;

namespace Mise.Core.Services
{

	/// <summary>
	/// Represents any third party credit card processor
	/// </summary>
	public interface ICreditCardProcessorService
	{
		/// <summary>
		/// Charge the card with the final amount, close the payment
		/// </summary>
		/// <param name = "payment">Payment that is storing this</param>
		/// <param name="authFromPrevious">The authorization code we got when we authorized the base</param>
		/// <param name="closeCallback">What to do once we're done</param>
		void ChargeCard(ICreditCardPayment payment, CreditCardAuthorizationCode authFromPrevious,
		                Action<ICreditCardPayment, CreditCardAuthorizationCode> closeCallback);

		/// <summary>
		/// For our previous authorization, we made a mistake and want to cancel it
		/// </summary>
		/// <param name="payment">Payment.</param>
		/// <param name="authFromPrevious">Auth from previous.</param>
		/// <param name="closeCallback">Close callback.</param>
		void VoidAuthorization(ICreditCardPayment payment, CreditCardAuthorizationCode authFromPrevious, Action<ICreditCardPayment, CreditCardAuthorizationCode> closeCallback);

		void AuthorizeCard(ICreditCardPayment payment,
		                   Action<ICreditCardPayment, CreditCardAuthorizationCode> authCallback);

		/// <summary>
		/// Setup our credit card processer to register someone
		/// </summary>
		/// <returns>The payment I.</returns>
		Task<string> SetPaymentID(Guid accountID, PersonName name, Money authorizationAmount);

		/// <summary>
		/// Polls the processor for the card, then returns it
		/// </summary>
		/// <returns>The card after authorization.</returns>
		Task<CreditCard> GetCardAfterAuthorization (string paymentID);
	}
}

