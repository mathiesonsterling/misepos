using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Mise.Core.Common.Entities;
using Mise.Core.Common.Events.DTOs;
using Mise.Core.Entities;
using Mise.Core.Entities.Base;
using Mise.Core.Services;
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

        readonly EventDataTransportObjectFactory _dtoFactory;
	    private readonly ILogger _logger;
	    private readonly IJSONSerializer _serializer;
		public MemoryClientDAL(ILogger logger, IJSONSerializer jsonSerializer)
		{
			_serializer = jsonSerializer;
		    _logger = logger;
            _dtoFactory = new EventDataTransportObjectFactory(_serializer);
			_entityDB = new Dictionary<Guid, DatabaseEntityItem>();
			_eventDB = new Dictionary<Guid, DatabaseEventItem>();
		}


	    #region IClientDAL implementation

        public Task CleanItemsBefore(DateTimeOffset minDate, int maxNumberEntites = Int32.MaxValue, int maxNumberEvents = Int32.MaxValue)
        {
            return Task.Run(() =>
            {
                var eventsToRemove = _eventDB.Values.Where(ev => ev.CreatedDate < minDate).ToList();

                if (eventsToRemove.Count() > maxNumberEntites)
                {
                    var diff = maxNumberEntites - eventsToRemove.Count();
                    var moreItems =
                        _eventDB.Values.Where(ev => ev.CreatedDate >= minDate).OrderBy(ev => ev.CreatedDate).Take(diff);
                    eventsToRemove.AddRange(moreItems);
                }
            });
        }

	    public Task<IEnumerable<EventDataTransportObject>> GetUnsentEvents()
	    {
	        var unsents = _eventDB.Values.Where(dbo => dbo.HasBeenSent == false).Select(dbo => dbo as EventDataTransportObject);
	        return Task.FromResult(unsents);
	    }

	    public Task AddEventsThatFailedToSend(IEnumerable<IEntityEventBase> events)
	    {
            //store these as our eventdatatransportobject
            var dtos = events.Select(ev => _dtoFactory.ToDataTransportObject(ev))
                .Select(dto => new DatabaseEventItem(dto, false){TimesAttemptedToSend=1});


            foreach (var e in dtos)
            {
                if (_eventDB.ContainsKey(e.ID))
                {
                    //overwrite the item, but make sure the times to send stays up to date
                    var item = _eventDB[e.ID];
                    e.TimesAttemptedToSend = item.TimesAttemptedToSend++;
                    _eventDB[e.ID] = e;
                }
                else
                {
                    _eventDB.Add(e.ID, e);
                }
            }

	        return Task.FromResult(true);
	    }

	    public Task MarkEventsAsSent(IEnumerable<IEntityEventBase> events)
	    {
	        foreach (var dbo in from ev in events where _eventDB.ContainsKey(ev.ID) select _eventDB[ev.ID])
	        {
	            dbo.HasBeenSent = true;
	        }

	        return Task.FromResult(true);
	    }


	    private IEnumerable<T> GetEntities<T>() where T : class, IEntityBase
		{
			return _entityDB.Values.Where(dv => dv.Type == typeof(T))
				.Select(dv => _serializer.Deserialize<T>(dv.JSON));
		}

		public Task<IEnumerable<T>> GetEntitiesAsync<T>() where T : class, IEntityBase, new()
		{
		    return Task.Run(() => GetEntities<T>());
		}

		private T GetEntityByID<T>(Guid id) where T : class, IEntityBase
		{
			if (_entityDB.ContainsKey(id)) {
				var ent = _entityDB[id];
				return _serializer.Deserialize<T>(ent.JSON);
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
			return true;
		}

		public Task<bool> StoreEventsAsync(IEnumerable<IEntityEventBase> events)
		{
		    return Task.Run(() => StoreEvents(events));
		}


		private bool UpsertEntities<T>(IEnumerable<T> entities) where T :class, IEntityBase, new()
		{
			var dbEnts = entities.Select(ent => new DatabaseEntityItem {
				ID = ent.ID,
				Status = ItemCacheStatus.ClientDB,
				JSON = _serializer.Serialize(ent),
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

		public Task<bool> UpsertEntitiesAsync<T>(IEnumerable<T> entities) where T:class, IEntityBase, new()
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
			return ents.Select(js => _serializer.Deserialize<T>(js));
		}


		public T GetByID<T>(Guid id) where T:class, IEntityBase
		{
			if (_entityDB.ContainsKey(id)) {
				var json = _entityDB[id].JSON;
				return _serializer.Deserialize<T>(json);
			}
			return null;
		}

	    public Task ResetDB()
	    {
	        _entityDB.Clear();
            _eventDB.Clear();

	        return Task.FromResult(true);
	    }

		public Task ReAddFailedSendEvents (IEnumerable<EventDataTransportObject> stillFailing)
		{
			return Task.FromResult (false);
		}
		#endregion
	}
}

