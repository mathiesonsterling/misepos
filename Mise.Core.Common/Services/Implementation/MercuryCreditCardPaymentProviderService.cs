using System;
using System.Threading.Tasks;
using Mise.Core.Entities;
using Mise.Core.Entities.Payments;
using Mise.Core.Services;
using Mise.Core.Services.WebServices;
using Mise.Core.ValueItems;

namespace Mise.Core.Common.Services.Implementation
{
    public class MercuryCreditCardProcessorService : ICreditCardProcessorService
    {
        public class MercuryAuthRequest
        {
            public long MerchantID { get; set; }
            public string Password { get; set; }
            public string TranType { get; set; }
            public string PartialAuth { get; set; }
            public decimal TotalAmount { get; set; }
            public string Frequency { get; set; }
            public string Invoice { get; set; }
            public string Memo { get; set; }
            public decimal TaxAmount { get; set; }
            public string CardHolderName { get; set; }
            public string AVSFields { get; set; }
            public string AVSAddress { get; set; }
            public string CVV { get; set; }
            public string CustomerCode { get; set; }
            public Uri ProcessCompleteUrl { get; set; }
            public Uri ReturnUrl { get; set; }
            public TimeSpan PageTimeoutDuration { get; set; }
            public string PageTimeoutIndicator { get; set; }
            public Uri LogoUrl { get; set; }
            public string PageTitle { get; set; }
            public string SubmitButtonText { get; set; }

            public MercuryAuthRequest()
            {

            }

            public MercuryAuthRequest(long merchantID, string password, Guid accountID, Uri processCompleteUrl, Uri returnUrl, Money total, MiseAppTypes app, PersonName name)
            {
                MerchantID = merchantID;
                Password = password;
                TranType = "PreAuth";
                PartialAuth = "off";
                TotalAmount = total.Dollars;
                Frequency = "Recurring";
                Invoice = "Mise Account " + accountID;
                Memo = "Mise " + app;
                TaxAmount = 0.00M;
                CardHolderName = name.ToSingleString();
                ProcessCompleteUrl = processCompleteUrl;
                ReturnUrl = returnUrl;
                LogoUrl = new Uri("http://mise.in/images/logo.png");
            }
        }

        private ICreditCardWebService _webService;
        private MiseAppTypes _appType = MiseAppTypes.StockboyMobile;
        public MercuryCreditCardProcessorService(ICreditCardWebService webService)
        {
            
            _webService = webService;
        }

        public void ChargeCard(ICreditCardPayment payment, CreditCardAuthorizationCode authFromPrevious,
            Action<ICreditCardPayment, CreditCardAuthorizationCode> closeCallback)
        {
            throw new NotImplementedException();
        }

        public void VoidAuthorization(ICreditCardPayment payment, CreditCardAuthorizationCode authFromPrevious,
            Action<ICreditCardPayment, CreditCardAuthorizationCode> closeCallback)
        {
            throw new NotImplementedException();
        }

        public void AuthorizeCard(ICreditCardPayment payment,
            Action<ICreditCardPayment, CreditCardAuthorizationCode> authCallback)
        {
            throw new NotImplementedException();
        }

        public Task<string> SetPaymentID(Guid accountID, PersonName name, Money authorizationAmount)
        {
            //check https://drive.google.com/a/misepos.com/file/d/0B5hRjdt8SvgTT1VHbm5qTDdLRUduZ3o4cDE2V1BtdXJCaGpv/view?usp=sharing for details
            var uri = GetProcessorUri();

            var requestObject = new MercuryAuthRequest(GetMerchantID(), GetPassword(), accountID, GetSuccessUri(),
                GetFailureUri(), authorizationAmount,
                _appType, name);

            return _webService.PostPaymentIDToWebService(GetProcessorUri() ,requestObject);
        }

        public Task<CreditCard> GetCardAfterAuthorization()
        {
            throw new NotImplementedException();
        }

        private Uri GetProcessorUri()
        {
#if DEBUG
            return new Uri("https://hc.mercurydev.net/hcws/hcservice.asmx");
#else
            return new Uri("https://hc.mercurypay.com/hcws/hcservice.asmx");
#endif
        }

        private Uri GetSuccessUri()
        {
            return new Uri("http://mise.in");
        }

        private Uri GetFailureUri()
        {
            return new Uri("http://mise.in");
        }

        private long GetMerchantID()
        {
#if DEBUG
            return 013163015566916;
#else
            return -1;
#endif
        }

        private string GetPassword()
        {
#if DEBUG
            return "password";
#else
            return "password";
#endif
        }
    }
}
