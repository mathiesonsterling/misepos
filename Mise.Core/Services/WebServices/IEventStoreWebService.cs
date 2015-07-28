using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities.Base;
using System.Diagnostics.CodeAnalysis;

namespace Mise.Core.Services.WebServices
{
    /// <summary>
    /// Represents a web service that a repository can send events to
    /// </summary>
    /// <typeparam name="TEventType"></typeparam>
    /// <typeparam name="TEntityType"></typeparam>
    public interface IEventStoreWebService<in TEntityType, in TEventType> where TEventType : IEntityEventBase
		where TEntityType : IEventStoreEntityBase<TEventType>
    {
        /// <summary>
        /// Sends events (in event source sense) the terminal has generated to the server.  Server then 
        /// determines the overall state of the restuarant from these
        /// </summary>
        /// <returns>The events.</returns>
		/// <param name = "updatedEntity">Updated version of the entity.  Allows us to do either event sourced
		/// or basic uploads</param>
        /// <param name="events">Events.</param>
        Task<bool> SendEventsAsync(TEntityType updatedEntity, IEnumerable<TEventType> events);

    }
}
