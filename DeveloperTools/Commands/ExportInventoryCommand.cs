using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Mise.Core.Common.Entities;
using Mise.Core.Common.Entities.DTOs;
using Mise.Core.Common.Entities.DTOs.AzureTypes;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Common.Services.Implementation.Serialization;
using Mise.Core.Services.UtilityServices;
using Mise.VendorManagement.Services.Implementation;

namespace DeveloperTools.Commands
{
    public class ExportInventoryCommand : BaseProgressReportingCommand
    {
        private readonly ILogger _logger;
        private readonly IMobileServiceClient _client;
        private readonly EntityDataTransportObjectFactory _entityDataTransportObjectFactory;
        public ExportInventoryCommand(IProgress<ProgressReport> progress, ILogger logger) : base(progress)
        {
            _logger = logger;

            _client = new MobileServiceClient(
                "https://stockboymobileservice.azure-mobile.net/",
                "vvECpsmISLzAxntFjNgSxiZEPmQLLG42"
            );

            _entityDataTransportObjectFactory = new EntityDataTransportObjectFactory(new JsonNetSerializer());
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

            var restType = typeof (Restaurant).ToString();
            var restAIs = await table.Where(ai => ai.MiseEntityType == restType)
                .Where(ai => ai.EntityID == lastInv.RestaurantID)
                .ToEnumerableAsync();

            var restAI = restAIs.FirstOrDefault();
           

            string fileName = "UnknownRestInv__" + lastInv.CreatedDate.ToString("d") + ".csv";
            if (restAI != null)
            {
                var rest = _entityDataTransportObjectFactory.FromDataStorageObject<Restaurant>(restAI.ToRestaurantDTO());
                fileName = rest.Name.ShortName+"__"+ lastInv.CreatedDate.ToString("d") + ".csv";
            }
            fileName = fileName.Replace("/", "_");

            Report("Exporting items to CSV");
            var service = new InventoryCSVExportService(_logger);

            await service.ExportInventoryToCsvFile(fileName, lastInv);

            Finish();
        }

        public override int NumSteps
        {
            get { return 5; }
        }
    }
}
