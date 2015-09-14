using System;
using Mise.Core.ValueItems;

namespace Mise.Core.Entities.Payments
{
	public class CreditCardPayment : BasePayment, ICreditCardPayment
	{
		public CreditCard Card { get; set;}

	    public override IPayment Clone()
	    {
	        return new CreditCardPayment
	        {
                Id = Guid.NewGuid(),
                CheckID = CheckID,
                EmployeeID = EmployeeID,
	            AmountCharged = AmountCharged,
	            TipAmount = TipAmount,
	            AuthorizationResult = AuthorizationResult,
	            PaymentProcessingStatus = PaymentProcessingStatus,
	            FinalAuthorizationCode = FinalAuthorizationCode,
	        };
	    }
		public Money AmountCharged {
			get;
			set;
		}
		public Money TipAmount {
			get;
			set;
		}
		public CreditCardAuthorizationCode AuthorizationResult {
			get;
			set;
		}
		public PaymentProcessingStatus PaymentProcessingStatus {
			get;
			set;
		}
		public CreditCardAuthorizationCode FinalAuthorizationCode {
			get;
			set;
		}
			
		public override PaymentType PaymentType {
			get { return PaymentType.InternalCreditCard;}
		}

		public override Money AmountToApplyToCheckTotal {
			get {
				if(PaymentProcessingStatus == PaymentProcessingStatus.BaseRejected || 
					PaymentProcessingStatus == PaymentProcessingStatus.FullAmountRejected){
					return Money.None;
				}
				return AmountCharged;
			}
		}

		public override Money DisplayAmount {
			get {
				return AmountCharged;
			}
		}

		/// <summary>
		/// Credit card payments don't open the cash drawer until the final close, so they're false here
		/// </summary>
		/// <value>true</value>
		/// <c>false</c>
		public override bool OpensCashDrawer {
			get {
				return false;
			}
		}
	}
}

