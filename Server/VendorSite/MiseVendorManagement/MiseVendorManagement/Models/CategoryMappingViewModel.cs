using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Mise.Core.Common.Entities.Inventory;

namespace MiseVendorManagement.Models
{
    /// <summary>
    /// All data that will be needed to go from a CSV file, through mapping it onto vendor items
    /// </summary>
    public class UploadCSVImportViewModel
    {
        public HttpPostedFile UploadedCsvFile { get; set; }

        public Guid VendorId { get; set; }
        /// <summary>
        /// Maps the string given in the CSV file to the Category 
        /// </summary>
        public Dictionary<string, ItemCategory> CategoryMappings { get; set; }  

        /// <summary>
        /// If true the units are in ounces
        /// </summary>
        public bool UseOunces { get; set; }
    }
}
