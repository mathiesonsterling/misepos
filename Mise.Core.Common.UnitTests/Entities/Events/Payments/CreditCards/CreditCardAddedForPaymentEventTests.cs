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
	public class CreditCardAddedForPaymentEventTests
	{
		[Test]
		public void TestPaymentIsCreatedAndSetsCreditCard(){
			var checkID = Guid.NewGuid ();

			var barTab = new RestaurantCheck {
				Id = checkID,
				PaymentStatus = CheckPaymentStatus.Closing,
			};
			barTab.AddOrderItem (new OrderItem {
				MenuItem = new MenuItem {
					Price = new Money (10.0M)
				}
			});

			var addedEv = new CreditCardAddedForPaymentEvent {
				EventOrder = new EventID{AppInstanceCode = Mise.Core.Entities.MiseAppTypes.UnitTests, OrderingID = 1},
				CheckID = checkID,
				EmployeeID = Guid.Empty,
				Amount = new Money (10.0M),
				CreatedDate = DateTime.Now,
				DeviceId = Guid.Empty.ToString (),
				RestaurantId = Guid.Empty,
				CreditCard = MockingTools.GetCreditCard ()

			};

			//ACT
            barTab.When(addedEv);

			//ASSERT
			Assert.IsNotNull (barTab);
			Assert.AreEqual (1, barTab.GetPayments().Count ());
            Assert.AreEqual(CheckPaymentStatus.Closing, barTab.PaymentStatus, "Check Payment status is in closing");

			var payment = barTab.GetPayments().First () as ICreditCardPayment;
			Assert.IsNotNull (payment, "payment");
			Assert.IsNotNull (payment.Card);

            Assert.AreEqual(1, barTab.CreditCards.Count(), "Credit card on entity");
		}
	}
}

