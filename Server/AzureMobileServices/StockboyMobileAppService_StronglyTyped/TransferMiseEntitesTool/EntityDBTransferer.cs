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
            var queue = new BlockingCollection<RestaurantEntityDataTransportObject>();
            var consumers = new List<IEntityConsumer>();
            for (var i = 0; i < numConsumers; i++)
            {
                var newConsum = consumerCreator();
                consumers.Add(newConsum);
            }

            var prodTask = producer.Produce(queue);

            //must start before await, or production will never hit!
            var allTasks = consumers.Select(
                c => c.Consume(queue)).ToList();

            foreach (var t in allTasks)
            {
                await t.ConfigureAwait(false);
            }

            var allProduced = await prodTask.ConfigureAwait(false);

            if (!allProduced)
            {
                var errors = producer.Errors;
                throw errors.Select(e => e.Item2).First();
            }
        }

        public async Task TransferRecords()
        {
                                                                                            

            var jsonSerializer = new JsonNetSerializer();

            //do consumption runs here
            await ProduceConsume(new RestaurantAccountProducer(), () => new RestaurantAccountConsumer(jsonSerializer))
                .ConfigureAwait(false);

            await ProduceConsume(new RestaurantProducer(), () => new RestaurantConsumer(jsonSerializer))
                .ConfigureAwait(false);

            await ProduceConsume(new EmployeeProducer(), () => new EmployeeConsumer(jsonSerializer))
                .ConfigureAwait(false);

            //inv categories are seeded by site, none here yet

            //vendors
            await ProduceConsume(new VendorProducer(), () => new VendorsConsumer(jsonSerializer), 3)
                .ConfigureAwait(false);
       
            await ProduceConsume(new ReceivingOrderProducer(), () => new ReceivingOrdersConsumer(jsonSerializer))
                .ConfigureAwait(false);

            await ProduceConsume(new InventoryProducer(), () => new InventoriesConsumer(jsonSerializer), 3)
                .ConfigureAwait(false);

            //ents that can work in parallel
            var pos = ProduceConsume(new PurchaseOrderProducer(), () => new PurchaseOrderConsumer(jsonSerializer));
            var appInvites = ProduceConsume(new ApplicationInvitationProducer(),
                () => new ApplicationInvitationConsumer(jsonSerializer));
            var pars = ProduceConsume(new ParProducer(), () => new ParsConsumer(jsonSerializer), 2);
            var miseEmp = ProduceConsume(new MiseEmployeeAccountProducer(),
                () => new MiseEmployeeAccountsConsumer(jsonSerializer));
            await Task.WhenAll(pos, appInvites, pars, miseEmp);
        }
    }
}
