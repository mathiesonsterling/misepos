using System.Collections.Generic;
using Mise.Core.Common.Entities;

namespace Mise.InventoryWebService.ServiceModelPortable.Responses
{
    public class RestaurantResponse
    {
        public IEnumerable<Core.Common.Entities.Restaurant> Results { get; set; }
    }
}
