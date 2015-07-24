using System;
using System.Threading.Tasks;

using Mise.Core.ValueItems;
using Mise.Core.Entities.Payments;
using Mise.Core.Services;

using Mise.Inventory.iOS.MercuryWebService;
using Mise.Core.Common.Services.Implementation;
using Mise.Core.Entities;
using XLabs.Forms.Validation;


namespace Mise.Inventory.iOS.Services
{
	/// <summary>
	/// Must be declared here, since we cannot consume ASMX services in a PCL!
	/// </summary>
	public class MercuryPaymentProcessorService : ICreditCardProcessorService
	{
		private readonly ILogger _logger;
		public MercuryPaymentProcessorService (ILogger logger)
		{
			_logger = logger;
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
			//create our request object

			bool isDevelopment = false;
			#if DEBUG
			isDevelopment = true;
			#endif

			var requestSettings = new MercuryPaymentProviderSettings (isDevelopment);
			
			var request = new InitPaymentRequest();
			request.MerchantID = requestSettings.MerchantID;
			request.Password = requestSettings.Password;
			request.TranType = requestSettings.TranType;
			request.TotalAmount = (double)authorizationAmount.Dollars;
			request.Frequency = requestSettings.Frequency;
			request.Invoice = "Mise Account " + accountID.ToString ();
			request.Memo = "Mise " + MiseAppTypes.StockboyMobile;
			request.TaxAmount = 0;
			request.CardHolderName = name.ToSingleString ();
			request.ProcessCompleteUrl = requestSettings.ProcessCompleteUrl;
			request.ReturnUrl = requestSettings.ReturnUrl;
			request.LogoUrl = requestSettings.LogoUrl;

			//send it
			var service = new MercuryWebService.HCService();
			var result = service.InitializePayment (request);

			if(result != null){
				if(result.ResponseCode == 0){
					return result.PaymentID;
				} else{
					throw new Exception (result.Message);
				}
			}
		}


		public Task<CreditCard> GetCardAfterAuthorization ()
		{
			//do this until we've got an answer, or we run over time
			throw new NotImplementedException ();
		}


		#endregion
	}
}

