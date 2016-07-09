using System;
using System.Data.Entity;
using System.Threading.Tasks;
using Mise.Core.Common.Entities;
using Mise.Core.Services.UtilityServices;
using Mise.Database.AzureDefinitions.Context;
using Mise.Database.AzureDefinitions.Entities.Accounts;
using dbRest = Mise.Database.AzureDefinitions.Entities.Restaurant.Restaurant;
namespace TransferMiseEntitesTool.Consumers
{
    class RestaurantConsumer : BaseConsumer<Restaurant, dbRest>
    {
        public RestaurantConsumer(IJSONSerializer jsonSerializer) : base(jsonSerializer)
        {
        }

        public override string EntityName => "Restaurant";

        protected override async Task<dbRest> SaveEntity(StockboyMobileAppServiceContext db, Restaurant entity)
        {
            RestaurantAccount acct = null;
            if (entity.AccountID.HasValue)
            {
                acct = await db.RestaurantAccounts.FirstOrDefaultAsync(a => a.EntityId == entity.AccountID.Value);
            }
            //now also construct all the inventory sections
            var dbEnt = new dbRest(entity, acct);
            db.Restaurants.Add(dbEnt);
            return dbEnt;
        }

        protected override Task<dbRest> GetSavedEntity(StockboyMobileAppServiceContext db, Guid id)
        {
            return db.Restaurants.FirstOrDefaultAsync(r => r.EntityId == id);
        }
    }
}