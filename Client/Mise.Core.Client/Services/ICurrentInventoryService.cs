using System;
using System.Threading.Tasks;
using Mise.Core.ValueItems.Inventory;
using Mise.Core.Entities.Inventory;
namespace Services
{
    /// <summary>
    /// Reports on the amount of items we believe currently in a restaurant
    /// </summary>
    public interface ICurrentInventoryService
    {
        /// <summary>
        /// If we thing we currently have an item in the restaurant
        /// </summary>
        /// <param name="lineItem">Line item.</param>
        Task<bool> IsItemCurrentlyInStock(IBaseBeverageLineItem lineItem);
    }
}

