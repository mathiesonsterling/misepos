using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Mise.Core.Client.Services;
using Mise.Core.Common.Entities.Vendors;
using Mise.Core.Common.Events.Vendors;
using Mise.Core.Common.Repositories.Base;
using Mise.Core.Common.Services;
using Mise.Core.Common.Services.WebServices;
using Mise.Core.Entities.Base;
using Mise.Core.Entities.Vendors;
using Mise.Core.Entities.Vendors.Events;
using Mise.Core.Repositories;
using Mise.Core.Services;
using Mise.Core.ValueItems;
using Mise.Core.Services.UtilityServices;

namespace Mise.Core.Client.Repositories
{
    public class ClientVendorRepository : BaseEventSourcedClientRepository<IVendor, IVendorEvent, Vendor>, IVendorRepository
    {
        private readonly IVendorWebService _vendorWebService;
        private static readonly Distance DefaultSearchRadius = new Distance{Kilometers = 80.46};
        private readonly IDeviceLocationService _deviceLocationService;

        public ClientVendorRepository(ILogger logger, IVendorWebService webService, IDeviceLocationService locationService, IResendEventsWebService resend)
            : base(logger, webService, resend)
        {
            _vendorWebService = webService;
            _deviceLocationService = locationService;
        }

        public Task<IEnumerable<IVendor>> GetVendorsAssociatedWithRestaurant(Guid restaurantID)
        {
            return Task.FromResult( 
                GetAll().Where(v => v.GetRestaurantIDsAssociatedWithVendor().Contains(restaurantID))
            );
        }

		public async Task<IEnumerable<IVendor>> GetVendorsNotAssociatedWithRestaurantWithinRadius(Guid restaurantID, Location deviceLocation, int maxResults){
			var vendorsInArea = await GetVendorsWithinRadius (DefaultSearchRadius, deviceLocation, maxResults);

			var alreadyKnown = await GetVendorsAssociatedWithRestaurant (restaurantID);
			return vendorsInArea.Except (alreadyKnown);
		}

        /// <summary>
		/// The distance we currently have vendors within
		/// </summary>
		/// <value>The current max radius.</value>
		public Distance CurrentMaxRadius{ get; private set; }

        private const int MAX_RADIUS_RESULTS = 10;

        protected override async Task<IEnumerable<Vendor>> LoadFromWebservice(Guid? restaurantID)
        {
            if (restaurantID.HasValue)
            {
                return await _vendorWebService.GetVendorsAssociatedWithRestaurant(restaurantID.Value);
            }

            var loc = await _deviceLocationService.GetDeviceLocation();
            var items = await GetVendorsWithinRadius(DefaultSearchRadius, loc, MAX_RADIUS_RESULTS);
            return items.Cast<Vendor>();
        }
			

        private async Task<IEnumerable<IVendor>> LoadWithinDistance(Location deviceLocation, Distance radius)
        {
            try
            {
                Loading = true;
                //we need to go back to the server, since we're looking for a bigger radius than is loaded
                var vendors = (await _vendorWebService.GetVendorsWithinSearchRadius(deviceLocation, radius)).ToList();
                CurrentMaxRadius = radius;

                //store them in cache
                Cache.UpdateCache(vendors);
                Loading = false;
                return vendors.AsEnumerable();
            }
            catch (Exception e)
            {
                Logger.HandleException(e);
                Logger.Log("Cannot retrieve vendors from web service!");
                return new List<IVendor>();
            }       
        }

        protected override IVendor CreateNewEntity()
        {
            return new Vendor();
        }


        public override Guid GetEntityID(IVendorEvent ev)
        {
            return ev.VendorID;
        }

        public Task<IEnumerable<IVendor>> GetVendorsWithinRadius(Distance radius, Location deviceLocation, int maxResults)
        {
            Loading = true;
            if (radius.GreaterThan(CurrentMaxRadius))
            {
                return LoadWithinDistance(deviceLocation, radius);
            }

            var items = GetAll().Where(v => v.StreetAddress != null && v.StreetAddress.StreetAddressNumber != null)
                .Select(
                    v => new {Distance = new Distance(deviceLocation, v.StreetAddress.StreetAddressNumber), Vendor = v})
                .Where(vd => radius.GreaterThan(vd.Distance))
                .OrderBy(vd => vd.Distance)
                .Take(maxResults)
                .Select(vd => vd.Vendor);
            Loading = false;
            return Task.FromResult(items);
        }
    }
}
