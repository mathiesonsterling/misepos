using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TransferMiseEntitesTool.Consumers;
using TransferMiseEntitesTool.Database;
using TransferMiseEntitesTool.Producers;

namespace TransferMiseEntitesTool
{
    class EntityDBTransferer
    {
        private static async Task ProduceConsume(BaseAzureEntitiesProducer producer,
           AzureEntityConsumer consumer)
        {
            using (var queue = new BlockingCollection<ExistingAzureEntity>())
            {
                var produceTask = Task.Run(async () => await producer.Produce(queue));
                var consumeTask = Task.Run(async () => await consumer.Consume(queue));

                var allProduced = await produceTask;
                await consumeTask;
            }
        }

        public async Task TransferRecords()
        {
            await ProduceConsume(new SimpleAzureEntityProducer(), new AzureEntityConsumer());                                                                                
        }
    }
}
