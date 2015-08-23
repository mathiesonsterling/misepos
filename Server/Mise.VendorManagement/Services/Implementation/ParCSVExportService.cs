using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using Mise.Core.Entities.Inventory;
using Mise.Core.Services.UtilityServices;

namespace Mise.VendorManagement.Services.Implementation
{
    public class ParCsvExportService : BaseCsvWriter, IParExportService
    {
        private readonly ILogger _logger;

        public ParCsvExportService(ILogger logger)
        {
            _logger = logger;
        }

        public Task ExportParToCsvFile(string filename, IPar par)
        {
            if (par.GetBeverageLineItems().Any() == false)
            {
                throw new ArgumentException("No line items in this Par");
            }

            return Task.Run(() =>
            {
                var writer = GetTextWriter(filename);
                using (writer)
                {
                    var csv = new CsvWriter(writer);
                    foreach (var li in par.GetBeverageLineItems())
                    {
                        csv.WriteField(li.DisplayName);
                        csv.WriteField(li.Container.DisplayName);
                        csv.WriteField(li.Quantity);

                        csv.NextRecord();
                    }
                }
            });
        }
    }
}
