using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Common.Entities.DTOs;

namespace TransferMiseEntitesTool.Consumers
{
    public interface IEntityConsumer
    {
        Task Consume(BlockingCollection<RestaurantEntityDataTransportObject> dtos);
    }
}
