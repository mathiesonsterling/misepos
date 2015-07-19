using System;
using System.Collections.Generic;
using Mise.Core.Entities.Base;
using Mise.Core.Entities.Inventory;
using Mise.Core.Entities.Payments;
using Mise.Core.Entities.Restaurant.Events;
using Mise.Core.ValueItems;

namespace Mise.Core.Entities.Restaurant
{
    /// <summary>
    /// Represents a restaurant
    /// </summary>
    public interface IRestaurant : IRestaurantEntityBase, IEventStoreEntityBase<IRestaurantEvent>
    {
		/// <summary>
		/// If this restaurant has a Mise Account associated with it, it's here
		/// </summary>
		Guid? AccountID{ get;}

		/// <summary>
		/// Name of the restaurant
		/// </summary>
		/// <value>The name.</value>
        RestaurantName Name { get; }  

		StreetAddress StreetAddress{get;}

		PhoneNumber PhoneNumber{get;}

        /// <summary>
        /// Gets the discounts or gratuities this restaurant has
        /// </summary>
        /// <value>The possible discounts.</value>
        IEnumerable<IDiscount> GetPossibleDiscounts();

        /// <summary>
        /// How many registers (able to take cash payments) are up right now
        /// </summary>
        int NumberOfActiveCashRegisters { get; }

        /// <summary>
        /// How many registers (able to take credit payments) are up right now
        /// </summary>
        int NumberOfActiveCreditRegisters { get; }

        /// <summary>
        /// How many terminals, that can take orders, are currently up
        /// </summary>
        int NumberOfActiveOrderTerminals { get; }

        /// <summary>
        /// Unique, but friendly ID to be used for web lookups
        /// </summary>
        string FriendlyID { get; }

        IEnumerable<IMiseTerminalDevice> GetTerminals();
        void AddTerminal(IMiseTerminalDevice device);

        Uri RestaurantServerLocation { get; }

        /// <summary>
        /// If this is true, this is just here to support a user, but is not verified as a real restaurant
        /// </summary>
        bool IsPlaceholder { get;}

        /// <summary>
        /// Gets the current estimated inventory
        /// </summary>
        /// <returns></returns>
        Guid? CurrentInventoryID { get; }
        /// <summary>
        /// Gets the last time we measured inventory
        /// </summary>
        /// <returns></returns>
        Guid? LastMeasuredInventoryID { get; }

        /// <summary>
        /// Get any inventory sections that have been setup
        /// </summary>
        /// <returns></returns>
        IEnumerable<IRestaurantInventorySection> GetInventorySections();
    }
}
