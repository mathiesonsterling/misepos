using System;
using System.Collections.Generic;
using System.Linq;
using Mise.Core.Client.Entities.Categories;
using Mise.Core.Client.Entities.Inventory.LineItems;
using Mise.Core.Client.Entities.People;
using Mise.Core.Entities.Inventory;
using Mise.Core.ValueItems.Inventory;

namespace Mise.Core.Client.Entities.Inventory
{
    public class ReceivingOrder : BaseDbEntity<IReceivingOrder, Core.Common.Entities.Inventory.ReceivingOrder>
    {
        public ReceivingOrder()
        {
            LineItems = new List<ReceivingOrderBeverageLineItem>();
        }

        public ReceivingOrder(IReceivingOrder source, Restaurant.Restaurant rest, Employee receivedBy, PurchaseOrder po, 
            Vendor.Vendor vendor, IEnumerable<InventoryCategory> cats)
            : base(source)
        {
            Restaurant = rest;
            RestaurantId = rest.Id;

            ReceivedByEmployee = receivedBy;
            ReceivedByEmployeeId = receivedBy?.Id;

            DateReceived = source.DateReceived;

            PurchaseOrder = po;
            PurchaseOrderId = po?.Id;

            Vendor = vendor;
            VendorId = vendor?.Id;

            Status = source.Status;
            Notes = source.Notes;
            InvoiceID = source.InvoiceID;

            LineItems =
                source.GetBeverageLineItems().Select(li => new ReceivingOrderBeverageLineItem(li, this, cats)).ToList();
        }


        public Restaurant.Restaurant Restaurant
        {
            get;
            set;
        }

        public string RestaurantId { get; set; }

        public DateTimeOffset DateReceived { get; set; }

        /// <summary>
        /// If set, the purchase order this RO is associated with
        /// </summary>
        public PurchaseOrder PurchaseOrder { get; set; }

        public string PurchaseOrderId { get; set; }

        public Vendor.Vendor Vendor { get; set; }

        public string VendorId { get; set; }

        public ReceivingOrderStatus Status { get; set; }

        public Employee ReceivedByEmployee { get; set; }

        public string ReceivedByEmployeeId { get; set; }

        public string Notes { get; set; }

        public string InvoiceID
        {
            get;
            set;
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
