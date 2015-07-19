using System;
using ServiceStack;

namespace Mise.InventoryWebService.ServiceModelPortable.Responses
{
    [Route("/restaurants/{RestaurantID}/vendors/")]
    [Route("/vendors/{VendorName}")]
    [Route("/vendors/{Longitude}/{Latitude}/{RadiusInKm}")]
    public class Vendor : IReturn<VendorResponse>
    {
        public Guid? VendorID { get; set; }

        /// <summary>
        /// If given, return Vendors with this restaurant
        /// </summary>
        public Guid? RestaurantID { get; set; }

        /// <summary>
        /// If this and Location are given, we'll keep it to within a radius of the location
        /// </summary>
        public double? Longitude { get; set; }

        public double? Latitude { get; set; }
        public double? RadiusInKm { get; set; }

        public string VendorName { get; set; }
    }
}