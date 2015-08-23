using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities.Inventory;

namespace Mise.VendorManagement.Services
{
    public interface IParExportService
    {
        Task ExportParToCsvFile(string filename, IPar par);
    }
}
