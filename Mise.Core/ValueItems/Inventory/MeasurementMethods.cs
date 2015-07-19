using System;

namespace Mise.Core.ValueItems.Inventory
{
    /// <summary>
    /// Describes different ways a inventory can be taken
    /// </summary>
    [Flags]
    public enum MeasurementMethods
    {
		Unmeasured,

        FromReceivingOrder,
        /// <summary>
        /// We looked at inventory, counted it, etc
        /// </summary>
        VisualEstimate,
        /// <summary>
        /// We weighed the inventory
        /// </summary>
        Weight,
        /// <summary>
        /// We have been using recipes to estimate inventory, based on the previous and sales
        /// </summary>
        POSEstimate
    }
}
