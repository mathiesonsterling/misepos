using System;
using Mise.Core.ValueItems;
using Mise.Core.Entities.Check;
using Mise.Core.Entities.Base;

namespace Mise.Core.Entities.Payments
{
    public enum DiscountType
    {
        DiscountAmount,
        DiscountPercentage,
        DiscountPercentageAfterMinimumCashTotal
    }
	/// <summary>
	/// Represents either a discount or gratuity charge.  Can be either a percentage or amount
	/// </summary>
	public interface IDiscount : IRestaurantEntityBase
	{
        /// <summary>
        /// Needed for deserializing
        /// </summary>
        DiscountType DiscountType { get; }

		/// <summary>
		/// Name of the discount
		/// </summary>
		/// <value>The name.</value>
		string Name{get;}

		/// <summary>
		/// Apply the discount, and get the amount of money to add (can be negative!)
		/// </summary>
		/// <returns>The discount amount.</returns>
		/// <param name="total">Total for the check (can't access this internally without a wrap around)</param>
		Money GetDiscountAmount (Money total);

		/// <summary>
		/// Lets us see if this discount can apply to the check
		/// </summary>
		/// <returns><c>true</c> if this instance can apply to check the specified check; otherwise, <c>false</c>.</returns>
		/// <param name="check">Check.</param>
		bool CanApplyToCheck (ICheck check);

		/// <summary>
		/// If this discount adds or removes money due from the check
		/// </summary>
		bool AddsMoney{get;}
	}
}

