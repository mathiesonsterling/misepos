using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Common.Entities;
using Mise.Core.Services.UtilityServices;
using Mise.Database.AzureDefinitions.Context;
using dbRest = Mise.Database.AzureDefinitions.Entities.Restaurant.Restaurant;
namespace TransferMiseEntitesTool.Consumers
{
  class RestaurantConsumer : BaseConsumer<Restaurant, dbRest>
  {
    public RestaurantConsumer(IJSONSerializer jsonSerializer) : base(jsonSerializer)
    {
    }

    protected override Task<dbRest> SaveEntity(StockboyMobileAppServiceContext db, Restaurant entity)
    {
	    //now also construct all the inventory sections
        var dbEnt = new dbRest(entity);
        db.Restaurants.Add(dbEnt);
        return Task.FromResult(dbEnt);
    }

      protected override Task<dbRest> GetSavedEntity(StockboyMobileAppServiceContext db, Guid id)
      {
          return db.Restaurants.FirstOrDefaultAsync(r => r.EntityId == id);
      }
  }
}