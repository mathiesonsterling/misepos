using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities.Inventory;

namespace Mise.VendorManagement.Services
{
    public interface IInventoryExportService
    {
        /// <summary>
        /// Generates a CSV file with the items seperated by section
        /// </summary>
        /// <param name="inventory"></param>
        /// <returns></returns>
        Task<byte[]> ExportInventoryToCsvBySection(IInventory inventory);

        /// <summary>
        /// Export the inventory items, but by the total amount they have across all sections
        /// </summary>
        /// <param name="inventory"></param>
        /// <returns></returns>
        Task<byte[]> ExportInventoryToCSVAggregated(IInventory inventory);

        Task<byte[]> ExportParToCSV(IPar par);

        Task<byte[]> ExportReceivingOrderToCSV(IReceivingOrder ro);
    }
}
