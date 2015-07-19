using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities.Base;
using Mise.Core.ValueItems;

namespace Mise.Core.Repositories
{
    public interface IEventSourcedEntityRepository<out TEntity, in TEventType> : IRepository
        where TEntity:IEventStoreEntityBase<TEventType> 
        where TEventType : IEntityEventBase
    { 
        TEntity ApplyEvent(TEventType empEvent);
        TEntity ApplyEvents(IEnumerable<TEventType> events);

        IEnumerable<TEntity> GetAll();

        TEntity GetByID(Guid entityID);

        Guid GetEntityID(TEventType ev);

        Task<CommitResult> Commit(Guid entityID);

		/// <summary>
		/// Commit, but only if we can reach our destination.  Otherwise throw an exception
		/// </summary>
		/// <param name="entityID">entity to commit</param>
		Task<CommitResult> CommitOnlyImmediately(Guid entityID);

        /// <summary>
        /// Commits any dirty item in the database.  Should only really be used on the server
        /// </summary>
        /// <returns></returns>
        Task<bool> CommitAll();

        void CancelTransaction(Guid entityID);

        bool StartTransaction(Guid entityID);

        /// <summary>
        /// If we're in a transaction, and it has values not yet stored
        /// </summary>
        /// <value><c>true</c> if dirty; otherwise, <c>false</c>.</value>
        bool Dirty { get; }

    }
}
