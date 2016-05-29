using System.Collections.Generic;
using System.Linq;
using Mise.Core.Entities.Base;
using Mise.Core.Entities.Inventory;
using Mise.Core.ValueItems.Inventory;

namespace Mise.Core.Common.Entities.Inventory
{
	public class ParBeverageLineItem : BaseBeverageLineItem, IParBeverageLineItem
	{
		public ParBeverageLineItem(){
			Categories = new List<InventoryCategory> ();
		}

        public ICloneableEntity Clone()
        {
            var newItem = CloneRestaurantBase(new ParBeverageLineItem());
            newItem.MiseName = MiseName;
            newItem.UPC = UPC;
            newItem.Container = Container;
            newItem.Quantity = Quantity;
			newItem.Categories = Categories.Select (c => c).ToList ();
            newItem.CaseSize = CaseSize;
            newItem.DisplayName = DisplayName;
            return newItem;
        }

	    public bool Equals(IParBeverageLineItem other)
	    {
	        if (other == null)
	        {
	            return false;
	        }

	        var res = MiseName == other.MiseName
	                  && UPC == other.UPC
	                  && Container.Equals(other.Container)
	                  && Quantity == other.Quantity
	                  && CaseSize == other.CaseSize
	                  && DisplayName == other.DisplayName;

	        var otherCats = other.GetCategories().Select(c => c.Name).ToList();
	        res = res && Categories.Count == otherCats.Count;

	        if (res == false)
	        {
	            return false;
	        }
	        return Categories.All(c => otherCats.Contains(c.Name));
	    }
    }
}
