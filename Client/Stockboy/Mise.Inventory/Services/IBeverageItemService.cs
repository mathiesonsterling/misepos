using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Mise.Core.Entities.Inventory;
using Mise.Core.ValueItems;
using Mise.Core.ValueItems.Inventory;
using Mise.Core.Entities.Vendors;


namespace Mise.Inventory
{
	/// <summary>
	/// Service that can provide all the possible beverage items we currently have
	/// </summary>
	public interface IBeverageItemService
	{
		/// <summary>
		/// Gets the items that we might want to know
		/// This is
		/// items in the PAR
		/// (a) items we currently have in inventory
		/// (b) items we used to have in inventory
		/// (c) items vendors we buy from have
		/// (d) items vendors around us have
		/// </summary>
		/// <returns>The possible items.</returns>
		Task<IEnumerable<IBaseBeverageLineItem>> GetPossibleItems (int maxItems = int.MaxValue);

		/// <summary>
		/// Tells us that we want to look at vendors for a maximum distance
		/// </summary>
		/// <returns>The vendor search region.</returns>
		/// <param name="newMaxDistance">New max distance.</param>
		Task ExpandVendorSearchRegion (Distance newMaxDistance);

		/// <summary>
		/// Searches for a string in all our items
		/// </summary>
		/// <returns>The item.</returns>
		/// <param name="seachString">Seach string.</param>
		/// <param name = "maxItems"></param>
		Task<IEnumerable<IBaseBeverageLineItem>> FindItem(string seachString, int maxItems = int.MaxValue);

		Task<IEnumerable<LiquidContainer>> GetAllPossibleContainerSizes();
	}
}

