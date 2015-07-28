using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

            public SQLiteDatabaseEventItem(EventDataTransportObject dto, bool hasBeenSent)
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
            public EventDataTransportObject ToDTO()
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
            [MaxLength(1024)]

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

            public bool HasBeenSent { get; set; }
            public int TimesAttemptedToSend { get; set; }
            // ReSharper restore MemberCanBePrivate.Local
            #endregion
        }

	    private class SQLiteDatabaseEntityItem
	    {
	        
	    }

		private SQLiteConnection _db;
		private readonly ILogger _logger;
        /// <summary>
        /// We're going to temporarily use the Memory client dal for entity storage
        /// </summary>
	    private readonly IClientDAL _tempMemDAL;
	    private object _dbLock;
		public SQLiteClietDAL (ILogger logger, IJSONSerializer serializer, ISQLite sqlLiteConnectorService)
		{
			_logger = logger;
		    _db = sqlLiteConnectorService.GetDatabase();
            _dbLock = new object();

            _tempMemDAL = new MemoryClientDAL(logger, serializer);

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
	                _db.CreateTable<SQLiteDatabaseEventItem>();
	            }
	        }
	    }

	    public Task<IEnumerable<T>> GetEntitiesAsync<T>() where T : class, IEntityBase, new()
	    {
	        return _tempMemDAL.GetEntitiesAsync<T>();
	    }

	    public Task<bool> StoreEventsAsync(IEnumerable<IEntityEventBase> events)
	    {
	        return _tempMemDAL.StoreEventsAsync(events);
	    }

	    public Task<bool> UpsertEntitiesAsync(IEnumerable<IEntityBase> entities)
	    {
	        return _tempMemDAL.UpsertEntitiesAsync(entities);
	    }

	    public Task CleanItemsBefore(DateTimeOffset minDate, int maxNumberEntites = Int32.MaxValue, int maxNumberEvents = Int32.MaxValue)
	    {
	        return _tempMemDAL.CleanItemsBefore(minDate, maxNumberEntites, maxNumberEvents);
	    }

	    public Task<IEnumerable<EventDataTransportObject>> GetUnsentEvents()
	    {
	        return Task.Run(() =>
	        {
	            lock (_dbLock)
	            {
	                var items = _db.Table<SQLiteDatabaseEventItem>()
	                    .Where(ev => ev.HasBeenSent == false)
	                    .OrderBy(ev => ev.LastUpdatedDate)
	                    .Select(s => s);

	                var dtos = items.AsEnumerable().Select(dbDTO => dbDTO.ToDTO());

	                return dtos;
	            }
	        });
	    }

	    public Task AddEventsThatFailedToSend(IEnumerable<IEntityEventBase> events)
	    {
	        return Task.Run(() =>
	        {
	            lock (_dbLock)
	            {
	                var eventsList = DowncastEvents(events);

	                foreach (var dbItem in eventsList.Select(ev => new SQLiteDatabaseEventItem(ev, true)))
	                {
	                    _db.InsertOrReplace(dbItem);
	                }
	            }
	        });
	    }

	    public Task MarkEventsAsSent(IEnumerable<IEntityEventBase> events)
	    {
	        return Task.Run(() =>
	        {
	            lock (_dbLock)
	            {
	                var ids = events.Select(ev => ev.ID);
	                var dbItems = _db.Table<SQLiteDatabaseEventItem>()
	                    .Where(dbEv => ids.Contains(dbEv.ID));

	                foreach (var dbItem in dbItems)
	                {
	                    dbItem.HasBeenSent = true;
	                }

	                _db.UpdateAll(dbItems);
	            }
	        });
	    }

	    private static IEnumerable<EventDataTransportObject> DowncastEvents(IEnumerable<IEntityEventBase> events)
	    {
            var eventsList = events.Select(ev => ev as EventDataTransportObject).ToList();
            if (eventsList.Any(ev => ev == null))
            {
                throw new ArgumentException("Items must be of type EventDataTransportObject to be stored");
            }
	        return eventsList;
	    } 
	}
}

