using System;
using Mise.Core.Services;

namespace Services
{
    /// <summary>
    /// Version to do subscriptions through apple
    /// </summary>
    public class AppleSubscriptionService : ICreditCardProcessorService
    {
        #region ICreditCardProcessorService implementation
        public void ChargeCard(Mise.Core.Entities.Payments.ICreditCardPayment payment, Mise.Core.ValueItems.CreditCardAuthorizationCode authFromPrevious, Action<Mise.Core.Entities.Payments.ICreditCardPayment, Mise.Core.ValueItems.CreditCardAuthorizationCode> closeCallback)
        {
            throw new NotImplementedException();
        }
        public void VoidAuthorization(Mise.Core.Entities.Payments.ICreditCardPayment payment, Mise.Core.ValueItems.CreditCardAuthorizationCode authFromPrevious, Action<Mise.Core.Entities.Payments.ICreditCardPayment, Mise.Core.ValueItems.CreditCardAuthorizationCode> closeCallback)
        {
            throw new NotImplementedException();
        }
        public void AuthorizeCard(Mise.Core.Entities.Payments.ICreditCardPayment payment, Action<Mise.Core.Entities.Payments.ICreditCardPayment, Mise.Core.ValueItems.CreditCardAuthorizationCode> authCallback)
        {
            throw new NotImplementedException();
        }
        public System.Threading.Tasks.Task<string> SetPaymentID(Guid accountID, Mise.Core.ValueItems.PersonName name, Mise.Core.ValueItems.Money authorizationAmount)
        {
            throw new NotImplementedException();
        }
        public System.Threading.Tasks.Task<Mise.Core.ValueItems.CreditCard> GetCardAfterAuthorization(string paymentID)
        {
            throw new NotImplementedException();
        }
        public System.Threading.Tasks.Task<Mise.Core.ValueItems.CreditCard> SendCardToProcessorForSubscription(Mise.Core.ValueItems.PersonName cardName, Mise.Core.ValueItems.CreditCardNumber number)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}

