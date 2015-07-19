using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Mise.Core.Common.Entities;
using Mise.Core.Entities;
using Mise.Core.Entities.Base;
using Mise.Core.Entities.Check;
using Mise.Core.Entities.Menu;
using Mise.Core.Entities.People;
using Mise.Core.Services.UtilityServices;
using Mise.Core.ValueItems;

namespace Mise.Core.Common.Services.Implementation.DAL
{
	/// <summary>
	/// Client DAL storing things in a hash table
	/// </summary>
	public class MemoryClientDAL : IClientDAL
	{
		readonly Dictionary<Guid, DatabaseEntityItem> _entityDB;

		readonly Dictionary<Guid, DatabaseEventItem> _eventDB;

		public MemoryClientDAL(IJSONSerializer jsonSerializer)
		{
			Serializer = jsonSerializer;
			_entityDB = new Dictionary<Guid, DatabaseEntityItem>();
			_eventDB = new Dictionary<Guid, DatabaseEventItem>();
		}

		public IJSONSerializer Serializer {
			get;
			set;
		}

		public Core.Services.ILogger Logger {
			get;
			set;
		}


	    #region IClientDAL implementation

        public Task CleanItemsBefore(DateTimeOffset minDate, int maxNumberEntites = Int32.MaxValue, int maxNumberEvents = Int32.MaxValue)
        {
            return Task.Run(() =>
            {
                var eventsToRemove = _eventDB.Values.Where(ev => ev.CreatedOn < minDate).ToList();

                if (eventsToRemove.Count() > maxNumberEntites)
                {
                    var diff = maxNumberEntites - eventsToRemove.Count();
                    var moreItems =
                        _eventDB.Values.Where(ev => ev.CreatedOn >= minDate).OrderBy(ev => ev.CreatedOn).Take(diff);
                    eventsToRemove.AddRange(moreItems);
                }
            });
        }

		public bool UpdateEventStatuses(IEnumerable<IEntityEventBase> events, ItemCacheStatus status)
		{
			if (events == null)
				return false;

			foreach (var e in events) {
				if (_eventDB.ContainsKey(e.ID) == false) {
					return false;
				}
				_eventDB[e.ID].Status = status;
			}

			return true;
		}

		public Task<bool> UpdateEventStatusesAsync(IEnumerable<IEntityEventBase> events, ItemCacheStatus status)
		{
			return Task.Factory.StartNew(() => UpdateEventStatuses(events, status));
		}

		public IEnumerable<T> GetEntities<T>() where T : class, IEntityBase
		{
			return _entityDB.Values.Where(dv => dv.Type == typeof(T))
				.Select(dv => Serializer.Deserialize<T>(dv.JSON));
		}

		public Task<IEnumerable<T>> GetEntitiesAsync<T>() where T : class, IEntityBase
		{
			return Task.Factory.StartNew(() => GetEntities<T>());
		}

		public T GetEntityByID<T>(Guid id) where T : class, IEntityBase
		{
			if (_entityDB.ContainsKey(id)) {
				var ent = _entityDB[id];
				return Serializer.Deserialize<T>(ent.JSON);
			}

			return null;
		}

		public Task<T> GetEntityByIDAsync<T>(Guid id) where T : class, IRestaurantEntityBase
		{
			return Task.Factory.StartNew(() => GetEntityByID<T>(id));
		}

		public IMiseTerminalDevice GetDeviceSettings()
		{
			var res = GetEntities<MiseTerminalDevice>().FirstOrDefault();
			if (res == null) {
				res = new MiseTerminalDevice {
					TopLevelCategoryID = Guid.Empty,
					CreatedDate = DateTime.Now,
					ID = Guid.NewGuid(), 
					RequireEmployeeSignIn = false,
					TableDropChecks = false,
					CreditCardReaderType = CreditCardReaderType.AudioReader,
					HasCashDrawer = true,
					RestaurantID = Guid.NewGuid()
				};
			}
			return res;
		}


		#endregion

		#region IDAL implementation

		public bool StoreEvents(IEnumerable<IEntityEventBase> events)
		{
			var newStuff = events.Select(e => new DatabaseEventItem {
				ID = e.ID,
				Status = ItemCacheStatus.TerminalDB,
				JSON = Serializer.Serialize(e),
				RevisionNumber = e.EventOrderingID.OrderingID,
				Type = e.GetType(),
                CreatedOn = e.CreatedDate
			});

			foreach (var e in newStuff) {
				if (_eventDB.ContainsKey (e.ID)) {
					_eventDB [e.ID] = e;
				} else {
					_eventDB.Add (e.ID, e);
				}
			}

			return true;
		}

		public Task<bool> StoreEventsAsync(IEnumerable<IEntityEventBase> events)
		{
			var res = StoreEvents (events);
			return Task.FromResult(res);
		}


		public bool UpsertEntities(IEnumerable<IEntityBase> entities)
		{
			var dbEnts = entities.Select(ent => new DatabaseEntityItem {
				ID = ent.ID,
				Status = ItemCacheStatus.TerminalDB,
				JSON = Serializer.Serialize(ent),
				RevisionNumber = ent.Revision,
				Type = ent.GetType()
			});
			foreach (var ent in dbEnts) {
				if (_entityDB.ContainsKey(ent.ID)) {
					_entityDB[ent.ID] = ent;
				} else {
					_entityDB.Add(ent.ID, ent);
				}
			}

			return true;
		}

		public Task<bool> UpsertEntitiesAsync(IEnumerable<IEntityBase> entities)
		{
			return Task.FromResult(UpsertEntities(entities));
		}

		/// <summary>
		/// Gets all possible entities of the type
		/// </summary>
		/// <returns></returns>
		public IEnumerable<T> GetAll<T>() where T:class, IEntityBase
		{
			var ents = _entityDB.Values.Select(db => db.JSON);
			return ents.Select(js => Serializer.Deserialize<T>(js));
		}


		public T GetByID<T>(Guid id) where T:class, IEntityBase
		{
			if (_entityDB.ContainsKey(id)) {
				var json = _entityDB[id].JSON;
				return Serializer.Deserialize<T>(json);
			}
			return null;
		}

		#endregion
	}
}

