using Mise.Core.ValueItems;

namespace Mise.Core.Entities.Payments
{
    /// <summary>
    /// Payment where we're comping a money amount, not just items
    /// </summary>
	public class CompAmountPayment : BasePayment, ICompPayment
    {
        public override IPayment Clone()
        {
            return new CompAmountPayment
            {
                CheckID = CheckID,
                EmployeeID = EmployeeID,
                AmountComped = AmountComped,
                Reason = Reason
            };
        }

        public override PaymentType PaymentType
        {
            get { return PaymentType.CompAmount; }
        }

        public override Money AmountToApplyToCheckTotal
        {
            get { return AmountComped; }
        }
			
		public override bool OpensCashDrawer {
			get {
				return false;
			}
		}
			

		public override Money DisplayAmount{get{return AmountComped;}}

        public Money AmountComped { get; set; }
	    public string Reason { get; set; }
    }
}
