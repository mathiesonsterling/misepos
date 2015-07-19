using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mise.Core.Common.Services;
using Mise.Core.Entities.Base;
using Mise.Core.ValueItems;

namespace Mise.Core.Common.Services.DAL
{
	public interface IRestaurantServerDAL : IDAL
	{

	    Task<IEnumerable<T>> GetEntitiesAsync<T>(Guid? restaurantID) where T : class, IEntityBase;
            
		Task<T> GetEntityByIDAsync<T> (Guid id) where T : class, IEntityBase;

		bool Delete<T>(T entity) where T : class, IEntityBase;

	    /// <summary>
	    /// For a group of events, update the status they have in persistence
	    /// </summary>
	    /// <returns><c>true</c>, if event statuses was updated, <c>false</c> otherwise.</returns>
	    /// <param name="events">Events.</param>
	    /// <param name="status">Status.</param>
	    Task<bool> UpdateEventStatusesAsync(IEnumerable<IEntityEventBase> events, ItemCacheStatus status);
	}
}

