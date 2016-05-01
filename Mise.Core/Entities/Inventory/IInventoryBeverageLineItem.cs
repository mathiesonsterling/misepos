using System;
using System.Collections.Generic;
using Mise.Core.Entities.Base;
using Mise.Core.ValueItems;
using Mise.Core.ValueItems.Inventory;

namespace Mise.Core.Entities.Inventory
{
    /// <summary>
    /// Represents an beverage item in inventory in the restaurant
    /// </summary>
    public interface IInventoryBeverageLineItem : IRestaurantEntityBase, IBaseBeverageLineItem, ICloneableEntity
    {
        Guid? VendorBoughtFrom { get; }

        /// <summary>
        /// The amount of the item currently left.  This must be smaller than the container capacity!
        /// </summary>
        LiquidAmount CurrentAmount { get; }

        LiquidContainerShape Shape { get; }

        /// <summary>
        /// Price given by the user through the website.  RO prices should take precedence always!
        /// </summary>
        Money PricePaid { get; }

        /// <summary>
        /// Flagged enum showing how we got the current amount
        /// </summary>
        MeasurementMethods MethodsMeasuredLast { get; }

		int NumPartialBottles{get;}

        /// <summary>
        /// Gives a list of the percentage of container for EACH partial bottle
        /// </summary>
        /// <value>The partial bottle percentages.</value>
        IEnumerable<decimal> GetPartialBottlePercentages();

		int NumFullBottles{get;}

		/// <summary>
		/// If true, we've actually measured this line item, if not we haven't
		/// </summary>
		/// <value><c>true</c> if this instance has been measured; otherwise, <c>false</c>.</value>
		bool HasBeenMeasured{get;}

        /// <summary>
        /// The position this item was taken in the inventory (per section)
        /// </summary>
        int InventoryPosition { get; }
    }
}
