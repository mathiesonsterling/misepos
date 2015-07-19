using Mise.Core.ValueItems;
using Mise.Core.ValueItems.Inventory;

namespace Mise.Database.StorableEntities.ValueItems
{
    /// <summary>
    /// Simple class to store the liquid container in a graph node
    /// </summary>
    public class LiquidContainerGraphNode
    {
        public LiquidContainerGraphNode()
        {
            
        }

        public LiquidContainerGraphNode(LiquidContainer source)
        {
            Millilters = source.AmountContained.Milliliters;
            SpecificGravity = source.AmountContained.SpecificGravity;
            WeightEmptyGrams = source.WeightEmpty != null ? source.WeightEmpty.Grams : (decimal?)null;
            WeightFullGrams = source.WeightFull != null ? source.WeightFull.Grams : (decimal?) null;
            DBName = "m:" + source.AmountContained.Milliliters;
            if (SpecificGravity.HasValue)
            {
                DBName += "|sg:" + SpecificGravity.Value;
            }
            if (WeightEmptyGrams.HasValue)
            {
                DBName += "|weg:" + WeightEmptyGrams.Value;
            }
            if (WeightFullGrams.HasValue)
            {
                DBName += "|wfg:" + WeightFullGrams.Value;
            }
        }

        public LiquidContainer Rehydrate()
        {
            return new LiquidContainer
            {
                AmountContained = new LiquidAmount {Milliliters = Millilters, SpecificGravity = SpecificGravity},
                WeightEmpty = WeightEmptyGrams.HasValue ? new Weight {Grams = WeightEmptyGrams.Value} : null,
                WeightFull = WeightFullGrams.HasValue? new Weight {Grams = WeightFullGrams.Value} : null
            };
        }

        /// <summary>
        /// We have trouble comparing this, so we have a unique ID for this generated off the fields
        /// </summary>
        public string DBName { get; set; }

        public decimal? WeightFullGrams { get; set; }

        public decimal? WeightEmptyGrams { get; set; }

        public decimal? SpecificGravity { get; set; }

        public decimal Millilters { get; set; }
    }
}
