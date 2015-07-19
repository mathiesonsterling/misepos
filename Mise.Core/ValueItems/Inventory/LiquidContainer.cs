using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Core.ValueItems.Inventory
{
    public class LiquidContainer : IEquatable<LiquidContainer>, ITextSearchable
    {
        #region Standard sizes

        public static LiquidContainer Bottle750ML
        {
            get { return new LiquidContainer { AmountContained = new LiquidAmount { Milliliters = 750 }, DisplayName = "750ml Bottle" }; }
        }

        public static LiquidContainer Bottle1L
        {
            get { return new LiquidContainer { AmountContained = new LiquidAmount { Milliliters = 1000 }, DisplayName = "1L Bottle" }; }
        }

        public static LiquidContainer Bottle375ML
        {
            get { return new LiquidContainer { AmountContained = new LiquidAmount { Milliliters = 375 }, DisplayName = "375ml Bottle" }; }
        }

        public static LiquidContainer Can12oz
        {
            get { return new LiquidContainer { AmountContained = LiquidAmount.FromLiquidOunces(12.0M), DisplayName = "12oz Can" }; }
        }

        public static LiquidContainer Can16oz
        {
            get
            {
                return new LiquidContainer
                {
                    AmountContained = LiquidAmount.FromLiquidOunces(16.0M),
                    DisplayName = "16oz Can"
                };
            }
        }

        public static LiquidContainer Can10oz
        {
            get
            {
                return new LiquidContainer
                {
                    AmountContained = LiquidAmount.FromLiquidOunces(10.0M),
                    DisplayName = "10oz Bottle"
                };
            }
        }
        public static LiquidContainer Bottle12oz
        {
            get
            {
                return new LiquidContainer
                {
                    AmountContained = LiquidAmount.FromLiquidOunces(12.0M),
                    DisplayName = "12oz Bottle"
                };
            }
        }

        public static LiquidContainer Bottle7oz
        {
            get
            {
                return new LiquidContainer
                {
                    AmountContained = LiquidAmount.FromLiquidOunces(7.0M),
                    DisplayName = "7oz Bottle"
                };
            }
        }

        public static LiquidContainer Bottle330ml
        {
            get
            {
                return new LiquidContainer
                {
                    AmountContained = new LiquidAmount { Milliliters = 330 },
                    DisplayName = "330ml Bottle"
                };
            }
        }

        public static LiquidContainer Bottle500ml
        {
            get
            {
                return new LiquidContainer
                {
                    AmountContained = new LiquidAmount { Milliliters = 500 },
                    DisplayName = "500ml Bottle"
                };
            }
        }
        public static IEnumerable<LiquidContainer> GetStandardBarSizes()
        {
            return new List<LiquidContainer>
            {
                Bottle750ML,
                Bottle375ML,
                Bottle330ml,
                Bottle1L,
                Bottle500ml,
                Bottle1L,
                Bottle12oz,
                Can12oz,
                Bottle7oz,
            };
        }
        #endregion

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

        public bool Equals(LiquidContainer other)
        {
            if (string.IsNullOrEmpty(_displayName) == false)
            {
                if (_displayName != other.DisplayName)
                {
                    return false;
                }
            }

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
            return (AmountContained != null && AmountContained.Milliliters.ToString().Contains(searchString))
                || (DisplayName != null && DisplayName.ToUpper().Contains(searchString.ToUpper()));
        }

        #endregion
    }


}
