﻿using System;
using System.Collections.Generic;
using Mise.Core.Entities.Inventory;

namespace Mise.Core.ValueItems.Inventory
{
    public class LiquidContainer : IEquatable<LiquidContainer>, ITextSearchable
    {
        public LiquidContainer() { }

        public LiquidContainer(ILiquidContainerEntity source)
        {
            BusinessId = source.BusinessId;
            DisplayName = source.DisplayName;
            AmountContained = source.AmountContained;
            WeightEmpty = source.WeightEmpty;
            WeightFull = source.WeightFull;
            Shape = source.Shape;
        }

        #region Standard sizes

        public static LiquidContainer Bottle750ML => new LiquidContainer { 
            AmountContained = new LiquidAmount { Milliliters = 750 }, 
            DisplayName = "750ml Bottle"
        };

        public static LiquidContainer Bottle1L => new LiquidContainer { AmountContained = new LiquidAmount { Milliliters = 1000 }, 
            DisplayName = "1L Bottle"
        };

        /// <summary>
        /// Handle of booze
        /// </summary>
        public static LiquidContainer Bottle1_75ML => new LiquidContainer
        {
            AmountContained = new LiquidAmount {Milliliters = 1750M},
            DisplayName = "1.75L Bottle"
        };

        public static LiquidContainer Bottle375ML => new LiquidContainer
        {
            AmountContained = new LiquidAmount {Milliliters = 375},
            DisplayName = "375ml Bottle"
        };

        public static LiquidContainer Can12Oz => new LiquidContainer { 
            AmountContained = LiquidAmount.FromLiquidOunces(12.0M), 
            DisplayName = "12oz Can",
            Shape = LiquidContainerShape.DefaultCanShape
        };

        public static LiquidContainer Can16Oz => new LiquidContainer
        {
            AmountContained = LiquidAmount.FromLiquidOunces(16.0M),
            DisplayName = "16oz Can",
            Shape = LiquidContainerShape.DefaultCanShape
        };

        public static LiquidContainer Can10Oz => new LiquidContainer
        {
            AmountContained = LiquidAmount.FromLiquidOunces(10.0M),
            DisplayName = "10oz Bottle",
            Shape = LiquidContainerShape.DefaultCanShape
        };

        public static LiquidContainer Can250ML => new LiquidContainer
        {
            AmountContained = new LiquidAmount {Milliliters = 250M},
            DisplayName = "250ml Can",
            Shape = LiquidContainerShape.DefaultCanShape
        };

        public static LiquidContainer Bottle12Oz => new LiquidContainer
        {
            AmountContained = new LiquidAmount {Milliliters = 354.882M },
            DisplayName = "12oz Bottle",
            Shape = LiquidContainerShape.DefaultBeerBottleShape	
        };

        public static LiquidContainer Bottle16Oz => new LiquidContainer
        {
            AmountContained = new LiquidAmount {Milliliters = 473.176M },
            DisplayName = "16oz Bottle",
            Shape = LiquidContainerShape.DefaultBeerBottleShape	
        };

        public static LiquidContainer Bottle7Oz => new LiquidContainer
        {
            AmountContained = LiquidAmount.FromLiquidOunces(7.0M),
            DisplayName = "7oz Bottle",
            Shape = LiquidContainerShape.DefaultBeerBottleShape	
        };

        public static LiquidContainer Bottle40Oz => new LiquidContainer
        {
            AmountContained = LiquidAmount.FromLiquidOunces(40M),
            DisplayName = "40oz Bottle",
            Shape = LiquidContainerShape.DefaultBeerBottleShape	
        };

        public static LiquidContainer Bottle4OzBitters => new LiquidContainer
        {
            AmountContained = new LiquidAmount {Milliliters = 118.294M},
            DisplayName = "4oz Bottle (Bitters)",
            Shape = LiquidContainerShape.DefaultBottleShape
        };

        public static LiquidContainer Bottle330ML => new LiquidContainer
        {
            AmountContained = new LiquidAmount { Milliliters = 330 },
            DisplayName = "330ml Bottle"
        };

        public static LiquidContainer Bottle500ML => new LiquidContainer
        {
            AmountContained = new LiquidAmount { Milliliters = 500 },
            DisplayName = "500ml Bottle"
        };

        public static LiquidContainer Keg => new LiquidContainer
        {
            AmountContained = new LiquidAmount {Milliliters = 58673.8827M},
            DisplayName = "Keg (Half Barrel)",
            Shape = LiquidContainerShape.DefaultKegShape
        };

        public static LiquidContainer HalfKeg => new LiquidContainer
        {
            AmountContained = new LiquidAmount {Milliliters = 29336.94135M},
            DisplayName = "Pony Keg (Quarter Barrel)",
            Shape = LiquidContainerShape.DefaultKegShape
        };

        public static LiquidContainer TorpedoKeg => new LiquidContainer {
            AmountContained = new LiquidAmount{ Milliliters = 19800M },
            DisplayName = "Sixth Barrel",
            Shape = LiquidContainerShape.DefaultKegShape
        };

        public static LiquidContainer ImportKeg => new LiquidContainer {
            AmountContained = new LiquidAmount{Milliliters = 50000M},
            DisplayName = "Import Keg (50L)",
            Shape = LiquidContainerShape.DefaultKegShape
        };

        public static IEnumerable<LiquidContainer> GetStandardBarSizes()
        {
            return new List<LiquidContainer>
            {
                Bottle750ML,
                Bottle375ML,
                Bottle330ML,
                Bottle1L,
                Bottle1_75ML,
                Bottle500ML,
                Bottle1L,
                Bottle12Oz,
                Can12Oz,
                Bottle16Oz,
                Bottle7Oz,
                Can16Oz,
                Can10Oz,
                Bottle40Oz,
                Keg,
                HalfKeg,
				ImportKeg,
				TorpedoKeg,
                Can250ML,
                Bottle4OzBitters
            };
        }
        #endregion

        /// <summary>
        /// If set, this container only shows up for this restaurant
        /// </summary>
        public Guid? BusinessId { get; set; }

        private string _displayName;
        public string DisplayName
        {
            get
            {
                if (string.IsNullOrEmpty(_displayName))
                {
                    return AmountContained.GetInMilliliters() + " ml";
                }
                return _displayName;
            }
            set
            {
                _displayName = value;
            }
        }

        /// <summary>
        /// The total amount of liquid this container can hold
        /// </summary>
        public LiquidAmount AmountContained { get; set; }

        /// <summary>
        /// How much the container weighs when empty
        /// </summary>
        public Weight WeightEmpty { get; set; }

        /// <summary>
        /// How much the container weighs when full (also the amount * specific gravity should tie in with WeightFull - WeightEmpty!)
        /// </summary>
        public Weight WeightFull { get; set; }

		/// <summary>
		/// If set, the container shape we use for this container
		/// </summary>
		/// <value>The shape.</value>
		public LiquidContainerShape Shape{get;set;}

        public bool Equals(LiquidContainer other)
        {
            if (AmountContained.Equals(other.AmountContained) == false)
            {
                return false;
            }
            if ((WeightEmpty == null && other.WeightEmpty != null)
                || (WeightEmpty != null && other.WeightEmpty == null))
            {
                return false;
            }
            if (WeightEmpty != null && other.WeightEmpty != null && WeightEmpty.Equals(other.WeightEmpty) == false)
            {
                return false;
            }

            if ((WeightFull == null && other.WeightFull != null)
                || (WeightFull != null && other.WeightFull == null))
            {
                return false;
            }

            if (WeightFull != null && other.WeightFull != null && WeightFull.Equals(other.WeightFull) == false)
            {
                return false;
            }

			if ((Shape != null && other.Shape == null) || (other.Shape != null && Shape == null)) {
				return false;
			}

			if (Shape != null && other.Shape != null && Shape.Equals (other.Shape) == false) {
				return false;
			}
            return true;
        }

        public override bool Equals(object obj)
        {
            var other = obj as LiquidContainer;
            if (other == null)
            {
                return false;
            }
            return Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (AmountContained != null ? AmountContained.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (WeightEmpty != null ? WeightEmpty.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (WeightFull != null ? WeightFull.GetHashCode() : 0);
                return hashCode;
            }
        }

        public class ContainerComparer : IEqualityComparer<LiquidContainer>
        {
            public bool Equals(LiquidContainer x, LiquidContainer y)
            {
                return x.Equals(y);
            }

            public int GetHashCode(LiquidContainer lc)
            {
                return lc.GetHashCode();
            }
        }

        #region ITextSearchable implementation

        public bool ContainsSearchString(string searchString)
        {
			return (AmountContained != null && AmountContained.Milliliters.ToString ().Contains (searchString))
			|| (DisplayName != null && DisplayName.ToUpper ().Contains (searchString.ToUpper ()))
			|| (Shape != null && Shape.ContainsSearchString (searchString));
        }

        #endregion
    }


}
