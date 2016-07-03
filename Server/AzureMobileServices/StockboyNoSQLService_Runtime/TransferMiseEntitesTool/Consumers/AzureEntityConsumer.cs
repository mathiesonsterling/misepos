using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using TransferMiseEntitesTool.Database;

namespace TransferMiseEntitesTool.Consumers
{
    class AzureEntityConsumer
    {

        protected readonly IList<Tuple<ExistingAzureEntity, Exception>> Errors;

        protected virtual int BatchSize => 100;

        public virtual int MaxQueueSize => 2500;

        public AzureEntityConsumer()
        {
            Errors = new List<Tuple<ExistingAzureEntity, Exception>>();
        }

        public IEnumerable<Tuple<ExistingAzureEntity, Exception>> ErroredObjects => Errors;

        public virtual async Task Consume(BlockingCollection<ExistingAzureEntity> dtos)
        {
            Errors.Clear();
            var numAdded = 0;
            using (var db = new NewEntityDB())
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
                    string modernType = string.Empty;
                    switch (dto.MiseEntityType)
                    {
                        case "Mise.Core.Common.Entities.Employee":
                            modernType = "Mise.Core.Common.Entities.People.Employee";
                            break;
                        default:
                            modernType = dto.MiseEntityType;
                            break;
                    }

                    //transform
                    var newEnt = new AzureEntityStorage
                    {
                        CreatedAt = dto.CreatedAt,
                        Deleted = dto.Deleted,
                        EntityID = dto.EntityID,
                        EntityJSON = dto.EntityJSON,
                        Id = dto.Id,
                        LastUpdatedDate = dto.LastUpdatedDate,
                        MiseEntityType = modernType,
                        RestaurantID = dto.RestaurantID,
                        UpdatedAt = dto.UpdatedAt,
                    };

                    db.AzureEntityStorages.Add(newEnt);
                    numAdded++;

                    if (numAdded > 10)
                    {
                        await SaveDB(db);
                        numAdded = 0;
                    }
                }

                await SaveDB(db);
                Console.WriteLine($"Completed consumer");
            }
        }

        private static async Task SaveDB(DbContext db)
        {
            try
            {
                await db.SaveChangesAsync();
                Console.WriteLine($"Successfully wrote batch");
            }
            catch (Exception e)
            {
                var msg = e.Message +  "::" + e.StackTrace;
                await Console.Error.WriteLineAsync(msg);
                Console.WriteLine(msg);
                throw;
            }
        }
    }
}
