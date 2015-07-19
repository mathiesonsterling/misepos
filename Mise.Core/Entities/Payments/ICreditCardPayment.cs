using System;

using Mise.Core.ValueItems;
namespace Mise.Core.Entities.Payments
{
	public interface ICreditCardPayment : IProcessingPayment
	{
		/// <summary>
		/// Credit card that was used to make the payment
		/// </summary>
		/// <value>The card.</value>
		CreditCard Card{get;}

		/// <summary>
		/// Amount of money paid towards the check
		/// </summary>
		/// <value>The amount charged.</value>
		Money AmountCharged{get;}

		/// <summary>
		/// Amount of money that was put on the tip line
		/// </summary>
		/// <value>The tip amount.</value>
		Money TipAmount{ get; set;}

		/// <summary>
		/// Stores the results we got from our call to authorize (pre-tip)
		/// </summary>
		/// <value>The authorization result.</value>
		CreditCardAuthorizationCode AuthorizationResult{get;set;}

	}
}

