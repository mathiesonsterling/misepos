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

namespace MiseReporting
{
    public class ManagementDAL
    {
        private const string RESTAURANT_ID_LIST_KEY = "allCachedRestaurantIds";
        private const string INVENTORY_ID_LIST_KEY = "allCachedInventoryIds";

        private readonly EntityDataTransportObjectFactory _entityFactory;
        private readonly string _invType;
        private readonly string _empType;
        private readonly string _restType;
        private readonly string _roType;
        private readonly string _vendorType;
        public ManagementDAL()
        {
            _entityFactory = new EntityDataTransportObjectFactory(new JsonNetSerializer());
            _invType = typeof (Inventory).ToString();
            _empType = typeof (Employee).ToString();
            _restType = typeof (Restaurant).ToString();
            _roType = typeof (ReceivingOrder).ToString();
            _vendorType = typeof (Vendor).ToString();
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

            public static T GetById<T>(Guid inventoryId) where T:class, IEntityBase
            {
                return HttpRuntime.Cache.Get(inventoryId.ToString()) as T;
            }

            public static IEnumerable<T> GetForRestaurant<T>(Guid restaurantId) where T:IEntityBase
            {
                var key = GetKeyForRestaurant(restaurantId, typeof(T));
                return HttpRuntime.Cache.Get(key) as IEnumerable<T>;
            }

            public static void UpsertForRestaurant<T>(Guid restaurantId, IEnumerable<T> inventories) where T:IEntityBase
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
                return RESTAURANT_INVENTORY_LIST_KEY + ":" +itemType + "::"+ restaurantId;
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
            using (var db = new AzureNonTypedEntities())
            {
                ais = await db.AzureEntityStorages.Where(a => a.MiseEntityType == _invType && a.RestaurantID == restaurantId).ToListAsync();
            }
            var dtos = ais.Select(ai => ai.ToRestaurantDTO());
            var invs = dtos.Select(dto => _entityFactory.FromDataStorageObject<Inventory>(dto)).ToList();

            SubItemCache.UpsertForRestaurant(restaurantId, invs.Cast<IInventory>());
            return invs;
        }

        public async Task<IInventory> GetInventoryById(Guid invId)
        {
            var cached = SubItemCache.GetById<IInventory>(invId);
            if (cached != null)
            {
                return cached;
            }

            using (var db = new AzureNonTypedEntities())
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

            var sec = inv.Sections.FirstOrDefault(s => s.LineItems.Select(l => l.Id).Contains(li.Id));

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

            using (var db = new AzureNonTypedEntities())
            {
                var oldVer = await db.AzureEntityStorages.FirstOrDefaultAsync(
                        ai => ai.EntityID == inventory.Id && ai.MiseEntityType == _invType);
                if (oldVer == null)
                {
                    throw new InvalidOperationException("Cannot find vendor to update in DB");
                }
                var newVerDTO = new AzureEntityStorage(_entityFactory.ToDataTransportObject(inventory));
                oldVer.EntityJSON = newVerDTO.EntityJSON;
                oldVer.LastUpdatedDate = newVerDTO.LastUpdatedDate;
                db.Entry(oldVer).State = EntityState.Modified;
                await db.SaveChangesAsync();
            }

            SubItemCache.Upsert(inventory);
        }

        public async Task<IEnumerable<IRestaurant>> GetAllRestaurants()
        {
            if (CachedRestaurantList != null && CachedRestaurantList.Any())
            {
                var cachedItems = CachedRestaurantList
                    .Select(id => HttpRuntime.Cache.Get(id.ToString()))
                    .Select(r => r as IRestaurant);
                return cachedItems;
            }

            IEnumerable<AzureEntityStorage> ais;
            using (var db = new AzureNonTypedEntities())
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
            using (var db = new AzureNonTypedEntities())
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
            using (var db = new AzureNonTypedEntities())
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

            using (var db = new AzureNonTypedEntities())
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
            using (var db = new AzureNonTypedEntities())
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
            using (var db = new AzureNonTypedEntities())
            {
                var ais = await db.AzureEntityStorages.Where(ai => ai.MiseEntityType == _empType && (ai.Deleted == false)).ToListAsync();
                var dtos = ais.Select(ai => ai.ToRestaurantDTO());
                return dtos.Select(dto => _entityFactory.FromDataStorageObject<Employee>(dto));
            }
        }
        public async Task<IEmployee> GetEmployeeById(Guid id)
        {
            AzureEntityStorage ai;
            using (var db = new AzureNonTypedEntities())
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
            using (var db = new AzureNonTypedEntities())
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
            using (var db = new AzureNonTypedEntities())
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
            using (var db = new AzureNonTypedEntities())
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

        public async Task CreateEmployee(Employee emp)
        {
            var dto = _entityFactory.ToDataTransportObject(emp);
            var ai = new AzureEntityStorage(dto);

            using (var db = new AzureNonTypedEntities())
            {
                db.AzureEntityStorages.Add(ai);
                await db.SaveChangesAsync();
            }
        }

        public async Task UpdateEmployee(Employee emp)
        {
            using (var db = new AzureNonTypedEntities())
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
            using (var db = new AzureNonTypedEntities())
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
            AzureEntityStorage ai = null;
            using (var db = new AzureNonTypedEntities())
            {
                ai = await db.AzureEntityStorages.FirstOrDefaultAsync(a => a.MiseEntityType == _roType && a.EntityID == id);
            }
            var ro = _entityFactory.FromDataStorageObject<ReceivingOrder>(ai.ToRestaurantDTO());
            SubItemCache.Upsert(ro);
            return ro;
        }

        public async Task<IVendor> GetVendorById(Guid id)
        {
            AzureEntityStorage ai;
            using (var db = new AzureNonTypedEntities())
            {
                ai = await db.AzureEntityStorages.FirstOrDefaultAsync(a => a.MiseEntityType == _vendorType && a.EntityID == id);
            }
            return _entityFactory.FromDataStorageObject<Vendor>(ai.ToRestaurantDTO());
        }

        public async Task<IEnumerable<IAccount>> GetAccountsByEmail(EmailAddress email)
        {
            var miseAcctType = typeof (MiseEmployeeAccount).ToString();
            var restAcctType = typeof (RestaurantAccount).ToString();

            var res = new List<IAccount>();
            using (var db = new AzureNonTypedEntities())
            {
                var miseAcctAis = await
                    db.AzureEntityStorages.Where(
                        ai => ai.MiseEntityType == miseAcctType && ai.EntityJSON.Contains(email.Value)).ToListAsync();
                var miseAccts =
                    miseAcctAis.Select(
                        a => _entityFactory.FromDataStorageObject<MiseEmployeeAccount>(a.ToRestaurantDTO()));
                res.AddRange(miseAccts);

                //get the rest accounts
                var restAcctAis = await db.AzureEntityStorages.Where(
                        ai => ai.MiseEntityType == restAcctType && ai.EntityJSON.Contains(email.Value)).ToListAsync();
                var restAccts =
                    restAcctAis.Select(a => _entityFactory.FromDataStorageObject<RestaurantAccount>(a.ToRestaurantDTO()));
                res.AddRange(restAccts);
            }

            return res;
        }
    }
}
