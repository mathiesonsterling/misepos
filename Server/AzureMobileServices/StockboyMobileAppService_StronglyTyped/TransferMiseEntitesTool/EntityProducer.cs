using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Mise.Core.Common.Entities.DTOs;
using TransferMiseEntitesTool.Database;

namespace TransferMiseEntitesTool
{
    class EntityProducer
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

        private readonly List<AzureEntityStorage> _failedTransfers;
        
        public EntityProducer(BlockingCollection<RestaurantEntityDataTransportObject> restaurants, BlockingCollection<RestaurantEntityDataTransportObject> employees,
            BlockingCollection<RestaurantEntityDataTransportObject> inventories, BlockingCollection<RestaurantEntityDataTransportObject> vendors, BlockingCollection<RestaurantEntityDataTransportObject> receivingOrders,
            BlockingCollection<RestaurantEntityDataTransportObject> purchaseOrders, BlockingCollection<RestaurantEntityDataTransportObject> pars, BlockingCollection<RestaurantEntityDataTransportObject> miseEmployeeAccounts,
            BlockingCollection<RestaurantEntityDataTransportObject> restaurantAccounts, BlockingCollection<RestaurantEntityDataTransportObject> applicationInvitations)
        {
            _restaurants = restaurants;
            _employees = employees;
            _inventories = inventories;
            _vendors = vendors;
            _receivingOrders = receivingOrders;
            _purchaseOrders = purchaseOrders;
            _pars = pars;
            _miseEmployeeAccounts = miseEmployeeAccounts;
            _restaurantAccounts = restaurantAccounts;
            _applicationInvitations = applicationInvitations;

            _failedTransfers = new List<AzureEntityStorage>();
        }

        public List<AzureEntityStorage> Produce()
        {
            using (var source = new AzureNonTypedEntities())
            {
                source.Database.CommandTimeout = 180;
                foreach (var dto in source.AzureEntityStorages)
                {
                    try
                    {
                        var restDTO = dto.ToRestaurantDTO();
                        switch (dto.MiseEntityType)
                        {
                            case "Mise.Core.Common.Entities.Accounts.MiseEmployeeAccount":
                                _miseEmployeeAccounts.Add(restDTO);
                                break;
                            case "Mise.Core.Common.Entities.Accounts.RestaurantAccount":
                                _restaurantAccounts.Add(restDTO);
                                break;
                            case "Mise.Core.Common.Entities.ApplicationInvitation":
                                _applicationInvitations.Add(restDTO);
                                break;
                            case "Mise.Core.Common.Entities.Employee":
                                _employees.Add(restDTO);
                                break;
                            case "Mise.Core.Common.Entities.Inventory.Inventory":
                                _inventories.Add(restDTO);
                                break;
                            case "Mise.Core.Common.Entities.Inventory.Par":
                                _pars.Add(restDTO);
                                break;
                            case "Mise.Core.Common.Entities.Inventory.PurchaseOrder":
                                _purchaseOrders.Add(restDTO);
                                break;
                            case "Mise.Core.Common.Entities.Inventory.ReceivingOrder":
                                _receivingOrders.Add(restDTO);
                                break;
                            case "Mise.Core.Common.Entities.Restaurant":
                                _restaurants.Add(restDTO);
                                break;
                            case "Mise.Core.Common.Entities.Vendors.Vendor":
                                _vendors.Add(restDTO);
                                break;
                            default:
                                _failedTransfers.Add(dto);
                                break;
                        }
                    }
                    catch (Exception e)
                    {
                        var msg = e.Message;
                        _failedTransfers.Add(dto);
                    }
                }
            }

            _miseEmployeeAccounts.CompleteAdding();
            _restaurantAccounts.CompleteAdding();
            _applicationInvitations.CompleteAdding();
            _employees.CompleteAdding();
            _inventories.CompleteAdding();
            _pars.CompleteAdding();
            _purchaseOrders.CompleteAdding();
            _receivingOrders.CompleteAdding();
            _restaurants.CompleteAdding();
            _vendors.CompleteAdding();
            return _failedTransfers;
        }
    }
}
