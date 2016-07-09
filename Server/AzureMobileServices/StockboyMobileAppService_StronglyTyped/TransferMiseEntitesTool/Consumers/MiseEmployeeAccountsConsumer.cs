using System;
using System.Data.Entity;
using System.Threading.Tasks;
using Mise.Core.Common.Entities.Accounts;
using Mise.Core.Entities.Accounts;
using Mise.Core.Services.UtilityServices;
using Mise.Database.AzureDefinitions.Context;
using DBAcc = Mise.Database.AzureDefinitions.Entities.Accounts.MiseEmployeeAccount;
namespace TransferMiseEntitesTool.Consumers
{
    class MiseEmployeeAccountsConsumer : BaseConsumer<MiseEmployeeAccount, DBAcc>
    {
        public MiseEmployeeAccountsConsumer(IJSONSerializer jsonSerializer) : base(jsonSerializer)
        {
        }

        public override string EntityName => "MiseEmployeeAccounts";

        protected override Task<DBAcc> SaveEntity(StockboyMobileAppServiceContext db, MiseEmployeeAccount entity)
        {
            var downAcc = entity;

            if (downAcc != null)
            {
                var dbEnt = new DBAcc(downAcc);
                db.MiseEmployeeAccounts.Add(dbEnt);

                return Task.FromResult(dbEnt);
            }

            throw new ArgumentException("Account is not a MiseEmployeeAccount");
        }

        protected override Task<DBAcc> GetSavedEntity(StockboyMobileAppServiceContext db, Guid id)
        {
            return db.MiseEmployeeAccounts.FirstOrDefaultAsync(me => me.EntityId == id);
        }
    }
}
