using System;
using System.Threading.Tasks;
using Mise.Core.Entities.Payments;
using Mise.Core.Services;
using Mise.Core.Services.UtilityServices;
using Mise.Core.ValueItems;

namespace Mise.Core.Common.Services.Implementation
{
    public class StripePaymentProcessorService : ICreditCardProcessorService
    {
        private readonly ILogger _logger;
        private readonly IClientStripeFacade _stripeClient;
        public StripePaymentProcessorService(ILogger logger, IClientStripeFacade stripeClient){
            _logger = logger;
            _stripeClient = stripeClient;
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
            throw new NotImplementedException ();
        }
        public Task<CreditCard> GetCardAfterAuthorization (string paymentID)
        {
            throw new NotImplementedException ();
        }

        public async Task<CreditCard> SendCardToProcessorForSubscription (PersonName cardName, CreditCardNumber number)
        {
            try{
                var mToken = await _stripeClient.SendForToken(cardName, number);
                //send the number - we won't store, but we want a unique hash
                var card = new CreditCard {Name = cardName, ProcessorToken = mToken};

                return card;
            } catch(Exception e){
                _logger.HandleException(e);
                throw;
            }
        }
        #endregion
    }
}

