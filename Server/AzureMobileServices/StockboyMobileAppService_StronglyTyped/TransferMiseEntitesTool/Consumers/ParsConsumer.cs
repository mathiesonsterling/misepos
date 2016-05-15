using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities.Inventory;
using Mise.Core.Services.UtilityServices;
using Mise.Database.AzureDefinitions.Context;
using Mise.Database.AzureDefinitions.Entities.Inventory;

namespace TransferMiseEntitesTool.Consumers
{
    class ParsConsumer : BaseConsumer<IPar>
    {
        public ParsConsumer(IJSONSerializer jsonSerializer) : base(jsonSerializer)
        {
        }

        protected override async Task SaveEntity(StockboyMobileAppServiceContext db, IPar entity)
        {
            var rest = await db.Restaurants.FirstOrDefaultAsync(r => entity.RestaurantID == r.EntityId);
            var invCats = await db.InventoryCategories.Where(ic => ic != null).ToListAsync();
            var creatingEmployee = await db.Employees.FirstOrDefaultAsync(e => e.EntityId == entity.CreatedByEmployeeID);
            var dbEnt = new Par(entity, rest, creatingEmployee, invCats);

            db.Pars.Add(dbEnt);
        }

        protected override Task<bool> DoesEntityExist(StockboyMobileAppServiceContext db, Guid id)
        {
            return db.Pars.AnyAsync(p => p.EntityId == id);
        }
    }
}
