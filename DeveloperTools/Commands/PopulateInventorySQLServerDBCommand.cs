using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Mise.Core.Common.Entities.DTOs.AzureTypes;
using Mise.Core.Common.Events.DTOs.AzureTypes;
using Mise.Core.Services.UtilityServices;

namespace DeveloperTools.Commands
{
    class PopulateInventorySqlServerDBCommand : BaseProgressReportingCommand
    {
        private readonly ILogger _logger;
        private readonly Uri _uri;
        private readonly bool _addDemo;

        private Microsoft.WindowsAzure.MobileServices.IMobileServiceClient _client;
        public PopulateInventorySqlServerDBCommand(IProgress<ProgressReport> progress, ILogger logger, Uri uri, bool addDemo) : base(progress)
        {
            _logger = logger;
            _uri = uri;
            _addDemo = addDemo;

            _client = new MobileServiceClient(
                "https://stockboymobileservice.azure-mobile.net/",
                "vvECpsmISLzAxntFjNgSxiZEPmQLLG42"
            );
        }

        public override async Task Execute()
        {
            //delete items
            await DeleteCurrentDBItems();

            //get our fake web service as source


        }

        public async Task DeleteCurrentDBItems()
        {
            var entTable = _client.GetTable<AzureEntityStorage>();
            var  entities = await entTable.ToEnumerableAsync();
            var delEntities = entities.Select(ent => entTable.DeleteAsync(ent));

            var evTable = _client.GetTable<AzureEventStorage>();
            var events = await evTable.ToEnumerableAsync();
            var delEvents = events.Select(ev => evTable.DeleteAsync(ev));

            await Task.WhenAll(delEntities).ConfigureAwait(false);
            await Task.WhenAll(delEvents).ConfigureAwait(false);
        }
        public override int NumSteps { get; }
    }
}
