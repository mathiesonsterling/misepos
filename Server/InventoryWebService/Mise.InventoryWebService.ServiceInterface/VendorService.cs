using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Mise.Core.Common.Entities.Vendors;
using Mise.Core.Repositories;
using Mise.Core.ValueItems;
using Mise.InventoryWebService.ServiceModelPortable.Responses;
using ServiceStack;
using Vendor = Mise.InventoryWebService.ServiceModelPortable.Responses.Vendor;

namespace Mise.InventoryWebService.ServiceInterface
{
    public class VendorService : Service
    {
        private const int MAX_RESULTS_WITHOUT_ID = 10;
        private readonly IVendorRepository _vendorRepository;
        public VendorService(IVendorRepository vendorRepository)
        {
            _vendorRepository = vendorRepository;
        }
        public async Task<object> Get(Vendor request)
        {
            if (_vendorRepository.Loading)
            {
                Thread.Sleep(100);
                if (_vendorRepository.Loading)
                {
                    //throw new InvalidOperationException("Server has not yet loaded");
                    throw new HttpError(HttpStatusCode.ServiceUnavailable, "Server has not yet loaded");
                }
            }
            IEnumerable<Core.Common.Entities.Vendors.Vendor> vendorsReturned = null;
            var restaurantsToGetPrices = new List<Guid>();
            if (request.VendorID.HasValue)
            {
                vendorsReturned = new[] {_vendorRepository.GetByID(request.VendorID.Value) as Core.Common.Entities.Vendors.Vendor};
                if (request.RestaurantID.HasValue)
                {
                    restaurantsToGetPrices.Add(request.RestaurantID.Value);
                }
            }

            if (vendorsReturned == null && request.RestaurantID.HasValue)
            {
                var vendors = await _vendorRepository.GetVendorsAssociatedWithRestaurant(request.RestaurantID.Value);
                vendorsReturned = vendors.Cast<Core.Common.Entities.Vendors.Vendor>();
                restaurantsToGetPrices.Add(request.RestaurantID.Value);
            }

            if (vendorsReturned == null && request.Latitude.HasValue && request.Longitude.HasValue && request.RadiusInKm.HasValue)
            {
                //TODO limit this number of results!
                var location = new Location {Latitude = request.Latitude.Value, Longitude = request.Longitude.Value};
                var radius = new Distance {Kilometers = request.RadiusInKm.Value};

                var vendorsRad = await _vendorRepository.GetVendorsWithinRadius(radius, location, MAX_RESULTS_WITHOUT_ID);
                vendorsReturned = vendorsRad.Cast<Core.Common.Entities.Vendors.Vendor>();

            }

            //find by search on name
            if (vendorsReturned == null && string.IsNullOrEmpty(request.VendorName) == false)
            {
                var vendorsByName = _vendorRepository.GetAll()
                        .Where(v => v.Name.ToUpper().Contains(request.VendorName.ToUpper()))
                        .Take(MAX_RESULTS_WITHOUT_ID);
                return new VendorResponse
                {
                    Results = vendorsByName.Cast<Core.Common.Entities.Vendors.Vendor>()
                };
            }

            if (vendorsReturned != null)
            {
                var vendorsRes = vendorsReturned.ToList();
                //remove all prices that this request shouldn't know about
                foreach (var v in vendorsRes)
                {
                    foreach (var li in v.VendorBeverageLineItems)
                    {
                        var restKeysToRemove =
                            li.PricePerUnitForRestaurant.Keys.Where(rest => restaurantsToGetPrices.Contains(rest) == false)
                            .ToList();
                        foreach (var rest in restKeysToRemove)
                        {
                            li.PricePerUnitForRestaurant.Remove(rest);
                        }
                    }
                }

                return new VendorResponse
                {
                    Results = vendorsRes
                };
            }


            throw HttpError.NotFound("No vendors found");
        }
    }
}
