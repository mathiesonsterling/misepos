using Mise.Core.Entities.Accounts;
using Mise.Core.ValueItems;

namespace Mise.Core.Client.Entities.Accounts
{
    public class AccountCreditCardPayment : BaseDbEntity<IAccountPayment, Core.Common.Entities.Accounts.AccountCreditCardPayment>
    {
        public AccountCreditCardPayment()
        {
            CardUsed = new Core.Client.ValueItems.CreditCard();
        }

        public AccountCreditCardPayment(Core.Common.Entities.Accounts.AccountCreditCardPayment source, RestaurantAccount acct)
	    	:base(source)
        {
            CardUsed = new Core.Client.ValueItems.CreditCard(source.CardUsed);
            Status = source.Status;
            RestaurantAccount = acct;
            RestaurantAccountId = acct.Id;
            Amount = source.Amount.Dollars;
        }

        public Core.Client.ValueItems.CreditCard CardUsed { get; set; }

        public PaymentProcessingStatus Status { get; set; }

        public RestaurantAccount RestaurantAccount { get; set; }
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
