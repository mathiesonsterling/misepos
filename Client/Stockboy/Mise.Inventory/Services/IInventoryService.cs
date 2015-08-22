using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Mise.Core.ValueItems.Inventory;
using Mise.Core.Entities.Inventory;
using Mise.Core.ValueItems;


namespace Mise.Inventory.Services
{
	public interface IInventoryService
	{
		Task LoadLatest ();

		Task<IInventoryBeverageLineItem> AddLineItemToCurrentInventory (string name, ICategory category, string upc, 
			int quantity, int caseSize, LiquidContainer container);
		Task<IInventoryBeverageLineItem> AddLineItemToCurrentInventory (IBaseBeverageLineItem source, int quantity);

		/// <summary>
		/// Updates the current inventory with the items received from a RO
		/// </summary>
		/// <returns>The line items from received order to current.</returns>
		/// <param name="lis">Lis.</param>
		//Task AddLineItemsFromReceivedOrderToCurrent(IEnumerable<IReceivingOrderLineItem> lis, Guid vendorID);

		/// <summary>
		/// Gets the inventory the user is currently working on
		/// </summary>
		/// <returns>The selected inventory.</returns>
		Task<IInventory> GetSelectedInventory ();
		Task<IInventory> GetLastCompletedInventory();
		Task<IInventory> GetFirstCompletedInventory();

		Task<IInventoryBeverageLineItem> GetLineItemToMeasure();
		Task<IEnumerable<IInventoryBeverageLineItem>> GetLineItemsForCurrentSection();

		Task StartNewInventory();

		/// <summary>
		/// Given a line item, select it to be measured
		/// </summary>
		/// <returns>The line item for measurement.</returns>
		/// <param name="li"></param>
		Task MarkLineItemForMeasurement (IInventoryBeverageLineItem li);

		/// <summary>
		/// Take in a measurement we've made visually!
		/// </summary>
		/// <returns>The current line item.</returns>
		/// <param name="fullBottles">Full bottles.</param>
		/// <param name="partials">Partials.</param>
		Task MeasureCurrentLineItemVisually (int fullBottles, ICollection<decimal> partials);
		Task DeleteLineItem (IInventoryBeverageLineItem li);
		Task MoveLineItemToPosition(IInventoryBeverageLineItem li, int position);
		Task MoveLineItemUpInList(IInventoryBeverageLineItem li);
		Task MoveLineItemDownInList(IInventoryBeverageLineItem li);

	    Task<IInventorySection> GetCurrentInventorySection();
	    Task SetCurrentInventorySection(IInventorySection section);

		Task MarkSectionAsComplete ();

		/// <summary>
		/// marks the current inventory as completed
		/// </summary>
		/// <returns>The inventory as complete.</returns>
		Task MarkInventoryAsComplete();

		Task AddNewSection (string sectionName, bool hasPartialBottles, 
			bool isDefaultInventorySection);

	    Task<IEnumerable<IInventory>> GetCompletedInventoriesForCurrentRestaurant(DateTimeOffset? start, DateTimeOffset? end);

		Task<bool> HasInventoryPriorToDate (Guid restaurantID, DateTimeOffset date);
	}
}

