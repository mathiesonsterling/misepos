using Microsoft.Azure.Mobile.Server;
using Mise.Database.AzureDefinitions.ValueItems;

namespace Mise.Database.AzureDefinitions.Entities.Vendor
{
  public class VendorPrivateRestaurantPrice : EntityData
  {
  	  public VendorBeverageLineItem LineItem { get; set; }
      public Restaurant.Restaurant Restaurant { get; set; }
      public MoneyDb PriceCharged { get; set; }
  }
}