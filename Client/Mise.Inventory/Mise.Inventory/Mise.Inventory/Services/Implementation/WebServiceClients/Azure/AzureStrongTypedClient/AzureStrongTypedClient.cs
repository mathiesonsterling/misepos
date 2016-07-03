using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;
using Mise.Core.Client.Entities;
using Mise.Core.Client.Entities.Accounts;
using Mise.Core.Client.Entities.Categories;
using Mise.Core.Client.Entities.Inventory;
using Mise.Core.Client.Entities.People;
using Mise.Core.Client.Entities.Restaurant;
using Mise.Core.Client.Entities.Vendor;
using Mise.Core.Client.Services;
using Mise.Core.Common.Services.WebServices;
using Mise.Core.Entities.Accounts;
using Mise.Core.Entities.Base;
using Mise.Core.Entities.Inventory;
using Mise.Core.Entities.Inventory.Events;
using Mise.Core.Entities.People.Events;
using Mise.Core.Entities.Restaurant.Events;
using Mise.Core.Entities.Vendors.Events;
using Mise.Core.Services.UtilityServices;
using Mise.Core.ValueItems;
using DbInventory = Mise.Core.Client.Entities.Inventory.Inventory;
using DbInventoryLineItem = Mise.Core.Client.Entities.Inventory.LineItems.InventoryBeverageLineItem;

using DbRestaurantInventorySection = Mise.Core.Client.Entities.Inventory.RestaurantInventorySection;
using DbInventorySection = Mise.Core.Client.Entities.Inventory.InventorySection;

using DbPar = Mise.Core.Client.Entities.Inventory.Par;
using DbParLineItem = Mise.Core.Client.Entities.Inventory.LineItems.ParBeverageLineItem;
using DbVendorRestaurant = Mise.Core.Client.Entities.Vendor.VendorRestaurantRelationships;
namespace Mise.Inventory.Services.Implementation.WebServiceClients.Azure.AzureStrongTypedClient
{
    public class AzureStrongTypedClient :  IInventoryApplicationWebService
    {
        private readonly IMobileServiceClient _client;
        private readonly ILogger _logger;
        private Guid? _restaurantId;

        private IMobileServiceSyncTable<RestaurantAccount> _restaurantAccountTable;
        private IMobileServiceSyncTable<RestaurantAccount> RestaurantAccountTable =>  _restaurantAccountTable ?? (_restaurantAccountTable = _client.GetSyncTable<RestaurantAccount>());

        private IMobileServiceSyncTable<Restaurant> RestaurantTable => _restaurantTable ?? (_restaurantTable = _client.GetSyncTable<Restaurant>());
        private IMobileServiceSyncTable<Restaurant> _restaurantTable;

        private IMobileServiceSyncTable<DbRestaurantInventorySection> _restaurantInventorySectionsTable;
        private IMobileServiceSyncTable<DbRestaurantInventorySection> RestaurantInventorySectionsTable => _restaurantInventorySectionsTable ?? (_restaurantInventorySectionsTable = _client.GetSyncTable<DbRestaurantInventorySection> ());

        private IMobileServiceSyncTable<Employee> _employeeTable;
        public IMobileServiceSyncTable<Employee> EmployeeTable => _employeeTable ?? (_employeeTable = _client.GetSyncTable<Employee>());

        private IMobileServiceSyncTable<EmployeeRestaurantRelationships> _employeeRestaurantTable;
        private IMobileServiceSyncTable<EmployeeRestaurantRelationships> EmployeeRestaurantRelationshipsTable => _employeeRestaurantTable ??
                                                                                                            (_employeeRestaurantTable = _client.GetSyncTable<EmployeeRestaurantRelationships>());

        private IMobileServiceSyncTable<Vendor> _vendorTable;
        private IMobileServiceSyncTable<Vendor> VendorTable => _vendorTable ?? (_vendorTable = _client.GetSyncTable<Vendor>());
        private IMobileServiceSyncTable<DbVendorRestaurant> _vendorRestaurantRelationshipsTable;
        private IMobileServiceSyncTable<DbVendorRestaurant> VendorRestaurantRelationshipsTable => 
          _vendorRestaurantRelationshipsTable ?? (_vendorRestaurantRelationshipsTable = _client.GetSyncTable<DbVendorRestaurant> ());

        private IMobileServiceSyncTable<ReceivingOrder> _receivingOrderTable;
        private IMobileServiceSyncTable<ReceivingOrder> ReceivingOrderTable
            => _receivingOrderTable ?? (_receivingOrderTable = _client.GetSyncTable<ReceivingOrder>());

        private IMobileServiceSyncTable<PurchaseOrder> _purchaseOrderTable;

        private IMobileServiceSyncTable<PurchaseOrder> PurchaseOrderTable
            => _purchaseOrderTable ?? (_purchaseOrderTable = _client.GetSyncTable<PurchaseOrder>());

        private IMobileServiceSyncTable<ApplicationInvitation> _applicationInvitationTable;

        private IMobileServiceSyncTable<ApplicationInvitation> ApplicationInvitationTable
            =>
                _applicationInvitationTable ??
                (_applicationInvitationTable = _client.GetSyncTable<ApplicationInvitation>());

        private IMobileServiceSyncTable<DbPar> _parTable;
        private IMobileServiceSyncTable<DbPar> ParTable => _parTable ?? (_parTable = _client.GetSyncTable<Par>());
        private IMobileServiceSyncTable<DbParLineItem> _parBeverageLineItemTable;
        private IMobileServiceSyncTable<DbParLineItem> ParBeverageLineItemTable => _parBeverageLineItemTable ?? (_parBeverageLineItemTable = _client.GetSyncTable<DbParLineItem> ());

        private IMobileServiceSyncTable<DbInventory> _inventoryTable;

        private IMobileServiceSyncTable<DbInventory> InventoryTable
            => _inventoryTable ?? (_inventoryTable = _client.GetSyncTable<DbInventory>());

        private IMobileServiceSyncTable<DbInventorySection> _inventorySectionTable;
        private IMobileServiceSyncTable<DbInventorySection> InventorySectionTable => _inventorySectionTable ?? (_inventorySectionTable = _client.GetSyncTable<DbInventorySection> ());

        private IMobileServiceSyncTable<DbInventoryLineItem> _inventoryBeverageLineItemsTable;
        private IMobileServiceSyncTable<DbInventoryLineItem> InventoryBeverageLineItemsTable =>
            _inventoryBeverageLineItemsTable ?? (_inventoryBeverageLineItemsTable = _client.GetSyncTable<DbInventoryLineItem> ());


        private IMobileServiceSyncTable<InventoryCategory> _categoriesTable;

        private IMobileServiceSyncTable<InventoryCategory> CategoriesTable
            => _categoriesTable ?? (_categoriesTable = _client.GetSyncTable<InventoryCategory>());

        public bool Synched{ get; private set; }
        public static MobileServiceSQLiteStore DefineTables(MobileServiceSQLiteStore store)
        {
            store.DefineTable<RestaurantAccount>();

            store.DefineTable<Restaurant>();
            store.DefineTable<DbRestaurantInventorySection>();

            store.DefineTable<Employee>();
            store.DefineTable<Vendor>();
            store.DefineTable<ReceivingOrder>();
            store.DefineTable<PurchaseOrder>();
            store.DefineTable<ApplicationInvitation>();

            store.DefineTable<DbPar>();
            store.DefineTable<DbParLineItem>();

            store.DefineTable<InventoryCategory> ();

            store.DefineTable<DbInventory>();
            store.DefineTable<DbInventorySection>();
            store.DefineTable<DbInventoryLineItem>();
            return store;
        }

        public AzureStrongTypedClient(ILogger logger, IMobileServiceClient client, IDeviceConnectionService connService)
        {
            _logger = logger;
            _client = client;
            connService.ConnectionStateChanged += async (sender, args) => {
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
                        SyncTable("RestaurantAccounts", RestaurantAccountTable),
                        SyncTable("Restaurants", RestaurantTable),
                        SyncTable("RestaurantInventorySections", RestaurantInventorySectionsTable),

                        SyncTable("Employees",EmployeeTable),
                        SyncTable("Vendors", VendorTable),
                        SyncTable("ReceivingOrders", ReceivingOrderTable),
                        SyncTable("PurchaseOrders", PurchaseOrderTable),
                        SyncTable("ApplicationInvitations", ApplicationInvitationTable),
                        SyncTable("ParsFor", _parTable),
                        SyncTable("Inventories", InventoryTable),
                        SyncTable("InventoryCategories", CategoriesTable),
                        SyncTable("InventorySections", InventorySectionTable),
                        //SyncTable("InventoryBeverageLineItems", _inventoryBeverageLineItemsTable)
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

        public async Task<IAccount> GetAccountById(Guid id)
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

        public async Task<IAccount> GetAccountFromEmail(EmailAddress email)
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

        private async Task<bool> UpdateEntity<TEnt, TDb> (TEnt entity, IMobileServiceSyncTable<TDb> table, TDb dbEnt)
            where TDb : EntityData
            where TEnt : IEntityBase
        {
            try {
                var existsTask = table
                    .Where (a => a.Id == entity.Id.ToString ())
                    .Take (1)
                    .ToEnumerableAsync ();

                var busItem = dbEnt;
                var exists = await existsTask.ConfigureAwait (false);
                if (exists.Any ()) {
                    try {
                        await table.UpdateAsync (busItem).ConfigureAwait (false);
                    } catch (Exception e) {
                        _logger.HandleException (e);
                        throw;
                    }
                } else {
                    try {
                        await table.InsertAsync (busItem).ConfigureAwait (false);
                    } catch (Exception e) {
                        _logger.HandleException (e);
                        throw;
                    }
                }

                return true;
            } catch (Exception e) {
                _logger.HandleException (e);
                throw;
            }
        }

        private Task<bool> UpdateEntity<TEnt, TDb>(TEnt entity, IMobileServiceSyncTable<TDb> table, Func<TEnt, TDb> createDb) 
            where TDb : EntityData
            where TEnt : IEntityBase
        {
            return UpdateEntity (entity, table, createDb (entity));
        }

        public Task<bool> SendEventsAsync(Core.Common.Entities.Accounts.RestaurantAccount updatedEntity, IEnumerable<IAccountEvent> events)
        {
            return UpdateEntity(updatedEntity, _restaurantAccountTable, ent => new RestaurantAccount(ent));
        }

        #endregion

        #region IApplicationInvitationWebService implementation

        public async Task<IEnumerable<Core.Common.Entities.ApplicationInvitation>> GetInvitationsForRestaurant(Guid restaurantID)
        {
            var items = await ApplicationInvitationTable
                //.Where(ai => ai.RestaurantId == restaurantID.ToString ())
                .ToEnumerableAsync();


            var busItems = items.Where(a => a!= null && a.RestaurantId == restaurantID.ToString())
                                .Select(i => i.ToBusinessEntity());
            return busItems;
        }

        public async Task<IEnumerable<Core.Common.Entities.ApplicationInvitation>> GetInvitationsForEmail(EmailAddress email)
        {
            var items = await ApplicationInvitationTable
                .ToEnumerableAsync();

            var aiItems = items.Where (ai => ai != null
                     && !ai.Deleted
                     && ai.DestinationEmployee.Emails.Contains (email.Value)
                                       || ai.InvitingEmployee.Emails.Contains (email.Value));
            var busItems = aiItems.Select(i => i.ToBusinessEntity());
            return busItems;
        }

        #endregion

        #region IEventStoreWebService implementation

        public async Task<bool> SendEventsAsync(Core.Common.Entities.ApplicationInvitation updatedEntity, IEnumerable<IApplicationInvitationEvent> events)
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
            return await UpdateEntity(updatedEntity, _applicationInvitationTable, 
                ent => new ApplicationInvitation(ent, miseApp, destEmps.FirstOrDefault(), invitEmps.FirstOrDefault(), rest.FirstOrDefault()));
        }


        public async Task<IEnumerable<Core.Common.Entities.Inventory.Inventory>> GetInventoriesForRestaurant(Guid restaurantID)
        {
            var invs = await InventoryTable.Where(i => i != null && !i.Deleted && i.Restaurant != null
                            && i.Restaurant.EntityId == restaurantID)
                .Select(i => i.ToBusinessEntity())
                .ToListAsync();

            foreach (var inv in invs) {
                var sections = await InventorySectionTable.Where (s => s.InventoryId == inv.Id.ToString ()).ToListAsync ();

                var tasks = sections.Select (s => LoadLineItemsInSection (s));

                Task.WaitAll (tasks.ToArray ());

                inv.Sections = sections.Select (s => s.ToBusinessEntity ()).ToList ();
            }

            return invs;
        }

        public async Task LoadLineItemsInSection (DbInventorySection section)
        {
            var lis = await InventoryBeverageLineItemsTable.Where (li => li.InventorySectionId == section.Id)
                                                           .ToListAsync ()
                                                           .ConfigureAwait (false);
            section.LineItems = lis;
        }

        public async Task<bool> SendEventsAsync (Core.Common.Entities.Inventory.Inventory updatedEntity,
            IEnumerable<IInventoryEvent> events)
        {
            var rests = await RestaurantTable.Where (r => r.EntityId == updatedEntity.RestaurantID)
                                              .Take (1).ToListAsync ();
            var rest = rests.First ();

            var cats = await CategoriesTable.Where (c => !c.Deleted).ToEnumerableAsync ();

            var vendors = await VendorTable.Where (v => !v.Deleted).ToEnumerableAsync ();

            var emps = await EmployeeTable.Where (e => e.EntityId == updatedEntity.CreatedByEmployeeID).ToEnumerableAsync ();

            var dbInventory = new DbInventory (updatedEntity, rest, emps.ToList ());
            foreach (var sec in updatedEntity.GetSections ()) {
                await SendInventorySection (sec, dbInventory, emps, cats, vendors).ConfigureAwait (false);
            }

            updatedEntity.Sections = null;

            //update the sections
            return await UpdateEntity (updatedEntity, InventoryTable, dbInventory);
        }

        private async Task<bool> SendInventorySection (IInventorySection sec, DbInventory inventory, 
                                                 IEnumerable<Employee> emps, IEnumerable<InventoryCategory> cats,
                                                      IEnumerable<Vendor> vendors)
        {
            var restSec = await RestaurantInventorySectionsTable
                .LookupAsync (sec.RestaurantInventorySectionID.ToString ());

            var completedBy = sec.LastCompletedBy.HasValue
                                 ? await EmployeeTable.LookupAsync (sec.LastCompletedBy.ToString ())
                                 : null;
            var currentlyInUseBy = sec.CurrentlyInUseBy.HasValue
                                      ? await EmployeeTable.LookupAsync (sec.CurrentlyInUseBy.ToString ())
                                      : null;

            var dbSection = new DbInventorySection (sec, inventory, completedBy, restSec,
                                                   currentlyInUseBy, vendors, cats);

            //do the line items
            foreach (var li in sec.GetInventoryBeverageLineItemsInSection ()) {
                var dbLi = new DbInventoryLineItem (li, dbSection, vendors, cats);

                await UpdateEntity (li, InventoryBeverageLineItemsTable, dbLi).ConfigureAwait (false);
            }
            return await UpdateEntity (sec, InventorySectionTable, dbSection);
        }

        #endregion

        #region IPurchaseOrderWebService implementation

        public async Task<IEnumerable<IPurchaseOrder>> GetPurchaseOrdersForRestaurant(Guid restaurantId)
        {
            var res = await _purchaseOrderTable
                .Where(po => po.Restaurant != null && po.Restaurant.RestaurantID == restaurantId)
                .Take(1)
                .ToEnumerableAsync();

            return res.Select(p => p.ToBusinessEntity());
        }

        #endregion

        #region IEventStoreWebService implementation

        public async Task<bool> SendEventsAsync(Core.Common.Entities.Inventory.PurchaseOrder updatedEntity, IEnumerable<IPurchaseOrderEvent> events)
        {
            var cats = await CategoriesTable.Where(c => !c.Deleted).ToEnumerableAsync();
            var vendors = await VendorTable.Where(v => !v.Deleted).ToEnumerableAsync();
            var rest = await RestaurantTable.Where(r => !r.Deleted && r.EntityId == updatedEntity.RestaurantID).Take(1).ToEnumerableAsync();
            var emp = await EmployeeTable.Where(e => !e.Deleted && e.EntityId == updatedEntity.CreatedByEmployeeID).Take(1).ToEnumerableAsync();
            return await UpdateEntity(updatedEntity, _purchaseOrderTable, 
                e => new PurchaseOrder(updatedEntity, cats, vendors, 
                    rest.FirstOrDefault(), emp.FirstOrDefault()));
        }

        #endregion

        #region IReceivingOrderWebService implementation

        public Task<IEnumerable<Core.Common.Entities.Inventory.ReceivingOrder>> GetReceivingOrdersForRestaurant(Guid restaurantID)
        {
            return _receivingOrderTable.Where(r => r.Restaurant != null && r.Restaurant.RestaurantID == restaurantID)
                .Select(r => r.ToBusinessEntity())
                .ToEnumerableAsync();
        }

        #endregion

        #region IEventStoreWebService implementation

        public async Task<bool> SendEventsAsync(Core.Common.Entities.Inventory.ReceivingOrder updatedEntity, IEnumerable<IReceivingOrderEvent> events)
        {
            var rests = await RestaurantTable.Where(r => updatedEntity.RestaurantID == r.EntityId)
                .Take(1).ToEnumerableAsync();
            var emps = await EmployeeTable.Where(r => updatedEntity.ReceivedByEmployeeID == r.EntityId).Take(1).ToEnumerableAsync();
            var cats = await _categoriesTable.Where(c => !c.Deleted).ToEnumerableAsync();
            var vendor = await _vendorTable.Where(v => updatedEntity.VendorID == v.EntityId).Take(1).ToEnumerableAsync();

            var pos = await _purchaseOrderTable.Where(p => p.EntityId == updatedEntity.PurchaseOrderID).Take(1).ToEnumerableAsync();
            return await UpdateEntity(updatedEntity, _receivingOrderTable, e => new ReceivingOrder(updatedEntity, rests.FirstOrDefault(),
                emps.FirstOrDefault(), pos.FirstOrDefault(), vendor.FirstOrDefault(), cats));
        }

        #endregion

      
        #region IParWebService implementation

        public async Task<Core.Common.Entities.Inventory.Par> GetCurrentPAR(Guid restaurantID)
        {
            var pars = await GetPARsForRestaurant (restaurantID);

            return pars
                .Where (p => p.IsCurrent)
                .Where(p => p.GetBeverageLineItems ().Count () > 0)
                .OrderByDescending (p => p.LastUpdatedDate)
                .FirstOrDefault ();
        }

        public async Task<IEnumerable<Core.Common.Entities.Inventory.Par>> GetPARsForRestaurant(Guid restaurantID)
        {
            try {
                var items = await ParTable.Where (p => !p.Deleted
                                                     && p.RestaurantId == restaurantID.ToString ())
                    .ToListAsync ();

                var rest = await RestaurantTable.LookupAsync (restaurantID.ToString ());
                foreach (var item in items) {
                    var lis = await ParBeverageLineItemTable.Where (pli => pli.ParId == item.EntityId.ToString ()).ToListAsync ();

                    item.ParLineItems = lis;

                    var emp = await EmployeeTable.LookupAsync (item.CreatedByEmployeeId);
                    item.CreatedByEmployee = emp;

                    item.Restaurant = rest;
                }
                return items.Select (p => p.ToBusinessEntity ());
            } catch (Exception e) {
                _logger.HandleException (e);
                throw;
            }
        }

        #endregion

        #region IEventStoreWebService implementation

        public async Task<bool> SendEventsAsync(Core.Common.Entities.Inventory.Par updatedEntity, IEnumerable<IParEvent> events)
        {
            var categories = await CategoriesTable.ToEnumerableAsync ();

            var rest = await RestaurantTable.LookupAsync (updatedEntity.RestaurantID.ToString ());
            var emp = await EmployeeTable.LookupAsync (updatedEntity.CreatedByEmployeeID.ToString ());
            var dbEnt = new Par (updatedEntity, rest, emp);

            foreach (var li in updatedEntity.ParLineItems) {
                var dbLI = new DbParLineItem (li, dbEnt, categories);
                await UpdateEntity (li, ParBeverageLineItemTable, dbLI).ConfigureAwait (false);
            }

            return await UpdateEntity (updatedEntity, ParTable, dbEnt);
        }

        #endregion

        #region IVendorWebService implementation

        public Task<IEnumerable<Core.Common.Entities.Vendors.Vendor>> GetVendorsWithinSearchRadius(Location currentLocation, Distance radius)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Core.Common.Entities.Vendors.Vendor>> GetVendorsAssociatedWithRestaurant(Guid restaurantID)
        {
            var vendorRests = await VendorRestaurantRelationshipsTable.Where (rv => rv.RestaurantId == restaurantID.ToString ()).ToEnumerableAsync ();

            var vendorIds = vendorRests.Select (v => v.VendorId).ToList ();

            var vendors = await VendorTable.Where (v => vendorIds.Contains (v.Id)).ToEnumerableAsync ();

            return vendors.Select (v => v.ToBusinessEntity ());
        }

        #endregion

        #region IEventStoreWebService implementation

        public Task<bool> SendEventsAsync(Core.Common.Entities.Vendors.Vendor updatedEntity, IEnumerable<IVendorEvent> events)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IInventoryRestaurantWebService implementation

        public async Task<IEnumerable<Core.Common.Entities.Restaurant>> GetRestaurants(Location deviceLocation, Distance maxDistance)
        {
            var rests = await RestaurantTable.ToEnumerableAsync ();
            return rests.Select (r => r.ToBusinessEntity ());
        }

        public async Task<Core.Common.Entities.Restaurant> GetRestaurant(Guid restaurantID)
        {
            try
            {
                var restDb = await RestaurantTable.Where(e => !e.Deleted && e.Id == restaurantID.ToString())
                    .Take(1)
                    .ToEnumerableAsync();
                if (restDb.Any())
                {
                    var sections = await RestaurantInventorySectionsTable
                        .Where (rs => rs.RestaurantId == restaurantID.ToString ())
                        .ToListAsync ();
                    var rest = restDb.First();
                    rest.InventorySections = sections;

                    return rest.ToBusinessEntity ();
                }
                return null;
            } 
            catch(Exception e)
            {
                _logger.HandleException(e);
                throw;
            }
        }

        #endregion

        #region IEventStoreWebService implementation

        public Task<bool> SendEventsAsync(Core.Common.Entities.Restaurant updatedEntity, IEnumerable<IRestaurantEvent> events)
        {
            var sections = updatedEntity.InventorySections.Select (s => new RestaurantInventorySection (s));
            return UpdateEntity(updatedEntity, RestaurantTable, ent => new Restaurant(ent, null, sections));
        }

        #endregion

        #region IInventoryEmployeeWebService implementation

        public async Task<IEnumerable<Core.Common.Entities.People.Employee>> GetEmployeesAsync()
        {
            var dbEmps = await EmployeeTable.Where(e => !e.Deleted).ToEnumerableAsync();
            return dbEmps.Select(e => e.ToBusinessEntity());
        }

        public async Task<IEnumerable<Core.Common.Entities.People.Employee>> GetEmployeesForRestaurant(Guid restaurantID)
        {
            var dbEmps = await EmployeeTable.Where(e => !e.Deleted 
                && e.RestaurantsEmployedAtIds.Contains(restaurantID.ToString()))
                .ToEnumerableAsync();

            return dbEmps.Select(e => e.ToBusinessEntity());
        }

        public async Task<Core.Common.Entities.People.Employee> GetEmployeeByPrimaryEmailAndPassword(EmailAddress email, Password password)
        {
            var dbEmps = await EmployeeTable
                .Where(e => e.PrimaryEmail.Contains (email.Value))
                .ToEnumerableAsync();

            var emp = dbEmps.Where(e => IsValidEmployee(e, email, password)).FirstOrDefault();
            if (emp != null)
            {
                return emp.ToBusinessEntity();
            }

            return null;
        }

        private bool IsValidEmployee(Employee e, EmailAddress email, Password pwd){
            return e.Emails.Contains(email.Value) && e.PasswordHash == pwd.HashValue;
        }

        public async Task<bool> IsEmailRegistered(EmailAddress email)
        {
            var emps = await EmployeeTable.Where(e => e.Emails.Contains(email.Value)).ToEnumerableAsync();
            return emps.Any();
        }

        #endregion

        #region IEventStoreWebService implementation

        public async Task<bool> SendEventsAsync(Core.Common.Entities.People.Employee updatedEntity, IEnumerable<IEmployeeEvent> events)
        {
            var ids = updatedEntity.GetRestaurantIDs().ToList();
            var rests = await RestaurantTable.Where(r => ids.Contains(r.EntityId))
                .ToListAsync();

            return await UpdateEntity(updatedEntity, EmployeeTable, ent => new Employee(ent, rests));
        }

        #endregion
    }
}

