using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Common.Entities.DTOs;
using Mise.Core.Common.Events.DTOs;
using Mise.Core.Common.Services.DAL;
using Mise.Core.Entities.Base;
using Mise.Core.Entities.Check.Events;
using Mise.Core.Entities.People.Events;
using Mise.Core.Services;
using Mise.Core.Services.UtilityServices;
using Mise.Core.ValueItems;
namespace Mise.Database.SQLite
{
    public class SQLiteRestaurantServerDAL : IRestaurantServerDAL
    {
        private readonly ILogger _logger;
        private readonly EventDataTransportObjectFactory _eventDTOFactory;
        private readonly EntityDataTransportObjectFactory _entityDTOFactory;
        private readonly string _fileName;

        private readonly object _lock;
        public SQLiteRestaurantServerDAL(ILogger logger, IJSONSerializer jsonSerializer, string fileName)
        {
            _logger = logger;
            _eventDTOFactory = new EventDataTransportObjectFactory(jsonSerializer);
            _entityDTOFactory = new EntityDataTransportObjectFactory(jsonSerializer);
            _fileName = fileName;
            _lock = new object();
        }

        /// <summary>
        /// If we don't have our structure up, create it!
        /// </summary>
        public void Init()
        {
            //do we exist?
            if (File.Exists(_fileName) == false)
            {
                lock (_lock)
                {
                    var createTask = CreateTablesAsync();
                    try
                    {
                        createTask.Wait();
                    }
                    catch (Exception e)
                    {
                        _logger.HandleException(e);
                        throw;
                    }
                }
            }
        }


        public Task<bool> StoreEventsAsync(IEnumerable<IEntityEventBase> events)
        {
            if (events == null)
            {
                throw new ArgumentException("Given null events list!");
            }
            return Task.Factory.StartNew(() =>
            {

                try
                {
                    var dtos = new List<EventDataTransportObject>();

                    foreach (var e in events)
                    {
                        var ce = e as ICheckEvent;
                        if (ce != null)
                        {
                            dtos.Add(_eventDTOFactory.ToDataTransportObject(ce));
                        }
                        else
                        {
                            var empE = e as IEmployeeEvent;
                            if (empE != null)
                            {
                                dtos.Add(_eventDTOFactory.ToDataTransportObject(empE));
                            }
                        }
                    }

                    //add these to the DB
                    lock (_lock)
                    {
                        var t = InsertEvents(dtos);
                        return t.Result;
                    }
                }
                catch (Exception ex)
                {
                    _logger.HandleException(ex);
                    return false;
                }
            });
        }

        public Task<bool> UpsertEntitiesAsync(IEnumerable<IEntityBase> entities)
        {
            return Task.Factory.StartNew(() =>
            {
                //JSON them, and then store them
                var dtos = entities.Select(e => _entityDTOFactory.ToDataTransportObject(e));

                lock (_lock)
                {
                    try
                    {
                        var t = UpsertEntities(dtos);
                        return t.Result;
                    }
                    catch (Exception e)
                    {
                        _logger.HandleException(e);
                        throw;
                    }
                }
            });

        }

        public Task<IEnumerable<T>> GetEntitiesAsync<T>(Guid? restaurantID) where T : class, IEntityBase
        {
            return Task.Factory.StartNew(() =>
            {
                lock (_lock)
                {
                    var dtoTask = GetEntities(restaurantID);
                    return dtoTask.Result.Select(dto => _entityDTOFactory.FromDataStorageObject<T>(dto));
                }
            });
        }

        public Task<IEnumerable<T>> GetEntitiesAsync<T>() where T : class, IEntityBase
        {
            return GetEntitiesAsync<T>(null);
        }


        public Task<T> GetEntityByIDAsync<T>(Guid id) where T : class, IEntityBase
        {
            return Task.Factory.StartNew(() =>
            {
                lock (_lock)
                {
                    var dtoTask = GetEntityByID(id);
                    var dto = dtoTask.Result;
                    return _entityDTOFactory.FromDataStorageObject<T>(dto);
                }
            });
        }

        public bool Delete<T>(T entity) where T : class, IEntityBase
        {
            throw new NotImplementedException();
        }


        public Task<bool> UpdateEventStatusesAsync(IEnumerable<IEntityEventBase> events, ItemCacheStatus status)
        {
            return Task.Factory.StartNew(() =>
            {
                lock (_lock)
                {
                    var ids = events.Select(ev => ev.ID);
                    var t = UpdateEventStatus(ids, status);
                    return t.Result;
                }
            });
        }

        private SQLiteConnection GetSQLiteConnection()
        {
            var connString = "URI=file:" + _fileName;
            var conn = new SQLiteConnection(connString);


            return conn;
        }
        private const string UPDATE_EVENT_STATUS_TEMPLATE = "UPDATE Events SET ItemCacheStatus = @ItemCacheStatus WHERE ID = @ID";

        private async Task<bool> UpdateEventStatus(IEnumerable<Guid> eventIDs, ItemCacheStatus status)
        {
            var tasks = new List<Task>();
            using (var conn = GetSQLiteConnection())
            {
                await conn.OpenAsync();
                using (var tran = conn.BeginTransaction())
                {
                    foreach (var id in eventIDs)
                    {
                        var cmd = new SQLiteCommand(UPDATE_EVENT_STATUS_TEMPLATE, conn, tran);
                        cmd.Parameters.Add(new SQLiteParameter("@ItemCacheStatus", status.ToString()));
                        cmd.Parameters.Add(new SQLiteParameter("@ID", id.ToString()));
                        tasks.Add(cmd.ExecuteNonQueryAsync());
                    }

                    Task.WaitAll(tasks.ToArray());
                    tran.Commit();
                }
                conn.Close();
            }

            return true;
        }

        private async Task<RestaurantEntityDataTransportObject> GetEntityByID(Guid id)
        {
           RestaurantEntityDataTransportObject found = null;
            using (var conn = GetSQLiteConnection())
            {
                await conn.OpenAsync();

                var getQuery = ENTITY_GET_QUERY + " WHERE ID = @ID";
                var cmd = new SQLiteCommand(getQuery, conn);
                cmd.Parameters.Add(new SQLiteParameter("@ID", id));
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    var res = TransformEntityReader(reader);
                    found = res.FirstOrDefault();
                }
                conn.Close();
            }

            return found;
        }

        private const string ENTITY_GET_QUERY =
            "SELECT ID, RestaurantID, CreatedDate, LastUpdatedDate, Revision, JSON, ItemCacheStatus, Type FROM Entities";
        private async Task<IEnumerable<RestaurantEntityDataTransportObject>> GetEntities(Guid? restaurantID)
        {
            IEnumerable<RestaurantEntityDataTransportObject> res;
            using (var conn = GetSQLiteConnection())
            {
                await conn.OpenAsync();

                var getEntitiesText = ENTITY_GET_QUERY;
                if (restaurantID.HasValue)
                {
                    getEntitiesText += " WHERE RestaurantID = " + restaurantID.Value;
                }
                
                var cmd = new SQLiteCommand(getEntitiesText, conn);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    res = TransformEntityReader(reader);
                }

                conn.Close();
                return res;
            }
        }

        private static IEnumerable<RestaurantEntityDataTransportObject> TransformEntityReader(IDataReader reader)
        {
            while (reader.Read())
            {
                yield return new RestaurantEntityDataTransportObject
                {
                    CreatedDate = DateTime.Parse(reader["CreatedDate"].ToString()),
                    LastUpdatedDate = DateTime.Parse(reader["LastUpdatedDate"].ToString()),
					Revision = new EventID(reader["Revision"].ToString()),
                    JSON = reader["JSON"].ToString(),
                    ItemCacheStatus =
                        (ItemCacheStatus)
                            Enum.Parse(typeof (ItemCacheStatus), reader["ItemCacheStatus"].ToString()),
                    SourceType = Type.GetType(reader["Type"].ToString())
                };
            }
        }


        private const string CREATE_ENTITY_TABLE_STATEMENT =
"CREATE TABLE Entities(ID varchar(100), RestaurantID varchar(100), CreatedDate DATETIME, LastUpdatedDate DATETIME, Revision bigint, JSON Text, ItemCacheStatus varchar(255), Type varchar(1000))";

        private const string CREATE_EVENT_TABLE_STATEMENT = @"CREATE TABLE Events(ID varchar(100), RestaurantID varchar(100), BaseEntityID varchar(100), CreatedDate DATETIME, LastUpdatedDate DATETIME, JSON Text, ItemCacheStatus varchar(255), EventOrderingPrefix varchar(100), OrderingID int, EventType varchar(255))";

        private async Task CreateTablesAsync()
        {
            SQLiteConnection.CreateFile(_fileName);
            var conn = GetSQLiteConnection();
            await conn.OpenAsync();

            using (var transaction = conn.BeginTransaction())
            {
                //create our table
                var entityCmd = new SQLiteCommand(CREATE_ENTITY_TABLE_STATEMENT, conn, transaction);
                await entityCmd.ExecuteNonQueryAsync();
                var eventCmd = new SQLiteCommand(CREATE_EVENT_TABLE_STATEMENT, conn, transaction);
                await eventCmd.ExecuteNonQueryAsync();

                //TODO create indexes as well on ID, maybe LastUpdated and Revision as well
                transaction.Commit();
            }
            conn.Close();
        }

        private const string EVENT_UPDATE_TEMPLATE =
            @"INSERT INTO EVENTS(ID, RestaurantID, BaseEntityID, CreatedDate, LastUpdatedDate, JSON, ItemCacheStatus, EventOrderingPrefix, OrderingID, EventType) VALUES(@ID, @RestaurantID, @BaseEntityID, @CreatedDate, @LastUpdatedDate, @JSON, @ItemCacheStatus, @EventOrderingPrefix, @OrderingID, @EventType)";
        private async Task<bool> InsertEvents(IEnumerable<EventDataTransportObject> events)
        {
            var tasks = new List<Task>();
            using (var conn = GetSQLiteConnection())
            {
                await conn.OpenAsync();
                using (var trans = conn.BeginTransaction())
                {
                    var cmds = new List<SQLiteCommand>();
                    foreach (var ev in events)
                    {
                        
                        var cmd = new SQLiteCommand(EVENT_UPDATE_TEMPLATE, conn, trans);
                        cmd.Parameters.Add(new SQLiteParameter("@ID", ev.ID));
                        cmd.Parameters.Add(new SQLiteParameter("@RestaurantID", ev.RestaurantID));
                        cmd.Parameters.Add(new SQLiteParameter("@BaseEntityID", ev.EntityID));
                        cmd.Parameters.Add(new SQLiteParameter("@CreatedDate", ev.CreatedDate));
                        cmd.Parameters.Add(new SQLiteParameter("@LastUpdatedDate", ev.LastUpdatedDate));
                        cmd.Parameters.Add(new SQLiteParameter("@JSON", ev.JSON));
                        cmd.Parameters.Add(new SQLiteParameter("@ItemCacheStatus", ev.ItemCacheStatus));
                        cmd.Parameters.Add(new SQLiteParameter("@EventOrderingPrefix",
                            ev.EventOrderingID.AppInstanceCode));
                        cmd.Parameters.Add(new SQLiteParameter("@OrderingID", ev.EventOrderingID.OrderingID));
                        cmd.Parameters.Add(new SQLiteParameter("@EventType", ev.EventType));
                        cmds.Add(cmd);
                    }

                    tasks.AddRange(cmds.Select(cmd => cmd.ExecuteNonQueryAsync()));

                    Task.WaitAll(tasks.ToArray());
                    trans.Commit();
                }
                conn.Close();
            }
            return true;
        }

        private const string UPDATE_ENTITY_TEMPLATE =
    "UPDATE Entities SET CreatedDate = @CreatedDate, LastUpdatedDate = @LastUpdatedDate, Revision = @Revision, JSON = @JSON, Type=@Type, ItemCacheStatus=@ItemCacheStatus WHERE ID = @ID AND RestaurantID = @RestaurantID";

        private const string INSERT_ENTITY_TEMPLATE =
            "INSERT INTO Entities (ID, RestaurantID, CreatedDate, LastUpdatedDate, Revision, JSON, ItemCacheStatus, Type) VALUES (@ID, @RestaurantID, @CreatedDate, @LastUpdatedDate, @Revision, @JSON, @ItemCacheStatus, @Type)";
        private async Task<bool> UpsertEntities(IEnumerable<RestaurantEntityDataTransportObject> ents)
        {
            var tasks = new List<Task>();
            using (var conn = GetSQLiteConnection())
            {
                await conn.OpenAsync();

                HashSet<Guid> ids;
                try
                {
                    ids = await GetEntityIDs(conn);
                }
                catch (Exception e)
                {
                    _logger.HandleException(e);
                    ids = new HashSet<Guid>();
                }
                using (var trans = conn.BeginTransaction())
                {
                    foreach (var ent in ents)
                    {
                        SQLiteCommand cmd;

                        //do we have it?
                        if (ids.Contains(ent.ID))
                        {
                            cmd = new SQLiteCommand(UPDATE_ENTITY_TEMPLATE, conn);
                            cmd.Parameters.Add(new SQLiteParameter("@ID", ent.ID.ToString()));
                            cmd.Parameters.Add(new SQLiteParameter("@RestaurantID", ent.RestaurantID.ToString()));
                            cmd.Parameters.Add(new SQLiteParameter("@CreatedDate", ent.CreatedDate));
                            cmd.Parameters.Add(new SQLiteParameter("@LastUpdatedDate", ent.LastUpdatedDate));
                            cmd.Parameters.Add(new SQLiteParameter("@Revision", ent.Revision));
                            cmd.Parameters.Add(new SQLiteParameter("@JSON", ent.JSON));
                            cmd.Parameters.Add(new SQLiteParameter("@ItemCacheStatus", ent.ItemCacheStatus.ToString()));
                            cmd.Parameters.Add(new SQLiteParameter("@Type", ent.SourceType.ToString()));
                        }
                        else
                        {
                            cmd = new SQLiteCommand(INSERT_ENTITY_TEMPLATE, conn);
                            cmd.Parameters.Add(new SQLiteParameter("@ID", ent.ID.ToString()));
                            cmd.Parameters.Add(new SQLiteParameter("@RestaurantID", ent.RestaurantID.ToString()));
                            cmd.Parameters.Add(new SQLiteParameter("@CreatedDate", ent.CreatedDate));
                            cmd.Parameters.Add(new SQLiteParameter("@LastUpdatedDate", ent.LastUpdatedDate));
                            cmd.Parameters.Add(new SQLiteParameter("@Revision", ent.Revision));
                            cmd.Parameters.Add(new SQLiteParameter("@JSON", ent.JSON));
                            cmd.Parameters.Add(new SQLiteParameter("@ItemCacheStatus", ent.ItemCacheStatus.ToString()));
                            cmd.Parameters.Add(new SQLiteParameter("@Type", ent.SourceType.ToString()));
                        }
                        tasks.Add(cmd.ExecuteNonQueryAsync());
                    }

                    Task.WaitAll(tasks.ToArray());
                    trans.Commit();
                }

                conn.Close();
            }

            return true;
        }

        private const string IDS_QUERY = "SELECT ID FROM ENTITIES";

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private async Task<HashSet<Guid>> GetEntityIDs(SQLiteConnection openConnection)
        {
            var cmd = new SQLiteCommand(IDS_QUERY, openConnection){CommandType =  CommandType.Text};
            var res = new HashSet<Guid>();
            
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    var source = reader["id"].ToString();
                    Guid id;
                    if (Guid.TryParse(source, out id))
                    {
                        if (res.Contains(id) == false)
                        {
                            res.Add(id);
                        }
                    }
                    else
                    {
                        _logger.Log("Invalid value of " + source + " given for entity ID");
                    }
                }
            }

            return res;
        } 
    }
}