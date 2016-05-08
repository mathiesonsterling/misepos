using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Entities.Inventory;
using Mise.Core.ValueItems.Inventory;
using Mise.Database.AzureDefinitions.Entities.Inventory.LineItems;
using Mise.Database.AzureDefinitions.Entities.People;

namespace Mise.Database.AzureDefinitions.Entities.Inventory
{
    public class ReceivingOrder : BaseDbEntity<IReceivingOrder, Core.Common.Entities.Inventory.ReceivingOrder>
    {
        public Restaurant.Restaurant Restaurant
        {
            get;
            set;
        }

        public DateTimeOffset DateReceived { get; set; }

        /// <summary>
        /// If set, the purchase order this RO is associated with
        /// </summary>
        public PurchaseOrder PurchaseOrder { get; set; }

        public Vendor.Vendor Vendor { get; set; }

        public ReceivingOrderStatus Status { get; set; }

        public Employee ReceivedByEmployee { get; set; }

        public string Notes { get; set; }

        public string InvoiceID
        {
            get;
            set;
        }

        public ReceivingOrder()
        {
            LineItems = new List<ReceivingOrderBeverageLineItem>();
        }

        public List<ReceivingOrderBeverageLineItem> LineItems { get; set; }

        protected override Core.Common.Entities.Inventory.ReceivingOrder CreateConcreteSubclass()
        {
            return new Core.Common.Entities.Inventory.ReceivingOrder
            {
                DateReceived = DateReceived,
                PurchaseOrderID = PurchaseOrder?.EntityId,
                PurchaseOrderDate = PurchaseOrder?.CreatedAt,
                VendorID = Vendor.EntityId,
                Status = Status,
                ReceivedByEmployeeID = ReceivedByEmployee.EntityId,
                Notes = Notes,
                InvoiceID = InvoiceID,
                LineItems = LineItems.Select(li => li.ToBusinessEntity()).ToList()
            };
        }
    }
}
