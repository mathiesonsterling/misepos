using System;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Services.UtilityServices;
using Mise.Inventory.Services.Implementation.WebServiceClients.Azure;
using Mise.VendorManagement.Services.Implementation;

namespace DeveloperTools.Commands
{
    public class ExportParCommand : BaseCSVExportCommand
    {
        public ExportParCommand(IProgress<ProgressReport> progress, ILogger logger) : base(progress, logger)
        {
        }

        public override async Task Execute()
        {
            Report("Getting last par from DB . . . ");
            var parType = typeof (Par).ToString();

            var table = _client.GetTable<AzureEntityStorage>();

            Report("Getting pars");
            var pars = await table.Where(ai => ai.MiseEntityType == parType)
                .OrderByDescending(ai => ai.LastUpdatedDate)
                .ToEnumerableAsync();

            var lastParAI = pars.FirstOrDefault();

            if (lastParAI == null)
            {
                throw new InvalidOperationException("No pars found in DB");
            }
            Report("Deserializing par");
            var lastPar = _entityDataTransportObjectFactory.FromDataStorageObject<Par>(lastParAI.ToRestaurantDTO());

            Report("Getting restaurant for par");
            var fileName = await GetFileNameForRestaurant(table, lastPar, "Par");

            Report("Export items to CSV");
            var service = new CSVExportService(_logger);
            await service.ExportParToCSV(lastPar);

            Finish();
        }

        public override int NumSteps { get { return 5; } }
    }
}
