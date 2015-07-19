using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities;
using Mise.Core.Entities.Base;
using Mise.Core.Entities.Inventory;

namespace Mise.Core.Common.Entities.Inventory
{
	public class InventorySection : RestaurantEntityBase, IInventorySection
    {
        public InventorySection()
        {
            LineItems = new List<InventoryBeverageLineItem>();
        }

        public string Name { get; set; }

		public bool Completed {
			get;
			set;
		}

        public Guid RestaurantInventorySectionID { get; set; }

        public List<InventoryBeverageLineItem> LineItems { get; set; }
 
        public IEnumerable<IInventoryBeverageLineItem> GetInventoryBeverageLineItemsInSection()
        {
            return LineItems;
        }

	    public int GetNextItemPosition()
	    {
	        return LineItems.Count + 1;
	    }

	    public ICloneableEntity Clone()
        {
            var newItem = base.CloneRestaurantBase(new InventorySection());
            newItem.Name = Name;
            newItem.LineItems = LineItems.Select(li => li.Clone() as InventoryBeverageLineItem).ToList();
            newItem.RestaurantInventorySectionID = RestaurantInventorySectionID;
            return newItem;
        }
    }
}
