using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Common.Entities.Vendors;
using Mise.Core.Services.UtilityServices;
using Mise.Database.AzureDefinitions.Context;

namespace TransferMiseEntitesTool.Consumers
{
	class VendorsConsumer : BaseConsumer<Vendor>
	{
		public VendorsConsumer(IJSONSerializer jsonSerializer) : base(jsonSerializer)
		{
		}

		protected override async Task SaveEntity(StockboyMobileAppServiceContext db, Vendor entity)
		{
			var rests = await db.Restaurants.Where(r => entity.RestaurantsAssociatedIDs.Contains(r.RestaurantID)).ToListAsync();

			var createdBy = await db.Employees.FirstOrDefaultAsync(e => e.EntityId == entity.CreatedByEmployeeID);

		    var invCategories = await db.InventoryCategories.Where(ic => ic != null).ToListAsync();

			var dbEnt = new Mise.Database.AzureDefinitions.Entities.Vendor.Vendor(entity, createdBy, rests, invCategories);

		    db.Vendors.Add(dbEnt);
		}
	}
}