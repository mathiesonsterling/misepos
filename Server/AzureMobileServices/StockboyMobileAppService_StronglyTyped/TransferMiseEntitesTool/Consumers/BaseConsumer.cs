using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Common.Entities.DTOs;
using Mise.Core.Entities.Base;
using Mise.Core.Services.UtilityServices;
using Mise.Core.ValueItems;
using Mise.Database.AzureDefinitions.Context;
using Mise.Database.AzureDefinitions.ValueItems;


namespace TransferMiseEntitesTool.Consumers
{
    abstract class BaseConsumer<TEntityType> where TEntityType : class, IEntityBase
    {
        protected EntityDataTransportObjectFactory EntityFactory { get; }

        private readonly IList<Tuple<RestaurantEntityDataTransportObject, Exception>> _errors; 
        protected BaseConsumer(IJSONSerializer jsonSerializer)
        {
            EntityFactory = new EntityDataTransportObjectFactory(jsonSerializer);
            _errors = new List<Tuple<RestaurantEntityDataTransportObject, Exception>>();
        }

        public IEnumerable<Tuple<RestaurantEntityDataTransportObject, Exception>> ErroredObjects => _errors; 

        public async Task Consume(BlockingCollection<RestaurantEntityDataTransportObject> dtos)
        {
            using (var db = new StockboyMobileAppServiceContext())
            {
                db.Database.CommandTimeout = 500;
                foreach (var dto in dtos.GetConsumingEnumerable())
                {
                    try
                    {
                        var entity = EntityFactory.FromDataStorageObject<TEntityType>(dto);
                        await SaveEntity(db, entity);
                    }
                    catch (Exception e)
                    {
                        _errors.Add(new Tuple<RestaurantEntityDataTransportObject, Exception>(dto, e));
                    }
                }

                if (!_errors.Any())
                {
                    await db.SaveChangesAsync();
                }
                else
                {
                    throw new AggregateException(_errors.Select(t => t.Item2));
                }
            }
        }

      protected abstract Task SaveEntity(StockboyMobileAppServiceContext db, TEntityType entity);
    }
}
