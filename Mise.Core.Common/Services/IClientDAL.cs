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
	    /// Remove all items and events stored before the time given
	    /// </summary>
	    /// <param name="minDate"></param>
	    /// <param name="maxNumberEntites">If over this number left by the min date, delete older till we'll below</param>
	    /// <param name="maxNumberEvents"></param>
	    /// <returns></returns>
	    Task CleanItemsBefore(DateTimeOffset minDate, int maxNumberEntites = int.MaxValue, int maxNumberEvents = int.MaxValue);

        /// <summary>
        /// Get any events which are waiting to be resent
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<EventDataTransportObject>> GetUnsentEvents();

	    Task AddEventsThatFailedToSend(IEnumerable<IEntityEventBase> events);

	    Task MarkEventsAsSent(IEnumerable<IEntityEventBase> events);
	}
}

