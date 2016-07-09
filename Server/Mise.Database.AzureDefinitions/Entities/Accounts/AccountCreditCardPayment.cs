using System.ComponentModel.DataAnnotations.Schema;
using Mise.Core.Entities.Accounts;
using Mise.Core.ValueItems;
using CreditCard = Mise.Database.AzureDefinitions.ValueItems.CreditCard;

namespace Mise.Database.AzureDefinitions.Entities.Accounts
{
    public class AccountCreditCardPayment : BaseDbEntity<IAccountPayment, Core.Common.Entities.Accounts.AccountCreditCardPayment>
    {
        public AccountCreditCardPayment()
        {
            CardUsed = new CreditCard();
        }

        public AccountCreditCardPayment(Core.Common.Entities.Accounts.AccountCreditCardPayment source, RestaurantAccount acct)
	    	:base(source)
        {
            CardUsed = new CreditCard(source.CardUsed);
            Status = source.Status;
            RestaurantAccount = acct;
            RestaurantAccountId = acct.Id;
            Amount = source.Amount.Dollars;
        }

        public CreditCard CardUsed { get; set; }

        public PaymentProcessingStatus Status { get; set; }

        public RestaurantAccount RestaurantAccount { get; set; }
        [ForeignKey("RestaurantAccount")]
        public string RestaurantAccountId { get; set; }
        public decimal Amount { get; set; }
        protected override Core.Common.Entities.Accounts.AccountCreditCardPayment CreateConcreteSubclass()
        {
            return new Core.Common.Entities.Accounts.AccountCreditCardPayment
            {
                CardUsed = CardUsed.ToValueItem(),
                Status = Status,
                AccountID = RestaurantAccount.EntityId,
                Amount = new Money(Amount)
            };
        }
    }
}
