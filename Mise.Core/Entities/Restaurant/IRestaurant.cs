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
        /// If this is true, this is just here to support a user, but is not verified as a real restaurant
        /// </summary>
        bool IsPlaceholder { get;}

        /// <summary>
        /// Get any inventory sections that have been setup
        /// </summary>
        /// <returns></returns>
        IEnumerable<IRestaurantInventorySection> GetInventorySections();

        IEnumerable<EmailAddress> GetEmailsToSendInventoryReportsTo();
    }
}
