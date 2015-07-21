using System;
using System.Linq;
using System.Collections.Generic;
using Mise.Core.Entities;
using Mise.Core.Entities.Base;
using Mise.Core.Entities.Inventory;
using Mise.Core.ValueItems;
using Mise.Core.ValueItems.Inventory;

namespace Mise.Core.Common.Entities.Inventory
{
	public class ReceivingOrderLineItem : BaseTaggableRestaurantEntity, IReceivingOrderLineItem
    {
		public ReceivingOrderLineItem(){
			Categories = new List<ItemCategory> ();
			LineItemPrice = Money.None;
			UnitPrice = Money.None;
		}

        public string DisplayName { get; set; }
        public string MiseName { get; set; }
        public string UPC { get; set; }
        public LiquidContainer Container { get; set; }
	

        /// <summary>
        /// Quantity of bottles listed
        /// </summary>
        public int Quantity { get; set; }

		public int? CaseSize { get; set;}

        /// <summary>
        /// How much we paid, per bottle
        /// </summary>
        public Money LineItemPrice { get; set; }
		public Money UnitPrice{get;set;}

		public bool ZeroedOut { get; set;}

        public ICloneableEntity Clone()
        {
            var newItem = CloneRestaurantBase(new ReceivingOrderLineItem());
            newItem.MiseName = MiseName;
            newItem.UPC = UPC;
            newItem.Container = Container;
            newItem.Quantity = Quantity;
            newItem.UnitPrice = UnitPrice;
			newItem.LineItemPrice = LineItemPrice;
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
			return (DisplayName != null && DisplayName.ToUpper().Contains (searchString.ToUpper()))
			|| (MiseName != null &&MiseName.ToUpper().Contains (searchString.ToUpper()))
			|| (Container != null && Container.ContainsSearchString (searchString))
			|| (Categories != null && Categories.Any(c => c.ContainsSearchString(searchString)));
		}
    }
}
