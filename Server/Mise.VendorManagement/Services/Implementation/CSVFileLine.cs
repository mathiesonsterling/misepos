using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.VendorManagement.Services.Implementation
{
    /// <summary>
    /// Represent our basic line for now.  CSV file can later be mapped to this
    /// </summary>
    public class CSVFileLine
    {
        public string ItemName { get; set; }
        /// <summary>
        /// Value of the container field, pre mapping
        /// </summary>
        public string ContainerValue { get; set; }
        public string Category { get; set; }
        public int? CaseSize { get; set; }
        public string UPC { get; set; }
        public decimal? PublicPrice { get; set; }
    }
}
