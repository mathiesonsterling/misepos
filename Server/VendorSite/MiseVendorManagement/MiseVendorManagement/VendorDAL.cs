using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Common.Entities.DTOs;
using Mise.Core.Common.Entities.Vendors;
using Mise.Core.Common.Services.Implementation.Serialization;
using Mise.Core.Entities.Vendors;
using MiseVendorManagement.Database;

namespace MiseVendorManagement
{
    public class VendorDAL
    {
        private readonly EntityDataTransportObjectFactory _entityFactory;
        private readonly string _vendorType;
        public VendorDAL()
        {
            _entityFactory = new EntityDataTransportObjectFactory(new JsonNetSerializer());
            _vendorType = typeof (Vendor).ToString();
        }

        public async Task<IEnumerable<IVendor>> GetAllVendors()
        {
            using (var db = new AzureDB())
            {
                var ais = await db.AzureEntityStorages.Where(ai => ai.MiseEntityType == _vendorType).ToListAsync();
                var dtos = ais.Select(ai => ai.ToRestaurantDTO());
                var vendors = dtos.Select(dto => _entityFactory.FromDataStorageObject<Vendor>(dto));
                return vendors.Cast<IVendor>();
            }
        }

        public Task<IVendor> GetVendor(Guid vendorId)
        {
            return Task.Run(() =>
            {
                using (var db = new AzureDB())
                {
                    var ai =
                        db.AzureEntityStorages.FirstOrDefault(
                            a => a.EntityID == vendorId && a.MiseEntityType == _vendorType);
                    if (ai == null)
                    {
                        return null;
                    }

                    var dto = ai.ToRestaurantDTO();
                    var vendor = _entityFactory.FromDataStorageObject<Vendor>(dto);
                    return (IVendor) vendor;
                }
            });
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
        }  
    }
}
