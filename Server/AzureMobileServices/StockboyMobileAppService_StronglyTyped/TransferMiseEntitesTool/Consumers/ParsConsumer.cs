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
using dbPar = Mise.Database.AzureDefinitions.Entities.Inventory.Par;
namespace TransferMiseEntitesTool.Consumers
{
    class ParsConsumer : BaseConsumer<Mise.Core.Common.Entities.Inventory.Par, dbPar>
    {
        public ParsConsumer(IJSONSerializer jsonSerializer) : base(jsonSerializer)
        {
        }

        public override string EntityName => "Par";

        protected override async Task<dbPar> SaveEntity(StockboyMobileAppServiceContext db, 
            Mise.Core.Common.Entities.Inventory.Par entity)
        {
            var rest = await db.Restaurants.FirstOrDefaultAsync(r => entity.RestaurantID == r.EntityId);
            var invCats = await db.InventoryCategories.Where(ic => ic != null).ToListAsync();
            var creatingEmployee = await db.Employees.FirstOrDefaultAsync(e => e.EntityId == entity.CreatedByEmployeeID);
            var dbEnt = new Par(entity, rest, creatingEmployee, invCats);

            db.Pars.Add(dbEnt);

            return dbEnt;
        }

        protected override Task<Par> GetSavedEntity(StockboyMobileAppServiceContext db, Guid id)
        {
            return db.Pars.FirstOrDefaultAsync(p => p.EntityId == id);
        }
    }
}
