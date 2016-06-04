using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Entities.Inventory;
using Mise.Database.AzureDefinitions.ValueItems;
using InventoryCategory = Mise.Database.AzureDefinitions.Entities.Categories.InventoryCategory;

namespace Mise.Database.AzureDefinitions.Entities.Inventory.LineItems
{
    public class ReceivingOrderBeverageLineItem : BaseLiquidLineItemEntity<IReceivingOrderLineItem, ReceivingOrderLineItem>
    {
        public ReceivingOrderBeverageLineItem()
        {
            LineItemPrice = new MoneyDb();
            UnitPrice = new MoneyDb();
        }

        public ReceivingOrderBeverageLineItem(IReceivingOrderLineItem source, ReceivingOrder ro,
	        IEnumerable<InventoryCategory> cats)
            : base(source, cats)
        {
            LineItemPrice = new MoneyDb(source.LineItemPrice);
            UnitPrice = new MoneyDb(source.UnitPrice);
            ZeroedOut = source.ZeroedOut;
	        ReceivingOrder = ro;
	        ReceivingOrderId = ro.Id;
        }

        /// <summary>
        /// How much we paid, total for quantity
        /// </summary>
        public MoneyDb LineItemPrice { get; set; }

        public MoneyDb UnitPrice { get; set; }

        public bool ZeroedOut { get; set; }

	    public ReceivingOrder ReceivingOrder { get; set; }
	    [ForeignKey("ReceivingOrder")]
	    public string ReceivingOrderId { get; set; }

        protected override ReceivingOrderLineItem CreateConcreteLineItemClass()
        {
            return new ReceivingOrderLineItem
            {
                LineItemPrice = LineItemPrice.ToValueItem(),
                UnitPrice = UnitPrice.ToValueItem(),
                ZeroedOut = ZeroedOut
            };
        }
    }
}
