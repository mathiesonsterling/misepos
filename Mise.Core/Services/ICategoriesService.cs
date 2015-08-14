using System;
using System.Collections.Generic;
using Mise.Core.Entities.Inventory;
using Mise.Core.ValueItems.Inventory;

namespace Mise.Core
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
	}
}

