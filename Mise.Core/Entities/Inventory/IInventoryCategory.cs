using System;
using System.Collections.Generic;

using Mise.Core.ValueItems.Inventory;
namespace Mise.Core.Entities.Inventory
{
    /// <summary>
    /// Represents a category which we put items in an inventory under
    /// </summary>
    public interface IInventoryCategory : ICategory
    {
        /// <summary>
        /// Get the containers that make sense for this category.  Empty is up to app to decide
        /// </summary>
        /// <returns>The preferred containers.</returns>
        IEnumerable<LiquidContainer> GetPreferredContainers();

        bool IsAssignable { get; set; }
    }
}

