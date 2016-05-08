using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities.Accounts;

namespace Mise.Database.AzureDefinitions.Entities.Accounts
{
    public class MiseEmployeeAccount : BaseAccountEntity<IAccount, Core.Common.Entities.Accounts.MiseEmployeeAccount>
    {
        protected override Core.Common.Entities.Accounts.MiseEmployeeAccount CreateAccountSubobject()
        {
            return new Core.Common.Entities.Accounts.MiseEmployeeAccount();
        }
    }
}
