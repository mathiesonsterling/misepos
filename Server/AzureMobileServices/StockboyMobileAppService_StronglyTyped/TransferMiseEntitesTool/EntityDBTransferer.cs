using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Common.Entities.DTOs;
using Mise.Core.Common.Services.Implementation.Serialization;
using Mise.Core.Entities.Base;
using TransferMiseEntitesTool.Consumers;
using TransferMiseEntitesTool.Producers;

namespace TransferMiseEntitesTool
{
    class EntityDBTransferer
    {
        private readonly BlockingCollection<RestaurantEntityDataTransportObject> _restaurants;
        private readonly BlockingCollection<RestaurantEntityDataTransportObject> _employees;
        private readonly BlockingCollection<RestaurantEntityDataTransportObject> _inventories;
        private readonly BlockingCollection<RestaurantEntityDataTransportObject> _vendors;
        private readonly BlockingCollection<RestaurantEntityDataTransportObject> _receivingOrders;
        private readonly BlockingCollection<RestaurantEntityDataTransportObject> _purchaseOrders;
        private readonly BlockingCollection<RestaurantEntityDataTransportObject> _pars;
        private readonly BlockingCollection<RestaurantEntityDataTransportObject> _miseEmployeeAccounts;
        private readonly BlockingCollection<RestaurantEntityDataTransportObject> _restaurantAccounts;
        private readonly BlockingCollection<RestaurantEntityDataTransportObject> _applicationInvitations;

        public EntityDBTransferer()
        {
            _restaurantAccounts = new BlockingCollection<RestaurantEntityDataTransportObject>(); //
            _applicationInvitations = new BlockingCollection<RestaurantEntityDataTransportObject>(); //
            _restaurants = new BlockingCollection<RestaurantEntityDataTransportObject>(); //
            _employees = new BlockingCollection<RestaurantEntityDataTransportObject>(); //
            _inventories = new BlockingCollection<RestaurantEntityDataTransportObject>(); //
            _vendors = new BlockingCollection<RestaurantEntityDataTransportObject>(); //
            _receivingOrders = new BlockingCollection<RestaurantEntityDataTransportObject>(); //
            _purchaseOrders = new BlockingCollection<RestaurantEntityDataTransportObject>(); //
            _pars = new BlockingCollection<RestaurantEntityDataTransportObject>();//
            _miseEmployeeAccounts = new BlockingCollection<RestaurantEntityDataTransportObject>();
        }

        private async Task ProduceConsume<TEnt>(BaseAzureEntitiesProducer producer, BaseConsumer<TEnt> consumer) where TEnt : class, IEntityBase
        {
            var queue = new BlockingCollection<RestaurantEntityDataTransportObject>();
            var prodTask = Task.Run(() => producer.Produce(queue));
            var consumeTask = Task.Run(() => consumer.Consume(queue));

            await Task.WhenAll(prodTask, consumeTask);
        }

        public async Task TransferRecords()
        {
            var producer = new EntityProducer(_restaurants, _employees, _inventories, _vendors, _receivingOrders, _purchaseOrders, _pars, _miseEmployeeAccounts,
                _restaurantAccounts, _applicationInvitations);

            var jsonSerializer = new JsonNetSerializer();
            var undone = await producer.Produce();

            if (undone.Any())
            {
                //we've had an error loading some!
                var numWrong = undone.Count();
            }

            //var restAccountProducer = new RestaurantAccountProducer(_restaurantAccounts);
            //do consumption runs here
            var restAccounts = new RestaurantAccountConsumer(jsonSerializer);
            await restAccounts.Consume(_restaurantAccounts);

	        var rests = new RestaurantConsumer(jsonSerializer);
	        await rests.Consume(_restaurants);

            var emps = new EmployeeConsumer(jsonSerializer);
            await emps.Consume(_employees);

            //inv categories

            //vendors
            var vendors = new VendorsConsumer(jsonSerializer);
	        await vendors.Consume(_vendors);

            //rest sections should be done already by restaurants?

            var appInvites = new ApplicationInvitationConsumer(jsonSerializer);
	        var inventories = new InventoriesConsumer(jsonSerializer);
            var pars = new ParsConsumer(jsonSerializer);
            var purchaseOrders = new PurchaseOrderConsumer(jsonSerializer);
            var receivingOrders = new ReceivingOrdersConsumer(jsonSerializer);
            var miseEmpAccounts = new MiseEmployeeAccountsConsumer(jsonSerializer);
            var otherTasks = new List<Task>
            {
                appInvites.Consume(_applicationInvitations),
				inventories.Consume(_inventories),
                pars.Consume(_pars),
                purchaseOrders.Consume(_purchaseOrders),
                receivingOrders.Consume(_receivingOrders),
                miseEmpAccounts.Consume(_miseEmployeeAccounts)
            };

	        await Task.WhenAll(otherTasks);
        }
    }
}
