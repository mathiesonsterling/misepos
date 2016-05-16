using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public Task Produce(BlockingCollection<RestaurantEntityDataTransportObject> queue)
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
                    try
                    {
                        var restDTO = dto.ToRestaurantDTO();
                        queue.Add(restDTO);
                    }
                    catch (Exception e)
                    {
                        _errors.Add(new Tuple<AzureEntityStorage, Exception>(dto, e));
                    }
                }
            }

            return Task.FromResult(_errors.Any());
        }
    }
}
