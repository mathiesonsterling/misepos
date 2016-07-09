using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TransferMiseEntitesTool.Database;

namespace TransferMiseEntitesTool.Producers
{
    public abstract class BaseAzureEntitiesProducer
    {

        private List<Tuple<ExistingAzureEntity, Exception>> _errors;
        public IEnumerable<Tuple<ExistingAzureEntity, Exception>> Errors => _errors;

        protected BaseAzureEntitiesProducer()
        {
            _errors = new List<Tuple<ExistingAzureEntity, Exception>>();
        }

        public Task<bool> Produce(BlockingCollection<ExistingAzureEntity> queue)
        {
            _errors = new List<Tuple<ExistingAzureEntity, Exception>>();
            using (var db = new AzureNonTypedEntities())
            {
                db.Database.CommandTimeout = 500;
                var dtos = db.AzureEntityStorages
                    .Where(a => !a.Deleted);

                foreach (var dto in dtos)
                {

                    var restDTO = dto;
                    var couldAdd = queue.TryAdd(restDTO);
                    while (!couldAdd)
                    {
                        Thread.Sleep(1000);
                        couldAdd = queue.TryAdd(restDTO);
                    }
                }

                queue.CompleteAdding();
            }

            Console.WriteLine($"Completed adding with {_errors.Count()} errors");
            return Task.FromResult(!_errors.Any());
        }
    }
}
