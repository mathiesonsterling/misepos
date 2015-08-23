using System;
using System.Collections.Generic;
using System.Linq;
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

		public Guid InventoryID {
			get;
			set;
		}

        public string Name { get; set; }

		public bool Completed {
            get { return LastCompletedBy.HasValue; }
		}

        public Guid? LastCompletedBy { get; set; }

		public Guid? CurrentlyInUseBy {
			get;
			set;
		}

		public DateTimeOffset? TimeCountStarted{get;set;}

	    public bool ContainsSearchString(string searchString)
	    {
	        return (string.IsNullOrWhiteSpace(Name) == false && String.Equals(Name, searchString, StringComparison.CurrentCultureIgnoreCase))
	               || (LineItems.Any(li => li.ContainsSearchString(searchString)));
	    }

        public Guid RestaurantInventorySectionID { get; set; }

        public List<InventoryBeverageLineItem> LineItems { get; set; }
 
        public IEnumerable<IInventoryBeverageLineItem> GetInventoryBeverageLineItemsInSection()
        {
            return LineItems;
        }

	    public int GetNextItemPosition()
	    {
			//we'll do BASIC style line numbers, so we can get lots of reorders without having to get to 
			//n2 perf
			return (LineItems.Count + 1) * 10;
	    }

	    public ICloneableEntity Clone()
        {
            var newItem = CloneRestaurantBase(new InventorySection());
            newItem.Name = Name;
            newItem.LineItems = LineItems.Select(li => li.Clone() as InventoryBeverageLineItem).ToList();
            newItem.RestaurantInventorySectionID = RestaurantInventorySectionID;
            return newItem;
        }
    }
}
