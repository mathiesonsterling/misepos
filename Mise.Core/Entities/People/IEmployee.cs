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
    public interface IEmployee : IUser, ICanOrderItems, IEventStoreEntityBase<IEmployeeEvent>
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

        /// <summary>
        /// If not null, the time this employee last logged into the inventory app on this machine
        /// </summary>
        DateTimeOffset? LastTimeLoggedIntoInventoryApp { get; }

        string LastDeviceIDLoggedInWith { get; }

        bool CurrentlyLoggedIntoInventoryApp { get; set; }
    }

}
