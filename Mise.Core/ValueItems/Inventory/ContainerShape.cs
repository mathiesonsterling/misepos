using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Core.ValueItems.Inventory
{
    public interface IContainerShape<TAmountMeasureContained>
    {
        /// <summary>
        /// Given the percentage of the container filled, determine the actual amount contained
        /// </summary>
        /// <param name="percentage"></param>
        /// <param name="containerCapacity">Total amount the container can hold</param>
        /// <returns></returns>
        TAmountMeasureContained GetAmountContainedFromPercentageOfHeight(decimal percentage, TAmountMeasureContained containerCapacity);
    }

    /// <summary>
    /// Represents the shape of a container, especially as projected onto a 2D surface for measurement
    /// </summary>
    public class LiquidContainerShape : IContainerShape<LiquidAmount>
    {
        /// <summary>
        /// This describes the width of the bottle, from bottom up, as a percentage of its height
        /// </summary>
        public int[] ProfileWidths { get; set; }


        public LiquidAmount GetAmountContainedFromPercentageOfHeight(decimal percentage, LiquidAmount containerCapacity)
        {
            if (percentage < 0 || percentage > 1.0M)
            {
                throw new ArgumentException("Percentage must be between 0 and 1");
            }

            //get the nearest width to us
            var tenthUp = Math.Round(percentage * 10, MidpointRounding.AwayFromZero);

            //get the total amount of the liquid we're talking about
            var totalAmount = ProfileWidths.Sum(s => s);

            //get how many are below
            var totalHeld = 0.0M;
            for (var itemAt = 0; itemAt <= tenthUp; itemAt++)
            {
                totalHeld += ProfileWidths[itemAt];
            }

            //now we can calculate
            return containerCapacity.Multiply(totalHeld/totalAmount);
        }

        #region defaults
        public static LiquidContainerShape DefaultBottle
        {
            get
            {
                return new LiquidContainerShape
                {
                    ProfileWidths = new[] { 30, 33, 33, 33, 33, 33, 30, 17, 9, 9 }
                };
            }
        }
        #endregion
    }
}
