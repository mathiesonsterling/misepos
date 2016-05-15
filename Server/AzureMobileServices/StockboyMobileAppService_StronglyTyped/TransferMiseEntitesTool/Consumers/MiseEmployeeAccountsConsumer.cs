using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Common.Entities.Accounts;
using Mise.Core.Entities.Accounts;
using Mise.Core.Services.UtilityServices;
using Mise.Database.AzureDefinitions.Context;

namespace TransferMiseEntitesTool.Consumers
{
    class MiseEmployeeAccountsConsumer : BaseConsumer<IAccount>
    {
        public MiseEmployeeAccountsConsumer(IJSONSerializer jsonSerializer) : base(jsonSerializer)
        {
        }

        protected override Task SaveEntity(StockboyMobileAppServiceContext db, IAccount entity)
        {
            var downAcc = entity as MiseEmployeeAccount;
            if (downAcc != null)
            {
                var dbEnt = new Mise.Database.AzureDefinitions.Entities.Accounts.MiseEmployeeAccount(downAcc);
                db.MiseEmployeeAccounts.Add(dbEnt);

                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }
    }
}
