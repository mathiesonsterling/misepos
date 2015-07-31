using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mise.Core.Common.Events.DTOs;
using Mise.Core.Common.Services.Implementation.DAL;
using Mise.Core.Entities.Base;
using Mise.Core.ValueItems;
using Mise.Core.Entities.Check;
using Mise.Core.Entities.People;
using Mise.Core.Entities;
using Mise.Core.Entities.Menu;
using Mise.Core.Services.UtilityServices;
using Mise.Core.Services;

namespace Mise.Core.Common.Services
{
	/// <summary>
	/// A DAL which can be used for terminal processing
	/// </summary>
	public interface IClientDAL : IDAL
	{
        /// <summary>
        /// Get any events which are waiting to be resent
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<EventDataTransportObject>> GetUnsentEvents();

	    Task AddEventsThatFailedToSend(IEnumerable<IEntityEventBase> events);

		/// <summary>
		/// When we tried to resend, but still fail
		/// </summary>
		/// <returns>The add failed send events.</returns>
		/// <param name="stillFailing">Still failing.</param>
		Task ReAddFailedSendEvents (IEnumerable<EventDataTransportObject> stillFailing);

	    Task MarkEventsAsSent(IEnumerable<IEntityEventBase> events);

        /// <summary>
        /// Clear all items in the database
        /// </summary>
        /// <returns></returns>
	    Task ResetDB();
	}
}

