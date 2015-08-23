using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Mise.Core.Common;
using Mise.Core.Common.Entities.DTOs;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Common.Entities.Vendors;
using Mise.Core.Common.Services.Implementation.Serialization;
using Mise.Core.Entities;
using Mise.Core.Entities.Vendors;
using Mise.Core.Services;
using Mise.Core.Services.UtilityServices;
using Mise.Core.ValueItems;
using Mise.Core.ValueItems.Inventory;
using Mise.Inventory;
using Mise.Inventory.Services.Implementation.WebServiceClients.Azure;
using Mise.VendorManagement.Services.Implementation;

namespace DeveloperTools.Commands
{
    class ImportVendorPriceListCommand : BaseProgressReportingCommand
    {
        private readonly ILogger _logger;
        private readonly Uri _dbLoc;
        private readonly string _fileName;
        private readonly string _vendorName;
        private readonly EmailAddress _vendorEmail;
        private readonly StreetAddress _vendorStreetAddress;

        private readonly int _numSteps;
        private readonly IMobileServiceClient _client;
        private readonly EntityDataTransportObjectFactory _entityDataTransportObjectFactory;
        public ImportVendorPriceListCommand(ILogger logger, Uri dbLocation, string fileName, string vendorName, EmailAddress vendorEmail, StreetAddress vendorAddress, BuildLevel level,
            IProgress<ProgressReport> progress) : base(progress)
        {
            _logger = logger;
            _dbLoc = dbLocation;
            _fileName = fileName;
            _vendorName = vendorName;
            _vendorEmail = vendorEmail;
            _vendorStreetAddress = vendorAddress;

            var mobSer = AzureServiceLocator.GetAzureMobileServiceLocation(level);
            _client = new MobileServiceClient(
               mobSer.Uri.ToString(),
                mobSer.AppKey
            );

            _entityDataTransportObjectFactory = new EntityDataTransportObjectFactory(new JsonNetSerializer());
            _numSteps = 10;
        }

        public override async Task Execute()
        {
            var importService = new VendorCSVImportService(_logger, 15);

            //we need to create the new vendor
            //TODO this should come from UI at some point
            var mappings = new Dictionary<string, ItemCategory>
            {
                {"absinthe/oddball".ToUpper(), CategoriesService.Unknown},
                {"02 absinthe/oddball".ToUpper(), CategoriesService.Unknown},
                {"agave".ToUpper(), CategoriesService.Agave},
                {"AMARO", CategoriesService.LiquerAmaro},
                {"american whiskey".ToUpper(), CategoriesService.WhiskeyAmerican},
                {"bitters".ToUpper(), CategoriesService.NonAlcoholic},
                {"brandy".ToUpper(), CategoriesService.Brandy},
                {"fortified & aromatized".ToUpper(), CategoriesService.WineFortified},
                {"gin".ToUpper(), CategoriesService.Gin},
                {"02 gin".ToUpper(), CategoriesService.Gin},
                {"LIQUEUR", CategoriesService.Liquer},
                {"NA", CategoriesService.Unknown},
                {"RUM", CategoriesService.Rum},
                {"03 RUM", CategoriesService.Rum},
                {"SCOTCH", CategoriesService.WhiskeyScotch},
                {"VODKA", CategoriesService.Vodka},
                { "01 VODKA", CategoriesService.Vodka},
                {"WORLD WHISKY", CategoriesService.WhiskeyWorld},
                { "04 MEZCAL", CategoriesService.AgaveMezcal},
                {"04 SOTOL", CategoriesService.Agave},
                {"04 TEQUILA", CategoriesService.AgaveTequila},
                {"05 AMERICAN", CategoriesService.WhiskeyAmerican},
                {"05 BOURBON", CategoriesService.WhiskeyBourbon},
                {"05 RYE", CategoriesService.WhiskeyRye},
                {"06 CANADIAN", CategoriesService.WhiskeyCanadian}
            };

            var vendorID = Guid.NewGuid();
            Report("Parsing file " + _fileName);
            var lineItems = importService.ParseDataFile(_fileName, vendorID, "Spirit", "Unit Volume",
                LiquidAmountUnits.OuncesLiquid, "Category", mappings, null, null, null).ToList();
            Report("Parsed " + lineItems.Count() + " from file");

            if (lineItems.Any() == false)
            {
                Finish();
                return;
            }

            //assign the hash to the event - we can use this later to determine if the file was already uploaded?
            var eventID = new EventID {AppInstanceCode = MiseAppTypes.VendorDataImport, OrderingID = _fileName.GetHashCode()};
            foreach (var li in lineItems)
            {
                li.Revision = eventID;
                li.CreatedDate = DateTime.UtcNow;
                li.LastUpdatedDate = DateTime.UtcNow;
            }

            //make the vendor with items
            Report("Setting up vendor . . . ");
             var vendor = new Vendor
            {
                CreatedDate = DateTime.UtcNow,
                LastUpdatedDate = DateTime.UtcNow,
                Revision = eventID,
                EmailToOrderFrom = _vendorEmail,
                ID = vendorID,
                Name = _vendorName,
                StreetAddress = _vendorStreetAddress,
                Verified = true,
                VendorBeverageLineItems = lineItems
            };

            Report("Connecting to DB . . . ");

            var config = new DevToolsConfigs { Neo4JConnectionDBUri = _dbLoc };

            //TODO see if the vendor exists already
            Report("Retrieving current vendors");

            var vendors = await RetrieveCurrentVendors();

            Report("Checking if Vendor " + _vendorName + " already exists . . . ");
            var found = vendors.Any(v => v.IsSameVendor(vendor));
            if (found)
            {
                _logger.Error("Vendor " + vendor.Name + " already exists!");
                Finish();
                throw new Exception("Vendor already exists");
            }

            //save the vendor
            Report("Saving vendor in DB . . . ");

            var restDTO = _entityDataTransportObjectFactory.ToDataTransportObject(vendor);
            var azureItem = new AzureEntityStorage(restDTO);
            var table = _client.GetTable<AzureEntityStorage>();
            await table.InsertAsync(azureItem);

            Finish();
        }

        private async Task<IEnumerable<IVendor>> RetrieveCurrentVendors()
        {
            var table = _client.GetTable<AzureEntityStorage>();
            var vendType = typeof (Vendor).ToString();
            var vendorStorage = await table.Where(ai => ai.MiseEntityType == vendType).ToEnumerableAsync();
            var dtos = vendorStorage.Select(ai => ai.ToRestaurantDTO());
            var vends = dtos.Select(dto => _entityDataTransportObjectFactory.FromDataStorageObject<Vendor>(dto));

            return vends;
        }

        public override int NumSteps
        {
            get { return _numSteps; }
        }
    }
}
