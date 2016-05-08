using System.Threading.Tasks;
using Mise.Core.Common.Entities.People;
using Mise.Core.Services.UtilityServices;
using TransferMiseEntitesTool.Database.StockboyMobileAppServiceService.Models;

namespace TransferMiseEntitesTool.Consumers
{
	class EmployeeConsumer :BaseConsumer<Employee>
	{
		public EmployeeConsumer(IJSONSerializer jsonSerializer) : base(jsonSerializer)
		{
		}

		protected override async Task SaveEntity(DestinationContext db, Employee entity)
		{
			var emails = entity.GetEmailAddresses();
			var allEmails = await base.AddAnyMissingEmails(db, emails);

			var dbEnt = new Mise.Database.AzureDefinitions.Entities.People.Employee(entity, allEmails);
		}
	}
}