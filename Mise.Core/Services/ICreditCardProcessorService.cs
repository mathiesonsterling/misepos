using System;
using System.Threading.Tasks;
using Mise.Core.Entities.Payments;
using Mise.Core.ValueItems;

namespace Mise.Core.Services
{

	/// <summary>
	/// Represents the payment provider for integrated credit cards
	/// </summary>
	public interface ICreditCardProcessorService
	{
		/// <summary>
		/// The logger.
		/// </summary>
		ILogger Logger{ get; set; }

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
		/// Send the card to the processor, and get a clean version with the token populated
		/// </summary>
		/// <returns>The card.</returns>
		/// <param name="card">Card.</param>
		Task<CreditCard> AuthorizeCard(CreditCardNumber card);
	}
}

