using System;
using System.Linq;
using Mise.Core.Common.Entities;
using Mise.Core.Common.Events.Payments.CreditCards;
using Mise.Core.Entities.Payments;
using Mise.Core.ValueItems;
using NUnit.Framework;

namespace Mise.Core.Common.UnitTests.Entities.Events.Payments.CreditCards
{
	[TestFixture]
	public class CreditCardAuthorizationCancelledEventTests
	{
		[Test]
		public void TestApplySetsPaymentStatusAndCheckStatus(){
			var paymentID = Guid.NewGuid ();
			var empId = Guid.NewGuid ();

			var barTab = new RestaurantCheck {
				Id = Guid.Empty,
				PaymentStatus = CheckPaymentStatus.PaymentApprovedWithoutTip,
			};

			var payment = new CreditCardPayment {
				Id = paymentID,
				CheckID = Guid.Empty,
				AmountCharged = new Money (100.0M),
				TipAmount = new Money (10.0M),
				PaymentProcessingStatus = PaymentProcessingStatus.BaseAuthorized,
				AuthorizationResult = new CreditCardAuthorizationCode {
					IsAuthorized = true
				}
			};

			barTab.AddPayment(payment);

			var ev = new CreditCardAuthorizationCancelledEvent {
				Id = Guid.Empty,
                EventOrder = new EventID(),
				CheckID = Guid.Empty,
				PaymentID = paymentID,
				EmployeeID = empId,
			};

			//ACT
            barTab.When(ev);
			var res = barTab;

			//ASSERT
			Assert.AreEqual (CheckPaymentStatus.Closing, res.PaymentStatus, "check payment status");
			Assert.IsFalse (res.GetPayments().Any());
			Assert.AreEqual (empId, res.LastTouchedServerID, "employee id set on check");

			Assert.IsNotNull (payment);
			Assert.AreEqual (payment.PaymentProcessingStatus, PaymentProcessingStatus.Cancelled, "payment status");
		}

		[Test]
		public void TestApplyToNonPresentPaymentThrows(){
			var paymentID = Guid.NewGuid ();
			var empId = Guid.NewGuid ();

			var barTab = new RestaurantCheck {
				Id = Guid.Empty,
				PaymentStatus = CheckPaymentStatus.PaymentApprovedWithoutTip,
			};
				

			var ev = new CreditCardAuthorizationCancelledEvent {
				Id = Guid.Empty,
				CheckID = Guid.Empty,
				PaymentID = paymentID,
				EmployeeID = empId,
			};

			//ACT
			var thrown = false;
			try{
                barTab.When(ev);
			} catch(Exception){
				thrown = true;
			}

			//ASSERT
			Assert.IsTrue (thrown);
		}
	}
}

