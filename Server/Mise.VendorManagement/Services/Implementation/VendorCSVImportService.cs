using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvHelper;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Common.Entities.Vendors;
using Mise.Core.Common.Services.Implementation;
using Mise.Core.Entities;
using Mise.Core.Services.UtilityServices;
using Mise.Core.ValueItems;
using Mise.Core.ValueItems.Inventory;

namespace Mise.VendorManagement.Services.Implementation
{
    public class VendorCSVImportService : IVendorCSVImportService
    {
        private readonly ILogger _logger;
        private readonly int _maxFailures;
        public VendorCSVImportService(ILogger logger, int maxFailures=5)
        {
            _logger = logger;
            _maxFailures = maxFailures;

            //load the possible categories
        }
        private static readonly Dictionary<decimal, decimal> OzToCommonMlDic = new Dictionary<decimal, decimal>
        {
            {25.4M, 750},
            {33.8M, 1000},
            {12.7M, 375}
        }; 
        public IEnumerable<string> GetColumnNames(MemoryStream stream)
        {
            var csv = ReadFile(stream);

            if (csv.Read())
            {
                return csv.FieldHeaders.AsEnumerable();
            }
            throw new ArgumentException("Cannot read file ");
        }

        public IEnumerable<string> GetPossibleMatchesForColumnName(string name)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetPossibleCategoriesInFile(MemoryStream stream, string categoriesColumnName)
        {
            var res = new HashSet<string>();
            var csv = ReadFile(stream);
            while (csv.Read())
            {
                var cat = csv.GetField<string>(categoriesColumnName);
                if (res.Contains(cat) == false)
                {
                    res.Add(cat);
                }
            }

            return res.AsEnumerable();
        }

  
        public class MappingException : Exception
        {
            public string SourceColumnName { get; set; }
            public MappingException(string sourceName, Exception inner) : base("Error mapping category " + sourceName, inner)
            {
                SourceColumnName = sourceName;
            }
        }

        public class ContainerException : Exception
        {
            public string SourceValue { get; set; }

            public ContainerException(string source, Exception inner)
                : base("Invalid container value of " + source, inner)
            {
                SourceValue = source;
            }
        }

        public IEnumerable<VendorBeverageLineItem> ParseDataFile(MemoryStream file, Guid vendorID, string itemNameColumn, string containerColumn,
            LiquidAmountUnits containerUnit, string categoryColumn, IDictionary<string, ItemCategory> categoryMapping, string caseSizeColumn,
            string upcColumn, string priceColumn)
        {
            var csv = ReadFile(file);

            var i = 0;
            var numErrors = 0;
            var res = new List<VendorBeverageLineItem>();
            while (csv.Read())
            {
                try
                {
                    var newItem = new VendorBeverageLineItem
                    {
                        Id = Guid.NewGuid(),
                        CreatedDate = DateTime.UtcNow,
                        LastUpdatedDate = DateTime.UtcNow,
                        Revision = new EventID {AppInstanceCode = MiseAppTypes.VendorManagement, OrderingID = i++},
                        VendorID = vendorID,
                        NameInVendor = csv.GetField<string>(itemNameColumn),
                    };

                    newItem.DisplayName = newItem.NameInVendor;

                    var containerString = csv.GetField<string>(containerColumn);
                    decimal containerVal;
                    if (decimal.TryParse(containerString, out containerVal))
                    {
                        newItem.Container = CreateContainer(containerVal, containerUnit);
                    }
                    else
                    {
                        throw new ContainerException(containerString, null);
                    }

                    var category = csv.GetField<string>(categoryColumn);
                    var mapKey = string.IsNullOrEmpty(category) == false ? category.Trim().ToUpper() : string.Empty;
                    try
                    {
                        newItem.Categories = new List<ItemCategory>
                        {
                            string.IsNullOrEmpty(mapKey)
                                ? CategoriesService.Unknown
                                : categoryMapping[mapKey]

                        };
                    }
                    catch (Exception e)
                    {
                        throw new MappingException(mapKey, e);
                    }

                    if (string.IsNullOrEmpty(caseSizeColumn) == false)
                    {
                        newItem.CaseSize = csv.GetField<int>(caseSizeColumn);
                    }

                    if (string.IsNullOrEmpty(upcColumn) == false)
                    {
                        newItem.UPC = csv.GetField<string>(upcColumn);
                    }
                    if (string.IsNullOrEmpty(priceColumn) == false)
                    {
                        var priceRaw = csv.GetField<string>(priceColumn);
                        if (string.IsNullOrEmpty(priceRaw) == false)
                        {
                            var priceFor = priceRaw.Replace("$", "").Trim();
                            decimal price;
                            if (decimal.TryParse(priceFor, out price))
                            {
                                newItem.PublicPricePerUnit = new Money(price);
                            }
                        }
                    }

                    res.Add(newItem);
                }
                catch (Exception e)
                {
                    _logger.HandleException(e);
                    if (numErrors++ > _maxFailures)
                    {
                        throw;
                    }
                }
            }

            return res;
        }

        private static LiquidContainer CreateContainer(decimal containerSize, LiquidAmountUnits unit)
        {
            decimal ml;
            if (unit == LiquidAmountUnits.OuncesLiquid)
            {
                var ounces = containerSize;
                if (OzToCommonMlDic.ContainsKey(ounces))
                {
                    ml = OzToCommonMlDic[ounces];
                }
                else
                {
                    ml = ounces*29.5735M;
                }
            }
            else
            {
                ml = containerSize;
            }

            return new LiquidContainer
            {
                AmountContained = new LiquidAmount {Milliliters = ml}
            };
        }

        private static CsvReader ReadFile(MemoryStream stream)
        {
            var fileReader = new StreamReader(stream);
            return new CsvReader(fileReader);
        }


    }
}
