using System;
using System.Linq;
using System.Collections.Generic;
using Mise.Core.Entities.Base;
using Mise.Core.Entities.Inventory;
using Mise.Core.ValueItems.Inventory;

namespace Mise.Core.Common.Entities.Inventory
{
	public class PurchaseOrderLineItem : BaseTaggableRestaurantEntity, IPurchaseOrderLineItem
    {
		public PurchaseOrderLineItem(){
			Categories = new List<ItemCategory> ();
		}

        public int Quantity { get; set; }
		public Guid? VendorID{get;set;}

		public int? CaseSize { get; set;}

        public string DisplayName { get; set; }
        public string MiseName { get; set; }
        public string UPC { get; set; }
        public LiquidContainer Container { get; set; }

        public ICloneableEntity Clone()
        {
            var newItem = CloneRestaurantBase(new PurchaseOrderLineItem());
            newItem.Quantity = Quantity;
            newItem.MiseName = MiseName;
            newItem.UPC = UPC;
            newItem.DisplayName = DisplayName;
            newItem.Container = Container;
			newItem.Categories = Categories.Select (c => c).ToList ();
            return newItem;
        }

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
			return DisplayName.ToUpper ().Contains (searchString.ToUpper ())
				|| (string.IsNullOrEmpty (MiseName) == false && MiseName.ToUpper ().Contains (searchString.ToUpper ()))
				|| Quantity.ToString ().ToUpper().Contains (searchString.ToUpper ())
			|| Container.ContainsSearchString (searchString)
				|| (Categories != null && Categories.Any(c => c.ContainsSearchString(searchString)));
		}
    }
}
