using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Common.Entities;
using Mise.Core.Services.UtilityServices;
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

	    var alreadyHave = await db.Emails.Where(e => allRestEmails.Contains(e.Value)).ToListAsync();

	    //create the others
	    var needCreating = allRestEmails
		    .Where(ne => !(alreadyHave.Select(e => e.Value).Contains(ne.Value)))
		    .Select(e => new EmailAddressDb {Value = e.Value});
	    foreach (var newEmail in needCreating)
	    {
		    db.Emails.Add(newEmail);
	    }
	    await db.SaveChangesAsync();

	    var fullEmails = await db.Emails.Where(e => allRestEmails.Contains(e.Value)).ToListAsync();

	    //now also construct all the inventory sections
        var dbEnt = new Mise.Database.AzureDefinitions.Entities.Restaurant.Restaurant(entity, fullEmails);
        db.Restaurants.Add(dbEnt);
    }
  }
}