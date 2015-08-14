using System;
using System.Collections.Generic;
using System.Linq;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Entities.Inventory;
using Mise.Core.ValueItems;
using Mise.Core.ValueItems.Inventory;

namespace Mise.Database.StorableEntities.Inventory
{
    public class ReceivingOrderGraphNode : IStorableEntityGraphNode
    {
        public ReceivingOrderGraphNode()
        {
            
        }

        public ReceivingOrderGraphNode(IReceivingOrder source)
        {
            ID = source.ID;
            RestaurantID = source.RestaurantID;
            CreatedDate = source.CreatedDate;
            LastUpdatedDate = source.LastUpdatedDate;
            Revision = source.Revision.ToDatabaseString();

            Status = source.Status.ToString();
            ReceivedByEmployeeID = source.ReceivedByEmployeeID;
            Notes = source.Notes;
            InvoiceID = source.InvoiceID;
            DateReceived = source.DateReceived;
        }

        public IReceivingOrder Rehydrate(IEnumerable<ReceivingOrderLineItem> lineItems, Guid? purchaseOrderID, Guid vendorID)
        {
            return new ReceivingOrder
            {
                ID = ID,
                RestaurantID = RestaurantID,
                CreatedDate = CreatedDate,
                LastUpdatedDate = LastUpdatedDate,
                Revision = new EventID(Revision),

                LineItems = lineItems.ToList(),
                Status = (ReceivingOrderStatus)Enum.Parse(typeof(ReceivingOrderStatus), Status),
                PurchaseOrderID = purchaseOrderID,
                ReceivedByEmployeeID = ReceivedByEmployeeID,
                VendorID = vendorID,
                InvoiceID = InvoiceID,
                Notes = Notes,
                DateReceived = DateReceived
            };
        }

        public DateTimeOffset DateReceived { get; set; }
        public string Notes { get; set; }
        public string InvoiceID { get; set; }
        public Guid ReceivedByEmployeeID { get; set; }

        public string Status { get; set; }

        public DateTimeOffset LastUpdatedDate { get; set; }

        public DateTimeOffset CreatedDate { get; set; }

        public Guid RestaurantID { get; set; }

        public Guid ID { get; set; }
        public string Revision { get; set; }
    }
}
