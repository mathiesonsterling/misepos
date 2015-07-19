using System;
using Mise.Core.Entities.Inventory;
using Mise.Core.ValueItems;
using Mise.Core.Entities.Check;
namespace Mise.Core.Services
{
	/// <summary>
	/// Given an order item, calculate the taxes due on it
	/// </summary>
	public interface ISalesTaxCalculatorService
	{
		Money CalculateSalesTax(Money salesAmount);

		/// <summary>
		/// Calculate from the order item.  Used for differing sales tax on food and booze, etc
		/// </summary>
		/// <returns>The sales tax.</returns>
		/// <param name="check">Check.</param>
		Money CalculateSalesTax(ICheck check);

        /// <summary>
        /// Calculate how much sales tax we'll need to apply to a sales order
        /// </summary>
        /// <param name="purchaseOrder"></param>
        /// <returns></returns>
	    Money CalculateSalesTax(IPurchaseOrder purchaseOrder);
	}
}

