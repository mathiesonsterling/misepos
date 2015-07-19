using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mise.Core.Entities.Base;

namespace Mise.Core.Server.Services.DAL
{
    /// <summary>
    /// DAL to store events permanently for storage
    /// </summary>
    public interface IEventStorageDAL
    {
        /// <summary>
        /// Store the events we're given in persistant storage
        /// </summary>
        /// <param name="events"></param>
        /// <returns></returns>
        Task<bool> StoreEventsAsync(IEnumerable<IEntityEventBase> events);

        Task<IEnumerable<IEntityEventBase>> GetEventsSince(Guid? restaurantID, DateTimeOffset date);
    }
}
