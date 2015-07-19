using System.Linq;
using Mise.Core.Entities.Check;
using Mise.Core.ValueItems;
using Mise.Core.Entities.Base;
namespace Mise.Core.Entities.Payments
{
	/// <summary>
	/// A discount that kicks in only after a check has a specified total
	/// </summary>
    public sealed class DiscountPercentageAfterMinimumCashTotal : RestaurantEntityBase, IDiscount
	{
		/// <summary>
		/// The minimum amount the check must have for the discount to be applied
		/// </summary>
		/// <value>The minimum check amount.</value>
		public Money MinCheckAmount{get;set;}

	    public DiscountType DiscountType { get { return DiscountType.DiscountPercentageAfterMinimumCashTotal; } }
	    public string Name { get; set; }

        public Money GetDiscountAmount(Money total)
        {
            return total.Multiply(Percentage);
        }

        public bool AddsMoney
        {
            get
            {
                return Percentage > 0;
            }
        }

        public decimal Percentage { get; set; }

		public bool CanApplyToCheck (ICheck check)
		{
			var cashPayments = check.GetPayments().OfType<CashPayment> ();
			var cashTotal = cashPayments.Aggregate(Money.None, (current, cashP) => current.Add(cashP.AmountToApplyToCheckTotal));
		    return cashTotal.GreaterThanOrEqualTo (MinCheckAmount);
		}
	}
}

