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
		IEnumerable<ICategory> GetIABIngredientCategories();

		IEnumerable<ICategory> GetCustomCategoriesForRestaurant (Guid restaurantID);

	    LiquidContainerShape GetShapeForCategory(ICategory cat);

        /// <summary>
        /// Given the value an outside source has, get the possible categories it could refer to
        /// </summary>
        /// <param name="givenCategory"></param>
        /// <returns></returns>
	    IEnumerable<ICategory> GetPossibleCategories(string givenCategory);
	}
}

