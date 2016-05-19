using System;
using System.Data.Entity;
using System.Threading.Tasks;
using Mise.Core.Common.Entities.Accounts;
using Mise.Core.Services.UtilityServices;
using Mise.Database.AzureDefinitions.Context;
using DbAcct = Mise.Database.AzureDefinitions.Entities.Accounts.RestaurantAccount;
namespace TransferMiseEntitesTool.Consumers
{
    class RestaurantAccountConsumer : BaseConsumer<RestaurantAccount, DbAcct>
    {
        public RestaurantAccountConsumer(IJSONSerializer jsonSerializer) : base(jsonSerializer)
        {

        }

        protected override Task<DbAcct> SaveEntity(StockboyMobileAppServiceContext db, RestaurantAccount entity)
        {
            var dbEnt = new DbAcct(entity);
            db.RestaurantAccounts.Add(dbEnt);

            return Task.FromResult(dbEnt);
        }

        protected override Task<DbAcct> GetSavedEntity(StockboyMobileAppServiceContext db, Guid id)
        {
            return db.RestaurantAccounts.FirstOrDefaultAsync(ra => ra.EntityId == id);
        }
    }
}
