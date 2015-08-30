using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using Mise.Core.Common.Entities.Reports;
using Mise.Core.Entities.Inventory;
using Mise.Core.Services.UtilityServices;

namespace Mise.VendorManagement.Services.Implementation
{
    public class InventoryCSVExportService : BaseCsvWriter, IInventoryExportService
    {
        private readonly ILogger _logger;

        public InventoryCSVExportService(ILogger logger)
        {
            _logger = logger;
        }

        private static string GetCategoriesString(IEnumerable<ICategory> cats)
        {
            if (cats == null)
            {
                return string.Empty;
            }

            var catList = string.Empty;
            var categories = cats.ToList();
            if (categories.Any() == false)
            {
                return catList;
            }

            var catStrings = categories.Select(c => c.Name);
            catList = string.Join(",", catStrings);

            return catList;
        }

        public async Task<byte[]> ExportInventoryToCsvBySection(IInventory inventory)
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
                                foreach (var li in section.GetInventoryBeverageLineItemsInSection().OrderBy(li => li.DisplayName))
                                {
                                    csv.WriteField(section.Name);
                                    WriteLineItem(csv, li, li.Quantity);
                                }
                            }
                            await streamWriter.FlushAsync();
                            return ms.ToArray();
                        }
                    }
                }
            }

        }



        public async Task<byte[]> ExportInventoryToCSVAggregated(IInventory inventory)
        {
            var lineItems = inventory.GetBeverageLineItems().ToList();
            if (lineItems.Any() == false)
            {
                throw new ArgumentException("No line items in the given inventory!");
            }

            //group line items together into li, and total quantity
            var itemDic = new Dictionary<string, Tuple<IInventoryBeverageLineItem, decimal>>();
            foreach (var li in lineItems)
            {
                var key = BaseReport.GetListItemKey(li);
                if (itemDic.ContainsKey(key))
                {
                    var existing = itemDic[key];
                    var updatedWithQuant = existing.Item2 + li.Quantity;
                    itemDic[key] = new Tuple<IInventoryBeverageLineItem, decimal>(existing.Item1, updatedWithQuant);
                }
                else
                {
                    itemDic.Add(key, new Tuple<IInventoryBeverageLineItem, decimal>(li, li.Quantity));
                }
            }

            using (var ms = new MemoryStream())
            {
                using (var streamWriter = new StreamWriter(ms))
                {
                    using (var csv = new CsvWriter(streamWriter))
                    {

                            foreach (var li in itemDic.Values.OrderBy(t => t.Item1.DisplayName))
                            {
                                WriteLineItem(csv, li.Item1, li.Item2);
                            }
                        await streamWriter.FlushAsync();
                        return ms.ToArray();
                    }
                }
            }
        }

        private void WriteLineItem(ICsvWriter csv, IInventoryBeverageLineItem li, decimal quantity)
        {
            csv.WriteField(li.DisplayName);
            csv.WriteField(li.Container.DisplayName);


            var numpartials = li.GetPartialBottlePercentages().Any()
                ? li.GetPartialBottlePercentages().Sum(p => p)
                : 0;


            csv.WriteField(quantity);

            csv.WriteField(li.CurrentAmount.Milliliters);
            csv.WriteField(li.NumFullBottles);

            csv.WriteField(numpartials);

            csv.WriteField(GetCategoriesString(li.GetCategories()));
            csv.NextRecord();
        }
    }
}
