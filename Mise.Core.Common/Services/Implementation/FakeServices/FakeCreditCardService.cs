using System;
using System.Threading.Tasks;

using Mise.Core.Services;
using Mise.Core.Entities.Payments;
using Mise.Core.ValueItems;
namespace Mise.Core.Common
{
	public class FakeCreditCardService : ICreditCardProcessorService
	{
		public FakeCreditCardService ()
		{
		}

		#region ICreditCardProcessorService implementation

		public void ChargeCard (ICreditCardPayment payment, CreditCardAuthorizationCode authFromPrevious, Action<ICreditCardPayment, CreditCardAuthorizationCode> closeCallback)
		{
			throw new NotImplementedException ();
		}

		public void VoidAuthorization (ICreditCardPayment payment, CreditCardAuthorizationCode authFromPrevious, Action<ICreditCardPayment, CreditCardAuthorizationCode> closeCallback)
		{
			throw new NotImplementedException ();
		}

		public void AuthorizeCard (ICreditCardPayment payment, Action<Mise.Core.Entities.Payments.ICreditCardPayment, Mise.Core.ValueItems.CreditCardAuthorizationCode> authCallback)
		{
			throw new NotImplementedException ();
		}

		public Task<CreditCard> AuthorizeCard (CreditCardNumber card)
		{
			throw new NotImplementedException ();
		}

		/// <summary>
		/// Gets the payment ID we'll send to be given back a token after we register
		/// </summary>
		/// <returns>The payment I.</returns>
		public Task<string> SetPaymentID(Guid accountID, PersonName name, Money amount){
			return Task.FromResult ("testPaymentID");
		}

		public Task<CreditCard> GetCardAfterAuthorization (string paymentID){
			return Task.FromResult (
				new CreditCard {
					ProcessorToken = new CreditCardProcessorToken {
						Processor = CreditCardProcessors.FakeProcessor,
						Token = "fakeToken"
					}
				}
			);	
		}

		#endregion
	}
}

