using System;
using System.Linq;
using Mise.Core.Common.Entities;
using Mise.Core.Common.Events.Payments.CreditCards;
using Mise.Core.Common.UnitTests.Tools;
using Mise.Core.Entities.Check;
using Mise.Core.Entities.Menu;
using Mise.Core.Entities.Payments;
using Mise.Core.ValueItems;
using NUnit.Framework;

namespace Mise.Core.Common.UnitTests.Entities.Events.Payments.CreditCards
{
	[TestFixture]
	public class CreditCardAuthorizedEventTests
	{
		[Test]
		public void TestSetsCheckAndPaymentStatus(){
			var paymentID = Guid.NewGuid ();

			var barTab = new RestaurantCheck {
				Id = Guid.Empty,
				PaymentStatus = CheckPaymentStatus.PaymentPending,
			};
			barTab.AddOrderItem(new OrderItem {
				MenuItem = new MenuItem {
					Price = new Money (10.0M)
				}
			});
			barTab.AddPayment(
				new CreditCardPayment{
					Id = paymentID,
					CheckID = Guid.Empty,
					AmountCharged = new Money(10.0M),
					PaymentProcessingStatus = PaymentProcessingStatus.SentForBaseAuthorization
				}
			);

			var authCode = new CreditCardAuthorizationCode {
				AuthorizationKey = "testKey",
				PaymentProviderName = "test"
			};
			var ev = new CreditCardAuthorizedEvent {
				Id = Guid.Empty,
				CheckID = Guid.Empty,
                EventOrder = new EventID(),
				PaymentID = paymentID,
				Amount = new Money(10.0M),
				AuthorizationCode = authCode,
				CreditCard = MockingTools.GetCreditCard()
			};

			//ACT
            barTab.When(ev);
		    var res = barTab;

			Assert.IsNotNull (res);
			Assert.AreEqual (CheckPaymentStatus.PaymentApprovedWithoutTip, res.PaymentStatus);
			Assert.AreEqual (1, res.GetPayments().Count());

			var payment = res.GetPayments().First () as IProcessingPayment;
			Assert.IsNotNull (payment);
			Assert.AreEqual (PaymentProcessingStatus.BaseAuthorized, payment.PaymentProcessingStatus);

			var checkPayment = payment as ICreditCardPayment;
			Assert.IsNotNull (checkPayment);
			Assert.IsNotNull (checkPayment.AuthorizationResult);
			Assert.AreEqual ("testKey", checkPayment.AuthorizationResult.AuthorizationKey);
			Assert.AreEqual ("test", checkPayment.AuthorizationResult.PaymentProviderName);
		}

		[Test]
		public void TestUnmatchedPaymentThrows(){
			var paymentID = Guid.NewGuid ();

			var barTab = new RestaurantCheck {
				Id = Guid.Empty,
				PaymentStatus = CheckPaymentStatus.PaymentPending,
			};
			barTab.AddOrderItem (new OrderItem {
				MenuItem = new MenuItem {
					Price = new Money (10.0M)
				}
			});
			barTab.AddPayment(
				new CreditCardPayment{
					Id = paymentID,
					CheckID = Guid.Empty,
					AmountCharged = new Money(10.0M),
					PaymentProcessingStatus = PaymentProcessingStatus.SentForBaseAuthorization
				}
			);

			var ev = new CreditCardAuthorizedEvent {
				Id = Guid.Empty,
                EventOrder = new EventID(),
				CheckID = Guid.Empty,
				PaymentID = Guid.NewGuid (),
				Amount = new Money(10.0M),
				CreditCard = MockingTools.GetCreditCard()
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

		    Assert.IsTrue (threw);
		}
	}
}

