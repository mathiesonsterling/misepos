using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Entities.Inventory;
using Mise.Database.AzureDefinitions.ValueItems;

namespace Mise.Database.AzureDefinitions.Entities.Inventory.LineItems
{
    public class ReceivingOrderBeverageLineItem : BaseLiquidLineItemEntity<IReceivingOrderLineItem, ReceivingOrderLineItem>
    {
        /// <summary>
        /// How much we paid, total for quantity
        /// </summary>
        public Money LineItemPrice { get; set; }

        public Money UnitPrice { get; set; }

        public bool ZeroedOut { get; set; }

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
