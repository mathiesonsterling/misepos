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
                        csv.WriteField("Section");
                        WriteInventoryLineItemHeader(csv);
                        csv.NextRecord();
                        foreach (var section in inventory.GetSections().OrderBy(s => s.Name))
                        {
                            foreach (var li in section.GetInventoryBeverageLineItemsInSection().OrderBy(li => li.DisplayName))
                            {
                                csv.WriteField(section.Name);
                                WriteInventoryLineItem(csv, li, li.Quantity);
                            }
                        }
                        await streamWriter.FlushAsync();
                        return ms.ToArray();
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
                        WriteInventoryLineItemHeader(csv);
                        csv.NextRecord();
                        foreach (var li in itemDic.Values.Where(t => t.Item2 > 0).OrderBy(t => t.Item1.DisplayName))
                        {
                            WriteInventoryLineItem(csv, li.Item1, li.Item2);
                        }
                        await streamWriter.FlushAsync();
                        return ms.ToArray();
                    }
                }
            }
        }

        public async Task<byte[]> ExportParToCSV(IPar par)
        {
            var lineItems = par.GetBeverageLineItems().ToList();
            if (lineItems.Any() == false)
            {
                throw new ArgumentException("No line items in the par!");
            }

            using (var ms = new MemoryStream())
            {
                using (var streamWriter = new StreamWriter(ms))
                {
                    using (var csv = new CsvWriter(streamWriter))
                    {
                        //write the header
                        csv.WriteField("Name");
                        csv.WriteField("Container");
                        csv.WriteField("Quantity");
                        csv.WriteField("Categories");
                        csv.NextRecord();
                        foreach (var li in lineItems.OrderBy(li => li.DisplayName))
                        {
                            csv.WriteField(li.DisplayName);
                            csv.WriteField(li.Container.DisplayName);

                            csv.WriteField(li.Quantity);

                            csv.WriteField(GetCategoriesString(li.GetCategories()));
                            csv.NextRecord();
                        }
                        await streamWriter.FlushAsync();
                        return ms.ToArray();
                    }
                }
            }
        }

        public async Task<byte[]> ExportReceivingOrderToCSV(IReceivingOrder ro)
        {
            var lineItems = ro.GetBeverageLineItems().ToList();
            if (lineItems.Any() == false)
            {
                throw new ArgumentException("No line items in the receiving order!");
            }

            using (var ms = new MemoryStream())
            {
                using (var streamWriter = new StreamWriter(ms))
                {
                    using (var csv = new CsvWriter(streamWriter))
                    {
                        //write the header
                        csv.WriteField("Name");
                        csv.WriteField("Container");
                        csv.WriteField("Quantity");
                        csv.WriteField("Categories");
                        csv.WriteField("Price");
                        csv.NextRecord();
                        foreach (var li in lineItems.OrderBy(li => li.DisplayName))
                        {
                            csv.WriteField(li.DisplayName);
                            csv.WriteField(li.Container.DisplayName);

                            csv.WriteField(li.Quantity);

                            csv.WriteField(GetCategoriesString(li.GetCategories()));
                            csv.WriteField(li.LineItemPrice != null ? li.LineItemPrice.Dollars.ToString("c") : "N/A");
                            csv.NextRecord();
                        }
                        await streamWriter.FlushAsync();
                        return ms.ToArray();
                    }
                }
            }
        }

        private static void WriteInventoryLineItemHeader(ICsvWriter csv)
        {
            csv.WriteField("Name");
            csv.WriteField("Container");
            csv.WriteField("Quantity");

            csv.WriteField("Amount in ML");
            csv.WriteField("Number of Full Bottles");

            csv.WriteField("Number of Partial Bottles");

            csv.WriteField("Categories");
        }

        private static void WriteInventoryLineItem(ICsvWriter csv, IInventoryBeverageLineItem li, decimal quantity)
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
