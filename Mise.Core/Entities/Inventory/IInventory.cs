using System;
using System.Collections.Generic;
using Mise.Core.Entities.Base;
using Mise.Core.Entities.Inventory.Events;
namespace Mise.Core.Entities.Inventory
{
    /// <summary>
    /// Represents an invetory the restaurant holds, or measured
    /// </summary>
    public interface IInventory : IRestaurantEntityBase, IEventStoreEntityBase<IInventoryEvent>, ICloneableEntity
    {
        /// <summary>
        /// Gets all beverage line items in this inventory
        /// </summary>
        /// <returns></returns>
        IEnumerable<IInventoryBeverageLineItem> GetBeverageLineItems();

        /// <summary>
        /// Gets the different sections this item holds
        /// </summary>
        /// <returns></returns>
        IEnumerable<IInventorySection> GetSections(); 
            
        /// <summary>
        /// The last time the inventory was fully taken physically
        /// </summary>
        DateTimeOffset? DateCompleted { get; }

        Guid CreatedByEmployeeID { get;  }

        /// <summary>
        /// If true, this is the running inventory for the restaurant
        /// </summary>
        bool IsCurrent { get; }
    }
}
