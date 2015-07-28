using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mise.Core.Entities.Base;

namespace Mise.Core.Common.Services
{
	/// <summary>
	/// Represents a DAL which persists events and entites.
	/// The actual storage can vary depending on what service layer we're on!
	/// </summary>
	public interface IDAL
	{
        Task<IEnumerable<T>> GetEntitiesAsync<T>() where T : class, IEntityBase;

        /// <summary>
        /// Persist a collection of events in an async manner
        /// </summary>
        /// <param name="events"></param>
        /// <returns></returns>
		Task<bool> StoreEventsAsync(IEnumerable<IEntityEventBase> events);

	    Task<bool> UpsertEntitiesAsync(IEnumerable<IEntityBase> entities);

	}
}

