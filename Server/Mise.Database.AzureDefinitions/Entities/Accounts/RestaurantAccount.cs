using System;
using System.Collections.Generic;
using System.Linq;
using Mise.Core.Entities.Accounts;
using Mise.Core.ValueItems;
using CreditCard = Mise.Database.AzureDefinitions.ValueItems.CreditCard;

namespace Mise.Database.AzureDefinitions.Entities.Accounts
{
    public class RestaurantAccount : BaseAccountEntity<IBusinessAccount, Core.Common.Entities.Accounts.RestaurantAccount>
    {
        public RestaurantAccount()
        {
            CurrentCard = new CreditCard();
        }
     
        public RestaurantAccount(Core.Common.Entities.Accounts.RestaurantAccount source) : base(source)
        {
            BillingCycle = source.BillingCycle;
            CurrentCard = new CreditCard(source.CurrentCard);
            PaymentPlan = source.PaymentPlan;
            PaymentPlanSetupWithProvider = source.PaymentPlanSetupWithProvider;

            Charges = source.Charges.Select(c => new AccountCharge(c)).ToList();
            CreditCardPayments = source.Payments.Select(p => new AccountCreditCardPayment(p)).ToList();
            Credits = source.AccountCredits.Select(c => new AccountCredit(c)).ToList();
        }

        public TimeSpan BillingCycle { get; set; }

        public CreditCard CurrentCard { get; set; }

        public MisePaymentPlan PaymentPlan
        {
            get;
            set;
        }

        public bool PaymentPlanSetupWithProvider
        {
            get;
            set;
        }

        public List<AccountCharge> Charges { get; set; }
        public List<AccountCreditCardPayment> CreditCardPayments { get; set; }
        public List<AccountCredit> Credits { get; set; }
          
        protected override Core.Common.Entities.Accounts.RestaurantAccount CreateAccountSubobject()
        {
            return new Core.Common.Entities.Accounts.RestaurantAccount
            {
                BillingCycle = BillingCycle,
                CurrentCard = CurrentCard.ToValueItem(),
                PaymentPlan = PaymentPlan,
                PaymentPlanSetupWithProvider = PaymentPlanSetupWithProvider,
                Charges = Charges.Select(c => c.ToBusinessEntity()).ToList(),
                AccountCredits = Credits.Select(c => c.ToBusinessEntity()).ToList(),
                Payments = CreditCardPayments.Select(c => c.ToBusinessEntity()).ToList()
            };
        }
    }
}
