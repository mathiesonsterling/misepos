using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
		//Dependencies
		IJSONSerializer Serializer{ get; set; }

		ILogger Logger{ get; set; }

	    /// <summary>
	    /// Remove all items and events stored before the time given
	    /// </summary>
	    /// <param name="minDate"></param>
	    /// <param name="maxNumberEntites">If over this number left by the min date, delete older till we'll below</param>
	    /// <param name="maxNumberEvents"></param>
	    /// <returns></returns>
	    Task CleanItemsBefore(DateTimeOffset minDate, int maxNumberEntites = int.MaxValue, int maxNumberEvents = int.MaxValue);
	}
}

