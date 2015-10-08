using System;
using System.ComponentModel;
using System.Linq;
using Mise.Core.Entities.Inventory;
using Mise.Core.Entities.People;
using Mise.Core.Entities.Vendors;

namespace MiseReporting.Models
{
    public class PurchaseOrderViewModel
    {
        /// <summary>
        /// Id of the purchase order PO id
        /// </summary>
        public Guid Id { get; set; }
        public Guid POId { get; set; }

        [DisplayName("Created Date")]
        public DateTime DateCreated { get; set; }

        [DisplayName("Last Updated")]
        public DateTime LastUpdated { get; set; }

        public string VendorName { get; set; }

        public string EmployeeName { get; set; }

        public int NumLineItems { get; set; }

        public string Status { get; set; }

        public Guid? LinkedReceivingOrderId { get; set; }

        public PurchaseOrderViewModel(IPurchaseOrder po, IPurchaseOrderPerVendor poForVendor, IVendor vendor, IEmployee emp, IReceivingOrder linkedReceivingOrder)
        {
            POId = po.Id;
            Id = poForVendor.Id;
            DateCreated = poForVendor.CreatedDate.ToLocalTime().DateTime;
            LastUpdated = poForVendor.LastUpdatedDate.ToLocalTime().DateTime;
            NumLineItems = poForVendor.GetLineItems().Count();
            Status = poForVendor.Status.ToString();

            if (vendor != null)
            {
                VendorName = vendor.Name;
            }

            if (emp != null)
            {
                EmployeeName = emp.DisplayName;
            }

            if (linkedReceivingOrder != null)
            {
                LinkedReceivingOrderId = linkedReceivingOrder.Id;
            }
        }
    }
}
