using System;
using System.Threading.Tasks;
using Mise.Core.Services;
using Mise.Core.Entities.Payments;
using Mise.Core.ValueItems;
namespace Services
{
    /*
    /// <summary>
    /// Version to do subscriptions through apple
    /// </summary>
    public class AppleSubscriptionService : ICreditCardProcessorService
    {
        
        private readonly InAppPurchaseManager _purchaseManager;
        public AppleSubscriptionService(InAppPurchaseManager purchaseManager)
        {
            _purchaseManager = purchaseManager;
        }

        private const string SHARED_SECRET = "7aaca5056f2e4455ad9d4c0eefd43ec4";

        #region ICreditCardProcessorService implementation
        public void ChargeCard(ICreditCardPayment payment, CreditCardAuthorizationCode authFromPrevious, Action<ICreditCardPayment, CreditCardAuthorizationCode> closeCallback)
        {
            throw new NotImplementedException();
        }
        public void VoidAuthorization(ICreditCardPayment payment, CreditCardAuthorizationCode authFromPrevious, Action<ICreditCardPayment, CreditCardAuthorizationCode> closeCallback)
        {
            throw new NotImplementedException();
        }
        public void AuthorizeCard(ICreditCardPayment payment, Action<ICreditCardPayment, CreditCardAuthorizationCode> authCallback)
        {
            throw new NotImplementedException();
        }
        public Task<string> SetPaymentID(Guid accountID, PersonName name, Money authorizationAmount)
        {
            throw new NotImplementedException();
        }
        public Task<CreditCard> GetCardAfterAuthorization(string paymentID)
        {
            throw new NotImplementedException();
        }
        public Task<CreditCard> SendCardToProcessorForSubscription(PersonName cardName, CreditCardNumber number)
        {
            //do the apply pay stuff here
            if (_purchaseManager.CanMakePayments)
            {
                _purchaseManager.QueryInventory(new ["in.mise.stockboy.monthly.subscription"]);

            }

            foreach(var product in _purchaseManager)
            {
                if (!product.purchased)
                {
                    _purchaseManager.BuyProduct("in.mise.stockboy.monthly.subscription");
                }
                break;
            }
            throw new NotImplementedException();
        }
        #endregion
    }*/
}

