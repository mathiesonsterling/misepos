using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Common.Entities;
using Mise.Core.Common.Entities.Accounts;
using Mise.Core.Common.Entities.DTOs;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Common.Entities.People;
using Mise.Core.Common.Entities.Vendors;
using Mise.Core.Common.Services.Implementation.Serialization;
using TransferMiseEntitesTool.Consumers;
using TransferMiseEntitesTool.Database;

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
            //do consumption runs here
            var restAccounts = new RestaurantAccountConsumer(jsonSerializer);

            var allTasks = new List<Task>();

            allTasks.Add(Task.Run(() => restAccounts.Consume(_restaurantAccounts)));

            var undone = await production;
            await Task.WhenAll(allTasks);
        }
    }
}
