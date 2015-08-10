using System;
using System.Linq;
using Mise.Core.Entities;
using Mise.Core.Entities.Base;
using Mise.Core.Entities.Inventory;
using Mise.Core.Entities.Vendors;
using Mise.Core.ValueItems;
using Mise.Core.ValueItems.Inventory;
using System.Collections.Generic;
namespace Mise.Core.Common.Entities.Inventory
{
    /// <summary>
    /// Represents an item the restaurant has in inventory
    /// </summary>
	public class InventoryBeverageLineItem : BaseTaggableRestaurantEntity, IInventoryBeverageLineItem
    {
        public InventoryBeverageLineItem()
        {
            PricePaid = Money.None;
			MethodsMeasuredLast = MeasurementMethods.Unmeasured;
        }

        public Guid? VendorBoughtFrom { get; set; }


		/// <summary>
		/// How much stuff we actually have
		/// </summary>
		/// <value>The current amount.</value>
        public LiquidAmount CurrentAmount { get; set; }

        public LiquidContainerShape Shape
        {
            get
            {
                if (Container != null && Container.Shape != null)
                {
                    return Container.Shape;
                }

                //don't have container, let's get it via category
                var standCat = Categories?.FirstOrDefault(c => c.IsCustomCategory == false);
                if (standCat != null)
                {
					var catService = new CategoriesService ();
					return catService.GetShapeForCategory (standCat);
                }

				return LiquidContainerShape.DefaultBottleShape;
            }
        }

        public List<decimal> PartialBottleListing{ get; set;}

        public IEnumerable<decimal> GetPartialBottlePercentages()
        {
            if (PartialBottleListing == null)
            {
                return new List<decimal>();
            }
            return PartialBottleListing;
        }

		public int NumPartialBottles{get{ return PartialBottleListing.Count;
			}}
		
		public int NumFullBottles{get;set;}

		/// <summary>
		/// Total number of physical bottles that are part of this line item
		/// </summary>
		/// <value>The total bottles.</value>
		public decimal Quantity {
			get {
				var totalPartials = PartialBottleListing != null && PartialBottleListing.Any ()
					? PartialBottleListing.Sum (s => s)
					: 0.0M;
				return 
					NumFullBottles + totalPartials;
			}
		}

		public int? CaseSize {
			get;
			set;
		}

        public MeasurementMethods MethodsMeasuredLast { get; set; }

        public Money PricePaid { get; set; }

        public decimal GetPercentageFull()
        {
            return CurrentAmount.Milliliters / Container.AmountContained.Milliliters;
        }
        public ICloneableEntity Clone()
        {
            var newItem = CloneRestaurantBase(new InventoryBeverageLineItem());
            newItem.MiseName = MiseName;
            newItem.UPC = UPC;
            newItem.DisplayName = DisplayName;
            newItem.MethodsMeasuredLast = MethodsMeasuredLast;
            newItem.CurrentAmount = CurrentAmount;
            newItem.Container = Container;
            newItem.VendorBoughtFrom = VendorBoughtFrom;
            newItem.NumFullBottles = NumFullBottles;
			newItem.PartialBottleListing = PartialBottleListing.Select (d => d).ToList();
			newItem.Categories = Categories.Select (c => c).ToList ();
            return newItem;
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
        public string MiseName { get; set; }
        public string UPC { get; set; }
        public LiquidContainer Container { get; set; }

		public bool ContainsSearchString (string searchString)
		{
			return (DisplayName != null && DisplayName.ToUpper ().Contains (searchString.ToUpper ()))
				|| (MiseName != null && MiseName.ToUpper ().Contains (searchString.ToUpper ()))
				|| (Container != null && Container.ContainsSearchString (searchString))
				|| (Categories != null && Categories.Any(c => c.ContainsSearchString(searchString)));
		}
			
		public bool HasBeenMeasured {
			get {
				return MethodsMeasuredLast != MeasurementMethods.Unmeasured;
			}
		}

        public int InventoryPosition { get; set; }

        public List<ItemCategory> Categories{get;set;}
		public IEnumerable<ICategory> GetCategories(){
			return Categories;
		}

		public string CategoryDisplay {
			get {
				return Categories.Any () ? Categories.First ().Name : string.Empty;
			}
		}
    }
}
