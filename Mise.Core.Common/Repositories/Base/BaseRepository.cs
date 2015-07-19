using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Entities.Base;
using Mise.Core.ValueItems;

namespace Mise.Core.Common.Repositories.Base
{
    /// <summary>
    /// Basic repository that caches items for fast retrieval
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseRepository<T> where T : class, IEntityBase
    {
        /// <summary>
        /// Commits any transactions for the specified entity to permanence
        /// </summary>
        /// <param name="entityID"></param>
        public abstract Task<CommitResult> Commit(Guid entityID);

        public virtual IEnumerable<T> GetAll()
        {
            //TODO check if we need more in the cache
             return Cache.GetAll();
        }

        /// <summary>
        /// Clears all data from the repository
        /// </summary>
        /// <returns></returns>
        public virtual Task Clear()
        {
            Cache = new RepositoryCache<T>();
            return Task.FromResult(true);
        }
        /// <summary>
        /// All items we currently have in the cache
        /// </summary>
        protected RepositoryCache<T> Cache;

        protected BaseRepository()
        {
			Cache = new RepositoryCache<T>();
        }

		public virtual T GetByID(Guid id)
        {
            //TODO - if it's not in cache, check the DB
            return Cache.GetByID(id);
        }

        public virtual bool IsFullyCommitted {
			get;
			set;
        }

        /// <summary>
        /// Gets the last event ID that is currently stored in this repository
        /// </summary>
        /// <returns></returns>
        public EventID GetLastEventID()
        {
            var latest =  Cache.GetAll().OrderByDescending(e => e.LastUpdatedDate)
                .ThenByDescending(e => e.Revision == null ? -1 : e.Revision.OrderingID)
                .FirstOrDefault();
            if (latest != null)
            {
                return latest.Revision;
            }
            return null;
        }
    }
}
