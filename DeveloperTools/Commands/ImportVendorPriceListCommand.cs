using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Common;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Common.Entities.Vendors;
using Mise.Core.Entities;
using Mise.Core.Services;
using Mise.Core.ValueItems;
using Mise.Core.ValueItems.Inventory;
using Mise.Neo4J.Neo4JDAL;
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
        public ImportVendorPriceListCommand(ILogger logger, Uri dbLocation, string fileName, string vendorName, EmailAddress vendorEmail, StreetAddress vendorAddress,
            IProgress<ProgressReport> progress) : base(progress)
        {
            _logger = logger;
            _dbLoc = dbLocation;
            _fileName = fileName;
            _vendorName = vendorName;
            _vendorEmail = vendorEmail;
            _vendorStreetAddress = vendorAddress;

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
                {"agave".ToUpper(), CategoriesService.Agave},
                {"AMARO", CategoriesService.LiquerAmaro},
                {"american whiskey".ToUpper(), CategoriesService.WhiskeyAmerican},
                {"bitters".ToUpper(), CategoriesService.NonAlcoholic},
                {"brandy".ToUpper(), CategoriesService.Brandy},
                {"fortified & aromatized".ToUpper(), CategoriesService.WineFortified},
                {"gin".ToUpper(), CategoriesService.Gin},
                {"LIQUEUR", CategoriesService.Liquer},
                {"NA", CategoriesService.Unknown},
                {"RUM", CategoriesService.Rum},
                {"SCOTCH", CategoriesService.WhiskeyScotch},
                {"VODKA", CategoriesService.Vodka},
                {"WORLD WHISKY", CategoriesService.WhiskeyWorld}
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
            var graphDAL = new Neo4JEntityDAL(config, _logger);

            //TODO see if the vendor exists already
            Report("Retrieving current vendors");
            var vendors = (await graphDAL.GetVendorsAsync());

            Report("Checking if Vendor " + _vendorName + " already exists . . . ");
            var found = vendors.Any(v => v.IsSameVendor(vendor));
            if (found)
            {
                _logger.Error("Vendor " + vendor.Name + " already exists!");
                Finish();
                return;
            }

            //save the vendor
            Report("Saving vendor in DB . . . ");
            await graphDAL.AddVendorAsync(vendor);

            Finish();
        }

        public override int NumSteps
        {
            get { return _numSteps; }
        }
    }
}
