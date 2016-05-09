using System;
using Microsoft.Azure.Mobile.Server;
using Mise.Database.AzureDefinitions.ValueItems;

namespace Mise.Database.AzureDefinitions.Entities.Vendor
{
  public class VendorPrivateRestaurantPrice : EntityData
  {
      public VendorPrivateRestaurantPrice()
      {
      }

      public VendorPrivateRestaurantPrice(VendorBeverageLineItem lineItem, Restaurant.Restaurant restaurant,
          MoneyDb priceCharged)
      {
          Id = lineItem.EntityId + ":" + restaurant.RestaurantID;
          LineItem = lineItem;
          Restaurant = restaurant;
          PriceCharged = priceCharged;
      }

  	  public VendorBeverageLineItem LineItem { get; set; }
      public Restaurant.Restaurant Restaurant { get; set; }
      public MoneyDb PriceCharged { get; set; }
  }
}