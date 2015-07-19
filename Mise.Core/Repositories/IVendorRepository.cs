using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities.Vendors;
using Mise.Core.Entities.Vendors.Events;
using Mise.Core.ValueItems;
using Mise.Core.Repositories;
namespace Mise.Core.Repositories
{
    public interface IVendorRepository : IEventSourcedEntityRepository<IVendor, IVendorEvent>, IRepository
    {
        /// <summary>
        /// Gets all vendors that are located within a given radius
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="deviceLocation">The location which to get around</param>
        /// <param name="maxResults">Max number of results to return</param>
        /// <returns></returns>
        Task<IEnumerable<IVendor>> GetVendorsWithinRadius(Distance radius, Location deviceLocation, int maxResults);

		Task<IEnumerable<IVendor>> GetVendorsAssociatedWithRestaurant (Guid restaurantID);

		Task<IEnumerable<IVendor>> GetVendorsNotAssociatedWithRestaurantWithinRadius(Guid restaurantID, Location deviceLocation, int maxResults);

		Distance CurrentMaxRadius{get;}
    }
}
