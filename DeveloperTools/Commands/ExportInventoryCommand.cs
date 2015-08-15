using System;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Services.UtilityServices;
using Mise.Inventory.Services.Implementation.WebServiceClients.Azure;
using Mise.VendorManagement.Services.Implementation;

namespace DeveloperTools.Commands
{
    public class ExportInventoryCommand : BaseCSVExportCommand
    {

        public ExportInventoryCommand(IProgress<ProgressReport> progress, ILogger logger) : base(progress, logger)
        {

        }

        public override async Task Execute()
        {
            Report("Getting last inventory from DB . . . ");
            var invType = typeof (Inventory).ToString();

            var table = _client.GetTable<AzureEntityStorage>();

            Report("Getting inventories . . . . ");
            var inventories = await table
                .Where(ai => ai.MiseEntityType == invType)
                .OrderByDescending(ai => ai.LastUpdatedDate)
                .ToEnumerableAsync();

            var lastInvAI = inventories.FirstOrDefault();

            Report("Transforming from JSON to inventories");
            var lastInv = _entityDataTransportObjectFactory.FromDataStorageObject<Inventory>(lastInvAI.ToRestaurantDTO());

            Report("Getting restaurant for inventory");

            var fileName = await GetFileNameForRestaurant(table, lastInv, "Inventory");

            Report("Exporting items to CSV");
            var service = new InventoryCSVExportService(_logger);

            var bytes = await service.ExportInventoryToCsv(lastInv);


            Finish();
        }



        public override int NumSteps
        {
            get { return 5; }
        }
    }
}
