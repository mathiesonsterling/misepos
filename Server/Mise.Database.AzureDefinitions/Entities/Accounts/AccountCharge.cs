using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities;
using Mise.Core.Entities.Accounts;
using Mise.Core.ValueItems;
using Mise.Database.AzureDefinitions.ValueItems;

namespace Mise.Database.AzureDefinitions.Entities.Accounts
{
    public class AccountCharge : BaseDbEntity<IAccountCharge, Core.Common.Entities.Accounts.AccountCharge>
    {
        public AccountCharge()
        {
        }

        public AccountCharge(IAccountCharge source, RestaurantAccount account) :base(source)
        {
            App = source.App;
            RestaurantAccountId = source.AccountID.ToString();
            Amount = source.Amount.Dollars;
            DateStart = source.DateStart;
            DateEnd = source.DateEnd;

            RestaurantAccount = account;
        }

        public MiseAppTypes App { get; set; }

        public RestaurantAccount RestaurantAccount { get; set; }
        [ForeignKey("RestaurantAccount")]
        public string RestaurantAccountId { get; set; }

        public decimal Amount { get; set; }
        public DateTimeOffset DateStart { get; set; }
        public DateTimeOffset DateEnd { get; set; }
        protected override Core.Common.Entities.Accounts.AccountCharge CreateConcreteSubclass()
        {
            return new Core.Common.Entities.Accounts.AccountCharge
            {
                App = App,
                AccountID = Guid.Parse(RestaurantAccountId),
                Amount = new Money(Amount),
                DateStart = DateStart,
                DateEnd = DateEnd
            };
        }
    }
}
