using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Common.Entities.Vendors;
using Mise.Core.Common.Events.Vendors;
using Mise.Core.Entities.Base;
using Mise.Core.Entities.Vendors;
using Mise.Core.Entities.Vendors.Events;
using Mise.Core.Repositories;
using Mise.Core.Server.Repositories;
using Mise.Core.Server.Services;
using Mise.Core.Server.Services.DAL;
using Mise.Core.Services;
using Mise.Core.ValueItems;

namespace MiseInventoryService.Repositories
{
    public class VendorServerRepository : BaseAdminServiceRepository<IVendor, IVendorEvent>, IVendorRepository
    {
        public VendorServerRepository(ILogger logger, IEntityDAL entityDAL, IWebHostingEnvironment host) : base(logger, entityDAL, host)
        {
        }

        public override async Task<CommitResult> Commit(Guid entityID)
        {
            var bundle = UnderTransaction[entityID];
            var upsert = Cache.ContainsItem(entityID) ? EntityDAL.UpdateVendorAsync(bundle.NewVersion): EntityDAL.AddVendorAsync(bundle.NewVersion);

            await upsert.ConfigureAwait(false);
            return CommitResult.StoredInDB;
        }

        protected override IVendor CreateNewEntity()
        {
            return new Vendor();
        }

        protected override bool IsEventACreation(IEntityEventBase ev)
        {
            return ev is VendorCreatedEvent;
        }

        public override Guid GetEntityID(IVendorEvent ev)
        {
            return ev.VendorID;
        }

        protected override async Task LoadFromDB()
        {
            var res = await EntityDAL.GetVendorsAsync();
            Cache.UpdateCache(res);
        }

        public Task<IEnumerable<IVendor>> GetVendorsWithinRadius(Distance radius, Location deviceLocation, int maxResults)
        {
            var items = Cache.GetAll()
                .Where(v => v.StreetAddress != null && v.StreetAddress.StreetAddressNumber != null)
                .Select(
                    v => new
                    {
                        DistanceFromPoint = new Distance(deviceLocation, v.StreetAddress.StreetAddressNumber),
                        Vendor = v
                    })
                .Where(vl => radius.GreaterThan(vl.DistanceFromPoint))
                .OrderBy(vl => vl.DistanceFromPoint)
                .Take(maxResults)
                .Select(vl => vl.Vendor);
            return Task.FromResult(items);
        }

        public Task<IEnumerable<IVendor>> GetVendorsBySearchString(string searchString)
        {
            //TODO - at some point we'll want lucene to make this faster and better
            //TODO also push searchability up to the entity level, and have this work for all repositories
            return Task.Run(() =>
                Cache.GetAll()
                    .Where(
                        v => v.Name.Contains(searchString)
                             || v.PhoneNumber.ContainsSearchString(searchString)
                             || v.StreetAddress.ContainsSearchString(searchString)
                    ));
        }

        public Task<IEnumerable<IVendor>> GetVendorsAssociatedWithRestaurant(Guid restaurantID)
        {
            return Task.Run(() => GetAll().Where(v => v.GetRestaurantIDsAssociatedWithVendor().Contains(restaurantID)));
        }

        public async Task<IEnumerable<IVendor>> GetVendorsNotAssociatedWithRestaurantWithinRadius(Guid restaurantID, Location deviceLocation, int maxResults)
        {
            var vendors = await GetVendorsWithinRadius(CurrentMaxRadius, deviceLocation, maxResults);

            return vendors.Where(v => v.GetRestaurantIDsAssociatedWithVendor().Contains(restaurantID) == false);

        }

        public Distance CurrentMaxRadius { get; private set; }
    }
}
