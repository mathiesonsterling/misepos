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

        public async Task<byte[]> ExportInventoryToCsv(IInventory inventory)
        {
            {

                var lineItems = inventory.GetBeverageLineItems();
                if (lineItems.Any() == false)
                {
                    throw new ArgumentException("No line items in the given inventory!");
                }

                using (var ms = new MemoryStream())
                {
                    using (var streamWriter = new StreamWriter(ms))
                    {
                        using (var csv = new CsvWriter(streamWriter))
                        {
                            foreach (var section in inventory.GetSections())
                            {
                                foreach (var li in section.GetInventoryBeverageLineItemsInSection())
                                {
                                    csv.WriteField(section.Name);
                                    csv.WriteField(li.DisplayName);
                                    csv.WriteField(li.Container.DisplayName);


                                    var numpartials = li.GetPartialBottlePercentages().Any()
                                        ? li.GetPartialBottlePercentages().Sum(p => p)
                                        : 0;

                                    csv.WriteField(li.Quantity);

                                    csv.WriteField(li.CurrentAmount.Milliliters);
                                    csv.WriteField(li.NumFullBottles);

                                    csv.WriteField(numpartials);

                                    csv.NextRecord();
                                }
                            }
                            await streamWriter.FlushAsync();
                            return ms.ToArray();
                        }
                    }
                }
            }

        }

    }
}
