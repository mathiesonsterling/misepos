using System;
using System.Globalization;
using System.Linq;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Entities.Inventory;
using Mise.Core.ValueItems;
using Mise.Core.ValueItems.Inventory;

namespace Mise.Core.Client.Entities.Inventory
{
    public class LiquidContainer : BaseDbEntity<ILiquidContainerEntity, LiquidContainerEntity>
    {
        public LiquidContainer()
        {
            WidthsAsPercentageOfHeight = string.Empty;
        }

        public LiquidContainer(Core.ValueItems.Inventory.LiquidContainer source)
        {
            EntityId = Guid.NewGuid();
            Id = EntityId.ToString();
            BusinessId = source.BusinessId;
            DisplayName = source.DisplayName;
            AmountContainedInMl = source.AmountContained.Milliliters;
            AmountContainedSpecificGravity = source.AmountContained.SpecificGravity;
            WeightEmptyGrams = source.WeightEmpty?.Grams;
            WeightFullGrams = source.WeightFull?.Grams;
            ShapeName = source.Shape?.Name;

            if (source.Shape != null)
            {
                WidthsAsPercentageOfHeight = string.Join(",",
                    source.Shape.WidthsAsPercentageOfHeight.Select(s => s.ToString(CultureInfo.InvariantCulture)));
            }
        }

        public LiquidContainer(ILiquidContainerEntity source) : base(source)
        {
            BusinessId = source.BusinessId;
            DisplayName = source.DisplayName;
            AmountContainedInMl = source.AmountContained.Milliliters;
            AmountContainedSpecificGravity = source.AmountContained.SpecificGravity;
            WeightEmptyGrams = source.WeightEmpty?.Grams;
            WeightFullGrams = source.WeightFull?.Grams;
            ShapeName = source.Shape.Name;

            WidthsAsPercentageOfHeight = string.Join(",",
                source.Shape.WidthsAsPercentageOfHeight.Select(s => s.ToString(CultureInfo.InvariantCulture)));
        }

        public Guid? BusinessId { get; set; }
        public string DisplayName { get; set; }
        public decimal AmountContainedInMl { get; set; }
        public decimal? AmountContainedSpecificGravity { get; set; }
        public decimal? WeightEmptyGrams { get; set; }
        public decimal? WeightFullGrams { get; set; }

        public string ShapeName { get; set; }
        public string WidthsAsPercentageOfHeight { get; set; }

        protected override LiquidContainerEntity CreateConcreteSubclass()
        {
            var widths =
                WidthsAsPercentageOfHeight.Split(',')
                    .Select(s => s.Trim())
                    .Select(double.Parse)
                    .ToList();

            return new LiquidContainerEntity
            {
                Shape = new LiquidContainerShape
                {
                    Name = ShapeName,
                    WidthsAsPercentageOfHeight = widths
                },
                AmountContained = new LiquidAmount(AmountContainedInMl, AmountContainedSpecificGravity),
                BusinessId = BusinessId,
                DisplayName = DisplayName,
                WeightFull = WeightFullGrams.HasValue ? new Weight(WeightFullGrams.Value) : null,
                WeightEmpty = WeightEmptyGrams.HasValue ? new Weight(WeightEmptyGrams.Value) : null
            };
        }
    }
}
