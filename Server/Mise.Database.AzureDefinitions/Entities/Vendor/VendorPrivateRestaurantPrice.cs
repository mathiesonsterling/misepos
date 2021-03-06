﻿using System;
using System.ComponentModel.DataAnnotations.Schema;
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
          decimal priceCharged)
      {
          Id = lineItem.EntityId + ":" + restaurant.RestaurantID;
          VendorBeverageLineItem = lineItem;
          Restaurant = restaurant;
          PriceCharged = priceCharged;
      }

	  [ForeignKey("VendorBeverageLineItem")]
	  public string VendorBeverageLineItemId { get; set; }
  	  public VendorBeverageLineItem VendorBeverageLineItem { get; set; }

	  [ForeignKey("Restaurant")]
	  public string RestaurantId { get; set; }
      public Restaurant.Restaurant Restaurant { get; set; }

      public decimal PriceCharged { get; set; }
  }
}