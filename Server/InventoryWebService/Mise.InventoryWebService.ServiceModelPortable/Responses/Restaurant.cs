using System;
using ServiceStack;

namespace Mise.InventoryWebService.ServiceModelPortable.Responses
{
    [Route("/restaurants")]
    [Route("/restaurants/{RestaurantID}")]
    public class Restaurant : IReturn<RestaurantResponse>
    {
        public Guid? RestaurantID { get; set; }
 
        //TODO - do we need additional queries?  Long, lat, etc?
		public double? Longitude{get;set;}
		public double? Latitude{get;set;}
    }
}