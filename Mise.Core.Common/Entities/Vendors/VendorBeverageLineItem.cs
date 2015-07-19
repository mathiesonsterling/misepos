using System;
using System.Linq;
using System.Collections.Generic;
using Mise.Core.Entities.Base;
using Mise.Core.Entities.Vendors;
using Mise.Core.ValueItems;
using Mise.Core.ValueItems.Inventory;
using Mise.Core.Entities.Inventory;
using Mise.Core.Common.Entities.Inventory;
namespace Mise.Core.Common.Entities.Vendors
{
	public class VendorBeverageLineItem : EntityBase, IVendorBeverageLineItem
	{
		public VendorBeverageLineItem(){
			Categories = new List<ItemCategory> ();
            PricePerUnitForRestaurant = new Dictionary<Guid, Money>();
		}

		public Guid VendorID {
			get;
			set;
		}
		public LiquidContainer Container {
			get;
			set;
		}

		public string DetailDisplay {
			get {
				var res = string.Empty;
				if(Categories.Any()){
					res += Categories.First ().Name;
				}
				if(Container != null){
					res += "  " + Container.DisplayName;
				}
				return res;
			}
		}

		public string NameInVendor {
			get;
			set;
		}

		public int? CaseSize {
			get;
			set;
		}

		string _displayName;
		public string DisplayName { get{
				if(string.IsNullOrEmpty (_displayName)){
					return MiseName;
				}
				return _displayName;
			}
			set{
				_displayName = value;
			}
		}

	    public string MiseName {
			get;
			set;
		}
		public string UPC {
			get;
			set;
		}

		//the price published per unit
		public Money PublicPricePerUnit {
			get;
			set;
		}

		//last price given to this restaurant
		public Dictionary<Guid, Money> PricePerUnitForRestaurant{get;set;}

	    public Money GetLastPricePaidByRestaurantPerUnit(Guid restaurantID)
	    {
	        return PricePerUnitForRestaurant.ContainsKey(restaurantID) 
                ? PricePerUnitForRestaurant[restaurantID] 
                : null;
	    }

	    public Dictionary<Guid, Money> GetPricesForRestaurants()
	    {
	        return PricePerUnitForRestaurant;
	    }

	    public DateTimeOffset? LastTimePriceSet {
			get;
			set;
		}

	    public ICloneableEntity Clone()
	    {
	        var newItem = CloneEntityBase(new VendorBeverageLineItem());
	        newItem.MiseName = MiseName;
	        newItem.VendorID = VendorID;
	        newItem.Container = Container;
	        newItem.UPC = UPC;
	        newItem.PublicPricePerUnit = PublicPricePerUnit;
	        newItem.PricePerUnitForRestaurant = new Dictionary<Guid, Money>(PricePerUnitForRestaurant);
	        newItem.NameInVendor = NameInVendor;
			newItem.Categories = Categories.Select (c => c).ToList ();
	        return newItem;
	    }

		public bool ContainsSearchString (string searchString)
		{
			return (string.IsNullOrEmpty (MiseName) == false && MiseName.ToUpper ().Contains (searchString.ToUpper ()))
			|| (string.IsNullOrEmpty (NameInVendor) == false && NameInVendor.ToUpper ().Contains (searchString.ToUpper ()))
			|| (Container != null && Container.ContainsSearchString (searchString))
		    || (Categories != null && Categories.Any(c => c.ContainsSearchString(searchString)));
		}

		#region Tagging
		/// <summary>
		/// Tags which come from Mise and should not be altered
		/// </summary>
		/// <value>The mise tags.</value>
		public List<Tag> MiseTags{get;set;}

		/// <summary>
		/// Tags specific to this restaurant
		/// </summary>
		/// <value>The user tags.</value>
		public List<Tag> RestaurantTags{get;set;}

		public IEnumerable<Tag> GetTags ()
		{
			var all = new List<Tag> (MiseTags);
			all.AddRange (RestaurantTags);
			return all;
		}

		public bool AddTag (Tag tag)
		{
			if(RestaurantTags.Contains (tag)){
				return false;
			}

			RestaurantTags.Add (tag);
			return true;
		}

		public bool RemoveTag (Tag tag)
		{
			if(RestaurantTags.Contains (tag)){
				return false;
			}

			return RestaurantTags.Remove (tag);
		}
			
		#endregion
		public List<ItemCategory> Categories{get;set;}
		public IEnumerable<ICategory> GetCategories(){
			return Categories;
		}
	}
}

