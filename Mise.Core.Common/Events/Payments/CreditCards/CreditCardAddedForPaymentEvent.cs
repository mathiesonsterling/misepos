using Mise.Core.Entities;
using Mise.Core.ValueItems;

namespace Mise.Core.Common.Events.Payments.CreditCards
{
	/// <summary>
	/// Tells our check we added a credit card payment to it
	/// Note this does NOT start the payment authorization process, or put the check into PaymentProcessing.  That's CreditCardAuthorizationStarted!
	/// </summary>
	public class CreditCardAddedForPaymentEvent : BaseCreditCardEvent
	{
		public Money Amount { get; set;}

		#region implemented abstract members of BaseCheckEvent
		public override MiseEventTypes EventType {
            get { return MiseEventTypes.CreditCardAddedForPayment; }
		}
		#endregion
	}
}

