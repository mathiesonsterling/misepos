using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Common.Entities;
using Mise.Core.Services.UtilityServices;
using Mise.Core.ValueItems;
using Mise.Database.AzureDefinitions.Entities.Inventory;
using Mise.Database.AzureDefinitions.ValueItems;
using TransferMiseEntitesTool.Database.StockboyMobileAppServiceService.Models;

namespace TransferMiseEntitesTool.Consumers
{
  class RestaurantConsumer : BaseConsumer<Restaurant>
  {
    public RestaurantConsumer(IJSONSerializer jsonSerializer) : base(jsonSerializer)
    {
    }

    protected override async Task SaveEntity(DestinationContext db, Restaurant entity)
    {
      //get emails
	    var allRestEmails = entity.GetEmailsToSendInventoryReportsTo().Select(e => e.Value);
	    var fullEmails = await AddAnyMissingEmails(db, allRestEmails);

	    //now also construct all the inventory sections
        var dbEnt = new Mise.Database.AzureDefinitions.Entities.Restaurant.Restaurant(entity, fullEmails);
        db.Restaurants.Add(dbEnt);
    }
  }
}