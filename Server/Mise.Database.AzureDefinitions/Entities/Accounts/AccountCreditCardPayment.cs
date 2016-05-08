using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities.Accounts;
using Mise.Core.ValueItems;
using Mise.Database.AzureDefinitions.ValueItems;
using CreditCard = Mise.Database.AzureDefinitions.ValueItems.CreditCard;

namespace Mise.Database.AzureDefinitions.Entities.Accounts
{
    public class AccountCreditCardPayment : BaseDbEntity<IAccountPayment, Core.Common.Entities.Accounts.AccountCreditCardPayment>
    {
        public AccountCreditCardPayment()
        {
            CardUsed = new CreditCard();
            Amount = new MoneyDb();
        }

        public CreditCard CardUsed { get; set; }

        public PaymentProcessingStatus Status { get; set; }

        public Guid AccountID { get; set; }
        public MoneyDb Amount { get; set; }
        protected override Core.Common.Entities.Accounts.AccountCreditCardPayment CreateConcreteSubclass()
        {
            return new Core.Common.Entities.Accounts.AccountCreditCardPayment
            {
                CardUsed = CardUsed.ToValueItem(),
                Status = Status,
                AccountID = AccountID,
                Amount = Amount.ToValueItem()
            };
        }
    }
}
