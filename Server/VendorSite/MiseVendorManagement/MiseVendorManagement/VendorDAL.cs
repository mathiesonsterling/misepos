using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Mise.Core.Common.Entities.DTOs;
using Mise.Core.Common.Entities.Vendors;
using Mise.Core.Common.Services.Implementation.Serialization;
using Mise.Core.Entities.Vendors;
using MiseVendorManagement.Database;

namespace MiseVendorManagement
{
    public class VendorDAL
    {
        private const string VENDOR_ID_LIST_KEY = "allVendorIds";
        private readonly EntityDataTransportObjectFactory _entityFactory;
        private readonly string _vendorType;

        public VendorDAL()
        {
            _entityFactory = new EntityDataTransportObjectFactory(new JsonNetSerializer());
            _vendorType = typeof (Vendor).ToString();
        }

        private static IList<Guid> CachedList
        {
            get { return HttpRuntime.Cache.Get(VENDOR_ID_LIST_KEY) as IList<Guid>; }
            set
            {
                var existing = HttpRuntime.Cache.Get(VENDOR_ID_LIST_KEY);
                if (existing != null)
                {
                    HttpRuntime.Cache.Remove(VENDOR_ID_LIST_KEY);
                }
                HttpRuntime.Cache.Insert(VENDOR_ID_LIST_KEY, value);
            }
        } 

        public async Task<IEnumerable<IVendor>> GetAllVendors()
        {
            var realIds = CachedList;

            if (realIds != null && realIds.Any())
            {
                var cachedItems = realIds
                    .Select(id => HttpRuntime.Cache.Get(id.ToString()))
                    .Select(res => res as IVendor);

                return cachedItems;
            }

            using (var db = new AzureDB())
            {
                var ais = await db.AzureEntityStorages.Where(ai => ai.MiseEntityType == _vendorType).ToListAsync();
                var dtos = ais.Select(ai => ai.ToRestaurantDTO());
                var vendors = dtos.Select(dto => _entityFactory.FromDataStorageObject<Vendor>(dto)).ToList();

                foreach (var v in vendors)
                {
                    HttpRuntime.Cache[v.Id.ToString()] = v;
                }

                CachedList = vendors.Select(v => v.Id).ToList();
                return vendors;
            }
        }

        public async Task<IEnumerable<IVendor>> GetVendors(string searchString)
        {
            IEnumerable<IVendor> vendors;
            if (string.IsNullOrEmpty(searchString))
            {
                vendors = await GetAllVendors();
            }
            else
            {
                using (var db = new AzureDB())
                {
                    var ais =
                        await
                            db.AzureEntityStorages.Where(
                                ai => ai.MiseEntityType == _vendorType && ai.EntityJSON.Contains(searchString))
                                .ToListAsync();
                    var dtos = ais.Select(ai => ai.ToRestaurantDTO());
                    vendors = dtos.Select(dto => _entityFactory.FromDataStorageObject<Vendor>(dto));
                }
            }

            return vendors;
        } 

        public async Task<IVendor> GetVendor(Guid vendorId)
        {
            var cachedObj = HttpRuntime.Cache.Get(vendorId.ToString()) as IVendor;
            if (cachedObj != null)
            {
                return cachedObj;
            }

            using (var db = new AzureDB())
            {
                var ai =
                    await db.AzureEntityStorages.FirstOrDefaultAsync(
                        a => a.EntityID == vendorId && a.MiseEntityType == _vendorType);
                if (ai == null)
                {
                    return null;
                }

                var dto = ai.ToRestaurantDTO();
                var vendor = _entityFactory.FromDataStorageObject<Vendor>(dto);

                HttpRuntime.Cache.Insert(vendor.Id.ToString(), vendor);
                return vendor;
            }
        }

        public async Task InsertVendor(Vendor vendor)
        {
            var existing = await GetAllVendors();
            var alreadyHere = existing.Any(v => v.Name == vendor.Name && v.StreetAddress.Equals(vendor.StreetAddress));
            if (alreadyHere)
            {
                throw new InvalidOperationException("Vendor already exists at specified location!");
            }

            var dto = _entityFactory.ToDataTransportObject(vendor);
            var azureEnt = new AzureEntityStorage(dto);


            using (var db = new AzureDB())
            {
                db.AzureEntityStorages.Add(azureEnt);
                await db.SaveChangesAsync();
            }

            HttpRuntime.Cache.Insert(vendor.Id.ToString(), vendor);
            var list = CachedList ?? new List<Guid>();
            list.Add(vendor.Id);
            CachedList = list;
        }

        public async Task UpdateVendor(Vendor vendor)
        {
            using (var db = new AzureDB())
            {
                var oldVer = await db.AzureEntityStorages.FirstOrDefaultAsync(ai => ai.EntityID == vendor.Id && ai.MiseEntityType == _vendorType);
                if (oldVer == null)
                {
                    throw new InvalidOperationException("Cannot find vendor to update in DB");
                }

                //store the line items!
                if (vendor.VendorBeverageLineItems.Any() == false)
                {
                    var oldDeser = _entityFactory.FromDataStorageObject<Vendor>(oldVer.ToRestaurantDTO());
                    vendor.VendorBeverageLineItems = oldDeser.VendorBeverageLineItems.Select(li => li).ToList();
                }

                var newVerDTO = _entityFactory.ToDataTransportObject(vendor);
                oldVer.EntityJSON = newVerDTO.JSON;
                oldVer.LastUpdatedDate = newVerDTO.LastUpdatedDate;
                    
                db.Entry(oldVer).State = EntityState.Modified;
                await db.SaveChangesAsync();

                HttpRuntime.Cache.Remove(vendor.Id.ToString());
                HttpRuntime.Cache.Insert(vendor.Id.ToString(), vendor);
            }
        }

        public async Task UpdateVendorLineItem(VendorBeverageLineItem li)
        {
            using (var db = new AzureDB())
            {
                var vendor = await GetVendor(li.VendorID);
                var downCast = vendor as Vendor;

                var oldLineItem = downCast.VendorBeverageLineItems.FirstOrDefault(l => l.Id == li.Id);
                if (oldLineItem == null)
                {
                    throw new ArgumentException("Cannot find line item in vendor");
                }
                downCast.VendorBeverageLineItems.Remove(oldLineItem);
                downCast.VendorBeverageLineItems.Add(li);

                await UpdateVendor(downCast);
            }
        } 

        public async Task DeleteVendor(Guid id)
        {
            using (var db = new AzureDB())
            {
                var ai = await db.AzureEntityStorages.FirstOrDefaultAsync(a => a.EntityID == id && a.MiseEntityType == _vendorType);

                db.AzureEntityStorages.Remove(ai);
                await db.SaveChangesAsync();
            }

            var ids = CachedList;
            CachedList?.Remove(id);
        }  
    }
}
