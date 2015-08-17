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
        Task<byte[]> ExportInventoryToCsv(IInventory inventory);
    }
}
