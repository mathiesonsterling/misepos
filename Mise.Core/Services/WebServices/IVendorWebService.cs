using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities.Vendors;
using Mise.Core.Entities.Vendors.Events;
using Mise.Core.ValueItems;

namespace Mise.Core.Services.WebServices
{
    public interface IVendorWebService : IEventStoreWebService<IVendor, IVendorEvent>
    {
        Task<IEnumerable<IVendor>> GetVendorsWithinSearchRadius(Location currentLocation, Distance radius);

        Task<IEnumerable<IVendor>> GetVendorsAssociatedWithRestaurant(Guid restaurantID);
    }
}
