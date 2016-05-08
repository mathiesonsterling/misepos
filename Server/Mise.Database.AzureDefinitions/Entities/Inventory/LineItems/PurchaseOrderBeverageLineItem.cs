using System;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Entities.Inventory;

namespace Mise.Database.AzureDefinitions.Entities.Inventory.LineItems
{
    public class PurchaseOrderBeverageLineItem : BaseLiquidLineItemEntity<IPurchaseOrderLineItem, PurchaseOrderLineItem>
    {
        public Guid? VendorID { get; set; }

        protected override PurchaseOrderLineItem CreateConcreteLineItemClass()
        {
            return new PurchaseOrderLineItem
            {
                VendorID = VendorID
            };
        }
    }
}
