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
      //get emails
	    var allRestEmails = entity.GetEmailsToSendInventoryReportsTo();
	    var fullEmails = await db.GetEmailEntities(allRestEmails);

	    //now also construct all the inventory sections
        var dbEnt = new Mise.Database.AzureDefinitions.Entities.Restaurant.Restaurant(entity, fullEmails.ToList());
        db.Restaurants.Add(dbEnt);
    }
  }
}