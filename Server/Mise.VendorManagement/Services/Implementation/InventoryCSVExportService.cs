using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using Mise.Core.Entities.Inventory;
using Mise.Core.Services.UtilityServices;

namespace Mise.VendorManagement.Services.Implementation
{
    public class InventoryCSVExportService : BaseCsvWriter, IInventoryExportService
    {
        public readonly ILogger _logger;

        public InventoryCSVExportService(ILogger logger)
        {
            _logger = logger;
        }

        public Task ExportInventoryToCsvFile(string filename, IInventory inventory)
        {
            return Task.Run(() =>
            {
                var lineItems = inventory.GetBeverageLineItems();
                if (lineItems.Any() == false)
                {
                    throw new ArgumentException("No line items in the given inventory!");
                }

                var writer = GetTextWriter(filename);
                using (writer)
                {
                    var csv = new CsvWriter(writer);

                    foreach (var section in inventory.GetSections())
                    {
                        foreach (var li in section.GetInventoryBeverageLineItemsInSection())
                        {
                            csv.WriteField(section.Name);
                            csv.WriteField(li.DisplayName);
                            csv.WriteField(li.Container.DisplayName);


                            var numpartials = li.PartialBottlePercentages.Any()
                                ? li.PartialBottlePercentages.Sum(p => p)
                                : 0;

                            csv.WriteField(li.Quantity);

                            csv.WriteField(li.CurrentAmount.Milliliters);
                            csv.WriteField(li.NumFullBottles);

                            csv.WriteField(numpartials);

                            csv.NextRecord();
                        }
                    }
                }
            });
        }
    }
}
