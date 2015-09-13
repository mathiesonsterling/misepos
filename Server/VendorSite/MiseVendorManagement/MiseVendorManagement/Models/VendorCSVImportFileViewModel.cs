using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiseVendorManagement.Models
{
    public class VendorCSVImportFileViewModel
    {
        /// <summary>
        /// Should match the Vendor Id
        /// </summary>
        public Guid Id { get; set; }

        public string VendorName { get; set; }

        /// <summary>
        /// Information to retrieve the uploaded file from Azure storage
        /// </summary>
        public string FileKey { get; set; }

        public IEnumerable<string> ColumnNames { get; set; } 
        /*
        public string ItemNameColumnName { get; set; }
        public string ContainerColumnName { get; set; }
        public bool UseOunces { get; set; }
        public string CategoryColumnName { get; set; }
        public string CaseSizeColumnName { get; set; }
        public string UpcColumnName { get; set; }
        public string PriceColumnName { get; set; }*/
    }
}
