using System;

using Mise.Core;
using Mise.Core.ValueItems;

using Mise.Core.Services;
using Mise.Core.Entities.Payments;


namespace Mise.AndroidCommon.Services
{
	/// <summary>
	/// A service which pretends to process credit cards.  If it's divisible by 2, it passes
	/// </summary>
	public class FakeCreditCardProcessingService : ICreditCardProcessorService
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

		public FakeCreditCardProcessingService()
		{

		}

		public FakeCreditCardProcessingService(ILogger logger)
		{
			_logger = logger;
		}

		public void ChargeCard(ICreditCardPayment payment, CreditCardAuthorizationCode authFromPrevious, 
		                        Action<ICreditCardPayment, CreditCardAuthorizationCode> closeCallback)
		{
			if (closeCallback != null) {
				authFromPrevious.IsAuthorized = true;
				closeCallback(payment, authFromPrevious);
			}
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

		public void AuthorizeCard(ICreditCardPayment payment, Action<ICreditCardPayment, CreditCardAuthorizationCode>  authCallback)
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
				if (string.IsNullOrEmpty(payment.Card.FirstName) && string.IsNullOrEmpty(payment.Card.LastName)) {
					payment.Card.FirstName = "Testy";
					payment.Card.LastName = "McTesterson";
				}

				var authCode = new CreditCardAuthorizationCode {
					IsAuthorized = success,
					AuthorizationKey = Guid.NewGuid().ToString(),
					PaymentProviderName = "miseFakeProcessor"
				};
					
				//TODO change to be delayed!
				//Thread.Sleep (processingTime.Ticks);


				//give the callback
				if (authCallback != null) {
					authCallback(payment, authCode);
				}
			} catch (System.Exception e) {
				_logger.HandleException(e);
			}
		}
			
	}
}

