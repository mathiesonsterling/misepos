using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities.Accounts;
using Mise.Core.Client.ValueItems;

namespace Mise.Core.Client.Entities.Accounts
{
    public class AccountCredit : BaseDbEntity<IAccountPayment, Core.Common.Entities.Accounts.AccountCredit>
    {
        public AccountCredit()
        {
            ReferralCodeGiven = new ReferralCodeDb();
            Amount = new MoneyDb();
        }

        public AccountCredit(Core.Common.Entities.Accounts.AccountCredit source) :base(source)
        {
            ReferralCodeGiven = new ReferralCodeDb {Code = source.ReferralCodeGiven?.Code};
            AccountID = source.AccountID;
            Amount = new MoneyDb(source.Amount);
        }

        /// <summary>
        /// If here, this is the referral code we were given to get this credit
        /// </summary>
        public ReferralCodeDb ReferralCodeGiven { get; set; }

        public Guid AccountID { get; set; }

        public MoneyDb Amount { get; set; }
        protected override Core.Common.Entities.Accounts.AccountCredit CreateConcreteSubclass()
        {
            return new Core.Common.Entities.Accounts.AccountCredit
            {
                AccountID = AccountID,
                Amount = Amount.ToValueItem(),
                ReferralCodeGiven = ReferralCodeGiven.ToValueItem()
            };
        }
    }
}
