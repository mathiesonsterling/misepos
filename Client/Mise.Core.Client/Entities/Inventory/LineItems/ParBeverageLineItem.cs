using System.Collections.Generic;
using Mise.Core.Entities.Inventory;
using InventoryCategory = Mise.Core.Client.Entities.Categories.InventoryCategory;
using Par = Mise.Core.Client.Entities.Inventory.Par;

namespace Mise.Core.Client.Entities.Inventory.LineItems
{
    public class ParBeverageLineItem : BaseLiquidLineItemEntity<IParBeverageLineItem, Core.Common.Entities.Inventory.ParBeverageLineItem>
    {
        public ParBeverageLineItem() { }

        public ParBeverageLineItem(IParBeverageLineItem source, Par par, IEnumerable<InventoryCategory> categories)
	        : base(source, categories)
        {
	        Par = par;
	        ParId = par.Id;
        }

        protected override Core.Common.Entities.Inventory.ParBeverageLineItem CreateConcreteLineItemClass()
        {
            return new Core.Common.Entities.Inventory.ParBeverageLineItem();
        }

	    public Par Par { get; set; }

	    public string ParId { get; set; }
    }
}
