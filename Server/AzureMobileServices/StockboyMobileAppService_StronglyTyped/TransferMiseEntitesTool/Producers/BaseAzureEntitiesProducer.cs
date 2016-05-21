using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Mise.Core.Common.Entities.DTOs;
using TransferMiseEntitesTool.Database;

namespace TransferMiseEntitesTool.Producers
{
    abstract class BaseAzureEntitiesProducer
    {
        protected abstract string EntityTypeString { get; }

        private List<Tuple<AzureEntityStorage, Exception>> _errors;
        public IEnumerable<Tuple<AzureEntityStorage, Exception>> Errors => _errors;

        protected BaseAzureEntitiesProducer()
        {
            _errors = new List<Tuple<AzureEntityStorage, Exception>>();
        }

        public bool Produce(BlockingCollection<RestaurantEntityDataTransportObject> queue)
        {
            _errors = new List<Tuple<AzureEntityStorage, Exception>>();
            using (var db = new AzureNonTypedEntities())
            {
                db.Database.CommandTimeout = 500;
                var dtos = db.AzureEntityStorages
                    .Where(a => !a.Deleted)
                    .Where(a => a.MiseEntityType == EntityTypeString);

                foreach (var dto in dtos)
                {

                    var restDTO = dto.ToRestaurantDTO();
                    var couldAdd = queue.TryAdd(restDTO);
                    while (!couldAdd)
                    {
                        Thread.Sleep(1000);
                        couldAdd = queue.TryAdd(restDTO);
                    }
                }

                queue.CompleteAdding();
            }

            Console.WriteLine($"Completed adding for {EntityTypeString} with {_errors.Count()} errors");
            return !_errors.Any();
        }
    }
}
