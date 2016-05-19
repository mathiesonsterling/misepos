using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Common.Entities.DTOs;
using Mise.Core.Entities.Base;
using Mise.Core.Services.UtilityServices;
using Mise.Database.AzureDefinitions.Context;
using Mise.Database.AzureDefinitions.Entities.Categories;

namespace TransferMiseEntitesTool.Consumers
{
    /// <summary>
    /// Allows a second pass, so we can have our entity avaialble for later stuff
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TSavedEntity"></typeparam>
    abstract class BaseLineItemConsumer<TEntity, TSavedEntity> : BaseConsumer<TEntity, TSavedEntity> 
        where TEntity : class, IEntityBase
        where TSavedEntity : class
    {
        protected BaseLineItemConsumer(IJSONSerializer jsonSerializer) : base(jsonSerializer)
        {
        }

        public override async Task Consume(BlockingCollection<RestaurantEntityDataTransportObject> dtos)
        {
            var entities = new List<Tuple<TEntity, TSavedEntity>>();
            Errors.Clear();
            using (var db = new StockboyMobileAppServiceContext())
            {
                db.Database.CommandTimeout = 500;
                foreach (var dto in dtos.GetConsumingEnumerable())
                {
                    try
                    {
                        var entity = EntityFactory.FromDataStorageObject<TEntity>(dto);
                        var existing = await GetSavedEntity(db, dto.Id);
                        if (existing == null)
                        {    
                            var saved = await SaveEntity(db, entity);
                            entities.Add(new Tuple<TEntity, TSavedEntity>(entity, saved));
                        }
                        else
                        {
                            entities.Add(new Tuple<TEntity, TSavedEntity>(entity, existing));
                        }
                    }
                    catch (Exception e)
                    {
                        Errors.Add(new Tuple<RestaurantEntityDataTransportObject, Exception>(dto, e));
                    }
                }

                if (!Errors.Any())
                {
                    db.Database.Log = s =>
                    {
                        Debug.WriteLine(s);
                        Console.WriteLine(s);
                    };
                    await db.SaveChangesAsync();
                }
                else
                {
                    throw new AggregateException(Errors.Select(t => t.Item2));
                }

                //now do the line items for each
                var cats = await db.InventoryCategories.Where(ic => ic != null).ToListAsync();
                /*
                var liTasks = entities.Select(ent => SaveLineItems(db, ent.Item1, ent.Item2, cats));
                await Task.WhenAll(liTasks);*/

                foreach (var ent in entities)
                {
                    await SaveLineItems(db, ent.Item1, ent.Item2, cats);
                }
                await db.SaveChangesAsync();
            }
        }

        protected abstract Task SaveLineItems(StockboyMobileAppServiceContext db, TEntity entity, 
            TSavedEntity savedEntity, IEnumerable<InventoryCategory> cats);

    }
}
