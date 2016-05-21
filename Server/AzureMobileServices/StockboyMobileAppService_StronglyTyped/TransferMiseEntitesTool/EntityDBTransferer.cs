using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Common.Entities.DTOs;
using Mise.Core.Common.Services.Implementation.Serialization;
using TransferMiseEntitesTool.Consumers;
using TransferMiseEntitesTool.Producers;

namespace TransferMiseEntitesTool
{
    class EntityDBTransferer
    {
        private static async Task ProduceConsume(BaseAzureEntitiesProducer producer,
            Func<IEntityConsumer> consumerCreator, int numConsumers = 1)
        {
            var consumer = consumerCreator();
            using (var queue = new BlockingCollection<RestaurantEntityDataTransportObject>())
            {
                var allProduced = producer.Produce(queue);
                if (!allProduced)
                {
                    throw new Exception();
                }
                await consumer.Consume(queue);
            }
        }

        private static async Task ProduceConsumeOld(BaseAzureEntitiesProducer producer,
            Func<IEntityConsumer> consumerCreator, int numConsumers = 1)
        {
            var consumers = new List<IEntityConsumer>();
            for (var i = 0; i < numConsumers; i++)
            {
                var newConsum = consumerCreator();
                consumers.Add(newConsum);
            }

            using (
                var queue = new BlockingCollection<RestaurantEntityDataTransportObject>())
            {
                var allTasks = new List<Task>();
                
                //must start before await, or production will never hit!
                var prod = Task.Factory.StartNew( 
                    () => producer.Produce(queue), TaskCreationOptions.LongRunning);
                allTasks.Add(prod);
                allTasks.AddRange(
                    consumers.Select(
                        consumer => Task.Factory.StartNew(
                            async () => await consumer.Consume(queue), 
                            TaskCreationOptions.LongRunning)
                        )
                );
                foreach (var t in allTasks)
                {
                    await t;
                }
            }
        }

        public async Task TransferRecords()
        {                                                                                    
            var jsonSerializer = new JsonNetSerializer();

            //do consumption runs here
            await ProduceConsume(new RestaurantAccountProducer(), () => new RestaurantAccountConsumer(jsonSerializer));

            await ProduceConsume(new RestaurantProducer(), () => new RestaurantConsumer(jsonSerializer));

            await ProduceConsume(new EmployeeProducer(), () => new EmployeeConsumer(jsonSerializer));

            //inv categories are seeded by site, none here yet

            //vendors
            await ProduceConsume(new VendorProducer(), () => new VendorsConsumer(jsonSerializer), 3);
       
            await ProduceConsume(new ReceivingOrderProducer(), () => new ReceivingOrdersConsumer(jsonSerializer))
                .ConfigureAwait(false);

            //ents that can work in parallel
            var pos = ProduceConsume(new PurchaseOrderProducer(), () => new PurchaseOrderConsumer(jsonSerializer));
            var appInvites = ProduceConsume(new ApplicationInvitationProducer(),
                () => new ApplicationInvitationConsumer(jsonSerializer));
            var pars = ProduceConsume(new ParProducer(), () => new ParsConsumer(jsonSerializer), 2);
            var miseEmp = ProduceConsume(new MiseEmployeeAccountProducer(),
                () => new MiseEmployeeAccountsConsumer(jsonSerializer));
            await Task.WhenAll(pos, appInvites, pars, miseEmp);

            await ProduceConsume(new InventoryProducer(), 
                () => new InventoriesConsumer(jsonSerializer), 10)
                .ConfigureAwait(false);
        }
    }
}
