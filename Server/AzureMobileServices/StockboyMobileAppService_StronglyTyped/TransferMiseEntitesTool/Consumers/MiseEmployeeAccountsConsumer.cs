
using System.Linq;
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

        protected override async Task SaveEntity(StockboyMobileAppServiceContext db, IAccount entity)
        {
            var downAcc = entity as MiseEmployeeAccount;

            if (downAcc != null)
            {
                var emails = (await db.GetEmailEntities(entity.Emails)).ToList();
                var dbEnt = new Mise.Database.AzureDefinitions.Entities.Accounts.MiseEmployeeAccount(downAcc, emails);
                db.MiseEmployeeAccounts.Add(dbEnt);
            }
        }
    }
}
