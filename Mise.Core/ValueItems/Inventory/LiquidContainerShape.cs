using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Core.ValueItems.Inventory
{
    /// <summary>
    /// Describes the shape of a container of liquid, especially how to render and calculate the value for it
    /// </summary>
	public class LiquidContainerShape : IEquatable<LiquidContainerShape>, ITextSearchable
    {
        public LiquidContainerShape()
        {
            WidthsAsPercentageOfHeight = new List<double>();
        }

        /// <summary>
        /// Represents the widths at evenly spaced intervals, as a percentage of height.  Goes from bottom up
        /// </summary>
        public List<double> WidthsAsPercentageOfHeight { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// Given the zero-based index of the width measure, calculate how much percentage of the volume (0-1) is below it
        /// Exampe given 1, 2, 3, 4, 5, giving 2 to this would return 6 (3 + 2 + 1)
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        /// 
        public double GetPercentageContainedAt(int index)
        {

            if (index > WidthsAsPercentageOfHeight.Count)
            {
                throw new ArgumentException("Don't have " + index + " measurements!");
            }

            //we're full
            if (index + 1 == WidthsAsPercentageOfHeight.Count)
            {
                return 1.0;
            }

            //calc the total
            var totalWidths = WidthsAsPercentageOfHeight.Sum(w => w);

            double takenWidths = 0;
            for (var i = 0; i <= index; i++)
            {
                takenWidths += WidthsAsPercentageOfHeight[i];
            }

            var res = takenWidths/totalWidths;

            //if we're not at the top level, we should max out at 95%
            return res > 0.95 ? 0.95 : res;
        }


        public bool Equals(LiquidContainerShape other)
        {
             if (other == null)
            {
                return false;
            }
            //make sure each level contains the other!
            if (Name != other.Name)
            {
                return false;
            }

            if (WidthsAsPercentageOfHeight.Count != other.WidthsAsPercentageOfHeight.Count)
            {
                return false;
            }

            for (int i = 0; i < WidthsAsPercentageOfHeight.Count; i++)
            {
                if (Math.Abs(WidthsAsPercentageOfHeight[i] - other.WidthsAsPercentageOfHeight[i]) > .1)
                {
                    return false;
                }
            }
            return true;
        }

		#region ITextSearchable implementation

		public bool ContainsSearchString (string searchString)
		{
			return string.IsNullOrEmpty (Name) == false && Name.ToUpper ().Contains (searchString.ToUpper ());
		}

		#endregion

		#region Defaults
        public static LiquidContainerShape DefaultBottleShape
        {
            get
            {
                return new LiquidContainerShape
                {
                    Name = "Default Bottle",
                    WidthsAsPercentageOfHeight = {.3, .33, .33, .33, .33, .33, .3, .16, .1, .1}
                };
            }
        }

        public static LiquidContainerShape WineBottleShape
        {
            get
            {
                return new LiquidContainerShape
                {
                    Name = "Default Bottle",
                    WidthsAsPercentageOfHeight = { .233, .233, .233, .233, .233, .233, .233, .139, .08, .09 }
                };
            }
        }
        public static LiquidContainerShape DefaultCanShape
        {
            get
            {
                return new LiquidContainerShape
                {
                    Name = "Default Can",
                    WidthsAsPercentageOfHeight = {.5, .519, .519, .519, .519, .519, .519, .519, .519, .5}
                };
            }
        }

        public static LiquidContainerShape DefaultKegShape
        {
            get
            {
                return new LiquidContainerShape
                {
                    Name = "Default Keg",
                    WidthsAsPercentageOfHeight = {.705, .705, .75, .705, .705, .705, .75, .705, .705, .705}
                };
            }
        }

        public static LiquidContainerShape DefaultBeerBottleShape
        {
            get
            {
                return new LiquidContainerShape
                {
                    Name = "Beer Bottle",
                    WidthsAsPercentageOfHeight = {.25, .25, .25, .25, .25, .25, .143, .12, .11, .12}
                };
            }
        }

		public static LiquidContainerShape Box{
			get{
				return new LiquidContainerShape {
					Name = "Box",
					WidthsAsPercentageOfHeight = {1, 1, 1, 1, 1, 1, 1, 1, 1, 1}
				};
			}
		}
		public static IEnumerable<LiquidContainerShape> GetDefaultShapes(){
			return new List<LiquidContainerShape>{ 
				DefaultBottleShape, 
				DefaultCanShape, 
				WineBottleShape, 
				DefaultKegShape, 
				DefaultBeerBottleShape };
		}
		#endregion
    }
}
