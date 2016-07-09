using Mise.Core.Entities.Accounts;
using Mise.Core.ValueItems;

namespace Mise.Core.Client.Entities.Accounts
{
    public class AccountCredit : BaseDbEntity<IAccountPayment, Core.Common.Entities.Accounts.AccountCredit>
    {
        public AccountCredit()
        {
        }

        public AccountCredit(Core.Common.Entities.Accounts.AccountCredit source, RestaurantAccount acct) :base(source)
        {
            ReferralCodeGiven = source.ReferralCodeGiven?.Code;
            RestaurantAccount = acct;
            RestaurantAccountId = acct.Id;
            Amount = source.Amount.Dollars;
        }

        /// <summary>
        /// If here, this is the referral code we were given to get this credit
        /// </summary>
        public string ReferralCodeGiven { get; set; }

        public string RestaurantAccountId { get; set; }
        public RestaurantAccount RestaurantAccount { get; set; }

        public decimal Amount { get; set; }
        protected override Core.Common.Entities.Accounts.AccountCredit CreateConcreteSubclass()
        {
            return new Core.Common.Entities.Accounts.AccountCredit
            {
                AccountID = RestaurantAccount.EntityId,
                Amount = new Money(Amount),
                ReferralCodeGiven = new ReferralCode(ReferralCodeGiven)
            };
        }
    }
}
