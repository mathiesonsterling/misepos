using System;
using System.Collections.Generic;
using System.Linq;
using Mise.Core.Entities.Base;
using Mise.Core.ValueItems;

namespace Mise.Core.Common.Repositories.Base
{
	/// <summary>
	/// Basic storage that holds items in memory, usually for a repository
	/// </summary>
	public class RepositoryCache<T> where T:class, IEntityBase
	{

		class CacheObject
		{
			public ItemCacheStatus Status { get; set; }
			public T Object { get; set; }
		}

		readonly Dictionary<Guid, CacheObject> _cache;

		public RepositoryCache()
		{
			_cache = new Dictionary<Guid, CacheObject>();
		}


        /// <summary>
        /// If the cache holds the entity with the given ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
	    public bool ContainsItem(Guid id)
	    {
	        return _cache.ContainsKey(id);
	    }

		public IEnumerable<T> GetAll()
		{
			return _cache.Values.Select(v => v.Object);
		}

	    public T FirstOrDefault(Func<T, bool> func)
	    {
	        return _cache.Values.Select(co => co.Object).FirstOrDefault(func);
	    }

		public T GetByID(Guid id)
		{
			return _cache.ContainsKey(id) ? _cache[id].Object : null;
		}

		public void UpdateCache(T item, ItemCacheStatus status)
		{
			if (item == null)
			{
				throw new ArgumentException("Cannot update with null!");
			}

			if( _cache.ContainsKey(item.Id))
			{
				_cache [item.Id].Object = item;
				_cache[item.Id].Status = status;
			}
			else
			{
				var cacheItem = new CacheObject { Object = item, Status = status };
				_cache.Add(item.Id, cacheItem);
			}
		}


		/// <summary>
		/// When we get items from a clean source
		/// </summary>
		/// <param name="items">Items.</param>
		public void UpdateCache(IEnumerable<T> items, ItemCacheStatus status = ItemCacheStatus.Clean){
			foreach (var item in items) {
				UpdateCache (item, status);
			}
		}
	}
}

