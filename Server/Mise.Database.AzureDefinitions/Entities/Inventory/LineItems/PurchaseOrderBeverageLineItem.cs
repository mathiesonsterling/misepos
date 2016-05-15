﻿using System;
using System.Collections.Generic;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Entities.Inventory;
using InventoryCategory = Mise.Database.AzureDefinitions.Entities.Categories.InventoryCategory;

namespace Mise.Database.AzureDefinitions.Entities.Inventory.LineItems
{
    public class PurchaseOrderBeverageLineItem : BaseLiquidLineItemEntity<IPurchaseOrderLineItem, PurchaseOrderLineItem>
    {
        public PurchaseOrderBeverageLineItem() { }

        public PurchaseOrderBeverageLineItem(IPurchaseOrderLineItem source, IEnumerable<InventoryCategory> cats,
            PurchaseOrderPerVendor pov)
            : base(source, cats)
        {
            PurchaseOrderPerVendor = pov;
        }

        public PurchaseOrderPerVendor PurchaseOrderPerVendor { get; set; }

        protected override PurchaseOrderLineItem CreateConcreteLineItemClass()
        {
            return new PurchaseOrderLineItem
            {
                VendorID = PurchaseOrderPerVendor.Vendor.EntityId
            };
        }
    }
}
