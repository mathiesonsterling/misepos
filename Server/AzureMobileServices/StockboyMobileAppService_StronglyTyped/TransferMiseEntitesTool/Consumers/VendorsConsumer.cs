using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Common.Entities.Vendors;
using Mise.Core.Services.UtilityServices;
using Mise.Database.AzureDefinitions.Context;
using DBVendor = Mise.Database.AzureDefinitions.Entities.Vendor.Vendor;
namespace TransferMiseEntitesTool.Consumers
{
	class VendorsConsumer : BaseConsumer<Vendor, DBVendor>
	{
		public VendorsConsumer(IJSONSerializer jsonSerializer) : base(jsonSerializer)
		{
		}

	    protected override int BatchSize => 10;

	    public override string EntityName => "Vendors";

	    protected override async Task<DBVendor> SaveEntity(StockboyMobileAppServiceContext db, Vendor entity)
		{
			var rests = await db.Restaurants.Where(r => entity.RestaurantsAssociatedIDs.Contains(r.RestaurantID)).ToListAsync();

			var createdBy = await db.Employees.FirstOrDefaultAsync(e => e.EntityId == entity.CreatedByEmployeeID);

		    var invCategories = await db.InventoryCategories.Where(ic => ic != null).ToListAsync();

			var dbEnt = new DBVendor(entity, createdBy, rests, invCategories);

		    db.Vendors.Add(dbEnt);

		    return dbEnt;
		}

	    protected override Task<DBVendor> GetSavedEntity(StockboyMobileAppServiceContext db, Guid id)
	    {
	        return db.Vendors.FirstOrDefaultAsync(v => v.EntityId == id);
	    }

	}
}