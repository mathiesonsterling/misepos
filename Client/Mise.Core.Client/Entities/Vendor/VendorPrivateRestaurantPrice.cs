namespace Mise.Core.Client.Entities.Vendor
{
    
  public class VendorPrivateRestaurantPrice : EntityData
  {
      public VendorPrivateRestaurantPrice()
      {
      }

      public VendorPrivateRestaurantPrice(VendorBeverageLineItem lineItem, Restaurant.Restaurant restaurant,
          decimal priceCharged)
      {
          Id = lineItem.EntityId + ":" + restaurant.RestaurantID;
          VendorBeverageLineItem = lineItem;
          Restaurant = restaurant;
          PriceCharged = priceCharged;
      }

	  public string VendorBeverageLineItemId { get; set; }
  	  public VendorBeverageLineItem VendorBeverageLineItem { get; set; }

	  public string RestaurantId { get; set; }
      public Restaurant.Restaurant Restaurant { get; set; }

      public decimal PriceCharged { get; set; }
  }
}