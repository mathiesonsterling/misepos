using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Mise.Core.Common.Entities;
using Mise.Core.Common.Entities.Accounts;
using Mise.Core.Common.Entities.DTOs;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Common.Entities.Vendors;
using Mise.Core.Common.Services.Implementation.Serialization;
using Mise.Core.Entities.Accounts;
using Mise.Core.Entities.Base;
using Mise.Core.Entities.Inventory;
using Mise.Core.Entities.People;
using Mise.Core.Entities.Restaurant;
using Mise.Core.Entities.Vendors;
using Mise.Core.ValueItems;
using MiseWebsite.Models;

namespace MiseWebsite.Database.Implementation
{
    public class ManagementDAL : IManagementDAL
    {
        private const string RESTAURANT_ID_LIST_KEY = "allCachedRestaurantIds";

        private readonly EntityDataTransportObjectFactory _entityFactory;
        private readonly string _invType;
        private readonly string _empType;
        private readonly string _restType;
        private readonly string _roType;
        private readonly string _poType;
        private readonly string _parType;
        private readonly string _vendorType;
        public ManagementDAL()
        {
            _entityFactory = new EntityDataTransportObjectFactory(new JsonNetSerializer());
            _invType = typeof(Inventory).ToString();
            _empType = typeof(Employee).ToString();
            _restType = typeof(Restaurant).ToString();
            _roType = typeof(ReceivingOrder).ToString();
            _poType = typeof(PurchaseOrder).ToString();
            _parType = typeof(Par).ToString();
            _vendorType = typeof(Vendor).ToString();
        }

        private static IList<Guid> CachedRestaurantList
        {
            get { return HttpRuntime.Cache.Get(RESTAURANT_ID_LIST_KEY) as IList<Guid>; }
            set
            {
                var existing = HttpRuntime.Cache.Get(RESTAURANT_ID_LIST_KEY);
                if (existing != null)
                {
                    HttpRuntime.Cache.Remove(RESTAURANT_ID_LIST_KEY);
                }
                HttpRuntime.Cache.Insert(RESTAURANT_ID_LIST_KEY, value);
            }
        }

        private static class SubItemCache
        {
            private const string RESTAURANT_INVENTORY_LIST_KEY = "inventoriesByRestaurant";

            public static void Upsert(IEntityBase inv)
            {
                var existing = HttpRuntime.Cache.Get(inv.Id.ToString());
                if (existing != null)
                {
                    HttpRuntime.Cache.Remove(inv.Id.ToString());
                }
                HttpRuntime.Cache.Insert(inv.Id.ToString(), inv);
            }

            public static T GetById<T>(Guid inventoryId) where T : class, IEntityBase
            {
                return HttpRuntime.Cache.Get(inventoryId.ToString()) as T;
            }

            public static IEnumerable<T> GetForRestaurant<T>(Guid restaurantId) where T : IEntityBase
            {
                var key = GetKeyForRestaurant(restaurantId, typeof(T));
                return HttpRuntime.Cache.Get(key) as IEnumerable<T>;
            }

            public static void UpsertForRestaurant<T>(Guid restaurantId, IEnumerable<T> inventories) where T : IEntityBase
            {
                var key = GetKeyForRestaurant(restaurantId, typeof(T));
                var existing = HttpRuntime.Cache.Get(key);
                if (existing != null)
                {
                    HttpRuntime.Cache.Remove(key);
                }
                HttpRuntime.Cache.Insert(key, inventories);
                foreach (var inv in inventories)
                {
                    Upsert(inv);
                }
            }

            private static string GetKeyForRestaurant(Guid restaurantId, Type itemType)
            {
                return RESTAURANT_INVENTORY_LIST_KEY + ":" + itemType + "::" + restaurantId;
            }
        }

        public async Task<IEnumerable<IInventory>> GetInventoriesForRestaurant(Guid restaurantId)
        {
            var cached = SubItemCache.GetForRestaurant<IInventory>(restaurantId);
            if (cached != null)
            {
                return cached;
            }

            List<AzureEntityStorage> ais;
            using (var db = new AzureNonTypedEntitiesDbContext())
            {
                ais = await db.AzureEntityStorages.Where(a => a.MiseEntityType == _invType && a.RestaurantID == restaurantId).ToListAsync();
            }
            var dtos = ais.Select(ai => ai.ToRestaurantDTO());
            var invs = dtos.Select(dto => _entityFactory.FromDataStorageObject<Inventory>(dto)).ToList();

            SubItemCache.UpsertForRestaurant(restaurantId, invs.Cast<IInventory>());
            return invs;
        }

        public async Task<IEnumerable<IInventory>> GetInventoriesCompletedAfter(DateTimeOffset date)
        {
            List<AzureEntityStorage> ais;
            using (var db = new AzureNonTypedEntitiesDbContext())
            {
                ais = await db.AzureEntityStorages
                    .Where(ai => ai.LastUpdatedDate > date)
                    .Where(a => a.MiseEntityType == _invType)
                    .ToListAsync();
            }

            var dtos = ais.Select(ai => ai.ToRestaurantDTO());
            var invs = dtos.Select(dto => _entityFactory.FromDataStorageObject<Inventory>(dto)).ToList();

            return invs.Where(inv => inv.DateCompleted.HasValue && inv.DateCompleted.Value > date);
        }

        public async Task<IInventory> GetInventoryById(Guid invId)
        {
            var cached = SubItemCache.GetById<IInventory>(invId);
            if (cached != null)
            {
                return cached;
            }

            using (var db = new AzureNonTypedEntitiesDbContext())
            {
                var ai = await db.AzureEntityStorages.Where(a => a.MiseEntityType == _invType && a.EntityID == invId)
                            .FirstOrDefaultAsync();
                var dto = ai.ToRestaurantDTO();
                var inv = _entityFactory.FromDataStorageObject<Inventory>(dto);
                if (inv != null)
                {
                    SubItemCache.Upsert(inv);
                }
                return inv;
            }
        }


        public async Task<IInventoryBeverageLineItem> GetInventoryLineItem(Guid inventoryId, Guid lineItemId)
        {
            var inv = await GetInventoryById(inventoryId);
            var lineItem = inv.GetBeverageLineItems().FirstOrDefault(li => li.Id == lineItemId);
            if (lineItem == null)
            {
                throw new ArgumentException("Error, cannot find line item " + lineItemId);
            }
            return lineItem;
        }

        public async Task UpdateInventoryLineItem(Guid inventoryId, IInventoryBeverageLineItem li)
        {
            var newLineItem = li as InventoryBeverageLineItem;
            var inv = (await GetInventoryById(inventoryId)) as Inventory;
            if (inv == null)
            {
                throw new InvalidOperationException("Can't downgrade inventory");
            }
            var sec = inv.Sections.FirstOrDefault(s => s.LineItems.Select(l => l.Id).Contains(li.Id));
            if (sec == null)
            {
                throw new InvalidOperationException("Cannot find section with item " + li.Id);
            }
            var oldLineItem = sec.LineItems.FirstOrDefault(l => l.Id == li.Id);

            sec.LineItems.Remove(oldLineItem);
            sec.LineItems.Add(newLineItem);

            await UpdateInventory(inv);
        }

        public async Task UpdateInventory(IInventory inv)
        {
            var inventory = inv as Inventory;
            if (inventory == null)
            {
                throw new ArgumentException("Type given cannot be converted to concrete");
            }

            using (var db = new AzureNonTypedEntitiesDbContext())
            {
                var oldVer = await db.AzureEntityStorages.FirstOrDefaultAsync(
                        ai => ai.EntityID == inventory.Id && ai.MiseEntityType == _invType);
                if (oldVer == null)
                {
                    throw new InvalidOperationException("Cannot find inventory to update in DB");
                }
                var newVerDTO = new AzureEntityStorage(_entityFactory.ToDataTransportObject(inventory));
                oldVer.EntityJSON = newVerDTO.EntityJSON;
                oldVer.LastUpdatedDate = newVerDTO.LastUpdatedDate;
                db.Entry(oldVer).State = EntityState.Modified;
                await db.SaveChangesAsync();
            }

            SubItemCache.Upsert(inventory);
        }

        public async Task UpdateRestaurant(IRestaurant rest)
        {
            var restaurant = rest as Restaurant;
            if (restaurant == null)
            {
                throw new ArgumentException("Type given cannot be converted to concrete");
            }

            using (var db = new AzureNonTypedEntitiesDbContext())
            {
                var oldVer = await db.AzureEntityStorages.FirstOrDefaultAsync(
                        ai => ai.EntityID == restaurant.Id && ai.MiseEntityType == _restType);
                if (oldVer == null)
                {
                    throw new InvalidOperationException("Cannot find restaurant to update in DB");
                }
                var newVerDTO = new AzureEntityStorage(_entityFactory.ToDataTransportObject(restaurant));
                oldVer.EntityJSON = newVerDTO.EntityJSON;
                oldVer.LastUpdatedDate = newVerDTO.LastUpdatedDate;
                db.Entry(oldVer).State = EntityState.Modified;
                await db.SaveChangesAsync();

                HttpRuntime.Cache.Remove(rest.Id.ToString());
                HttpRuntime.Cache.Insert(rest.Id.ToString(), restaurant);
            }
        }

        public async Task<IEnumerable<IRestaurant>> GetAllRestaurants()
        {
            /*
            if (CachedRestaurantList != null && CachedRestaurantList.Any())
            {
                var cachedItems = CachedRestaurantList
                    .Select(id => HttpRuntime.Cache.Get(id.ToString()))
                    .Select(r => r as IRestaurant);
                return cachedItems;
            }*/

            IEnumerable<AzureEntityStorage> ais;
            using (var db = new AzureNonTypedEntitiesDbContext())
            {
                ais = await db.AzureEntityStorages.Where(ai => ai.MiseEntityType == _restType).ToListAsync();
            }
            var dtos = ais.Select(ai => ai.ToRestaurantDTO());
            var rests = dtos.Select(dto => _entityFactory.FromDataStorageObject<Restaurant>(dto)).ToList();

            foreach (var rest in rests)
            {
                HttpRuntime.Cache.Insert(rest.Id.ToString(), rest);
            }
            CachedRestaurantList = rests.Select(r => r.Id).ToList();

            return rests;
        }

        public async Task<IRestaurant> GetRestaurantById(Guid restaurantId)
        {
            var cached = HttpRuntime.Cache.Get(restaurantId.ToString()) as IRestaurant;
            if (cached != null)
            {
                return cached;
            }

            AzureEntityStorage ai;
            using (var db = new AzureNonTypedEntitiesDbContext())
            {
                ai = await db.AzureEntityStorages.FirstOrDefaultAsync(
                            a => a.MiseEntityType == _restType && a.EntityID == restaurantId);
            }
            if (ai == null)
            {
                return null;
            }
            var rest = _entityFactory.FromDataStorageObject<Restaurant>(ai.ToRestaurantDTO());
            HttpRuntime.Cache.Insert(rest.Id.ToString(), rest);
            return rest;
        }

        public async Task<IEnumerable<IRestaurant>> GetRestaurantsUnderAccont(IAccount acct)
        {
            using (var db = new AzureNonTypedEntitiesDbContext())
            {
                var ais = await db.AzureEntityStorages.Where(
                            a => a.MiseEntityType == _restType && a.MiseEntityType.Contains(acct.Id.ToString())).ToListAsync();

                var dtos = ais.Select(ai => ai.ToRestaurantDTO());
                var rests = dtos.Select(dto => _entityFactory.FromDataStorageObject<Restaurant>(dto));
                return rests.Where(r => r.AccountID.HasValue && r.AccountID.Value == acct.Id);
            }
        }

        public async Task InsertRestaurant(Restaurant rest)
        {
            var dto = _entityFactory.ToDataTransportObject(rest);
            var ai = new AzureEntityStorage(dto);

            using (var db = new AzureNonTypedEntitiesDbContext())
            {
                db.AzureEntityStorages.Add(ai);
                await db.SaveChangesAsync();
            }

            HttpRuntime.Cache.Insert(rest.Id.ToString(), rest);
            var list = CachedRestaurantList ?? new List<Guid>();
            list.Add(rest.Id);
            CachedRestaurantList = list;
        }

        public async Task DeleteRestaurant(Guid id)
        {
            using (var db = new AzureNonTypedEntitiesDbContext())
            {//delete all items for the restaurant as well
                var items = await db.AzureEntityStorages.Where(ai => ai.RestaurantID == id).ToListAsync();
                foreach (var item in items)
                {
                    item.Deleted = true;
                    db.Entry(item).State = EntityState.Modified;
                    db.AzureEntityStorages.Remove(item);
                }

                CachedRestaurantList?.Remove(id);
                HttpRuntime.Cache.Remove(id.ToString());

                //also get all employees that are listed here and remove the call for them
                var emps = (await GetAllEmployeesContaining(id.ToString())).Cast<Employee>();
                foreach (var emp in emps.Where(emp => emp.RestaurantsAndAppsAllowed.ContainsKey(id)))
                {
                    emp.RestaurantsAndAppsAllowed.Remove(id);
                }

                await db.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<IEmployee>> GetAllEmployees()
        {
            using (var db = new AzureNonTypedEntitiesDbContext())
            {
                var ais = await db.AzureEntityStorages.Where(ai => ai.MiseEntityType == _empType && (ai.Deleted == false)).ToListAsync();
                var dtos = ais.Select(ai => ai.ToRestaurantDTO());
                return dtos.Select(dto => _entityFactory.FromDataStorageObject<Employee>(dto));
            }
        }
        public async Task<IEmployee> GetEmployeeById(Guid id)
        {
            AzureEntityStorage ai;
            using (var db = new AzureNonTypedEntitiesDbContext())
            {
                ai =
                    await
                        db.AzureEntityStorages.FirstOrDefaultAsync(a => a.MiseEntityType == _empType && a.EntityID == id);
            }
            return _entityFactory.FromDataStorageObject<Employee>(ai.ToRestaurantDTO());
        }

        public async Task<IEmployee> GetEmployeeWhoCreatedInventory(IInventory inv)
        {
            AzureEntityStorage ai;
            using (var db = new AzureNonTypedEntitiesDbContext())
            {
                ai = await db.AzureEntityStorages.FirstOrDefaultAsync(
                            a => a.MiseEntityType == _empType && a.EntityID == inv.CreatedByEmployeeID);
            }

            if (ai == null)
            {
                return null;
            }

            var dto = ai.ToRestaurantDTO();
            return _entityFactory.FromDataStorageObject<Employee>(dto);
        }

        public async Task<IEnumerable<IEmployee>> GetAllEmployeesContaining(string search)
        {
            using (var db = new AzureNonTypedEntitiesDbContext())
            {
                var empAIs = await db.AzureEntityStorages.Where(
                            ai => ai.MiseEntityType == _empType && ai.EntityJSON.Contains(search)).ToListAsync();
                var dtos = empAIs.Select(ai => ai.ToRestaurantDTO());
                var emps = dtos.Select(dto => _entityFactory.FromDataStorageObject<Employee>(dto));
                return emps;
            }
        }

        public async Task<IEmployee> GetEmployeeWithEmail(EmailAddress email)
        {
            using (var db = new AzureNonTypedEntitiesDbContext())
            {
                var empAIs = await db.AzureEntityStorages.Where(
                    ai => ai.MiseEntityType == _empType && ai.EntityJSON.Contains(email.Value)).ToListAsync();

                var dtos = empAIs.Select(ai => ai.ToRestaurantDTO());
                var emps = dtos.Select(dto => _entityFactory.FromDataStorageObject<Employee>(dto));
                return emps.FirstOrDefault(emp => (emp.PrimaryEmail != null && emp.PrimaryEmail.Equals(email))
                                                  || emp.GetEmailAddresses().Any(em => em.Equals(email))
                    );
            }
        }

        public async Task<IEmployee> GetEmployeeWithEmailAndPassword(EmailAddress email, string password)
        {
            var emp = await GetEmployeeWithEmail(email);

            if (emp?.Password == null)
            {
                return null;
            }
            if (emp.Password.HashValue == password)
            {
                return emp;
            }
            var asPlain = new Password(password);

            return emp.Password.Equals(asPlain) ? emp : null;
        }

        public async Task CreateEmployee(Employee emp)
        {
            var dto = _entityFactory.ToDataTransportObject(emp);
            var ai = new AzureEntityStorage(dto);

            using (var db = new AzureNonTypedEntitiesDbContext())
            {
                db.AzureEntityStorages.Add(ai);
                await db.SaveChangesAsync();
            }
        }

        public async Task UpdateEmployee(Employee emp)
        {
            using (var db = new AzureNonTypedEntitiesDbContext())
            {
                //get the original to get the restaurants for now
                var orign = db.AzureEntityStorages.FirstOrDefault(a => a.EntityID == emp.Id);
                if (orign == null)
                {
                    throw new ArgumentException("No employee found for id " + emp.Id);
                }
                db.AzureEntityStorages.Remove(orign);

                var dto = _entityFactory.ToDataTransportObject(emp);
                var ai = new AzureEntityStorage(dto);
                db.AzureEntityStorages.Add(ai);
                await db.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<IReceivingOrder>> GetReceivingOrdersForRestaurant(Guid restaurantId)
        {
            var cached = SubItemCache.GetForRestaurant<IReceivingOrder>(restaurantId);
            if (cached != null)
            {
                return cached;
            }
            using (var db = new AzureNonTypedEntitiesDbContext())
            {
                var ais = await
                    db.AzureEntityStorages.Where(ai => ai.MiseEntityType == _roType && ai.RestaurantID == restaurantId)
                        .ToListAsync();
                var dtos = ais.Select(ai => ai.ToRestaurantDTO());
                var ros = dtos.Select(dto => _entityFactory.FromDataStorageObject<ReceivingOrder>(dto));

                SubItemCache.UpsertForRestaurant(restaurantId, ros.Cast<IReceivingOrder>());
                return ros;
            }
        }

        public async Task<IReceivingOrder> GetReceivingOrderById(Guid id)
        {
            var cached = SubItemCache.GetById<IReceivingOrder>(id);
            if (cached != null)
            {
                return cached;
            }
            AzureEntityStorage ai;
            using (var db = new AzureNonTypedEntitiesDbContext())
            {
                ai = await db.AzureEntityStorages.FirstOrDefaultAsync(a => a.MiseEntityType == _roType && a.EntityID == id);
            }
            var ro = _entityFactory.FromDataStorageObject<ReceivingOrder>(ai.ToRestaurantDTO());
            SubItemCache.Upsert(ro);
            return ro;
        }

        public async Task<IPurchaseOrder> GetPurchaseOrderById(Guid id)
        {
            var cached = SubItemCache.GetById<IPurchaseOrder>(id);
            if (cached != null)
            {
                return cached;
            }
            AzureEntityStorage ai;
            using (var db = new AzureNonTypedEntitiesDbContext())
            {
                ai = await db.AzureEntityStorages.FirstOrDefaultAsync(a => a.MiseEntityType == _poType && a.EntityID == id);
            }
            var po = _entityFactory.FromDataStorageObject<PurchaseOrder>(ai.ToRestaurantDTO());
            SubItemCache.Upsert(po);
            return po;
        }

        public async Task<IPar> GetParById(Guid id)
        {
            var cached = SubItemCache.GetById<IPar>(id);
            if (cached != null)
            {
                return cached;
            }
            AzureEntityStorage ai;
            using (var db = new AzureNonTypedEntitiesDbContext())
            {
                ai = await db.AzureEntityStorages.FirstOrDefaultAsync(a => a.MiseEntityType == _parType && a.EntityID == id);
            }
            var po = _entityFactory.FromDataStorageObject<Par>(ai.ToRestaurantDTO());
            SubItemCache.Upsert(po);
            return po;
        }

        public async Task<IVendor> GetVendorById(Guid id)
        {
            AzureEntityStorage ai;
            using (var db = new AzureNonTypedEntitiesDbContext())
            {
                ai = await db.AzureEntityStorages.FirstOrDefaultAsync(a => a.MiseEntityType == _vendorType && a.EntityID == id);
            }
            return _entityFactory.FromDataStorageObject<Vendor>(ai.ToRestaurantDTO());
        }

        public async Task<IEnumerable<EmailAddress>> GetEmailToSendReportToForRestaurant(Guid restaurantId)
        {
            var rest = await GetRestaurantById(restaurantId);
            if (rest == null)
            {
                return new List<EmailAddress>();
            }
            var overrides = rest.GetEmailsToSendInventoryReportsTo();
            if (overrides != null && overrides.Any())
            {
                return overrides;
            }

            if (rest.AccountID.HasValue)
            {
                var acc = await GetAccountById(rest.AccountID.Value);
                return new[] { acc.PrimaryEmail };
            }

            return new List<EmailAddress>();
        }

        public async Task<IAccount> GetAccountById(Guid id)
        {
            var miseAcctType = typeof(MiseEmployeeAccount).ToString();
            using (var db = new AzureNonTypedEntitiesDbContext())
            {
                var miseAcct =
                    await db.AzureEntityStorages.Where(ai => ai.MiseEntityType == miseAcctType && ai.EntityID == id)
                        .FirstOrDefaultAsync();

                return _entityFactory.FromDataStorageObject<RestaurantAccount>(miseAcct.ToRestaurantDTO());
            }
        }
        public async Task<IEnumerable<IAccount>> GetAccountsByEmail(EmailAddress email)
        {
            var miseAcctType = typeof(MiseEmployeeAccount).ToString();

            var res = new List<IAccount>();
            using (var db = new AzureNonTypedEntitiesDbContext())
            {
                var miseAcctAis = await
                    db.AzureEntityStorages.Where(
                        ai => ai.MiseEntityType == miseAcctType && ai.EntityJSON.Contains(email.Value)).ToListAsync();
                var miseAccts =
                    miseAcctAis.Select(
                        a => _entityFactory.FromDataStorageObject<MiseEmployeeAccount>(a.ToRestaurantDTO()));
                res.AddRange(miseAccts);

                //get the rest accounts
                var restAccounts = await GetRestaurantAccounts(email.Value);
                res.AddRange(restAccounts);
            }

            return res;
        }

        public async Task<IEnumerable<IAccount>> GetRestaurantAccounts(string searchString)
        {
            var restAcctType = typeof(RestaurantAccount).ToString();

            var res = new List<IAccount>();
            using (var db = new AzureNonTypedEntitiesDbContext())
            {
                //get the rest accounts
                var restAcctAis = await db.AzureEntityStorages.Where(
                        ai => ai.MiseEntityType == restAcctType && ai.EntityJSON.Contains(searchString)).ToListAsync();
                var restAccts =
                    restAcctAis.Select(a => _entityFactory.FromDataStorageObject<RestaurantAccount>(a.ToRestaurantDTO()));
                res.AddRange(restAccts);
            }

            return res;
        }

        public async Task<IEnumerable<IAccount>> GetAccountsWaitingForPaymentPlan()
        {
            var accountType = typeof(RestaurantAccount).ToString();
            const string MISSING_ACCOUNTS_TAG = "\"PaymentPlanSetupWithProvider\":false";

            IEnumerable<AzureEntityStorage> accountAIs;
            using (var context = new AzureNonTypedEntitiesDbContext())
            {
                accountAIs = await context.AzureEntityStorages.Where(a => a.MiseEntityType == accountType && a.EntityJSON.Contains(MISSING_ACCOUNTS_TAG))
                    .ToListAsync();
            }

            if (accountAIs == null)
            {
                return null;
            }

            var dtos = accountAIs.Select(ai => ai.ToRestaurantDTO());
            var accounts = dtos.Select(dto => _entityFactory.FromDataStorageObject<RestaurantAccount>(dto));
            return accounts;
        }

        public async Task UpdateAccount(IAccount account)
        {
            var downgrade = account as RestaurantAccount;
            if (downgrade == null)
            {
                return;
            }

            var typeString = typeof(RestaurantAccount).ToString();
            using (var db = new AzureNonTypedEntitiesDbContext())
            {
                var oldVer =
                    await
                        db.AzureEntityStorages.FirstOrDefaultAsync(
                            ai => ai.EntityID == account.Id && ai.MiseEntityType == typeString);
                if (oldVer == null)
                {
                    throw new InvalidOperationException("Unable to find existing account to update");
                }
                var dto = _entityFactory.ToDataTransportObject(downgrade);

                var newVer = new AzureEntityStorage(dto);
                oldVer.EntityJSON = newVer.EntityJSON;
                oldVer.LastUpdatedDate = DateTimeOffset.UtcNow;
                db.Entry(oldVer).State = EntityState.Modified;
                await db.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<SendEmailCSVFile>> GetLastSentEmails(int numBack)
        {
            using (var db = new StronglyTypedEntitiesDbContext())
            {
                var emails = await db.SendEmailCSVFiles.Where(e => e.Sent)
                    .OrderByDescending(e => e.CreatedAt)
                    .Take(numBack)
                    .ToListAsync();

                return emails;
            }
        }

        public async Task<SendEmailCSVFile> GetEmailForEntity(Guid entityId)
        {
            using (var db = new StronglyTypedEntitiesDbContext())
            {
                var found = await db.SendEmailCSVFiles.FirstOrDefaultAsync(e => e.EntityId == entityId);
                return found;
            }
        }

        public async Task<IEnumerable<SendEmailCSVFile>> GetUnsentEmails()
        {
            using (var db = new StronglyTypedEntitiesDbContext())
            {
                var items = await db.SendEmailCSVFiles.Where(e => e.Sent == false).ToListAsync();
                return items;
            }
        }

        public async Task CreateEmailRecord(SendEmailCSVFile email)
        {
            using (var db = new StronglyTypedEntitiesDbContext())
            {
                db.SendEmailCSVFiles.Add(email);
                await db.SaveChangesAsync();
            }
        }

        public async Task MarkEmailAsSent(SendEmailCSVFile email)
        {
            using (var db = new AzureNonTypedEntitiesDbContext())
            {
                email.Sent = true;
                db.Entry(email).State = EntityState.Modified;
                await db.SaveChangesAsync();
            }
        }
    }
}
