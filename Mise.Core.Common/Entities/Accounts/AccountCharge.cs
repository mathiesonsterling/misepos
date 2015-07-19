using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities;
using Mise.Core.Entities.Accounts;
using Mise.Core.Entities.Base;
using Mise.Core.ValueItems;

namespace Mise.Core.Common.Entities.Accounts
{
    public class AccountCharge : EntityBase, IAccountCharge, ICloneableEntity, ITextSearchable
    {
        public MiseAppTypes App { get; set; }
        public Guid AccountID { get; set; }
        public Money Amount { get; set; }
        public DateTimeOffset DateStart { get; set; }
        public DateTimeOffset DateEnd { get; set; }

        public ICloneableEntity Clone()
        {
            var item = CloneEntityBase(new AccountCharge());
            item.App = App;
            item.AccountID = AccountID;
            item.Amount = Amount;

            return item;
        }

        public bool ContainsSearchString(string searchString)
        {
            return App.ToString().Contains(searchString)
                   || Amount.ContainsSearchString(searchString)
                   || CreatedDate.ToString().Contains(searchString);
        }
    }
}
