using System;
using System.Threading.Tasks;
using Mise.Core.Entities.Payments;
using Mise.Core.Services;
using Mise.Core.ValueItems;

namespace Mise.Inventory.iOS.Services
{
    public class NoProcessorService : ICreditCardProcessorService
    {
        public void AuthorizeCard (ICreditCardPayment payment, Action<ICreditCardPayment, CreditCardAuthorizationCode> authCallback)
        {
            throw new NotImplementedException ();
        }

        public void ChargeCard (ICreditCardPayment payment, CreditCardAuthorizationCode authFromPrevious, Action<ICreditCardPayment, CreditCardAuthorizationCode> closeCallback)
        {
            throw new NotImplementedException ();
        }

        public Task<CreditCard> GetCardAfterAuthorization (string paymentID)
        {
            throw new NotImplementedException ();
        }

        public Task<CreditCard> SendCardToProcessorForSubscription (PersonName cardName, CreditCardNumber number)
        {
            throw new NotImplementedException ();
        }

        public Task<string> SetPaymentID (Guid accountID, PersonName name, Money authorizationAmount)
        {
            throw new NotImplementedException ();
        }

        public void VoidAuthorization (ICreditCardPayment payment, CreditCardAuthorizationCode authFromPrevious, Action<ICreditCardPayment, CreditCardAuthorizationCode> closeCallback)
        {
            throw new NotImplementedException ();
        }
    }
}

