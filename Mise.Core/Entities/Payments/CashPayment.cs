using System;
using Mise.Core.ValueItems;

namespace Mise.Core.Entities.Payments
{
	public class CashPayment : BasePayment, ICashPayment
	{
	    public override IPayment Clone()
	    {
	        return new CashPayment
	        {
                ID = Guid.NewGuid(),
                CheckID = CheckID,
                EmployeeID = EmployeeID,
	            AmountPaid = AmountPaid,
	            AmountTendered = AmountTendered,
	            ChangeGiven = ChangeGiven,
	        };
	    }
	    public override PaymentType PaymentType
	    {
	        get { return PaymentType.Cash; }
	    }

	    public override Money AmountToApplyToCheckTotal {
			get{ 
				return AmountPaid;
			}
		}

		public override Money DisplayAmount{get{return AmountPaid;}}

		public Money AmountPaid{ get; set; }
		public Money AmountTendered {
			get;
			set;
		}

		public Money ChangeGiven {
			get;
			set;
		}
			
		public override bool OpensCashDrawer {
			get {
				return true;
			}
		}
			
	}
}

