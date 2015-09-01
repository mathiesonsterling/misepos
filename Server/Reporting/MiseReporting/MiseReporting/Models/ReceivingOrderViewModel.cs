using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities.Inventory;
using Mise.Core.Entities.People;
using Mise.Core.Entities.Vendors;

namespace MiseReporting.Models
{
    public class ReceivingOrderViewModel
    {
        public Guid Id { get; set; }

        public DateTime? DateCreated { get; set; }

        public DateTime? LastUpdated { get; set; }

        [DisplayName("Received By")]
        public string ReceivedByEmployee { get; set; }

        public string VendorName { get; set; }

        public int NumLineItems { get; set; }

        public string InvoiceId { get; set;}

        public ReceivingOrderViewModel(IReceivingOrder ro, IVendor vendor, IEmployee emp)
        {
            Id = ro.ID;
            DateCreated = ro.CreatedDate.ToLocalTime().DateTime;
            LastUpdated = ro.CreatedDate.ToLocalTime().DateTime;
            InvoiceId = ro.InvoiceID;

            if (vendor != null)
            {
                VendorName = vendor.Name;
            }
            if (emp != null)
            {
                ReceivedByEmployee = emp.DisplayName;
            }
            NumLineItems = ro.GetBeverageLineItems().Count();
        }
    }
}
