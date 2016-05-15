using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Common.Entities;
using Mise.Core.Services.UtilityServices;
using Mise.Database.AzureDefinitions.Context;

namespace TransferMiseEntitesTool.Consumers
{
  class RestaurantConsumer : BaseConsumer<Restaurant>
  {
    public RestaurantConsumer(IJSONSerializer jsonSerializer) : base(jsonSerializer)
    {
    }

    protected override async Task SaveEntity(StockboyMobileAppServiceContext db, Restaurant entity)
    {

	    //now also construct all the inventory sections
        var dbEnt = new Mise.Database.AzureDefinitions.Entities.Restaurant.Restaurant(entity);
        db.Restaurants.Add(dbEnt);
    }

      protected override Task<bool> DoesEntityExist(StockboyMobileAppServiceContext db, Guid id)
      {
          return db.Restaurants.AnyAsync(r => r.EntityId == id);
      }
  }
}