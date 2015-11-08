using System;
using System.Threading.Tasks;
using Mise.Core.Services.UtilityServices;
using Mise.Core.ValueItems;

using Mise.Core.Services;
using Mise.Core.Entities.Payments;

namespace Mise.Core.Common.UnitTests.Tools
{
	public class TestingCreditCardService : ICreditCardProcessorService
	{
		private ILogger _logger;

		public ILogger Logger {
			get {
				return _logger;
			}
			set {
				_logger = value;
			}
		}

		public TestingCreditCardService(ILogger logger)
		{
			_logger = logger;
		}

		public virtual void ChargeCard(ICreditCardPayment payment, CreditCardAuthorizationCode authFromPrevious, 
		                               Action<ICreditCardPayment, CreditCardAuthorizationCode> closeCallback)
		{
			if (closeCallback == null)
				return;
			authFromPrevious.IsAuthorized = true;
			closeCallback(payment, authFromPrevious);
		}

		public void VoidAuthorization(ICreditCardPayment payment, CreditCardAuthorizationCode authFromPrevious, 
		                              Action<ICreditCardPayment, CreditCardAuthorizationCode> closeCallback)
		{
			//always pass I guess!

			var newAuth = new CreditCardAuthorizationCode {
				IsAuthorized = true,
				AuthorizationKey = Guid.NewGuid().ToString(),
				PaymentProviderName = authFromPrevious.PaymentProviderName
			};
			if (closeCallback != null) {
				closeCallback(payment, newAuth);
			}
		}

		public virtual void AuthorizeCard(ICreditCardPayment payment, Action<ICreditCardPayment, CreditCardAuthorizationCode>  authCallback)
		{
			try {
				//do our sync processing here - determine if we should pass or fail
				//determine how long to wait
				var random = new Random();

				var processingTime = new TimeSpan(0, 0, 1);
				switch (random.Next(10)) {
				case 1:
					processingTime = new TimeSpan(0, 0, 100);
					break;
				case 9:
					processingTime = new TimeSpan(0, 0, 1);
					break;
				}

				var success = (payment.AmountCharged.Dollars % .02M) == 0.0M;

				//if we weren't given a name, we should be adding one!
				if (payment.Card.Name == null) {
					payment.Card.Name = PersonName.TestName;
				}

				var authCode = new CreditCardAuthorizationCode {
					IsAuthorized = success,
					AuthorizationKey = Guid.NewGuid().ToString(),
					PaymentProviderName = "miseFakeProcessor"
				};
						
				//Thread.Sleep (processingTime.Ticks);


				//give the callback
				if (authCallback != null) {
					authCallback(payment, authCode);
				}
			} catch (Exception e) {
				_logger.HandleException(e);
			}
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
			return Task.FromResult(
				new CreditCard{ 
					ProcessorToken = new CreditCardProcessorToken{
						Processor = CreditCardProcessors.FakeProcessor,
						Token = "testProcessorToken"
					}
			});
		}

		public Task<CreditCard> SendCardToProcessorForSubscription (PersonName cardName, CreditCardNumber number)
		{
			throw new NotImplementedException ();
		}
	}
}

