using System;

namespace Mise.Core.Client.ValueItems.Inventory
{
    public class LiquidContainer : IDbValueItem<Core.ValueItems.Inventory.LiquidContainer>
    {
        public LiquidContainer()
        {
            AmountContained = new LiquidAmount();
            WeightEmpty = new Weight();
            WeightFull = new Weight();
            Shape = new LiquidContainerShape();
        }

        public LiquidContainer(Core.ValueItems.Inventory.LiquidContainer source) : this()
        {
            if (source != null)
            {
                AmountContained = new LiquidAmount(source.AmountContained);
                ContainerDisplayName = source.DisplayName;
                ContainerExclusiveToBusinessId = source.BusinessId;
                Shape = new LiquidContainerShape(source.Shape);
                WeightFull = new Weight(source.WeightFull);
                WeightEmpty = new Weight(source.WeightEmpty);
            }
        }

        public Guid? ContainerExclusiveToBusinessId { get; set; }

        public string ContainerDisplayName { get; set; }

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
        public LiquidContainerShape Shape { get; set; }

        public Core.ValueItems.Inventory.LiquidContainer ToValueItem()
        {
            return new Core.ValueItems.Inventory.LiquidContainer
            {
                AmountContained = AmountContained.ToValueItem(),
                DisplayName = ContainerDisplayName,
                BusinessId = ContainerExclusiveToBusinessId,
                Shape = Shape,
                WeightEmpty = WeightEmpty,
                WeightFull = WeightFull
            };
        }
    }
}
