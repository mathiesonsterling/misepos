using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Mise.Core.Common.Entities;
using Mise.Core.Common.Entities.Accounts;
using Mise.Core.Common.Entities.DTOs;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Common.Entities.Vendors;
using Mise.Core.Common.Services.Implementation.Serialization;
using Mise.Core.Server.Services.Implementation;
using Mise.Core.Services.UtilityServices;
using Mise.Inventory;
using Mise.Inventory.Services.Implementation.WebServiceClients.Azure;

namespace DeveloperTools.Commands
{
    class PopulateInventorySqlServerDBCommand : BaseProgressReportingCommand
    {
        private readonly ILogger _logger;
        private readonly bool _addDemo;

        private readonly IMobileServiceClient _client;
        private readonly EntityDataTransportObjectFactory _entityDataTransportObjectFactory;
        public PopulateInventorySqlServerDBCommand(IProgress<ProgressReport> progress, ILogger logger, BuildLevel level) : base(progress)
        {
            _logger = logger;
            _addDemo = level == BuildLevel.Development;

            var mobileServiceLocation = AzureServiceLocator.GetAzureMobileServiceLocation(level);
            _client = new MobileServiceClient(
               mobileServiceLocation.Uri.ToString(),
               mobileServiceLocation.AppKey
            );

            _entityDataTransportObjectFactory = new EntityDataTransportObjectFactory(new JsonNetSerializer());
        }

        private class DTOStorage : Dictionary<Guid, RestaurantEntityDataTransportObject>
        {
            public void AddRange(IEnumerable<RestaurantEntityDataTransportObject> dtos)
            {
                foreach (var dto in dtos)
                {
                    Add(dto);
                }
            }

            public void Add(RestaurantEntityDataTransportObject dto)
            {
                if (ContainsKey(dto.Id) == false)
                {
                    Add(dto.Id, dto);
                }
            }
        } 

        public override async Task Execute()
        {
            //delete items
            Report("Reset database");
            await DeleteCurrentDBItems();

            //get the fake service, and populate all parts of it!
            var fakeService = new FakeInventoryServiceDAL();

            var allDTOs = new DTOStorage();
            Report("Generating accounts");
            //accounts
            var accts = (await fakeService.GetAccountsAsync());
            var dtos = accts
                .Select(act => act as RestaurantAccount)
                .Select(act => _entityDataTransportObjectFactory.ToDataTransportObject(act));
            allDTOs.AddRange(dtos);



            //restaurants
            var rests = (await fakeService.GetRestaurantsAsync()).ToList();
            if (_addDemo == false)
            {
                rests = rests.Where(r => r.Id != Guid.Empty).ToList();
            }
            var restDTOs = rests
                .Select(r => r as Restaurant)
                .Select(r => _entityDataTransportObjectFactory.ToDataTransportObject(r));
            allDTOs.AddRange(restDTOs);

            Report("Added " + rests.Count + " Restaurants");

            var vendors = await fakeService.GetVendorsAsync();
            if (_addDemo == false)
            {
                vendors = vendors.Where(v => v.Id != Guid.Empty);
            }
            var vendDTOs = vendors
                .Select(v => v as Vendor)
                .Select(v => _entityDataTransportObjectFactory.ToDataTransportObject(v));
            allDTOs.AddRange(vendDTOs);
            Report("Added Vendors");

            foreach (var rest in rests)
            {
                var emps = await fakeService.GetEmployeesAsync(rest.Id);
                foreach (var emp in emps)
                {
                    if (emp.Id == Guid.Empty)
                    {
                        throw new Exception("Employee does not have ID");
                    }
                    try
                    {
                        var dc = emp as Employee;
                        allDTOs.Add(_entityDataTransportObjectFactory.ToDataTransportObject(dc));
                    }
                    catch (Exception ex)
                    {
                        _logger.HandleException(ex);
                        throw;
                    }
                }
                Report("Added employees to " + rest.Name.ShortName);


                var inventories = await fakeService.GetInventoriesAsync(rest.Id);
                foreach (var inv in inventories)
                {
                    if (inv.Id == Guid.Empty)
                    {
                        throw new Exception("Inventory does not have ID");
                    }
                    var actualInv = inv as Inventory;
                    allDTOs.Add(_entityDataTransportObjectFactory.ToDataTransportObject(actualInv));
                }
                Report("Added inventories to " + rest.Name.ShortName);

                var ros = await fakeService.GetReceivingOrdersAsync(rest.Id);
                foreach (var ro in ros)
                {
                    if (ro.Id == Guid.Empty)
                    {
                        throw new Exception("RO does not have ID");
                    }
                    var actualReceivingOrder = ro as ReceivingOrder;
                    allDTOs.Add(_entityDataTransportObjectFactory.ToDataTransportObject(actualReceivingOrder));
                }
                //pgBar.Value += perRestAmt / NUM_INNER_STEPS;
                Report("Added receiving orders to " + rest.Name.ShortName);
                var pos = await fakeService.GetPurchaseOrdersAsync(rest.Id);
                foreach (var po in pos.Select(p => p as PurchaseOrder))
                {
                    if (po.Id == Guid.Empty)
                    {
                        throw new Exception("PO does not have ID");
                    }

                    allDTOs.Add(_entityDataTransportObjectFactory.ToDataTransportObject(po));
                }
                Report("Added purchase orders to " + rest.Name.ShortName);
                //pgBar.Value += perRestAmt / NUM_INNER_STEPS;
                var pars = await fakeService.GetPARsAsync(rest.Id);
                foreach (var par in pars.Select(p => p as Par))
                {
                    if (par.Id == Guid.Empty)
                    {
                        throw new Exception("PAR does not have ID");
                    }
                    allDTOs.Add(_entityDataTransportObjectFactory.ToDataTransportObject(par));
                }
                Report("Added PARs to " + rest.Name.ShortName);
                //pgBar.Value += perRestAmt / NUM_INNER_STEPS;

            }
            var invites = await fakeService.GetApplicationInvitations();
            if (_addDemo == false)
            {
                invites = invites.Where(i => i.RestaurantID != Guid.Empty);
            }
            foreach (var invite in invites.Select(i => i as ApplicationInvitation))
            {
                allDTOs.Add(_entityDataTransportObjectFactory.ToDataTransportObject(invite));
            }
            Report("Added invitations");

            Report($"Storing {allDTOs.Count} entities in DB");

            var table = _client.GetTable<AzureEntityStorage>();
            /*
            var azureItems = allDTOs.Select(dto => new AzureEntityStorage(dto));
            var insertTasks = azureItems.Select(ai => table.InsertAsync(ai));

            try
            {
                await Task.WhenAll(insertTasks).ConfigureAwait(false);
            }
                catch (Exception e)
            {
                _logger.HandleException(e);
                throw;
            }
            */
            foreach (var dto in allDTOs.Values)
            {
                var ai = new AzureEntityStorage(dto);
                await table.InsertAsync(ai);
            }

            Finish();

        }

        public async Task DeleteCurrentDBItems()
        {
            var entTable = _client.GetTable<AzureEntityStorage>();
            var  entities = await entTable.ToEnumerableAsync();
            var delEntities = entities.Select(ent => entTable.DeleteAsync(ent));

            var evTable = _client.GetTable<AzureEventStorage>();
            var events = await evTable.ToEnumerableAsync();
            var delEvents = events.Select(ev => evTable.DeleteAsync(ev));

            await Task.WhenAll(delEntities).ConfigureAwait(false);
            await Task.WhenAll(delEvents).ConfigureAwait(false);
        }
        public override int NumSteps => 32;
    }
}
