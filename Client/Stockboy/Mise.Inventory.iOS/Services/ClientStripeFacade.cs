using System;
using System.Threading.Tasks;

using Mise.Core.ValueItems;
using Mise.Core.Common.Services;
using Mise.Core.Common.Services.Implementation;
namespace Mise.Inventory.iOS
{
    public class ClientStripeFacade : IClientStripeFacade
    {
        public ClientStripeFacade(){
            var settings = StripePaymentProviderSettingsFactory.GetSettings();
            Stripe.StripeClient.DefaultPublishableKey = settings.PublishableKey;
        }

        #region IClientStripeFacade implementation

        public async Task<CreditCardProcessorToken> SendForToken(PersonName cardName, CreditCardNumber number)
        {
            var stripeCard = new Stripe.Card
                {
                    Name = cardName.ToSingleString(),
                    AddressZip = number.BillingZip.ToString(),
                    CVC = number.CVC.ToString(),
                    Number = number.Number,
                    ExpiryMonth = number.ExpMonth,
                    ExpiryYear = number.ExpYear
                };

            var token = await Stripe.StripeClient.CreateToken(stripeCard);

            var mToken = new CreditCardProcessorToken{
                Processor = CreditCardProcessors.Stripe,
                Token = token.Id
            };

            return mToken;
        }

        #endregion
    }
}

