using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Common.Entities.DTOs;
using Mise.Core.Common.Entities.Vendors;
using Mise.Core.Common.Services.Implementation.Serialization;
using Mise.Core.Entities.Vendors;

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

        public Task<IEnumerable<IVendor>> GetAllVendors()
        {
            return Task.Run(() =>
            {
                using (var db = new AzureDB())
                {
                    var ais = db.AzureEntityStorages.Where(ai => ai.MiseEntityType == _vendorType).ToList();
                    var dtos = ais.Select(ai => ai.ToRestaurantDTO());
                    var vendors = dtos.Select(dto => _entityFactory.FromDataStorageObject<Vendor>(dto));
                    return vendors.Cast<IVendor>();
                }
            });
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

        public Task InsertVendor(Vendor vendor)
        {
            return Task.Run(() =>
            {
                var dto = _entityFactory.ToDataTransportObject(vendor);
                var azureEnt = new AzureEntityStorage(dto);

                using (var db = new AzureDB())
                {
                    db.AzureEntityStorages.Add(azureEnt);
                }
            });
        }

        public Task UpdateVendor(Vendor vendor)
        {
            return Task.Run(() =>
            {
                using (var db = new AzureDB())
                {
                    var oldVer = db.AzureEntityStorages.FirstOrDefault(ai => ai.EntityID == vendor.ID && ai.MiseEntityType == _vendorType);
                    if (oldVer == null)
                    {
                        throw new InvalidOperationException("Cannot find vendor to update in DB");
                    }

                    var newVerDTO = _entityFactory.ToDataTransportObject(vendor);

                    oldVer.EntityJSON = newVerDTO.JSON;
                    oldVer.LastUpdatedDate = newVerDTO.LastUpdatedDate;
                    
                    db.Entry(oldVer).State = EntityState.Modified;
                    return db.SaveChangesAsync();
                }
            });
        }
    }
}
