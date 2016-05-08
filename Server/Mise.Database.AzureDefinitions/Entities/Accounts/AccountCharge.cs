using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities;
using Mise.Core.Entities.Accounts;
using Mise.Database.AzureDefinitions.ValueItems;

namespace Mise.Database.AzureDefinitions.Entities.Accounts
{
    public class AccountCharge : BaseDbEntity<IAccountCharge, Core.Common.Entities.Accounts.AccountCharge>
    {
        public AccountCharge()
        {
            Amount = new MoneyDb();
        }

        public AccountCharge(IAccountCharge source)
        {
            App = source.App;
            AccountID = source.AccountID;
            Amount = new MoneyDb(source.Amount);
            DateStart = source.DateStart;
            DateEnd = source.DateEnd;
        }

        public MiseAppTypes App { get; set; }
        public Guid AccountID { get; set; }
        public MoneyDb Amount { get; set; }
        public DateTimeOffset DateStart { get; set; }
        public DateTimeOffset DateEnd { get; set; }
        protected override Core.Common.Entities.Accounts.AccountCharge CreateConcreteSubclass()
        {
            return new Core.Common.Entities.Accounts.AccountCharge
            {
                App = App,
                AccountID = AccountID,
                Amount = Amount.ToValueItem(),
                DateStart = DateStart,
                DateEnd = DateEnd
            };
        }
    }
}
