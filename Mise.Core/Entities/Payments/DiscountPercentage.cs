using Mise.Core.Entities.Check;
using Mise.Core.ValueItems;
using Mise.Core.Entities.Base;
namespace Mise.Core.Entities.Payments
{
	public class DiscountPercentage : RestaurantEntityBase, IDiscount
	{
	    public DiscountType DiscountType { get { return DiscountType.DiscountPercentage; } }
	    public string Name{get;set;}

		public virtual Money GetDiscountAmount (Money total)
		{
			return total.Multiply (Percentage);
		}

		public bool AddsMoney {
			get {
				return Percentage > 0;
			}
		}			

		public decimal Percentage{get;set;}

		public virtual bool CanApplyToCheck (ICheck check)
		{
			return check.Total.GreaterThanOrEqualTo (Money.None);
		}
	}
}

