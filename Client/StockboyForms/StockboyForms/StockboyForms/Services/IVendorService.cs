using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Mise.Core.Entities.Vendors;
using Mise.Core.Entities.Restaurant;
using Mise.Core.Entities.Inventory;
using Mise.Core.ValueItems;


namespace Mise.Inventory.Services
{
	public interface IVendorService
	{
		Task<IEnumerable<IVendor>> GetPossibleVendors ();
		Task<IEnumerable<IVendor>> GetVendorsAssociatedWithRestaurant (IRestaurant restaurant);

		/// <summary>
		/// Gets the vendor with the lowest price for an item.  If multiple are found, the most recent price wins
		/// </summary>
		/// <returns>The vendor with lowest price for item.</returns>
		/// <param name="li">Li.</param>
		/// <param name = "quantity">How many they wish to buy.  Lets minimum orders and the like come into play</param>
		/// <param name = "restaurantID"></param>
		Task<IVendor> GetBestVendorForItem (IBaseBeverageLineItem li, decimal quantity, IRestaurant restaurant);

		Task<IVendor> AddVendor(string name, StreetAddress address, PhoneNumber phoneNumber, EmailAddress email);

		Task AddLineItemsToVendorIfDontExist (Guid vendorID, IEnumerable<IReceivingOrderLineItem> li);

		Task SelectVendor (IVendor vendor);
		Task<IVendor> GetSelectedVendor();
	}
}

