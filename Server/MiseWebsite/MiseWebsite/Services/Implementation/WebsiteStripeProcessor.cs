using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Mise.Core.Common.Services;
using Mise.Core.Common.Services.Implementation;
using Mise.Core.ValueItems;
using Stripe;

namespace MiseWebsite.Services.Implementation
{
    public class WebsiteStripeProcessor : IClientStripeFacade
    {
        public WebsiteStripeProcessor()
        {
            var settings = StripePaymentProviderSettingsFactory.GetSettings();
            StripeConfiguration.SetApiKey(settings.PrivateKey);
        }
        public async Task<CreditCardProcessorToken> SendForToken(PersonName cardName, CreditCardNumber number)
        {
            var stripeCard = new Stripe.StripeCreditCardOptions
            {
                Name = cardName.ToSingleString(),
                AddressZip = number.BillingZip.ToString(),
                Cvc = number.CVC.ToString(),
                Number = number.Number,
                ExpirationMonth = number.ExpMonth.ToString(),
                ExpirationYear = number.ExpYear.ToString()
            };

            var tokenService = new StripeTokenService();

            var tokenOptions = new StripeTokenCreateOptions
            {
                Card = stripeCard
            };
            var token = await tokenService.CreateAsync(tokenOptions);

            var mToken = new CreditCardProcessorToken
            {
                Processor = CreditCardProcessors.Stripe,
                Token = token.ToString()
            };

            return mToken;
        }
    }
}