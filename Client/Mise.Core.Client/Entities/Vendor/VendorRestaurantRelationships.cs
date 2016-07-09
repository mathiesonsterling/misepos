namespace Mise.Core.Client.Entities.Vendor
{
    public class VendorRestaurantRelationships : EntityData
    {
        public VendorRestaurantRelationships() { }

        public VendorRestaurantRelationships(Core.Client.Entities.Vendor.Vendor vendor, Core.Client.Entities.Restaurant.Restaurant restaurant)
        {
            Vendor = vendor;
            Restaurant = restaurant;
            Id = vendor.EntityId + ":" + restaurant.RestaurantID;
        }

        public Core.Client.Entities.Vendor.Vendor Vendor { get; set; }
		public string VendorId { get; set; }


	    public Core.Client.Entities.Restaurant.Restaurant Restaurant { get; set; }
	    public string RestaurantId { get; set; }
    }
}
