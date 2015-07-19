using System.Linq;
using System;
using System.Collections.Generic;
using Mise.Core.Common.Events.Checks;
using Mise.Core.Entities.Check.Events.PaymentEvents.CreditCards;
using Mise.Core.ValueItems;
using Mise.Core.Common.Events;
using Mise.Core.Entities.Check;
using Mise.Core.Entities.Payments;
using System.Collections;


namespace Mise.Core.Common.Events.Payments.CreditCards
{
	public abstract class BaseCreditCardEvent : BaseCheckEvent, ICreditCardEvent
	{
		static readonly CheckIsRejectedSpecification Rejected = new CheckIsRejectedSpecification();
		static readonly CheckIsAuthorizedSpecification Authorized = new CheckIsAuthorizedSpecification ();
		static readonly CheckIsFullyAuthorizedSpecification FullyPaid = new CheckIsFullyAuthorizedSpecification();
		public Guid PaymentID { get; set;}

		public CreditCard CreditCard {
			get;
			set;
		}

		public static ICheck SetStatusBasedOnPayments(ICheck check){
			var ccPayments = check.GetPayments().OfType<IProcessingPayment> ().ToList();

			if(Rejected.IsSatisfiedBy (ccPayments)){
				check.PaymentStatus = CheckPaymentStatus.PaymentRejected;
				return check;
			}

			if(Authorized.IsSatisfiedBy (ccPayments)){
				//check if we're fully paid
				if(FullyPaid.IsSatisfiedBy (ccPayments)){
					check.PaymentStatus = CheckPaymentStatus.Closed;
				} else {
					check.PaymentStatus = CheckPaymentStatus.PaymentApprovedWithoutTip;
				}
			} else {
				check.PaymentStatus = CheckPaymentStatus.PaymentPending;
			}
			return check;
		}

		private class CheckIsRejectedSpecification{
			public bool IsSatisfiedBy(IEnumerable<IProcessingPayment> payments){
				return payments.Any(p => p.PaymentProcessingStatus == PaymentProcessingStatus.BaseRejected 
					|| p.PaymentProcessingStatus == PaymentProcessingStatus.FullAmountRejected);
			}
		}

		private class CheckIsAuthorizedSpecification{
			public bool IsSatisfiedBy(IEnumerable<IProcessingPayment> payments){
				return payments.All (p => p.PaymentProcessingStatus == PaymentProcessingStatus.BaseAuthorized
				|| p.PaymentProcessingStatus == PaymentProcessingStatus.Complete
				|| p.PaymentProcessingStatus == PaymentProcessingStatus.SentForFullAuthorization);
			}
		}

		private class CheckIsFullyAuthorizedSpecification{
			public bool IsSatisfiedBy(IEnumerable<IProcessingPayment> payments){
				return payments.All (p => p.PaymentProcessingStatus == PaymentProcessingStatus.Complete);
			}
		}
	}
}

