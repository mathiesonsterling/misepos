using System.Collections.Generic;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Entities.Inventory;
using InventoryCategory = Mise.Core.Client.Entities.Categories.InventoryCategory;
using PurchaseOrderPerVendor = Mise.Core.Client.Entities.Inventory.PurchaseOrderPerVendor;

namespace Mise.Core.Client.Entities.Inventory.LineItems
{
    public class PurchaseOrderBeverageLineItem : BaseLiquidLineItemEntity<IPurchaseOrderLineItem, PurchaseOrderLineItem>
    {
        public PurchaseOrderBeverageLineItem() { }

        public PurchaseOrderBeverageLineItem(IPurchaseOrderLineItem source, 
            IEnumerable<InventoryCategory> cats,
            PurchaseOrderPerVendor pov)
            : base(source, cats)
        {
            PurchaseOrderPerVendor = pov;
	        PurchaseOrderPerVendorId = pov.Id;
        }

        public PurchaseOrderPerVendor PurchaseOrderPerVendor { get; set; }

	    public string PurchaseOrderPerVendorId { get; set; }

        protected override PurchaseOrderLineItem CreateConcreteLineItemClass()
        {
            return new PurchaseOrderLineItem
            {
                VendorID = PurchaseOrderPerVendor.Vendor.EntityId
            };
        }

    }
}
