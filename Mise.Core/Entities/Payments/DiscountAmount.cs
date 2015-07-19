using Mise.Core.Entities.Check;
using Mise.Core.ValueItems;
using Mise.Core.Entities.Base;
namespace Mise.Core.Entities.Payments
{
	public class DiscountAmount : RestaurantEntityBase, IDiscount
	{
	    public DiscountType DiscountType { get { return DiscountType.DiscountAmount; } }
	    public string Name{get;set;}

		public Money GetDiscountAmount (Money total)
		{
			return AmountToAdd;
		}
			
		/// <summary>
		/// Amount of money to add or subtract (negative) from the check
		/// </summary>
		/// <value>The amount to add.</value>
		public Money AmountToAdd{ get; set;}

		public bool AddsMoney {
			get {
				return AmountToAdd.GreaterThanOrEqualTo (Money.None);
			}
		}

		public bool CanApplyToCheck (ICheck check)
		{
			return true;
		}
	}
}

