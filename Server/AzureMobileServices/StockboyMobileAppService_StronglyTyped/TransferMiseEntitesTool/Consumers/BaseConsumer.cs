using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Threading.Tasks;
using Mise.Core.Common.Entities.DTOs;
using Mise.Core.Entities.Base;
using Mise.Core.Services.UtilityServices;
using Mise.Database.AzureDefinitions.Context;

namespace TransferMiseEntitesTool.Consumers
{
    abstract class BaseConsumer<TEntityType, TSavedType> : IEntityConsumer
        where TEntityType : class, IEntityBase, new()
        where TSavedType : class, new()
    {
        protected EntityDataTransportObjectFactory EntityFactory { get; }
        public abstract string EntityName { get; }

        protected readonly IList<Tuple<RestaurantEntityDataTransportObject, Exception>> Errors;

        protected virtual int BatchSize => 100;

        public virtual int MaxQueueSize => 2500;

        protected BaseConsumer(IJSONSerializer jsonSerializer)
        {
            EntityFactory = new EntityDataTransportObjectFactory(jsonSerializer);
            Errors = new List<Tuple<RestaurantEntityDataTransportObject, Exception>>();
        }

        public IEnumerable<Tuple<RestaurantEntityDataTransportObject, Exception>> ErroredObjects => Errors;

        public virtual async Task Consume(BlockingCollection<RestaurantEntityDataTransportObject> dtos)
        {
            Errors.Clear();
            var numAdded = 0;
            using (var db = new StockboyMobileAppServiceContext())
            {
                db.Database.CommandTimeout = 500;
                /*
                db.Database.Log = s =>
                {
                    Debug.WriteLine(s);
                    Console.WriteLine(s);
                };*/
                foreach (var dto in dtos.GetConsumingEnumerable())
                {
                    var exists = await GetSavedEntity(db, dto.Id);
                    if (exists == null)
                    {
                        TEntityType entity;
                        try
                        {
                            entity = EntityFactory.FromDataStorageObject<TEntityType>(dto);
                        }
                        catch (Exception e)
                        {
                            var msg = e.Message;
                            throw;
                        }
                        await SaveEntity(db, entity);
                        numAdded++;
                        if (numAdded > BatchSize)
                        {
                            await SaveDB(db);
                            numAdded = 0;
                        }
                    }
                }

                if (numAdded > 0)
                {
                    await SaveDB(db);
                }
            }

            Console.WriteLine($"Completed consumer for {EntityName}");
        }

        private async Task SaveDB(DbContext db)
        {
            try
            {
                await db.SaveChangesAsync();
                Console.WriteLine($"Successfully wrote batch of {BatchSize} {EntityName}");
            }
            catch (Exception e)
            {
                var msg = e.Message +  "::" + e.StackTrace;
                await Console.Error.WriteLineAsync(msg);
                Console.WriteLine(msg);
                throw;
            }
        }
        protected abstract Task<TSavedType> SaveEntity(StockboyMobileAppServiceContext db, TEntityType entity);

        protected abstract Task<TSavedType> GetSavedEntity(StockboyMobileAppServiceContext db, Guid id);
    }
}
