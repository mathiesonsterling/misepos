
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Text;

using Mise.Core.Common.Services;
using Mise.Core.Entities.Check;
using Mise.Core.Entities.People;
using Mise.Core.Entities.Menu;

using Mise.Core.Entities.Base;
namespace Mise.AndroidCommon.Services
{
	/// <summary>
	/// Dummy ClientDAL.  Does nothing, but lets us avoid the serialization problems currently working on!
	/// </summary>
	public class NoSerialDAL : IClientDAL
	{
		public Mise.Core.Services.UtilityServices.IJSONSerializer Serializer {
			get;
			set;
		}

		public Mise.Core.Services.ILogger Logger {
			get;
			set;
		}

		#region IClientDAL implementation

		public Task CleanItemsBefore (DateTimeOffset minDate, int maxNumberEntites = 2147483647, int maxNumberEvents = 2147483647)
		{
			throw new NotImplementedException ();
		}

		public bool UpdateEventStatuses (IEnumerable<Mise.Core.Entities.Base.IEntityEventBase> events, Mise.Core.ValueItems.ItemCacheStatus status)
		{
			return true;
		}

		public Task<bool> UpdateEventStatusesAsync (IEnumerable<Mise.Core.Entities.Base.IEntityEventBase> events, Mise.Core.ValueItems.ItemCacheStatus status)
		{
			return Task.Factory.StartNew (() => true);
		}

		public IEnumerable<T> GetEntities<T>() where T: class, IEntityBase
		{
			return new List<T> ();
		}

		public Task<IEnumerable<T>> GetEntitiesAsync<T> () where T : class, IEntityBase
		{
			return Task.Factory.StartNew (() => GetEntities<T> ());
		}

		public IEnumerable<ICheck> GetChecks ()
		{
			return new List<ICheck> ();
		}

		public IEnumerable<IEmployee> GetEmployees ()
		{
			return new List<IEmployee> ();
		}

		public Mise.Core.Entities.IMiseTerminalDevice GetDeviceSettings ()
		{
			return null;
		}

		public Menu GetMenu ()
		{
			return null;
		}

		#endregion

		#region IDAL implementation

		public bool StoreEvents (IEnumerable<IEntityEventBase> events)
		{
			return true;
		}

		public Task<bool> StoreEventsAsync (IEnumerable<IEntityEventBase> events)
		{
			return Task.Factory.StartNew (() => true);
		}
			
		public Task<bool> UpsertEntitiesAsync (IEnumerable<IEntityBase> entities)
		{
			return Task.Factory.StartNew (() => true);
		}

		#endregion


	}
}

