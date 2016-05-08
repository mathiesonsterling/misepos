using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Database.AzureDefinitions.Entities.Inventory;
using Mise.Database.AzureDefinitions.Entities.People;
using Mise.Database.AzureDefinitions.Entities.Restaurant;
using Mise.Database.AzureDefinitions.Entities.Vendor;

namespace TransferMiseEntitesTool
{
    class EntityProducer
    {
        private readonly BlockingCollection<Restaurant> _restaurants;
        private readonly BlockingCollection<Employee> _employees;
        private readonly BlockingCollection<Inventory> _inventories;
        private readonly BlockingCollection<Vendor> _vendors;
        private readonly BlockingCollection<ReceivingOrder> _receivingOrders;
        private readonly BlockingCollection<PurchaseOrder> _purchaseOrders;
        private readonly BlockingCollection<Par> _pars;
              
        public EntityProducer()
        {
            
        }
    }
}
