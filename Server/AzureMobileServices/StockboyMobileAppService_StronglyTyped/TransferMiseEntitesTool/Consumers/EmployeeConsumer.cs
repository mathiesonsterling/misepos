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

		protected override Task SaveEntity(StockboyMobileAppServiceContext db, Employee entity)
		{
			var dbEnt = new Mise.Database.AzureDefinitions.Entities.People.Employee(entity);
		    db.Employees.Add(dbEnt);

            return Task.FromResult(true);
		}
	}
}