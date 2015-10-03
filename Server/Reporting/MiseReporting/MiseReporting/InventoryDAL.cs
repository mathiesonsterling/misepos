using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Common.Entities;
using Mise.Core.Common.Entities.DTOs;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Common.Events.ApplicationInvitations;
using Mise.Core.Common.Services.Implementation.Serialization;
using Mise.Core.Entities.Inventory;
using Mise.Core.Entities.People;
using Mise.Core.Entities.Restaurant;

namespace MiseReporting
{
    public class InventoryDAL
    {
        private readonly EntityDataTransportObjectFactory _entityFactory;
        private readonly string _invType;
        private readonly string _empType;
        private readonly string _restType;
        public InventoryDAL()
        {
            _entityFactory = new EntityDataTransportObjectFactory(new JsonNetSerializer());
            _invType = typeof (Inventory).ToString();
            _empType = typeof (Employee).ToString();
            _restType = typeof (Restaurant).ToString();
        }

        public async Task<IEnumerable<IInventory>> GetInventoriesForRestaurant(Guid restaurantId)
        {
            List<AzureEntityStorage> ais;
            using (var db = new AzureNonTypedEntities())
            {
                ais = await db.AzureEntityStorages.Where(a => a.MiseEntityType == _invType && a.RestaurantID == restaurantId).ToListAsync();
            }
            var dtos = ais.Select(ai => ai.ToRestaurantDTO());
            return dtos.Select(dto => _entityFactory.FromDataStorageObject<Inventory>(dto));
        }

        public async Task<IInventory> GetInventoryById(Guid invId)
        {
            using (var db = new AzureNonTypedEntities())
            {
                var ai = await db.AzureEntityStorages.Where(a => a.MiseEntityType == _invType && a.EntityID == invId)
                            .FirstOrDefaultAsync();
                var dto = ai.ToRestaurantDTO();
                var inv = _entityFactory.FromDataStorageObject<Inventory>(dto);
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
        }

        public async Task<IEnumerable<IRestaurant>> GetAllRestaurants()
        {
            IEnumerable<AzureEntityStorage> ais;
            using (var db = new AzureNonTypedEntities())
            {
                ais = await db.AzureEntityStorages.Where(ai => ai.MiseEntityType == _restType).ToListAsync();
            }
            var dtos = ais.Select(ai => ai.ToRestaurantDTO());
            return dtos.Select(dto => _entityFactory.FromDataStorageObject<Restaurant>(dto));
        }

        public async Task<IRestaurant> GetRestaurantById(Guid restaurantId)
        {
            AzureEntityStorage ai;
            using (var db = new AzureNonTypedEntities())
            {
                ai = await db.AzureEntityStorages.FirstOrDefaultAsync(
                            a => a.MiseEntityType == _restType && a.EntityID == restaurantId);
            }
            if (ai == null)
            {
                throw new ArgumentException("Error, cannot find restaurant " + restaurantId);
            }
            return _entityFactory.FromDataStorageObject<Restaurant>(ai.ToRestaurantDTO());
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
    }
}
