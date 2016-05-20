using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
                db.Database.Log = s =>
                {
                    Debug.WriteLine(s);
                    Console.WriteLine(s);
                };
                foreach (var dto in dtos.GetConsumingEnumerable())
                {
                    try
                    {
                        var exists = await GetSavedEntity(db, dto.Id);
                        if (exists == null)
                        {
                            var entity = EntityFactory.FromDataStorageObject<TEntityType>(dto);
                            await SaveEntity(db, entity);
                            numAdded++;
                            if (numAdded > BatchSize)
                            {
                                await db.SaveChangesAsync();
                                numAdded = 0;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Errors.Add(new Tuple<RestaurantEntityDataTransportObject, Exception>(dto, e));
                    }
                }

                if (!Errors.Any())
                {
                    if (numAdded > 0)
                    {
                        await db.SaveChangesAsync();
                    }
                }
                else
                {
                    throw new AggregateException(Errors.Select(t => t.Item2));
                }
            }
        }

        protected abstract Task<TSavedType> SaveEntity(StockboyMobileAppServiceContext db, TEntityType entity);

        protected abstract Task<TSavedType> GetSavedEntity(StockboyMobileAppServiceContext db, Guid id);
    }
}
