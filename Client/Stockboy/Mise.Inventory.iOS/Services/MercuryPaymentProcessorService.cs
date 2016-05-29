using System;
using System.Threading.Tasks;

using Mise.Core.ValueItems;
using Mise.Core.Entities.Payments;
using Mise.Core.Services;
using Mise.Core.Services.UtilityServices;

using Mise.Inventory.iOS.MercuryWebService;
using Mise.Core.Common.Services.Implementation;
using Mise.Core.Entities;


namespace Mise.Inventory.iOS.Services
{
	/// <summary>
	/// Must be declared here, since we cannot consume ASMX services in a PCL!
	/// </summary>
	public class MercuryPaymentProcessorService : ICreditCardProcessorService
	{
		private readonly ILogger _logger;
		private readonly MercuryPaymentProviderSettings _requestSettings;
		public MercuryPaymentProcessorService (ILogger logger)
		{
			_logger = logger;

			var isDevelopment = Mise.Inventory.DependencySetup.GetBuildLevel () != BuildLevel.Production;

			_requestSettings = new MercuryPaymentProviderSettings (isDevelopment);
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

		public void AuthorizeCard (ICreditCardPayment payment, Action<ICreditCardPayment, CreditCardAuthorizationCode> authCallback)
		{
			throw new NotImplementedException ();
		}

		public Task<string> SetPaymentID (Guid accountID, PersonName name, Money authorizationAmount)
		{
			return Task.Run (() => SetPaymentIDWorker (accountID, name, authorizationAmount));
		}

		private string SetPaymentIDWorker(Guid accountID, PersonName name, Money authorizationAmount){
			
			var request = new InitPaymentRequest();
			request.MerchantID = _requestSettings.MerchantID;
			request.Password = _requestSettings.Password;
			request.TranType = _requestSettings.TranType;
			request.TotalAmount = (double)authorizationAmount.Dollars;
			request.Frequency = _requestSettings.Frequency;
			request.Invoice = accountID.ToString ().Substring (0, 15);
			request.Memo = "Mise " + MiseAppTypes.StockboyMobile;
			request.TaxAmount = 0;
			request.CardHolderName = name.ToSingleString ();
			request.ProcessCompleteUrl = _requestSettings.ProcessCompleteUrl;
			request.ReturnUrl = _requestSettings.ReturnUrl;
			request.LogoUrl = _requestSettings.LogoUrl;
			request.OrderTotal = _requestSettings.OrderTotal;
			request.SubmitButtonText = _requestSettings.SubmitText;

			//send it
			var service = new HCService();
			service.Url = _requestSettings.WebServiceUrl;
			var result = service.InitializePayment (request);

			if(result != null){
				if(result.ResponseCode == 0){
					return result.PaymentID;
				} else{
					throw new Exception (result.Message);
				}
			}

			throw new Exception ("Null return from Mercury!");
		}


		public async Task<CreditCard> GetCardAfterAuthorization (string paymentID)
		{
			//do this until we've got an answer, or we run over time
		
			var maxTime = DateTimeOffset.UtcNow.Add(_requestSettings.MaxWaitTimeForResponse);
			var sleepTime = _requestSettings.StartWaitTime;
			var lastSleepTime = new TimeSpan();

			while(DateTimeOffset.UtcNow < maxTime){
				var cc = GetCreditCardFromMercury (paymentID);
				if(cc == null){
					await Task.Delay (sleepTime);
					lastSleepTime = sleepTime;
					sleepTime = sleepTime.Add(lastSleepTime);
				} else {
					return cc;
				}
			}

			return null;
		}

		private CreditCard GetCreditCardFromMercury(string paymentID){
			var request = new PaymentInfoRequest ();
			request.MerchantID = _requestSettings.MerchantID;
			request.Password = _requestSettings.Password;
			request.PaymentID = paymentID;

			var webService = new HCService ();
			var result = webService.VerifyPayment (request);

			if(result == null){
				return null;
			}

			if(result.ResponseCode != 0){
				_logger.Error ("Error processing message responseCode:" + result.ResponseCode
					+ " status " + result.Status + " message:" + result.StatusMessage);
				throw new System.Exception ("Error processing credit card " + result.DisplayMessage);
			}

			switch(result.Status.ToLower ()){
			case "approved":
				//split the date
				var expDateMonthString = result.ExpDate.Substring (0, 2);
				var expDateYearString = result.ExpDate.Substring (2, 2);

				var expDateMonth = int.Parse (expDateMonthString);
				var expDateYear = int.Parse (expDateYearString) + 2000;
				//make the cc
				return new CreditCard {
					Name = new PersonName (result.CardholderName),
					/*BillingZip = new ZipCode{Value = result.AVSZip}, 
					ExpMonth = expDateMonth,
					ExpYear = expDateYear,
					MaskedCardNumber = result.MaskedAccount,*/
					ProcessorToken = new CreditCardProcessorToken{
						Processor = CreditCardProcessors.Mercury,
						Token = result.Token
					}
				};
			case "declined":
				throw new System.Exception ("Credit card was declined!");
			case "blank":
				return null;
			case "invalid":
				throw new System.Exception (result.DisplayMessage);
			default:
				throw new System.Exception (result.DisplayMessage);
			}
		}
		#endregion

		public Task<CreditCard> SendCardToProcessorForSubscription (PersonName cardName, CreditCardNumber number)
		{
			throw new InvalidOperationException ("Mercury requires a call to their webpage");
		}
	}
}

