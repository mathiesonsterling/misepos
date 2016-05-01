using System;
using System.Collections.Generic;
using System.Linq;
using Mise.Core.Common.Services.Implementation;
using Mise.Core.Entities.Base;
using Mise.Core.Entities.Inventory;
using Mise.Core.ValueItems;
using Mise.Core.ValueItems.Inventory;

namespace Mise.Core.Common.Entities.Inventory
{
    /// <summary>
    /// Represents an item the restaurant has in inventory
    /// </summary>
	public class InventoryBeverageLineItem : BaseBeverageLineItem, IInventoryBeverageLineItem
    {
        public InventoryBeverageLineItem()
        {
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
                if (Container?.Shape != null)
                {
                    return Container.Shape;
                }

                //don't have container, let's get it via category
                var standCat = Categories?.FirstOrDefault(c => c.IsCustomCategory == false);
                if (standCat == null)
                {
                    return LiquidContainerShape.DefaultBottleShape;
                }
                var catService = new CategoriesService ();
                return catService.GetShapeForCategory (standCat);
            }
        }

        public Money PricePaid { get; set; }

        public List<decimal> PartialBottleListing{ get; set;}

        public IEnumerable<decimal> GetPartialBottlePercentages()
        {
            return PartialBottleListing ?? new List<decimal>();
        }

        public int NumPartialBottles => PartialBottleListing?.Count ?? 0;

        public int NumFullBottles{get;set;}

		/// <summary>
		/// Total number of physical bottles that are part of this line item
		/// </summary>
		/// <value>The total bottles.</value>
		public override decimal Quantity {
			get {
				var totalPartials = PartialBottleListing != null && PartialBottleListing.Any ()
					? PartialBottleListing.Sum (s => s)
					: 0.0M;
				return 
					NumFullBottles + totalPartials;
			}
		    set
		    {
		        //do nothing, shouldn't set!
		    }
		}

        public MeasurementMethods MethodsMeasuredLast { get; set; }


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
			newItem.PartialBottleListing = PartialBottleListing != null 
				? PartialBottleListing.Select (d => d).ToList ()
				: new List<decimal> ();
			newItem.Categories = Categories != null 
				? Categories.Select (c => c).ToList () 
				: new List<InventoryCategory>();
			newItem.InventoryPosition = InventoryPosition;
            newItem.PricePaid = PricePaid;
            return newItem;
        }
			
		public bool HasBeenMeasured => MethodsMeasuredLast != MeasurementMethods.Unmeasured;

        public int InventoryPosition { get; set; }
    }
}
