using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;

using Mise.Core.Common.Services.WebServices;
using Mise.Core.Services.UtilityServices;
using Mise.Core.Client.Entities;
using Mise.Core.Client.Entities.Restaurant;
using Mise.Core.Client.Entities.People;
using Mise.Core.Client.Entities.Vendor;
using Mise.Core.Client.Entities.Inventory;
using Mise.Core.Client.Entities.Accounts;
using DbInventory = Mise.Core.Client.Entities.Inventory.Inventory;
using System.Diagnostics.Tracing;
using Mise.Core.Entities.Inventory.Events;
using Microsoft.WindowsAzure.MobileServices.Sync;
using System.Threading;
using Mise.Core.Entities.Base;
using System.Linq.Expressions;
using Mise.Core.Client.Entities.Categories;
using Xamarin.Forms;
using System.Net.Http.Headers;


namespace Mise.Inventory.Services.Implementation.WebServiceClients.Azure.AzureStrongTypedClient
{
    public class AzureStrongTypedClient :  IInventoryApplicationWebService
    {
        private readonly IMobileServiceClient _client;
        private readonly ILogger _logger;
        private Guid? _restaurantId;

        private IMobileServiceSyncTable<Mise.Core.Client.Entities.Accounts.RestaurantAccount> _restaurantAccountTable;
        private IMobileServiceSyncTable<Mise.Core.Client.Entities.Restaurant.Restaurant> _restaurantTable;
        private IMobileServiceSyncTable<Employee> _employeeTable;
        private IMobileServiceSyncTable<Vendor> _vendorTable;
        private IMobileServiceSyncTable<ReceivingOrder> _receivingOrderTable;
        private IMobileServiceSyncTable<PurchaseOrder> _purchaseOrderTable;
        private IMobileServiceSyncTable<ApplicationInvitation> _applicationInvitationTable;
        private IMobileServiceSyncTable<Par> _parTable;
        private IMobileServiceSyncTable<DbInventory> _inventoryTable;
        private IMobileServiceSyncTable<InventoryCategory> _categoriesTable;

        public bool Synched{ get; private set; }
        public static MobileServiceSQLiteStore DefineTables(MobileServiceSQLiteStore store)
        {
            store.DefineTable<Mise.Core.Client.Entities.Accounts.RestaurantAccount>();
            store.DefineTable<Mise.Core.Client.Entities.Restaurant.Restaurant>();
            store.DefineTable<Employee>();
            store.DefineTable<Vendor>();
            store.DefineTable<ReceivingOrder>();
            store.DefineTable<PurchaseOrder>();
            store.DefineTable<ApplicationInvitation>();
            store.DefineTable<Par>();
            store.DefineTable<DbInventory>();
            store.DefineTable<InventoryCategory>();

            return store;
        }

        public AzureStrongTypedClient(ILogger logger, IMobileServiceClient client)
        {
            _logger = logger;
            _client = client;
        }

        #region IInventoryApplicationWebService implementation

        public Task SetRestaurantId(Guid restaurantId)
        {
            _restaurantId = restaurantId;

            //dump info here?
            return SynchWithServer();
        }

        private async Task SyncTable<TEnt>(string tableName, IMobileServiceSyncTable<TEnt> table,
            Expression<Func<TEnt, bool>> whereClause = null)
            where TEnt: IMiseClientEntity
        {
            if (table == null)
            {
                table = _client.GetSyncTable<TEnt>();
            }

            var query = table.CreateQuery().Where(i => !i.Deleted);
            if (whereClause != null)
            {
                query = query.Where(whereClause);
            }
            await table.PullAsync("all" + tableName, query);

        }
            
        public async Task<bool> SynchWithServer()
        {
            try
            {
                await _client.SyncContext.PushAsync();

                if(_restaurantId.HasValue)
                {
                    var allSyncTasks = new List<Task>{
                        SyncTable("RestaurantAccounts", _restaurantAccountTable),
                        SyncTable("RestaurantsFor" + _restaurantId.ToString(), _restaurantTable, 
                            r => r.EntityId == _restaurantId.Value),
                        SyncTable("EmployeesFor" + _restaurantId.ToString(), _employeeTable, 
                            e => e.RestaurantsEmployedAt.Where(r => r != null && r.Restaurant != null)
                                    .Select(r => r.Restaurant.RestaurantID)
                                    .Contains(_restaurantId.Value)),
                        SyncTable("Vendors", _vendorTable),
                        SyncTable("ReceivingOrdersFor" + _restaurantId.Value, _receivingOrderTable,
                            ro => ro.Restaurant != null && ro.Restaurant.RestaurantID == _restaurantId.Value),
                        SyncTable("PurchaseOrdersFor" + _restaurantId.Value, _purchaseOrderTable,
                            po => po.Restaurant != null && po.Restaurant.RestaurantID == _restaurantId.Value),
                        SyncTable("ApplicationInvitationsFor" + _restaurantId.Value, _applicationInvitationTable),
                        SyncTable("ParsFor" + _restaurantId.Value, _parTable, 
                            p => p.Restaurant != null && p.Restaurant.RestaurantID == _restaurantId.Value),
                        SyncTable("InventoriesFor" + _restaurantId.Value, _inventoryTable, i => i.Restaurant != null 
                            && i.Restaurant.RestaurantID == _restaurantId.Value),
                        SyncTable("InventoryCategories", _categoriesTable)
                    };

                    foreach(var t in allSyncTasks)
                    {
                        await t.ConfigureAwait(false);
                    }
                }
                else{
                    SyncTable("Restaurants", _restaurantTable);
                    SyncTable("Employees", _employeeTable);
                    SyncTable("ApplicationInvitations", _applicationInvitationTable);
                }

                Synched = true;
                return true;
            } 
            catch(MobileServiceInvalidOperationException me)
            {
                _logger.HandleException(me, LogLevel.Error);
                return false;
            }
            catch(Exception e)
            {
                _logger.HandleException(e);
                throw;
            }
        }

        #endregion

        #region IAccountWebService implementation

        public async Task<Mise.Core.Entities.Accounts.IAccount> GetAccountById(Guid id)
        {
            var items = await _restaurantAccountTable
                .Where(a => a.EntityId == id)
                .Take(1)
                .ToEnumerableAsync();

            var item = items.FirstOrDefault();
            if (item == null)
            {
                return null;
            }

            return item.ToBusinessEntity();
        }

        public async Task<Mise.Core.Entities.Accounts.IAccount> GetAccountFromEmail(Mise.Core.ValueItems.EmailAddress email)
        {
            var items = await _restaurantAccountTable
                .Where(a => a.PrimaryEmail != null && a.PrimaryEmail == email.Value)
                .Take(1)
                .ToEnumerableAsync();

            var item = items.FirstOrDefault();
            if (item == null)
            {
                return null;
            }
            return item.ToBusinessEntity();
        }

        #endregion

        #region IEventStoreWebService implementation

        private async Task<bool> UpdateEntity<TEnt, TDb>(TEnt entity, IMobileServiceSyncTable<TDb> table, Func<TEnt, TDb> createDb) 
            where TDb : EntityData
            where TEnt : IEntityBase
        {
            var existsTask = table
                .Where(a => a.Id == entity.Id.ToString())
                .Take(1)
                .ToEnumerableAsync();

            var busItem = createDb(entity);
            var exists = await existsTask.ConfigureAwait(false);
            if (exists.Any())
            {
                await table.UpdateAsync(busItem).ConfigureAwait(false);
            }
            else
            {
                await table.InsertAsync(busItem).ConfigureAwait(false);
            }

            return true;
        }

        public Task<bool> SendEventsAsync(Mise.Core.Common.Entities.Accounts.RestaurantAccount updatedEntity, IEnumerable<Mise.Core.Entities.Accounts.IAccountEvent> events)
        {
            return UpdateEntity(updatedEntity, _restaurantAccountTable, ent => new RestaurantAccount(ent));
        }

        #endregion

        #region IApplicationInvitationWebService implementation

        public async Task<IEnumerable<Mise.Core.Common.Entities.ApplicationInvitation>> GetInvitationsForRestaurant(Guid restaurantID)
        {
            var items = await _applicationInvitationTable
                .Where(ai => ai != null
                            && !ai.Deleted
                            && ai.Restaurant.RestaurantID == restaurantID)
                .ToEnumerableAsync();

            var busItems = items.Select(i => i.ToBusinessEntity());
            return busItems;
        }

        public async Task<System.Collections.Generic.IEnumerable<Mise.Core.Common.Entities.ApplicationInvitation>> GetInvitationsForEmail(Mise.Core.ValueItems.EmailAddress email)
        {
            var items = await _applicationInvitationTable
                .Where(ai => ai != null
                    && !ai.Deleted
                    && ai.DestinationEmployee.Emails.Contains(email.Value) 
                    || ai.InvitingEmployee.Emails.Contains(email.Value))
                .ToEnumerableAsync();

            var busItems = items.Select(i => i.ToBusinessEntity());
            return busItems;
        }

        #endregion

        #region IEventStoreWebService implementation

        public async Task<bool> SendEventsAsync(Mise.Core.Common.Entities.ApplicationInvitation updatedEntity, System.Collections.Generic.IEnumerable<Mise.Core.Entities.Restaurant.Events.IApplicationInvitationEvent> events)
        {
            var miseApp = new MiseApplication(updatedEntity.Application);
            var destEmps = await _employeeTable.Where(e => e.EntityId == updatedEntity.DestinationEmployeeID)
                                               .Take(1)
                                               .ToEnumerableAsync();
            var invitEmps = await _employeeTable.Where(e => e.EntityId == updatedEntity.InvitingEmployeeID)
                                                .Take(1)
                                                .ToEnumerableAsync();
            var rest = await _restaurantTable.Where(r => r.EntityId == updatedEntity.RestaurantID)
                                             .Take(1)
                                             .ToEnumerableAsync();
            return await UpdateEntity(updatedEntity, _applicationInvitationTable, 
                ent => new ApplicationInvitation(ent, miseApp, destEmps.FirstOrDefault(), invitEmps.FirstOrDefault(), rest.FirstOrDefault()));
        }


        public Task<IEnumerable<Mise.Core.Common.Entities.Inventory.Inventory>> GetInventoriesForRestaurant(Guid restaurantID)
        {
            return _inventoryTable.Where(i => i != null && !i.Deleted && i.Restaurant != null
                            && i.Restaurant.EntityId == restaurantID)
                .Select(i => i.ToBusinessEntity())
                .ToEnumerableAsync();
        }

        public async Task<bool>  SendEventsAsync(Mise.Core.Common.Entities.Inventory.Inventory updatedEntity, 
            IEnumerable<Mise.Core.Entities.Inventory.Events.IInventoryEvent> events)
        {
            var rests = await _restaurantTable.Where(r => r.EntityId == updatedEntity.RestaurantID)
                                              .Take(1).ToEnumerableAsync();
            var rest = rests.First();

            var cats = await _categoriesTable.Where(c => !c.Deleted).ToEnumerableAsync();

            var vendors = await _vendorTable.Where(v => !v.Deleted).ToEnumerableAsync();

            var emps = await _employeeTable.Where(e => e.EntityId == updatedEntity.CreatedByEmployeeID).ToEnumerableAsync();
            return await UpdateEntity(updatedEntity, _inventoryTable, e => new DbInventory(updatedEntity, rest,
                emps.ToList(), rest.InventorySections, cats, vendors));
        }

        #endregion

        #region IPurchaseOrderWebService implementation

        public async Task<System.Collections.Generic.IEnumerable<Mise.Core.Entities.Inventory.IPurchaseOrder>> GetPurchaseOrdersForRestaurant(Guid restaurantId)
        {
            var res = await _purchaseOrderTable
                .Where(po => po.Restaurant != null && po.Restaurant.RestaurantID == restaurantId)
                .Take(1)
                .ToEnumerableAsync();

            return res.Select(p => p.ToBusinessEntity());
        }

        #endregion

        #region IEventStoreWebService implementation

        public async Task<bool> SendEventsAsync(Mise.Core.Common.Entities.Inventory.PurchaseOrder updatedEntity, System.Collections.Generic.IEnumerable<Mise.Core.Entities.Inventory.Events.IPurchaseOrderEvent> events)
        {
            var cats = await _categoriesTable.Where(c => !c.Deleted).ToEnumerableAsync();
            var vendors = await _vendorTable.Where(v => !v.Deleted).ToEnumerableAsync();
            var rest = await _restaurantTable.Where(r => !r.Deleted && r.EntityId == updatedEntity.RestaurantID).Take(1).ToEnumerableAsync();
            var emp = await _employeeTable.Where(e => !e.Deleted && e.EntityId == updatedEntity.CreatedByEmployeeID).Take(1).ToEnumerableAsync();
            return await UpdateEntity(updatedEntity, _purchaseOrderTable, 
                e => new Mise.Core.Client.Entities.Inventory.PurchaseOrder(updatedEntity, cats, vendors, 
                    rest.FirstOrDefault(), emp.FirstOrDefault()));
        }

        #endregion

        #region IReceivingOrderWebService implementation

        public Task<System.Collections.Generic.IEnumerable<Mise.Core.Common.Entities.Inventory.ReceivingOrder>> GetReceivingOrdersForRestaurant(Guid restaurantID)
        {
            return _receivingOrderTable.Where(r => r.Restaurant != null && r.Restaurant.RestaurantID == restaurantID)
                .Select(r => r.ToBusinessEntity())
                .ToEnumerableAsync();
        }

        #endregion

        #region IEventStoreWebService implementation

        public async Task<bool> SendEventsAsync(Mise.Core.Common.Entities.Inventory.ReceivingOrder updatedEntity, IEnumerable<Mise.Core.Entities.Vendors.Events.IReceivingOrderEvent> events)
        {
            var rests = await _restaurantTable.Where(r => updatedEntity.RestaurantID == r.EntityId)
                .Take(1).ToEnumerableAsync();
            var emps = await _employeeTable.Where(r => updatedEntity.ReceivedByEmployeeID == r.EntityId).Take(1).ToEnumerableAsync();
            var cats = await _categoriesTable.Where(c => !c.Deleted).ToEnumerableAsync();
            var vendor = await _vendorTable.Where(v => updatedEntity.VendorID == v.EntityId).Take(1).ToEnumerableAsync();

            var pos = await _purchaseOrderTable.Where(p => p.EntityId == updatedEntity.PurchaseOrderID).Take(1).ToEnumerableAsync();
            return await UpdateEntity(updatedEntity, _receivingOrderTable, e => new ReceivingOrder(updatedEntity, rests.FirstOrDefault(),
                emps.FirstOrDefault(), pos.FirstOrDefault(), vendor.FirstOrDefault(), cats));
        }

        #endregion

      
        #region IParWebService implementation

        public Task<Mise.Core.Common.Entities.Inventory.Par> GetCurrentPAR(Guid restaurantID)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Mise.Core.Common.Entities.Inventory.Par>> GetPARsForRestaurant(Guid restaurantID)
        {
            var items = await _parTable.Where(p => !p.Deleted && p.Restaurant != null 
                                                && p.Restaurant.EntityId == restaurantID)
                .ToEnumerableAsync();

            return items.Select(p => p.ToBusinessEntity());
        }

        #endregion

        #region IEventStoreWebService implementation

        public Task<bool> SendEventsAsync(Mise.Core.Common.Entities.Inventory.Par updatedEntity, IEnumerable<Mise.Core.Entities.Inventory.Events.IParEvent> events)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IVendorWebService implementation

        public Task<IEnumerable<Mise.Core.Common.Entities.Vendors.Vendor>> GetVendorsWithinSearchRadius(Mise.Core.ValueItems.Location currentLocation, Mise.Core.ValueItems.Distance radius)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Mise.Core.Common.Entities.Vendors.Vendor>> GetVendorsAssociatedWithRestaurant(Guid restaurantID)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEventStoreWebService implementation

        public Task<bool> SendEventsAsync(Mise.Core.Common.Entities.Vendors.Vendor updatedEntity, IEnumerable<Mise.Core.Entities.Vendors.Events.IVendorEvent> events)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IInventoryRestaurantWebService implementation

        public Task<IEnumerable<Mise.Core.Common.Entities.Restaurant>> GetRestaurants(Mise.Core.ValueItems.Location deviceLocation, Mise.Core.ValueItems.Distance maxDistance)
        {
            throw new NotImplementedException();
        }

        public Task<Mise.Core.Common.Entities.Restaurant> GetRestaurant(Guid restaurantID)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEventStoreWebService implementation

        public Task<bool> SendEventsAsync(Mise.Core.Common.Entities.Restaurant updatedEntity, IEnumerable<Mise.Core.Entities.Restaurant.Events.IRestaurantEvent> events)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IInventoryEmployeeWebService implementation

        public Task<IEnumerable<Mise.Core.Common.Entities.People.Employee>> GetEmployeesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Mise.Core.Common.Entities.People.Employee>> GetEmployeesForRestaurant(Guid restaurantID)
        {
            throw new NotImplementedException();
        }

        public async Task<Mise.Core.Common.Entities.People.Employee> GetEmployeeByPrimaryEmailAndPassword(Mise.Core.ValueItems.EmailAddress email, Mise.Core.ValueItems.Password password)
        {
            var emps = await _employeeTable.Where(e => e.Emails.Contains(email.Value) && e.PasswordHash == password.HashValue)
                .Take(1)
                .ToEnumerableAsync();

            if (emps.Any())
            {
                return emps.First().ToBusinessEntity();
            }

            return null;
        }

        public async Task<bool> IsEmailRegistered(Mise.Core.ValueItems.EmailAddress email)
        {
            var emps = await _employeeTable.Where(e => e.Emails.Contains(email.Value)).ToEnumerableAsync();
            return emps.Any();
        }

        #endregion

        #region IEventStoreWebService implementation

        public Task<bool> SendEventsAsync(Mise.Core.Common.Entities.People.Employee updatedEntity, System.Collections.Generic.IEnumerable<Mise.Core.Entities.People.Events.IEmployeeEvent> events)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

