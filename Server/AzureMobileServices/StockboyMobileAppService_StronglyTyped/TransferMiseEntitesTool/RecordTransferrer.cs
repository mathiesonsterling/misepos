using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Common.Entities.DTOs;
using Mise.Core.Common.Services.Implementation.Serialization;
using TransferMiseEntitesTool.Consumers;

namespace TransferMiseEntitesTool
{
    class RecordTransferrer
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

        public RecordTransferrer()
        {
            _restaurantAccounts = new BlockingCollection<RestaurantEntityDataTransportObject>();
            _applicationInvitations = new BlockingCollection<RestaurantEntityDataTransportObject>();
            _restaurants = new BlockingCollection<RestaurantEntityDataTransportObject>();
            _employees = new BlockingCollection<RestaurantEntityDataTransportObject>();
            _inventories = new BlockingCollection<RestaurantEntityDataTransportObject>();
            _vendors = new BlockingCollection<RestaurantEntityDataTransportObject>();
            _receivingOrders = new BlockingCollection<RestaurantEntityDataTransportObject>();
            _purchaseOrders = new BlockingCollection<RestaurantEntityDataTransportObject>();
            _pars = new BlockingCollection<RestaurantEntityDataTransportObject>();
            _miseEmployeeAccounts = new BlockingCollection<RestaurantEntityDataTransportObject>();
        }

        public async Task TransferRecords()
        {
            var producer = new EntityProducer(_restaurants, _employees, _inventories, _vendors, _receivingOrders, _purchaseOrders, _pars, _miseEmployeeAccounts,
                _restaurantAccounts, _applicationInvitations);

            var production = Task.Run(() => producer.Produce());

            var jsonSerializer = new JsonNetSerializer();
	        var undone = await production;

            if (undone.Any())
            {
                //we've had an error loading some!
                var numWrong = undone.Count();
            }

            //do consumption runs here
            var restAccounts = new RestaurantAccountConsumer(jsonSerializer);

            //do accounts, then rests, then emps, then the rest
            await restAccounts.Consume(_restaurantAccounts);

	        var rests = new RestaurantConsumer(jsonSerializer);
	        await rests.Consume(_restaurants);

            var emps = new EmployeeConsumer(jsonSerializer);
            await emps.Consume(_employees);

        }
    }
}
