using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mise.Core.Common.Entities.Vendors;
using Mise.Core.Entities.Vendors;
using Mise.Core.Entities.Vendors.Events;
using Mise.Core.ValueItems;

namespace Mise.Core.Common.Services.WebServices
{
    public interface IVendorWebService : IEventStoreWebService<Vendor, IVendorEvent>
    {
        Task<IEnumerable<Vendor>> GetVendorsWithinSearchRadius(Location currentLocation, Distance radius);

        Task<IEnumerable<Vendor>> GetVendorsAssociatedWithRestaurant(Guid restaurantID);
    }
}
