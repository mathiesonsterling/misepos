using System.Collections.Generic;
using Mise.Core.Entities.Base;
using Mise.Core.Entities.People.Events;
using Mise.Core.ValueItems;
using Mise.Core.Entities.Check.Events;
using System;


namespace Mise.Core.Entities.People
{
    /// <summary>
    /// Represents an employee of a restaurant
    /// </summary>
    public interface IEmployee : ICanOrderItems, IEventStoreEntityBase<IEmployeeEvent>
    {
        /// <summary>
        /// Get the restaurants this employee works at for the type
        /// </summary>
        IEnumerable<Guid> GetRestaurantIDs(MiseAppTypes type);

        /// <summary>
        /// Get all restaurants, of any app type
        /// </summary>
        /// <returns></returns>
        IEnumerable<Guid> GetRestaurantIDs();

        IEnumerable<MiseAppTypes> GetAppsEmployeeCanUse(Guid restaurantID);

        PersonName Name { get; }

        /// <summary>
        /// Allows us to override the employee's name for display on the POS
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        /// Date the employee was hired
        /// </summary>
        DateTimeOffset HiredDate { get; }

		bool CurrentlyClockedInToPOS{ get; }

		/// <summary>
		/// Passcode this employee uses to clockin and out of the system
		/// </summary>
		/// <value>The passcode.</value>
		string Passcode{get;}

		/// <summary>
		/// Comparable hash of the employee's login password
		/// </summary>
		/// <value>The password hash.</value>
		Password Password{get;}

		/// <summary>
		/// If another service has a token associated with this employee
		/// </summary>
		/// <value>The O auth token.</value>
		OAuthToken OAuthToken{get;}

		/// <summary>
		/// Return if an employee can void items in a particular status
		/// </summary>
		/// <returns><c>true</c> if this instance can void the specified status; otherwise, <c>false</c>.</returns>
		/// <param name="status">Status.</param>
		bool CanVoid (OrderItemStatus status);

		/// <summary>
		/// Gets the remaining amount today that the employee can comp today
		/// </summary>
		/// <value>The comp budget.</value>
		Money CompBudget{get;}

        /// <summary>
        /// If true, the employee can comp amounts directly on the check, rather than through items
        /// </summary>
        bool CanCompAmount { get; }

		string PreferredColorName{get;}

		/// <summary>
		/// Where we can get the icon for the employee from
		/// </summary>
		/// <value>The employee icon URI.</value>
		Uri EmployeeIconUri{ get;}


        /// <summary>
        /// Return all the emails a person has
        /// </summary>
        /// <returns></returns>
        IEnumerable<EmailAddress> GetEmailAddresses();

        /// <summary>
        /// If not null, the time this employee last logged into the inventory app on this machine
        /// </summary>
        DateTimeOffset? LastTimeLoggedIntoInventoryApp { get; }
        string LastDeviceIDLoggedInWith { get; }

        bool CurrentlyLoggedIntoInventoryApp { get; set; }

        EmailAddress PrimaryEmail { get; set; }
    }

}
