using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Common.Entities.DTOs;
using Mise.Core.Common.Events.DTOs;
using Mise.Core.Common.Services.Implementation.DAL;
using Mise.Core.Entities;
using Mise.Core.Entities.Base;
using Mise.Core.Services;
using Mise.Core.Common.Services;
using Mise.Core.Services.UtilityServices;
using Mise.Core.ValueItems;
using SQLite;
namespace Mise.Inventory.Services.Implementation
{
	public class SQLiteClietDAL : IClientDAL
	{
        [Table("Events")]
	    private class SQLiteDatabaseEventItem
	    {
            public SQLiteDatabaseEventItem()
            {
                HasBeenSent = false;
                TimesAttemptedToSend = 0;
            }

            public SQLiteDatabaseEventItem(EventDataTransportObject dto, bool? hasBeenSent)
            {
                ID = dto.ID;
                CausedByID = dto.CausedByID;
                CreatedDate = dto.CreatedDate;
                DeviceID = dto.DeviceID;
                EntityID = dto.EntityID;
                EventOrderingID = dto.EventOrderingID.ToDatabaseString();
                EventType = dto.EventType.ToString();
                ItemCacheStatus = dto.ItemCacheStatus.ToString();
                JSON = dto.JSON;
                LastUpdatedDate = dto.LastUpdatedDate;
                RestaurantID = dto.RestaurantID;
                SourceType = dto.SourceType.ToString();

                HasBeenSent = hasBeenSent;
                TimesAttemptedToSend = 0;
            }

            /// <summary>
            /// Transform our DB object back to the generic DTO
            /// </summary>
            /// <returns></returns>
            public EventDataTransportObject ToDataTransportObject()
            {
                return new EventDataTransportObject
                {
                    ID = ID,
                    SourceType = Type.GetType(SourceType),
                    CausedByID = CausedByID,
                    CreatedDate = CreatedDate,
                    DeviceID = DeviceID,
                    EntityID = EntityID,
                    EventOrderingID = new EventID(EventOrderingID),
                    EventType = (MiseEventTypes)Enum.Parse(typeof(MiseEventTypes), EventType),
                    ItemCacheStatus = (ItemCacheStatus)Enum.Parse(typeof(ItemCacheStatus), ItemCacheStatus),
                    JSON = JSON,
                    LastUpdatedDate = LastUpdatedDate,
                    RestaurantID = RestaurantID,
                };
            }

            // ReSharper disable MemberCanBePrivate.Local
            #region Fields
            [PrimaryKey]
            public Guid ID { get; set; }

            /// <summary>
            /// Class this event was serailized from
            /// </summary>

            public string SourceType { get; set; }

            /// <summary>
            /// JSON representation of this event
            /// </summary>
            public string JSON { get; set; }

            public Guid EntityID { get; set; }

            [MaxLength(256)]
            public string EventType { get; set; }


            public Guid RestaurantID { get; set; }

            public string EventOrderingID { get; set; }

            public Guid CausedByID { get; set; }

            public DateTimeOffset CreatedDate { get; set; }

            public string DeviceID { get; set; }

            public DateTimeOffset LastUpdatedDate { get; set; }

            /// <summary>
            /// Status of the object across the layers
            /// </summary>
            public string ItemCacheStatus { get; set; }

            public bool? HasBeenSent { get; set; }

            public int TimesAttemptedToSend { get; set; }
            // ReSharper restore MemberCanBePrivate.Local
            #endregion
        }

        [Table("Entities")]
	    private class SQLiteDatabaseEntityItem
        {
            public SQLiteDatabaseEntityItem() { }

	        public SQLiteDatabaseEntityItem(RestaurantEntityDataTransportObject dto)
	        {
	            CreatedDate = dto.CreatedDate;
	            LastUpdatedDate = dto.LastUpdatedDate;
	            ID = dto.ID;
	            RestaurantID = dto.RestaurantID;
	            Revision = dto.Revision.ToDatabaseString();
	            ItemCacheStatus = dto.ItemCacheStatus.ToString();
	            JSON = dto.JSON;
	            SourceType = dto.SourceType.ToString();
	        }

	        public RestaurantEntityDataTransportObject ToDataTransportObject()
	        {
	            return new RestaurantEntityDataTransportObject
	            {
	                CreatedDate = CreatedDate,
	                LastUpdatedDate = LastUpdatedDate,
	                ID = ID,
	                RestaurantID = RestaurantID,
	                Revision = new EventID(Revision),
	                ItemCacheStatus = (ItemCacheStatus) Enum.Parse(typeof (ItemCacheStatus), ItemCacheStatus),
	                JSON = JSON,
	                SourceType = Type.GetType(SourceType)
	            };
	        }

            #region Fields
            [PrimaryKey]
// ReSharper disable MemberCanBePrivate.Local
            public Guid ID { get; set; }

            public DateTimeOffset CreatedDate { get; set; }
            public DateTimeOffset LastUpdatedDate { get; set; }


            public Guid? RestaurantID { get; set; }

            public string Revision { get; set; }

            /// <summary>
            /// The status of this object across all layers
            /// </summary>
            public string ItemCacheStatus { get; set; }

            /// <summary>
            /// JSON representation of the item
            /// </summary>
            public string JSON { get; set; }

            public string SourceType { get; set; }
            // ReSharper restore MemberCanBePrivate.Local
            #endregion

        }


		private readonly SQLiteConnection _db;
		private readonly ILogger _logger;

	    private readonly EntityDataTransportObjectFactory _entityDtoFactory;
		private readonly EventDataTransportObjectFactory _eventDtoFactory;
	    private object _dbLock;
		public SQLiteClietDAL (ILogger logger, IJSONSerializer serializer, ISQLite sqlLiteConnectorService)
		{
			_logger = logger;
		    _db = sqlLiteConnectorService.GetDatabase();
            _dbLock = new object();


            //make our entity DTO factory
            _entityDtoFactory = new EntityDataTransportObjectFactory(serializer);
			_eventDtoFactory = new EventDataTransportObjectFactory (serializer);

            //create our tables if they don't already exists
            CreateTablesIfNeeded();
		}

	    private void CreateTablesIfNeeded()
	    {
	        lock (_dbLock)
	        {
	            var info = _db.GetTableInfo("Events");
	            if (info.Any() == false)
	            {
                    _logger.Debug("Creating Events table");
	                _db.CreateTable<SQLiteDatabaseEventItem>();
	            }

	            var entityInfo = _db.GetTableInfo("Entities");
	            if (entityInfo.Any() == false)
	            {
                    _logger.Debug("Creating Entities table");
	                _db.CreateTable<SQLiteDatabaseEntityItem>();
	            }
	        }
	    }

	    public Task<IEnumerable<T>> GetEntitiesAsync<T>() where T : class, IEntityBase, new()
	    {
	        return Task.Run(() =>
	        {
	            var typeString = typeof (T).ToString();
	            IEnumerable<RestaurantEntityDataTransportObject> dtos;
	            lock (_dbLock)
	            {
	                _logger.Debug("Retrieving items of type " + typeString);
	                var dtosOfType = _db.Table<SQLiteDatabaseEntityItem>()
	                    .Where(dbItem => dbItem.SourceType == typeString);

	                var res = new List<RestaurantEntityDataTransportObject>();
                    //fails when we do LINQ, huh?
	                foreach (var dbItem in dtosOfType)
	                {
	                    var restDTO = dbItem.ToDataTransportObject();
	                    res.Add(restDTO);
	                }

	                dtos = res;
	            }

	            var realItems = dtos.Select(dto => _entityDtoFactory.FromDataStorageObject<T>(dto));
	            return realItems;
	        });
	    }


	    public Task<bool> StoreEventsAsync(IEnumerable<IEntityEventBase> events)
	    {
            throw new NotImplementedException("Not yet implemented to send");
	    }

	    public Task<bool> UpsertEntitiesAsync(IEnumerable<IEntityBase> entities)
	    {
	        return Task.Run(() =>
	        {
	            var dtos = entities.Select(ent => _entityDtoFactory.ToDataTransportObject(ent));
	            var storageItems = dtos.Select(dto => new SQLiteDatabaseEntityItem(dto));

	            lock (_dbLock)
	            {
	                foreach (var item in storageItems)
	                {
                        _logger.Debug("Upserting entity ID " + item.ID);
	                    _db.InsertOrReplace(item);
	                }
	            }

	            return true;
	        });
	    }


	    public Task<IEnumerable<EventDataTransportObject>> GetUnsentEvents()
	    {
	        return Task.Run(() =>
	        {
	            IEnumerable<EventDataTransportObject> dtos;
	            lock (_dbLock)
	            {
                    _logger.Debug("Retrieving unsent items from database");
	                var items = _db.Table<SQLiteDatabaseEventItem>()
	                    .Where(ev => ev.HasBeenSent == false)
	                    .OrderBy(ev => ev.LastUpdatedDate)
	                    .Select(s => s);

	                dtos = items.AsEnumerable().Select(dbObject => dbObject.ToDataTransportObject());
	            }
                return dtos;
	        });
	    }

		public Task ReAddFailedSendEvents (IEnumerable<EventDataTransportObject> stillFailing){
			return Task.Run (() => {
				var ids = stillFailing.Select (sf => sf.ID);
				lock (_dbLock) {
					var dbItems = _db.Table<SQLiteDatabaseEventItem> ()
					.Where (dbEv => ids.Contains (dbEv.ID));

					var updated = new List<SQLiteDatabaseEventItem> ();
					foreach (var dbItem in dbItems) {
						dbItem.TimesAttemptedToSend = dbItem.TimesAttemptedToSend + 1;
						dbItem.HasBeenSent = false;
						updated.Add (dbItem);
					}

					_db.UpdateAll (updated);
				}
			});
		}

	    public Task AddEventsThatFailedToSend(IEnumerable<IEntityEventBase> events)
	    {
	        return Task.Run(() =>
	        {
				var dtos = events.Select (ev => _eventDtoFactory.ToDataTransportObject (ev))
					.Select (dto => new SQLiteDatabaseEventItem(dto, false));	
	            lock (_dbLock)
	            {
	                foreach (var dbItem in dtos)
	                {
                        _logger.Debug("Inserting event ID " + dbItem.ID);
	                    _db.InsertOrReplace(dbItem);
	                }
	            }
	        });
	    }

	    public Task MarkEventsAsSent(IEnumerable<IEntityEventBase> events)
	    {
	        return Task.Run(() =>
	        {
				if(events == null){
					return;
				}
					var ids = events.Where(ev => ev != null).Select(ev => ev.ID);
	            lock (_dbLock)
	            {
	                var dbItems = _db.Table<SQLiteDatabaseEventItem>()
	                    .Where(dbEv => ids.Contains(dbEv.ID));

	                foreach (var dbItem in dbItems)
	                {
	                    dbItem.HasBeenSent = true;
	                }

                    _logger.Debug("Marking events as sent");
	                _db.UpdateAll(dbItems);
	            }
	        });
	    }

	    public Task ResetDB()
	    {
	        return Task.Run(() =>
	        {
	            lock (_db)
	            {
	                _db.DeleteAll<SQLiteDatabaseEntityItem>();
	                _db.DeleteAll<SQLiteDatabaseEventItem>();
	            }
	        });
	    }
			
	}
}

