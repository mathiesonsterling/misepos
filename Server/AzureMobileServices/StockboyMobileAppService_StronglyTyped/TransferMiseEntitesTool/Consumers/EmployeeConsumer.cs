using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Common.Entities.People;
using Mise.Core.Services.UtilityServices;
using Mise.Database.AzureDefinitions.Context;

namespace TransferMiseEntitesTool.Consumers
{
	class EmployeeConsumer :BaseConsumer<Employee>
	{
		public EmployeeConsumer(IJSONSerializer jsonSerializer) : base(jsonSerializer)
		{
		}

		protected override async Task SaveEntity(StockboyMobileAppServiceContext db, Employee entity)
		{
		    var restIds = entity.GetRestaurantIDs() ?? new List<Guid>();
		    var rests = await db.Restaurants.Where(r => restIds.Contains(r.RestaurantID)).ToListAsync();
			var dbEnt = new Mise.Database.AzureDefinitions.Entities.People.Employee(entity, rests);
		    db.Employees.Add(dbEnt);
		}

	    protected override Task<bool> DoesEntityExist(StockboyMobileAppServiceContext db, Guid id)
	    {
	        return db.Employees.AnyAsync(e => e.EntityId == id);
	    }
	}
}