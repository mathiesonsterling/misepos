using System.Collections.Generic;
using System.Linq;
using Mise.Core.Entities.Base;
using Mise.Core.Entities.Inventory;
using Mise.Core.ValueItems.Inventory;

namespace Mise.Core.Common.Entities.Inventory
{
	public class PARBeverageLineItem : BaseTaggableRestaurantEntity, IPARBeverageLineItem
	{
		public PARBeverageLineItem(){
			Categories = new List<ItemCategory> ();
		}

        public string MiseName { get; set; }

		string _displayName;
		public string DisplayName { 
            get
            {
                var name = string.IsNullOrEmpty(_displayName) ? MiseName : _displayName;
                return name;
            }
			set{
				_displayName = value;
			}
		}

        public string UPC { get; set; }
        public LiquidContainer Container { get; set; }

		public int? CaseSize { get; set;}

        public ICloneableEntity Clone()
        {
            var newItem = CloneRestaurantBase(new PARBeverageLineItem());
            newItem.MiseName = MiseName;
            newItem.UPC = UPC;
            newItem.Container = Container;
            newItem.Quantity = Quantity;
			newItem.Categories = Categories.Select (c => c).ToList ();
            newItem.CaseSize = CaseSize;
            newItem.DisplayName = _displayName;
            return newItem;
        }

        public decimal Quantity { get; set; }

		public List<ItemCategory> Categories{get;set;}
		public IEnumerable<ICategory> GetCategories(){
			return Categories;
		}
		public string CategoryDisplay {
			get {
				return Categories.Any () ? Categories.First ().Name : string.Empty;
			}
		}

		public bool ContainsSearchString (string searchString)
		{
			return (string.IsNullOrEmpty (MiseName) == false && MiseName.ToUpper ().Contains (searchString.ToUpper ()))
			|| (string.IsNullOrEmpty (DisplayName) == false && DisplayName.ToUpper ().Contains (searchString.ToUpper ()))
			|| Quantity.ToString ().Contains (searchString)
			|| (Container != null && Container.ContainsSearchString (searchString))
				|| (Categories != null && Categories.Any(c => c.ContainsSearchString(searchString)));
		}

	    public bool Equals(IPARBeverageLineItem other)
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
