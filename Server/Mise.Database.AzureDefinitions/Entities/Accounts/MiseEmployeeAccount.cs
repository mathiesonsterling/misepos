using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities.Accounts;
using Mise.Database.AzureDefinitions.ValueItems;

namespace Mise.Database.AzureDefinitions.Entities.Accounts
{
    public class MiseEmployeeAccount : BaseAccountEntity<IAccount, Core.Common.Entities.Accounts.MiseEmployeeAccount>
    {
        public MiseEmployeeAccount() { }

        public MiseEmployeeAccount(Core.Common.Entities.Accounts.MiseEmployeeAccount source) : base(source)
        {
            
        }

        protected override Core.Common.Entities.Accounts.MiseEmployeeAccount CreateAccountSubobject()
        {
            return new Core.Common.Entities.Accounts.MiseEmployeeAccount();
        }
    }
}
