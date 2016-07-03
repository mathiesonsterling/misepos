using System.Collections.Generic;
using System.Linq;
using Mise.Core.Client.Entities.Categories;
using Mise.Core.Client.Entities.Inventory.LineItems;
using Mise.Core.Entities.Inventory;
using Mise.Core.ValueItems.Inventory;

namespace Mise.Core.Client.Entities.Inventory
{
    public class PurchaseOrderPerVendor : BaseDbEntity<IPurchaseOrderPerVendor, Core.Common.Entities.Inventory.PurchaseOrderPerVendor>
    {
        public PurchaseOrderPerVendor()
        {
            Status = PurchaseOrderStatus.Created;
            PurchaseOrderBeverageLineItems = new List<PurchaseOrderBeverageLineItem>();
        }

        public PurchaseOrderPerVendor(IPurchaseOrderPerVendor pov, Vendor.Vendor vendor,
            IEnumerable<InventoryCategory> cats) : base(pov)
        {
            Vendor = vendor;
            VendorId = vendor.Id;
            Status = pov.Status;
            PurchaseOrderBeverageLineItems = pov.GetLineItems()
                .Select(li => new PurchaseOrderBeverageLineItem(li, cats, this)).ToList();
        }

        /// <summary>
        /// The vendor this applies to
        /// </summary>
        public Vendor.Vendor Vendor { get; set; }
        public string VendorId { get; set; }

        public List<PurchaseOrderBeverageLineItem> PurchaseOrderBeverageLineItems { get; set; }

	    public PurchaseOrder PurchaseOrder { get; set; }

	    public string PurchaseOrderId { get; set; }

        public PurchaseOrderStatus Status { get; set; }

        protected override Core.Common.Entities.Inventory.PurchaseOrderPerVendor CreateConcreteSubclass()
        {
            return new Core.Common.Entities.Inventory.PurchaseOrderPerVendor
            {
                VendorID = Vendor?.EntityId,
                VendorName = Vendor?.FullName,
                Status = Status,
                LineItems = PurchaseOrderBeverageLineItems.Select(li => li.ToBusinessEntity()).ToList()
            };
        }
    }
}
