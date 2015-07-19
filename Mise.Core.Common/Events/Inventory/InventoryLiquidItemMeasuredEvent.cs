using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Entities;
using Mise.Core.Entities.Inventory;
using Mise.Core.ValueItems.Inventory;

namespace Mise.Core.Common.Events.Inventory
{
    public class InventoryLiquidItemMeasuredEvent : BaseInventoryEvent
    {
        public override MiseEventTypes EventType
        {
            get { return MiseEventTypes.InventoryLiquidItemMeasured; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Guid RestaurantInventorySectionID { get; set; }

        public InventoryBeverageLineItem BeverageLineItem { get; set; }

		/// <summary>
		/// Number of bottles, regardless of how full
		/// </summary>
		/// <value>The number bottles measured.</value>
		public int NumFullBottlesMeasured{get;set;}

		/// <summary>
		/// Allows us to list out each partial bottle
		/// </summary>
		/// <value>The partial bottles.</value>
		public List<decimal> PartialBottles{ get; set;}

		public LiquidAmount AmountMeasured{get;set;}
	}
}
