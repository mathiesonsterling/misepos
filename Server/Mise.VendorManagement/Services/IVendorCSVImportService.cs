using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Common.Entities.Vendors;
using Mise.Core.ValueItems.Inventory;
using Mise.VendorManagement.Services.Implementation;

namespace Mise.VendorManagement.Services
{
    /// <summary>
    /// Given a file and a list of columns and category mappings, transform a CSV file into VendorLineItems
    /// </summary>
    public interface IVendorCSVImportService
    {
        IEnumerable<string> GetColumnNames(string fileName);

        /// <summary>
        /// Get all the values in the Categories column, so we can map them over to our Mise columns
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="categoriesColumnName"></param>
        /// <returns></returns>
        IEnumerable<string> GetPossibleCategoriesInFile(string fileName, string categoriesColumnName);

        /// <summary>
        /// Given the columns and category mapping, convert everything in our file to a VendorBeverageLineItem
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="vendorID"></param>
        /// <param name="itemNameColumn"></param>
        /// <param name="containerColumn"></param>
        /// <param name="containerUnit"></param>
        /// <param name="categoryColumn"></param>
        /// <param name="categoryMapping"></param>
        /// <param name="caseSizeColumn"></param>
        /// <param name="upcColumn"></param>
        /// <param name="priceColumn"></param>
        /// <returns></returns>
        IEnumerable<VendorBeverageLineItem> ParseDataFile(string fileName, Guid vendorID, string itemNameColumn, string containerColumn,
            LiquidAmountUnits containerUnit, string categoryColumn, Dictionary<string, ItemCategory> categoryMapping, string caseSizeColumn, string upcColumn, string priceColumn);
    }
}
