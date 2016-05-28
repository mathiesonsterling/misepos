using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Entities.Inventory;

namespace Mise.Core.Client.Entities.Inventory.LineItems
{
    public class ParBeverageLineItem : BaseLiquidLineItemEntity<IParBeverageLineItem, Core.Common.Entities.Inventory.ParBeverageLineItem>
    {
        public ParBeverageLineItem() { }

        public ParBeverageLineItem(IParBeverageLineItem source, IEnumerable<Categories.InventoryCategory> categories) : base(source, categories)
        {
            
        }

        protected override Core.Common.Entities.Inventory.ParBeverageLineItem CreateConcreteLineItemClass()
        {
            return new Core.Common.Entities.Inventory.ParBeverageLineItem();
        }
    }
}
