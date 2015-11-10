using System;
using System.Collections.Generic;
using Mise.Core.Entities.Inventory;
using Mise.Core.ValueItems.Inventory;

namespace Mise.Core.Services
{
	public interface ICategoriesService
	{
		/// <summary>
		/// Get the categories used in IAB categories
		/// </summary>
		/// <returns>The IAB ingredient categories.</returns>
		IEnumerable<IInventoryCategory> GetIABIngredientCategories();

		IEnumerable<ICategory> GetCustomCategoriesForRestaurant (Guid restaurantID);

	    LiquidContainerShape GetShapeForCategory(IInventoryCategory cat);

        /// <summary>
        /// Given the value an outside source has, get the possible categories it could refer to
        /// </summary>
        /// <param name="givenCategory"></param>
        /// <returns></returns>
	    IEnumerable<IInventoryCategory> GetPossibleCategories(string givenCategory);

        /// <summary>
        /// Gets all categories where we can add an item to
        /// </summary>
        /// <returns></returns>
	    IEnumerable<IInventoryCategory> GetAssignableCategories();

        /// <summary>
        /// For a category, get the list of preferred containers (if any)
        /// </summary>
        /// <returns>The preferred containers.</returns>
        /// <param name="cat">Cat.</param>
        IEnumerable<LiquidContainer> GetPreferredContainers(IInventoryCategory cat);
	}
}

