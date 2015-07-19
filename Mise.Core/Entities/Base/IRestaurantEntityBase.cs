using System;

namespace Mise.Core.Entities.Base
{
    /// <summary>
    /// An Entity which belongs to a restaurant
    /// </summary>
    public interface IRestaurantEntityBase : IEntityBase
    {
        /// <summary>
        /// Majority of entities are under restaurant, so this is here
        /// </summary>
        Guid RestaurantID { get; }
    }
}
