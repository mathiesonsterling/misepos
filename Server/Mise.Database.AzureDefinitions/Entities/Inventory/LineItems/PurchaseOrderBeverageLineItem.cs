using System;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Entities.Inventory;

namespace Mise.Database.AzureDefinitions.Entities.Inventory.LineItems
{
    public class PurchaseOrderBeverageLineItem : BaseLiquidLineItemEntity<IPurchaseOrderLineItem, PurchaseOrderLineItem>
    {
        public Vendor.Vendor Vendor { get; set; }

        protected override PurchaseOrderLineItem CreateConcreteLineItemClass()
        {
            return new PurchaseOrderLineItem
            {
                VendorID = Vendor.EntityId
            };
        }
    }
}
