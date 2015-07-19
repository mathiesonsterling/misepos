using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Core.ValueItems.Inventory
{
	public class LiquidContainer : IEquatable<LiquidContainer>, ITextSearchable
    {
		private string _displayName;
		public string DisplayName{get{
				if(string.IsNullOrEmpty (_displayName)){
					return AmountContained.GetInMilliliters () + " ml";
				}
				return _displayName;
			}
			set{
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
			if(string.IsNullOrEmpty (_displayName) == false){
				if(_displayName != other.DisplayName){
					return false;
				}
			}

            if (AmountContained.Equals(other.AmountContained) == false)
            {
                return false;
            }
            if ((WeightEmpty == null && other.WeightEmpty != null)
                ||( WeightEmpty != null && other.WeightEmpty == null))
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

		public bool ContainsSearchString (string searchString)
		{
			return (AmountContained != null && AmountContained.Milliliters.ToString ().Contains (searchString)) 
                || (DisplayName != null && DisplayName.ToUpper().Contains(searchString.ToUpper()));
		}

		#endregion
    }


}
