using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Common.Entities.Accounts;
using Mise.Core.Services.UtilityServices;
using Mise.Database.AzureDefinitions.Context;

namespace TransferMiseEntitesTool.Consumers
{
    class RestaurantAccountConsumer : BaseConsumer<RestaurantAccount>
    {
        public RestaurantAccountConsumer(IJSONSerializer jsonSerializer) : base(jsonSerializer)
        {

        }

        protected override async Task SaveEntity(StockboyMobileAppServiceContext db, RestaurantAccount entity)
        {
            var emails = await db.GetEmailEntities(entity.Emails);
            var dbEnt = new Mise.Database.AzureDefinitions.Entities.Accounts.RestaurantAccount(entity, emails.ToList());
            db.RestaurantAccounts.Add(dbEnt);
        }
    }
}
