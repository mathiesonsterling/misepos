using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Common.Entities.Accounts;
using Mise.Core.Common.Entities.DTOs;
using Mise.Core.Services.UtilityServices;
using TransferMiseEntitesTool.StockboyMobileAppServiceService.Models;

namespace TransferMiseEntitesTool.Consumers
{
    class RestaurantAccountConsumer : BaseConsumer<RestaurantAccount>
    {
        public RestaurantAccountConsumer(IJSONSerializer jsonSerializer) : base(jsonSerializer)
        {

        }

        protected override void SaveEntity(DestinationContext db, RestaurantAccount entity)
        {
            var dbEnt = new Mise.Database.AzureDefinitions.Entities.Accounts.RestaurantAccount(entity);
            db.RestaurantAccounts.Add(dbEnt);
        }
    }
}
