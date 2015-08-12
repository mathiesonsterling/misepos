using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Mise.Core.Common.Entities;
using Mise.Core.Common.Entities.DTOs;
using Mise.Core.Common.Services.Implementation.Serialization;
using Mise.Core.Entities.Base;
using Mise.Core.Services.UtilityServices;
using Mise.Inventory.Services.Implementation.WebServiceClients.Azure;

namespace DeveloperTools.Commands
{
    public abstract class BaseCSVExportCommand : BaseProgressReportingCommand
    {
        protected readonly ILogger _logger;
        protected readonly IMobileServiceClient _client;
        protected readonly EntityDataTransportObjectFactory _entityDataTransportObjectFactory;
        protected BaseCSVExportCommand(IProgress<ProgressReport> progress, ILogger logger) : base(progress)
        {
            _logger = logger;

            _client = new MobileServiceClient(
                "https://stockboymobileservice.azure-mobile.net/",
                "vvECpsmISLzAxntFjNgSxiZEPmQLLG42"
            );

            _entityDataTransportObjectFactory = new EntityDataTransportObjectFactory(new JsonNetSerializer());
        }

        protected async Task<string> GetFileNameForRestaurant(IMobileServiceTable<AzureEntityStorage> table, IRestaurantEntityBase lastInv, string entName)
        {
            var restType = typeof(Restaurant).ToString();
            var restAIs = await table.Where(ai => ai.MiseEntityType == restType)
                .Where(ai => ai.EntityID == lastInv.RestaurantID)
                .ToEnumerableAsync();

            var restAI = restAIs.FirstOrDefault();


            string fileName = "UnknownRest_" + entName + "__" + lastInv.CreatedDate.ToString("d") + ".csv";
            if (restAI != null)
            {
                var rest = _entityDataTransportObjectFactory.FromDataStorageObject<Restaurant>(restAI.ToRestaurantDTO());
                fileName = rest.Name.ShortName + "_" + entName + "__" + lastInv.CreatedDate.ToString("d") + ".csv";
            }
            fileName = fileName.Replace("/", "_");
            return fileName;
        }
    }
}
