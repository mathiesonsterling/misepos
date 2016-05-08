﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities.Inventory;
using Mise.Core.ValueItems.Inventory;
using Mise.Database.AzureDefinitions.Entities.Inventory.LineItems;

namespace Mise.Database.AzureDefinitions.Entities.Inventory
{
    public class PurchaseOrderPerVendor : BaseDbEntity<IPurchaseOrderPerVendor, Core.Common.Entities.Inventory.PurchaseOrderPerVendor>
    {
        /// <summary>
        /// The vendor this applies to
        /// </summary>
        public Vendor.Vendor Vendor { get; set; }

        public List<PurchaseOrderBeverageLineItem> LineItems { get; set; }

        public PurchaseOrderStatus Status { get; set; }

        protected override Core.Common.Entities.Inventory.PurchaseOrderPerVendor CreateConcreteSubclass()
        {
            return new Core.Common.Entities.Inventory.PurchaseOrderPerVendor
            {
                VendorID = Vendor?.EntityId,
                VendorName = Vendor?.Name,
                Status = Status,
                LineItems = LineItems.Select(li => li.ToBusinessEntity()).ToList()
            };
        }
    }
}
