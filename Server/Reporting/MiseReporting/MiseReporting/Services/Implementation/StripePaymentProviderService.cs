using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Common.Services.Implementation;
using Mise.Core.Entities.Accounts;
using Mise.Core.ValueItems;
using Stripe;
namespace MiseReporting.Services.Implementation
{
    public class StripePaymentProviderService : IPaymentProviderService
    {
        private readonly IStripePaymentProviderSettings _settings;
        public StripePaymentProviderService()
        {
            _settings = StripePaymentProviderSettingsFactory.GetSettings();
            //StripeConfiguration.SetApiKey(_settings.PrivateKey);
        }

        public async Task CreateSubscriptionForAccount(IAccount account)
        {
            var card = account.CurrentCard;
            if (card == null)
            {
                throw new ArgumentException("Account does not have a credit card!");
            }

            if (card.ProcessorToken == null)
            {
                throw new ArgumentException("Processor Token is not set");
            }
            if (card.ProcessorToken.Processor != CreditCardProcessors.Stripe)
            {
                throw new ArgumentException("Processor Token is not for Stripe!");
            }

            var planName = account.PaymentPlan.ToString();

            //make the customer if they don't exist
            var customers = (await GetCustomersWithEmail(account.PrimaryEmail)).ToList();
            StripeCustomer customer;
            if (!customers.Any())
            {
                customer = CreateCustomer(account, card, planName);
            }
            else
            {
                customer = customers.FirstOrDefault();
                AddSubscriptionToExistingCustomer(customer, planName);
            }
        }

        public async Task CancelSubscriptionForAccount(IAccount account)
        {
            var customers = await GetCustomersWithEmail(account.PrimaryEmail);
            var subscriptionService = new StripeSubscriptionService();

            //get all the subscriptions
            foreach (var customer in customers)
            {
                var subscriptions = subscriptionService.List(customer.Id);
                foreach (var sub in subscriptions)
                {
                    subscriptionService.Cancel(customer.Id, sub.Id);
                }
            }
        }

        private static void AddSubscriptionToExistingCustomer(StripeObject customer, string planName)
        {
            var subscriptionService = new StripeSubscriptionService();
            subscriptionService.Create(customer.Id, planName);
        }

        private StripeCustomer CreateCustomer(IAccount account, CreditCard card, string planName)
        {
            var source = new StripeSourceOptions
            {
                TokenId = card.ProcessorToken.Token
            };
            //create
            var myCustomer = new StripeCustomerCreateOptions
            {
                Email = account.PrimaryEmail.Value,
                Description = account.AccountHolderName.ToSingleString(),
                Source = source,
                PlanId = planName,
                TaxPercent = _settings.SalesTaxRate,
            };

            var customerService = new StripeCustomerService();
            var stripeCustomer = customerService.Create(myCustomer);
            return stripeCustomer;
        }

        private static Task<IEnumerable<StripeCustomer>> GetCustomersWithEmail(EmailAddress email)
        {
            var serv = new StripeCustomerService();
            var customers = serv.List();
            var matching = customers.Where(c => c.Email == email.Value);
            return Task.FromResult(matching);
        }
    }
}
