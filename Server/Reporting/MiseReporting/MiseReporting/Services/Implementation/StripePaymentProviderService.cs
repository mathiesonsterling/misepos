using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities.Accounts;
using Mise.Core.ValueItems;
using Stripe;
namespace MiseReporting.Services.Implementation
{
    public class StripePaymentProviderService : IPaymentProviderService
    {
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
            }

            var subscriptionService = new StripeSubscriptionService();
            var subscription = subscriptionService.Create(customer.Id, planName);
        }

        private static StripeCustomer CreateCustomer(IAccount account, CreditCard card, string planName)
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
                TaxPercent = 8.875M,
            };

            var customerService = new StripeCustomerService();
            var stripeCustomer = customerService.Create(myCustomer);
            return stripeCustomer;
        }

        private Task<IEnumerable<StripeCustomer>> GetCustomersWithEmail(EmailAddress email)
        {
            var serv = new StripeCustomerService();
            var customers = serv.List();
            var matching = customers.Where(c => c.Email == email.Value);
            return Task.FromResult(matching);
        }
    }
}
