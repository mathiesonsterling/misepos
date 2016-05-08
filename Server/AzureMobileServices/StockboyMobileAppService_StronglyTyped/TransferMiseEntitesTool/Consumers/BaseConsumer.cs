using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Common.Entities.DTOs;
using Mise.Core.Entities.Base;
using Mise.Core.Services.UtilityServices;
using TransferMiseEntitesTool.StockboyMobileAppServiceService.Models;

namespace TransferMiseEntitesTool.Consumers
{
    abstract class BaseConsumer<TEntityType> where TEntityType : class, IEntityBase
    {
        protected EntityDataTransportObjectFactory EntityFactory { get; private set; }

        protected BaseConsumer(IJSONSerializer jsonSerializer)
        {
            EntityFactory = new EntityDataTransportObjectFactory(jsonSerializer);
        }

        public async Task Consume(BlockingCollection<RestaurantEntityDataTransportObject> dtos)
        {
            using (var db = new DestinationContext())
            {
                foreach (var dto in dtos.GetConsumingEnumerable())
                {
                    var entity = EntityFactory.FromDataStorageObject<TEntityType>(dto);
                    SaveEntity(db, entity);
                }

                await db.SaveChangesAsync();
            }
        }

        protected abstract void SaveEntity(DestinationContext db, TEntityType entity);
    }
}
