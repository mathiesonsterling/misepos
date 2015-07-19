using System;
using System.Linq;
using Mise.Core.Common.Entities;
using Mise.Core.Common.Events.Payments.CreditCards;
using Mise.Core.Common.UnitTests.Tools;
using Mise.Core.Entities.Payments;
using Mise.Core.ValueItems;
using NUnit.Framework;

namespace Mise.Core.Common.UnitTests.Entities.Events.Payments.CreditCards
{
	[TestFixture]
	public class CreditCardCloseRequestedEventTests
	{

		[Test]
		public void TestSetsStatusWhenPaymentPresent(){
			var paymentID = Guid.NewGuid ();

			var barTab = new RestaurantCheck {
				ID = Guid.Empty,
				PaymentStatus = CheckPaymentStatus.PaymentApprovedWithoutTip,
			};
			barTab.AddPayment(
				new CreditCardPayment{
					ID = paymentID,
					CheckID = Guid.Empty,
					AmountCharged = new Money(100.0M),
					TipAmount = new Money(10.0M),
					Card = MockingTools.GetCreditCard ()
				}
			);

			var ev = new CreditCardCloseRequestedEvent {
				ID = Guid.Empty,
                EventOrderingID = new EventID(),
				CheckID = Guid.Empty,
				PaymentID = paymentID,
				AmountPaid = new Money(100.0M),
				TipAmount = new Money(10.0M),
				CreditCard = MockingTools.GetCreditCard (),
			};

			//ACT
            barTab.When(ev);
			var res = barTab;

			//ASSERt
			Assert.IsNotNull (res, "didn't get check back");
			Assert.AreEqual (CheckPaymentStatus.PaymentPending, res.PaymentStatus);

			var payment = res.GetPayments().First () as CreditCardPayment;
            Assert.IsNotNull(payment);
			Assert.IsNotNull (payment.Card);
		}

		[Test]
		public void TestNoPaymentThrowsException(){
			//ASSEMBLE
			var barTab = new RestaurantCheck {
				ID = Guid.Empty,
				PaymentStatus = CheckPaymentStatus.PaymentApprovedWithoutTip,
			};

			var ev = new CreditCardCloseRequestedEvent {
				ID = Guid.Empty,
                EventOrderingID = new EventID(),
				CheckID = Guid.Empty,
				AmountPaid = new Money(100.0M),
				TipAmount = new Money(10.0M),
				CreditCard = MockingTools.GetCreditCard (),
			};
				
			//ACT
		    bool threw = false;
		    try
		    {
		        barTab.When(ev);
		    }
		    catch (ArgumentException)
		    {
		        threw = true;
		    }

		    //ASSERT
			Assert.IsTrue(threw);
		}
	}
}

