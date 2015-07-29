using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;
using Mise.Azure.Annotations;
using Mise.Core.Common.Entities.DTOs;
using Mise.Core.Common.Events.DTOs;
using Mise.Core.Entities.Base;
using Mise.Core.Server.Services.DAL;
using Mise.Core.Services;
using Mise.Core.Services.UtilityServices;
using Mise.Core.ValueItems;
using LogLevel = Mise.Core.Services.LogLevel;

namespace Mise.Azure
{
    /// <summary>
    /// Store any incoming events into Azure, for later data mining
    /// </summary>
    public class AzureStorageDAL : IEventStorageDAL
    {
        /// <summary>
        /// Lightweight class for storage of events in a table
        /// Note our JSON cannot be more than 64K!
        /// </summary>
        private class AzureJSONEventStorageItem : TableEntity
        {
            public AzureJSONEventStorageItem(Guid? restaurantID, Guid id, string json, Type type, ItemCacheStatus status)
            {
                PartitionKey = GetPartitionKey(restaurantID);
                RowKey = id.ToString();
                ETag = "*";
                ID = id;
                JSON = json;
                Type = type;
                RestaurantID = restaurantID;
                CacheStatus = status;
            }

            public AzureJSONEventStorageItem()
            {

            }

            // ReSharper disable MemberCanBePrivate.Local
            public Guid ID { get; set; }

            public string JSON { [UsedImplicitly] get; set; }

            public Type Type { [UsedImplicitly] get; set; }

            public Guid? RestaurantID { [UsedImplicitly] get; set; }
            public ItemCacheStatus CacheStatus { [UsedImplicitly] get; set; }
            // ReSharper restore MemberCanBePrivate.Local
        }

        private readonly IJSONSerializer _serializer;
        private readonly ILogger _logger;

        private readonly CloudTable _eventsTable;
        private readonly CloudBlobContainer _entitiesContainer;

        private readonly EntityDataTransportObjectFactory _entityDTOFactory;
        private readonly EventDataTransportObjectFactory _eventDTOFactory;
        public AzureStorageDAL(IJSONSerializer serializer, string connString, ILogger logger)
        {
            _serializer = serializer;
            _logger = logger;
            _entityDTOFactory = new EntityDataTransportObjectFactory(_serializer);
            _eventDTOFactory = new EventDataTransportObjectFactory(_serializer);

            var cloudStorageAccount = CloudStorageAccount.Parse(connString);
            var cloudTableClient = cloudStorageAccount.CreateCloudTableClient();
            var cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();

            _eventsTable = cloudTableClient.GetTableReference("events");
            _eventsTable.CreateIfNotExists();

            _entitiesContainer = cloudBlobClient.GetContainerReference("entities");
            _entitiesContainer.CreateIfNotExists();
        }

        #region Events

        public Task<bool> StoreEventsAsync(IEnumerable<IEntityEventBase> events)
        {
            var batchOperation = new TableBatchOperation();

            var ourDTOs = events.Select(ev => new{DTO = _eventDTOFactory.ToDataTransportObject(ev), Type = ev.GetType()});
            foreach (var e in ourDTOs)
            {
                //make the dto for it
                var item = new AzureJSONEventStorageItem(e.DTO.RestaurantID, e.DTO.ID, _serializer.Serialize(e.DTO), e.Type,
                    ItemCacheStatus.Clean);
                batchOperation.Insert(item);
            }

            return _eventsTable.ExecuteBatchAsync(batchOperation)
                .ContinueWith(task =>
                {
                    if (task.IsFaulted)
                    {
                        _logger.Error("Error storing events in database!");
                        //TODO send this to alternate storage
                        if (task.Exception != null)
                        {
                            //log our exceptions
                            foreach (var ex in task.Exception.Flatten().InnerExceptions)
                            {
                                _logger.HandleException(ex);
                            }
                        }
                        return false;
                    }
                    return true;
                });
        }

        public Task<IEnumerable<IEntityEventBase>> GetEventsSince(Guid? restaurantID, DateTimeOffset date)
        {
            return Task.Run(() =>
            {
                var partKey = GetPartitionKey(restaurantID);
                var query =
                    new TableQuery<AzureJSONEventStorageItem>().Where(TableQuery.GenerateFilterCondition("PartitionKey",
                        QueryComparisons.Equal, partKey));
                var events = _eventsTable.ExecuteQuery(query);

                //deserial to our dtos
                //TODO figure out how to do deserial async here
                var dtos = events.Select(ev => _serializer.Deserialize<EventDataTransportObject>(ev.JSON));

                return dtos.Select(dto => _eventDTOFactory.ToEvent(dto));
            });
        }


        public IEnumerable<IEntityBase> UpsertEntities(IEnumerable<IRestaurantEntityBase> entities)
        {
            return (from ent in entities
                let partitionKey = GetPartitionKey(ent.RestaurantID)
                let rowKey = ent.ID.ToString()
                let json = _serializer.Serialize(ent)
                let storageItem = _entityDTOFactory.ToDataTransportObject(ent)
                let storageJSON = _serializer.Serialize(storageItem)
                let blob = UploadDocument(partitionKey, rowKey, storageJSON)
                select ent).ToList();
        }

        public async Task<bool> UpsertEntitiesAsync(IEnumerable<IEntityBase> entities)
        {
			var asRestEnt = entities.Cast<IRestaurantEntityBase> ();
            var list = new List<IEntityBase>();
            foreach (var ent in asRestEnt)
            {
                var partitionKey = GetPartitionKey(ent.RestaurantID);
                var rowKey = ent.ID.ToString();
                var storageItem = _entityDTOFactory.ToDataTransportObject(ent);
                var storageJSON = _serializer.Serialize(storageItem);
                await UploadDocumentAsync(partitionKey, rowKey, storageJSON);
                list.Add(ent);
            }
            return true;
        }


        T DeserToType<T>(string json) where T : class, IEntityBase
        {
            var deser = _serializer.Deserialize<RestaurantEntityDataTransportObject>(json);
            if (deser != null)
            {
                var thisType = deser.SourceType;
                var reqType = typeof (T);
                if (thisType == reqType || reqType.IsAssignableFrom(thisType))
                {
                    var realObj = _serializer.Deserialize(deser.JSON, thisType) as T;
                    return realObj;
                }
            }

            return null;
        }

		public IEnumerable<T> GetEntities<T>(Guid? restaurantID) where T : class, IEntityBase, new()
        {
            if (restaurantID.HasValue == false)
            {
                foreach (var ent in GetEntities<T>())
                {
                    yield return ent;
                }
            }

            var partitionKey = GetPartitionKey(restaurantID);
            var listItems = _entitiesContainer.GetDirectoryReference(partitionKey).ListBlobs();

            foreach (var item in listItems.OfType<CloudBlockBlob>())
            {
                var doc = DownloadDocument(item.Name);
                var obj = DeserToType<T>(doc);
                if (obj != null)
                {
                    yield return obj;
                }
            }
        }

		public async Task<IEnumerable<T>> GetEntitiesAsync<T>(Guid? restaurantID) where T : class, IEntityBase
        {
            if (restaurantID.HasValue == false)
            {
                return GetEntities<T>();
            }

            var partitionKey = GetPartitionKey(restaurantID);
            var listItems = _entitiesContainer.GetDirectoryReference(partitionKey).ListBlobs();

            var list = new List<T>();
            foreach (var item in listItems.OfType<CloudBlockBlob>())
            {
                var doc = await DownloadDocumentAsync(item.Name);
                var obj = DeserToType<T>(doc);
                if (obj != null)
                {
                    list.Add(obj);
                }
            }

            return list;
        }


        static IEnumerable<CloudBlockBlob> GetBlobsRecursive(CloudBlobDirectory dir)
        {
            var blobs = dir.ListBlobs();
            var res = new List<CloudBlockBlob>();
            foreach (var blob in blobs)
            {
                var item = blob as CloudBlockBlob;
                if (item != null)
                {
                    res.Add(item);
                }

                var directory = blob as CloudBlobDirectory;
                if (directory != null)
                {
                    res.AddRange(GetBlobsRecursive(directory));
                }
            }

            return res;
        } 

		public IEnumerable<T> GetEntities<T>() where T : class, IEntityBase
        {
            var blocks = new List<CloudBlockBlob>();
            try
            {
                var blobs = _entitiesContainer.ListBlobs();

                foreach (var blob in blobs)
                {
                    var item = blob as CloudBlockBlob;
                    if (item != null)
                    {
                        blocks.Add(item);
                    }
                    var dir = blob as CloudBlobDirectory;
                    if (dir != null)
                    {
                        blocks.AddRange(GetBlobsRecursive(dir));
                    }
                }
            }
            catch (Exception e)
            {
                _logger.HandleException(e);
            }

            foreach (var blob in blocks)
                {
                    var doc = DownloadDocument(blob.Name);
                    var deser = DeserToType<T>(doc);
                    if (deser != null)
                    {
                        yield return deser;
                    }
                }

        }

		public async Task<IEnumerable<T>> GetEntitiesAsync<T>() where T : class, IEntityBase
        {
            var blobs = _entitiesContainer.ListBlobs();

            var blocks = new List<CloudBlockBlob>();
            foreach (var blob in blobs)
            {
                var item = blob as CloudBlockBlob;
                if (item != null)
                {
                    blocks.Add(item);
                }
                var dir = blob as CloudBlobDirectory;
                if (dir != null)
                {
                    blocks.AddRange(GetBlobsRecursive(dir));
                }
            }

            var items = new List<T>();
            foreach (var blob in blocks)
            {
                var doc = await DownloadDocumentAsync(blob.Name);
                var deser = DeserToType<T>(doc);
                if (deser != null)
                {
                   items.Add(deser);
                }
            }

            return items;
        }

		public T GetEntityByID<T>(Guid id) where T : class, IEntityBase
        {
            return GetEntities<T>().FirstOrDefault(e => e.ID == id);
        }

		public async Task<T> GetEntityByIDAsync<T>(Guid id) where T : class, IEntityBase
        {
            var res = await Task.Factory.StartNew(() => GetEntityByID<T>(id));
            return res;
        }

		public T GetEntityByID<T>(Guid? restaurantID, Guid id) where T : class, IEntityBase
        {
            var partitionKey = GetPartitionKey(restaurantID);
            var filename = string.Format(@"{0}\{1}.json", partitionKey, id);
            try
            {
                var res = DownloadDocument(filename);
                return DeserToType<T>(res);
            }
            catch (Exception e)
            {
                _logger.HandleException(e);
                return null;
            }
        }

		public async Task<T> GetEntityByIDAsync<T>(Guid? restaurantID, Guid id) where T : class, IEntityBase
        {
            var partitionKey = GetPartitionKey(restaurantID);
            var filename = string.Format(@"{0}\{1}.json", partitionKey, id);
            try
            {
                var res = await DownloadDocumentAsync(filename);
                return DeserToType<T>(res);
            } 
            catch (Exception e)
            {
                _logger.HandleException(e);
                return null;
            }
        }

		public bool Delete<T>(T entity) where T : class, IEntityBase, IRestaurantEntityBase
        {
            if (entity == null)
            {
                return false;
            }

            var partitionKey = GetPartitionKey(entity.RestaurantID);
            var filename = string.Format(@"{0}\{1}.json", partitionKey, entity.ID);
            var blob = _entitiesContainer.GetBlockBlobReference(filename);
            if (blob.Exists() == false)
            {
                return false;
            }
            blob.Delete();

            return true;
        }

        #endregion

        /// <summary>
        /// Clears the entire database, use with caution
        /// </summary>
        public void Clear()
        {
            foreach (var cont in _entitiesContainer.ListBlobs().OfType<CloudBlockBlob>())
            {
                cont.DeleteIfExists();
            }

            _eventsTable.DeleteIfExists();

            //_entitiesContainer.Delete();
           // _eventsTable.Delete();
            Thread.Sleep(10000);
        }

        private string DownloadDocument(string blobName)
        {
            _logger.Log("action=DownloadDocument, status = started, blobName=" + blobName, LogLevel.Info);
            var blockBlob = _entitiesContainer.GetBlockBlobReference(blobName);

            using (var memory = new MemoryStream())
            using (var reader = new StreamReader(memory))
            {
                blockBlob.DownloadToStream(memory);
                memory.Seek(0, SeekOrigin.Begin);

                var s = reader.ReadToEnd();
                _logger.Log("action=DownloadDocument, status = completed, blobName=" + blobName, LogLevel.Info);
                return s;
            }
        }

        private async Task<string> DownloadDocumentAsync(string blobName)
        {
            _logger.Log("action=DownloadDocumentAsync, status = started, blobName=" + blobName, LogLevel.Info);
            var blockBlob = await _entitiesContainer.GetBlobReferenceFromServerAsync(blobName);

            using (var memory = new MemoryStream())
            using (var reader = new StreamReader(memory))
            {
                blockBlob.DownloadToStream(memory);
                memory.Seek(0, SeekOrigin.Begin);

                var s = reader.ReadToEnd();
                _logger.Log("action=DownloadDocument, status = completed, blobName=" + blobName, LogLevel.Info);
                return s;
            }
        }

        private CloudBlockBlob UploadDocument(string partitionKey, string rowKey, string document)
        {
            _logger.Log("action=UpdloadDocument, status = started, restaurantID=" + partitionKey + ", id=" + rowKey, LogLevel.Info);
            var filename = string.Format(@"{0}\{1}.json", partitionKey, rowKey);
            var blockBlob = _entitiesContainer.GetBlockBlobReference(filename);

            using (var memory = new MemoryStream())
            using (var writer = new StreamWriter(memory))
            {
                writer.Write(document);
                writer.Flush();
                memory.Seek(0, SeekOrigin.Begin);

                blockBlob.UploadFromStream(memory);
            }

            blockBlob.Properties.ContentType = "application/json";
            blockBlob.SetProperties();

            _logger.Log("action=UpdloadDocument, status = finished, restaurantID=" + partitionKey + ", id=" + rowKey, LogLevel.Info);
            return blockBlob;
        }

        private async Task<CloudBlockBlob> UploadDocumentAsync(string partitionKey, string rowKey, string document)
        {
            _logger.Log("action=UpdloadDocumentAsync, status = started, restaurantID=" + partitionKey + ", id=" + rowKey, LogLevel.Info);
            var filename = string.Format(@"{0}\{1}.json", partitionKey, rowKey);
            var blockBlob = _entitiesContainer.GetBlockBlobReference(filename);

            using (var memory = new MemoryStream())
            using (var writer = new StreamWriter(memory))
            {
                writer.Write(document);
                writer.Flush();
                memory.Seek(0, SeekOrigin.Begin);

                await blockBlob.UploadFromStreamAsync(memory);
            }

            blockBlob.Properties.ContentType = "application/json";
            await blockBlob.SetPropertiesAsync();

            _logger.Log("action=UpdloadDocumentAsync, status = finished, restaurantID=" + partitionKey + ", id=" + rowKey, LogLevel.Info);
            return blockBlob;
        }
        private static string GetPartitionKey(Guid? restaurantID)
        {
            return restaurantID.HasValue && restaurantID != Guid.Empty ? restaurantID.Value.ToString() : "none";
        }
    }
}
