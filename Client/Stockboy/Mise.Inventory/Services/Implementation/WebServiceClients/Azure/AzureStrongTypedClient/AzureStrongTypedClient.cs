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
using Mise.Core.Client.Entities.Accounts;
using DbInventory = Mise.Core.Client.Entities.Inventory.Inventory;
using Microsoft.WindowsAzure.MobileServices.Sync;
using Mise.Core.Entities.Base;
using System.Linq.Expressions;
using Mise.Core.Client.Services;
using Mise.Core.ValueItems;


namespace Mise.Inventory.Services.Implementation.WebServiceClients.Azure.AzureStrongTypedClient
{
    public class AzureStrongTypedClient :  IInventoryApplicationWebService
    {
        private readonly IMobileServiceClient _client;
        private readonly ILogger _logger;
        private readonly IDeviceConnectionService _connService;
        private Guid? _restaurantId;

        private IMobileServiceSyncTable<RestaurantAccount> _restaurantAccountTable;

        private IMobileServiceSyncTable<Core.Client.Entities.Restaurant.Restaurant> _restaurantTable;
        public IMobileServiceSyncTable<Core.Client.Entities.Restaurant.Restaurant> RestaurantTable
        {
            get{
                if (_restaurantTable == null)
                {
                    _restaurantTable = _client.GetSyncTable<Restaurant>();
                }
                return _restaurantTable;
            }
        }

        private IMobileServiceSyncTable<Mise.Core.Client.Entities.Inventory.RestaurantInventorySection> _restaurantInventorySectionTable { get; set; }
        public IMobileServiceSyncTable<Mise.Core.Client.Entities.Inventory.RestaurantInventorySection> RestaurantInventorySectionTable {
            get {
                if (_restaurantInventorySectionTable == null) {
                    _restaurantInventorySectionTable = _client.GetSyncTable<Mise.Core.Client.Entities.Inventory.RestaurantInventorySection> ();
                }

                return _restaurantInventorySectionTable;
            }
        }
        private IMobileServiceSyncTable<Employee> _employeeTable;
        public IMobileServiceSyncTable<Employee> EmployeeTable
        { 
            get 
            { 
                if(_employeeTable == null)
                {
                    _employeeTable = _client.GetSyncTable<Employee>();
                } 
                return _employeeTable;
            }
        }

        private IMobileServiceSyncTable<EmployeeRestaurantRelationships> _employeeRestaurantTable;
        public IMobileServiceSyncTable<EmployeeRestaurantRelationships> EmployeeRestaurantRelationships{
            get
            {
                if (_employeeRestaurantTable == null)
                {
                    _employeeRestaurantTable = _client.GetSyncTable<EmployeeRestaurantRelationships>();
                }
                return _employeeRestaurantTable;
            }
        }
        private IMobileServiceSyncTable<Core.Client.Entities.Vendor.Vendor> _vendorTable;
        public IMobileServiceSyncTable<Core.Client.Entities.Vendor.Vendor> VendorTable {
            get {
                if (_vendorTable == null) {
                    _vendorTable = _client.GetSyncTable<Vendor> ();
                }
                return _vendorTable;
            }
        }
        private IMobileServiceSyncTable<Mise.Core.Client.Entities.Inventory.ReceivingOrder> _receivingOrderTable;
        private IMobileServiceSyncTable<Mise.Core.Client.Entities.Inventory.ReceivingOrder> ReceivingOrderTable {
            get {
                if (_receivingOrderTable == null) {
                    _receivingOrderTable = _client.GetSyncTable<Core.Client.Entities.Inventory.ReceivingOrder> ();
                }
                return _receivingOrderTable;
            }
        }
        private IMobileServiceSyncTable<Mise.Core.Client.Entities.Inventory.PurchaseOrder> _purchaseOrderTable;
        private IMobileServiceSyncTable<Core.Client.Entities.Inventory.PurchaseOrder> PurchaseOrderTable {
            get {
                if (_purchaseOrderTable == null) {
                    _purchaseOrderTable = _client.GetSyncTable<Core.Client.Entities.Inventory.PurchaseOrder> ();
                }
                return _purchaseOrderTable;
            }
        }
        private IMobileServiceSyncTable<Mise.Core.Client.Entities.Accounts.ApplicationInvitation> _applicationInvitationTable;
        private IMobileServiceSyncTable<Mise.Core.Client.Entities.Accounts.ApplicationInvitation> ApplicationInvitationTable {
            get {
                if (_applicationInvitationTable == null) {
                    _applicationInvitationTable = _client.GetSyncTable<Mise.Core.Client.Entities.Accounts.ApplicationInvitation> ();
                }
                return _applicationInvitationTable;
            }
        }
        private IMobileServiceSyncTable<Mise.Core.Client.Entities.Inventory.Par> _parTable;
        private IMobileServiceSyncTable<Mise.Core.Client.Entities.Inventory.Par> ParTable {
            get {
                if (_parTable == null) {
                    _parTable = _client.GetSyncTable<Core.Client.Entities.Inventory.Par> ();
                }
                return _parTable;
            }
        }
        private IMobileServiceSyncTable<DbInventory> _inventoryTable;
        private IMobileServiceSyncTable<DbInventory> InventoryTable {
            get {
                if (_inventoryTable == null) {
                    _inventoryTable = _client.GetSyncTable<DbInventory> ();
                }
                return _inventoryTable;
            }
        }

        private IMobileServiceSyncTable<Core.Client.Entities.Inventory.InventorySection> _inventorySectionTable;
        private IMobileServiceSyncTable<Core.Client.Entities.Inventory.InventorySection> InventorySectionTable {
            get {
                if (_inventorySectionTable == null) {
                    _inventorySectionTable = _client.GetSyncTable<Core.Client.Entities.Inventory.InventorySection> ();
                }
                return _inventorySectionTable;
            }
        }

        private IMobileServiceSyncTable<Core.Client.Entities.Inventory.LineItems.InventoryBeverageLineItem> _inventoryLineItemTable;
        private IMobileServiceSyncTable<Core.Client.Entities.Inventory.LineItems.InventoryBeverageLineItem> InventoryBeverageLineItemTable {
            get {
                if (_inventoryLineItemTable == null) {
                    _inventoryLineItemTable = _client.GetSyncTable<Core.Client.Entities.Inventory.LineItems.InventoryBeverageLineItem> ();
                }
                return _inventoryLineItemTable;
            }
        }
        private IMobileServiceSyncTable<Mise.Core.Client.Entities.Categories.InventoryCategory> _categoriesTable;
        private IMobileServiceSyncTable<Core.Client.Entities.Categories.InventoryCategory> CategoriesTable {
            get {
                if (_categoriesTable == null) {
                    _categoriesTable = _client.GetSyncTable<Core.Client.Entities.Categories.InventoryCategory> ();
                }
                return _categoriesTable;
            }
        }

        public bool Synched{ get; private set; }


        public static MobileServiceSQLiteStore DefineTables(MobileServiceSQLiteStore store)
        {
            store.DefineTable<Mise.Core.Client.Entities.Accounts.RestaurantAccount>();
            store.DefineTable<Mise.Core.Client.Entities.Restaurant.Restaurant>();
            store.DefineTable<Mise.Core.Client.Entities.Inventory.RestaurantInventorySection>();

            store.DefineTable<Employee>();
            store.DefineTable<Vendor>();
            store.DefineTable<Mise.Core.Client.Entities.Inventory.ReceivingOrder>();
            store.DefineTable<Mise.Core.Client.Entities.Inventory.PurchaseOrder>();
            store.DefineTable<ApplicationInvitation>();
            store.DefineTable<Mise.Core.Client.Entities.Inventory.Par>();

            store.DefineTable<DbInventory>();
            store.DefineTable<Core.Client.Entities.Inventory.InventorySection>();
            store.DefineTable<Core.Client.Entities.Categories.InventoryCategory>();
            store.DefineTable<Core.Client.Entities.Inventory.LineItems.InventoryBeverageLineItem>();
            return store;
        }

        public AzureStrongTypedClient(ILogger logger, IMobileServiceClient client, IDeviceConnectionService connService)
        {
            _logger = logger;
            _client = client;
            _connService = connService;
            _connService.ConnectionStateChanged += async (sender, args) => {
                if(args.IsConnected){
                    await SynchWithServer();
                }
            };
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

            /*
            if (whereClause != null)
            {
                query = query.Where(whereClause);
            }*/
            await table.PullAsync(tableName, query);

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
                        SyncTable("Restaurants", RestaurantTable),

                        SyncTable("Employees",EmployeeTable),
                        SyncTable("Vendors", VendorTable),
                        SyncTable("ReceivingOrders", ReceivingOrderTable),
                        SyncTable("PurchaseOrders", PurchaseOrderTable),
                        SyncTable("ApplicationInvitations", ApplicationInvitationTable),
                        SyncTable("ParsFor", ParTable),
                        SyncTable("Inventories", InventoryTable),
                        SyncTable("InventorySections", InventorySectionTable),
                        SyncTable("InventoryLineItems", InventoryBeverageLineItemTable),
                        SyncTable("InventoryCategories", CategoriesTable)
                    };

                    foreach(var t in allSyncTasks)
                    {
                        try
                        {
                            await t.ConfigureAwait(false);
                        } catch(Exception e)
                        {
                            _logger.HandleException(e);
                            throw;
                        }
                    }
                }
                else{
                    var minSync = new List<Task>{
                        SyncTable("Restaurants", RestaurantTable),
                        SyncTable("Employees", EmployeeTable),
                        SyncTable("ApplicationInvitations", ApplicationInvitationTable)
                    };
                    foreach(var t in minSync)
                    {
                        await t.ConfigureAwait(false);
                    }
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

        public async Task<Core.Entities.Accounts.IAccount> GetAccountById(Guid id)
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

        public async Task<Core.Entities.Accounts.IAccount> GetAccountFromEmail(Mise.Core.ValueItems.EmailAddress email)
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

        public Task<bool> SendEventsAsync(Core.Common.Entities.Accounts.RestaurantAccount updatedEntity, IEnumerable<Mise.Core.Entities.Accounts.IAccountEvent> events)
        {
            return UpdateEntity(updatedEntity, _restaurantAccountTable, ent => new RestaurantAccount(ent));
        }

        #endregion

        #region IApplicationInvitationWebService implementation

        public async Task<IEnumerable<Core.Common.Entities.ApplicationInvitation>> GetInvitationsForRestaurant(Guid restaurantID)
        {
            var items = await ApplicationInvitationTable
                .Where(ai => ai != null
                            && !ai.Deleted
                            && ai.Restaurant.RestaurantID == restaurantID)
                .ToEnumerableAsync();

            var busItems = items.Select(i => i.ToBusinessEntity());
            return busItems;
        }

        public async Task<IEnumerable<Core.Common.Entities.ApplicationInvitation>> GetInvitationsForEmail(EmailAddress email)
        {
            var items = await ApplicationInvitationTable
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

        public async Task<bool> SendEventsAsync(Core.Common.Entities.ApplicationInvitation updatedEntity, System.Collections.Generic.IEnumerable<Mise.Core.Entities.Restaurant.Events.IApplicationInvitationEvent> events)
        {
            var miseApp = new MiseApplication(updatedEntity.Application);
            var destEmps = await EmployeeTable.Where(e => e.EntityId == updatedEntity.DestinationEmployeeID)
                                               .Take(1)
                                               .ToEnumerableAsync();
            var invitEmps = await EmployeeTable.Where(e => e.EntityId == updatedEntity.InvitingEmployeeID)
                                                .Take(1)
                                                .ToEnumerableAsync();
            var rest = await RestaurantTable.Where(r => r.EntityId == updatedEntity.RestaurantID)
                                             .Take(1)
                                             .ToEnumerableAsync();
            return await UpdateEntity(updatedEntity, ApplicationInvitationTable, 
                ent => new ApplicationInvitation(ent, miseApp, destEmps.FirstOrDefault(), invitEmps.FirstOrDefault(), rest.FirstOrDefault()));
        }


        public async Task<IEnumerable<Core.Common.Entities.Inventory.Inventory>> GetInventoriesForRestaurant(Guid restaurantID)
        {
            var invs = await InventoryTable.Where(i => i != null && !i.Deleted && i.Restaurant != null
                            && i.Restaurant.EntityId == restaurantID)
                .ToEnumerableAsync().ConfigureAwait (false);

            var res = new List<Core.Common.Entities.Inventory.Inventory> ();
            foreach (var inv in invs) {
                var commInv = await HydrateInventory (inv);
                res.Add (commInv);
            }

            return res;
        }

        private async Task<Core.Common.Entities.Inventory.Inventory> HydrateInventory (Core.Client.Entities.Inventory.Inventory inv)
        {
            var sections = await InventorySectionTable.Where (s => s.InventoryId == inv.Id).ToListAsync ().ConfigureAwait (false);

            foreach (var sec in sections) {
                var lis = await InventoryBeverageLineItemTable.Where (li => li.InventorySectionId == sec.Id)
                                                              .ToListAsync ().ConfigureAwait (false);

                sec.LineItems = lis;
            }

            inv.Sections = sections;

            return inv.ToBusinessEntity ();
        }

        public async Task<bool>  SendEventsAsync(Mise.Core.Common.Entities.Inventory.Inventory updatedEntity, 
            IEnumerable<Core.Entities.Inventory.Events.IInventoryEvent> events)
        {
            var rests = await RestaurantTable.Where(r => r.EntityId == updatedEntity.RestaurantID)
                                              .Take(1).ToEnumerableAsync();
            var rest = rests.First();

            var cats = await CategoriesTable.Where(c => !c.Deleted).ToEnumerableAsync();

            var vendors = await VendorTable.Where(v => !v.Deleted).ToEnumerableAsync();

            var emps = await EmployeeTable.Where(e => e.EntityId == updatedEntity.CreatedByEmployeeID).ToEnumerableAsync();
            return await UpdateEntity(updatedEntity, InventoryTable, e => new DbInventory(updatedEntity, rest,
                emps.ToList(), rest.InventorySections, cats, vendors));
        }

        #endregion

        #region IPurchaseOrderWebService implementation

        public async Task<IEnumerable<Core.Entities.Inventory.IPurchaseOrder>> GetPurchaseOrdersForRestaurant(Guid restaurantId)
        {
            var res = await PurchaseOrderTable
                .Where(po => po.Restaurant != null && po.Restaurant.RestaurantID == restaurantId)
                .Take(1)
                .ToEnumerableAsync();

            return res.Select(p => p.ToBusinessEntity());
        }

        #endregion

        #region IEventStoreWebService implementation

        public async Task<bool> SendEventsAsync(Mise.Core.Common.Entities.Inventory.PurchaseOrder updatedEntity, System.Collections.Generic.IEnumerable<Mise.Core.Entities.Inventory.Events.IPurchaseOrderEvent> events)
        {
            var cats = await CategoriesTable.Where(c => !c.Deleted).ToEnumerableAsync();
            var vendors = await VendorTable.Where(v => !v.Deleted).ToEnumerableAsync();
            var rest = await RestaurantTable.Where(r => !r.Deleted && r.EntityId == updatedEntity.RestaurantID).Take(1).ToEnumerableAsync();
            var emp = await EmployeeTable.Where(e => !e.Deleted && e.EntityId == updatedEntity.CreatedByEmployeeID).Take(1).ToEnumerableAsync();
            return await UpdateEntity(updatedEntity, PurchaseOrderTable, 
                e => new Mise.Core.Client.Entities.Inventory.PurchaseOrder(updatedEntity, cats, vendors, 
                    rest.FirstOrDefault(), emp.FirstOrDefault()));
        }

        #endregion

        #region IReceivingOrderWebService implementation

        public Task<IEnumerable<Core.Common.Entities.Inventory.ReceivingOrder>> GetReceivingOrdersForRestaurant(Guid restaurantID)
        {
            return ReceivingOrderTable.Where(r => r.Restaurant != null && r.Restaurant.RestaurantID == restaurantID)
                .Select(r => r.ToBusinessEntity())
                .ToEnumerableAsync();
        }

        #endregion

        #region IEventStoreWebService implementation

        public async Task<bool> SendEventsAsync(Core.Common.Entities.Inventory.ReceivingOrder updatedEntity, IEnumerable<Mise.Core.Entities.Vendors.Events.IReceivingOrderEvent> events)
        {
            var rests = await RestaurantTable.Where(r => updatedEntity.RestaurantID == r.EntityId)
                .Take(1).ToEnumerableAsync();
            var emps = await EmployeeTable.Where(r => updatedEntity.ReceivedByEmployeeID == r.EntityId).Take(1).ToEnumerableAsync();
            var cats = await CategoriesTable.Where(c => !c.Deleted).ToEnumerableAsync();
            var vendor = await VendorTable.Where(v => updatedEntity.VendorID == v.EntityId).Take(1).ToEnumerableAsync();

            var pos = await PurchaseOrderTable.Where(p => p.EntityId == updatedEntity.PurchaseOrderID).Take(1).ToEnumerableAsync();
            return await UpdateEntity(updatedEntity, ReceivingOrderTable, e => new Core.Client.Entities.Inventory.ReceivingOrder (updatedEntity, rests.FirstOrDefault(),
                emps.FirstOrDefault(), pos.FirstOrDefault(), vendor.FirstOrDefault(), cats));
        }

        #endregion

      
        #region IParWebService implementation

        public Task<Core.Common.Entities.Inventory.Par> GetCurrentPAR(Guid restaurantID)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Mise.Core.Common.Entities.Inventory.Par>> GetPARsForRestaurant(Guid restaurantID)
        {
            var items = await ParTable.Where(p => !p.Deleted && p.Restaurant != null 
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

        public async Task<Mise.Core.Common.Entities.Restaurant> GetRestaurant(Guid restaurantID)
        {
            try
            {
                var restDb = await RestaurantTable.Where(e => !e.Deleted && e.Id == restaurantID.ToString())
                    .Take(1)
                    .ToEnumerableAsync();
                if (restDb.Any())
                {
                    //hydrate it
                    return await HydrateRestaurant(restDb.First());
                }
                return null;
            } 
            catch(Exception e)
            {
                _logger.HandleException(e);
                throw;
            }
        }

        private async Task<Core.Common.Entities.Restaurant> HydrateRestaurant (Restaurant rest)
        {
            //get the inventory sections
            var mySections = await RestaurantInventorySectionTable
                .Where (rs => rs.RestaurantId == rest.Id).ToListAsync ();

            rest.InventorySections = mySections;

            return rest.ToBusinessEntity ();
        }

        #endregion

        #region IEventStoreWebService implementation

        public Task<bool> SendEventsAsync(Mise.Core.Common.Entities.Restaurant updatedEntity, IEnumerable<Mise.Core.Entities.Restaurant.Events.IRestaurantEvent> events)
        {
            return UpdateEntity(updatedEntity, RestaurantTable, ent => new Restaurant(ent, null));
        }

        #endregion

        #region IInventoryEmployeeWebService implementation

        public async Task<IEnumerable<Mise.Core.Common.Entities.People.Employee>> GetEmployeesAsync()
        {
            var dbEmps = await EmployeeTable.Where(e => !e.Deleted).ToEnumerableAsync();
            return dbEmps.Select(e => e.ToBusinessEntity());
        }

        public async Task<IEnumerable<Mise.Core.Common.Entities.People.Employee>> GetEmployeesForRestaurant(Guid restaurantID)
        {
            var dbEmps = await EmployeeTable.Where(e => !e.Deleted 
                && e.RestaurantsEmployedAtIds.Contains(restaurantID.ToString()))
                .ToEnumerableAsync();

            return dbEmps.Select(e => e.ToBusinessEntity());
        }

        public async Task<Mise.Core.Common.Entities.People.Employee> GetEmployeeByPrimaryEmailAndPassword(Mise.Core.ValueItems.EmailAddress email, Mise.Core.ValueItems.Password password)
        {
            var dbEmps = await EmployeeTable
                .ToEnumerableAsync();

            var emp = dbEmps.Where(e => IsValidEmployee(e, email, password)).FirstOrDefault();
            if (emp != null)
            {
                return emp.ToBusinessEntity();
            }

            return null;
        }

        private bool IsValidEmployee(Employee e, Mise.Core.ValueItems.EmailAddress email, Password pwd){
            return e.Emails.Contains(email.Value) && e.PasswordHash == pwd.HashValue;
        }

        public async Task<bool> IsEmailRegistered(Mise.Core.ValueItems.EmailAddress email)
        {
            var emps = await EmployeeTable.Where(e => e.Emails.Contains(email.Value)).ToEnumerableAsync();
            return emps.Any();
        }

        #endregion

        #region IEventStoreWebService implementation

        public async Task<bool> SendEventsAsync(Mise.Core.Common.Entities.People.Employee updatedEntity, System.Collections.Generic.IEnumerable<Mise.Core.Entities.People.Events.IEmployeeEvent> events)
        {
            var ids = updatedEntity.GetRestaurantIDs().ToList();
            var rests = await RestaurantTable.Where(r => ids.Contains(r.EntityId))
                .ToListAsync();

            return await UpdateEntity(updatedEntity, EmployeeTable, ent => new Employee(ent, rests));
        }

        #endregion
    }
}

