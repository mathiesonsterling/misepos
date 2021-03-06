﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Entities.Inventory;
using Mise.Core.ValueItems;
using InventoryCategory = Mise.Database.AzureDefinitions.Entities.Categories.InventoryCategory;

namespace Mise.Database.AzureDefinitions.Entities.Inventory.LineItems
{
    public class ReceivingOrderBeverageLineItem : BaseLiquidLineItemEntity<IReceivingOrderLineItem, ReceivingOrderLineItem>
    {
        public ReceivingOrderBeverageLineItem()
        {
        }

        public ReceivingOrderBeverageLineItem(IReceivingOrderLineItem source, ReceivingOrder ro,
	        IEnumerable<InventoryCategory> cats)
            : base(source, cats)
        {
            LineItemPrice = source.LineItemPrice.Dollars;
            UnitPrice = source.UnitPrice.Dollars;
            ZeroedOut = source.ZeroedOut;
	        ReceivingOrder = ro;
	        ReceivingOrderId = ro.Id;
        }

        /// <summary>
        /// How much we paid, total for quantity
        /// </summary>
        public decimal LineItemPrice { get; set; }

        public decimal UnitPrice { get; set; }

        public bool ZeroedOut { get; set; }

	    public ReceivingOrder ReceivingOrder { get; set; }
	    [ForeignKey("ReceivingOrder")]
	    public string ReceivingOrderId { get; set; }

        protected override ReceivingOrderLineItem CreateConcreteLineItemClass()
        {
            return new ReceivingOrderLineItem
            {
                LineItemPrice = new Money(LineItemPrice),
                UnitPrice = new Money(UnitPrice),
                ZeroedOut = ZeroedOut
            };
        }
    }
}
