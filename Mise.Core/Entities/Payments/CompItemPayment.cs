using System;
using Mise.Core.ValueItems;

namespace Mise.Core.Entities.Payments
{
    /// <summary>
    /// Payment made by comping an item, not an amount
    /// </summary>
	public class CompItemPayment : BasePayment, ICompPayment
	{
		#region implemented abstract members of BasePayment

        public override IPayment Clone()
        {
            return new CompItemPayment
            {
                ID = Guid.NewGuid(),
                CheckID = CheckID,
                EmployeeID = EmployeeID,
                Amount = Amount,
                Reason = Reason,
				OrderItemID = OrderItemID
            };
        }
	    public override PaymentType PaymentType
	    {
	        get { return PaymentType.CompItem; }
	    }

		public override Money DisplayAmount{get{return Money.None;}}

	    public override Money AmountToApplyToCheckTotal {
			get {
				//we don't apply any money, because we exclude comped Items from the total
				return Money.None;
			}
		}
			

		public override bool OpensCashDrawer {
			get {
				return false;
			}
		}
			
		public Guid OrderItemID{get;set;}	
		#endregion

		public Money Amount{ get; set;}

		public string Reason { get; set;}

	}
}

