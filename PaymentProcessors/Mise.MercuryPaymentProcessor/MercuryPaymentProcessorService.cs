using System;
using System.Threading.Tasks;
using Mise.Core.Services;
using Mise.Core.Entities.Payments;
using Mise.Core.ValueItems;

using Mise.MercuryPaymentProcessor.HCService;
namespace Mise.MercuryPaymentProcessor
{
	public class MercuryPaymentProcessorService : ICreditCardProcessorService
	{
		public MercuryPaymentProcessorService ()
		{
		}

		#region ICreditCardProcessorService implementation

		public void ChargeCard (ICreditCardPayment payment, CreditCardAuthorizationCode authFromPrevious, Action<ICreditCardPayment, Mise.Core.ValueItems.CreditCardAuthorizationCode> closeCallback)
		{
			throw new NotImplementedException ();
		}

		public void VoidAuthorization (ICreditCardPayment payment, CreditCardAuthorizationCode authFromPrevious, Action<Mise.Core.Entities.Payments.ICreditCardPayment, Mise.Core.ValueItems.CreditCardAuthorizationCode> closeCallback)
		{
			throw new NotImplementedException ();
		}

		public void AuthorizeCard (ICreditCardPayment payment, Action<ICreditCardPayment, CreditCardAuthorizationCode> authCallback)
		{
			throw new NotImplementedException ();
		}

		public Task<string> SetPaymentID (Guid accountID, PersonName name, Money authorizationAmount)
		{
			throw new NotImplementedException ();
		}

		public Task<CreditCard> GetCardAfterAuthorization ()
		{
			throw new NotImplementedException ();
		}

		#endregion
	}
}

