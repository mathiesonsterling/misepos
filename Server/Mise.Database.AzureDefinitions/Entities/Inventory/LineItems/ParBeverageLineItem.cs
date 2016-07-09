using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Entities.Inventory;

namespace Mise.Database.AzureDefinitions.Entities.Inventory.LineItems
{
    public class ParBeverageLineItem : BaseLiquidLineItemEntity<IParBeverageLineItem, Core.Common.Entities.Inventory.ParBeverageLineItem>
    {
        public ParBeverageLineItem() { }

        public ParBeverageLineItem(IParBeverageLineItem source, Par par, IEnumerable<Categories.InventoryCategory> categories)
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

	    [ForeignKey("Par")]
	    public string ParId { get; set; }
    }
}
