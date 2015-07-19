using System;
using System.Linq;
using Mise.Core.Entities;
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
    public class CreditCardAuthorizationStartedEventTests
    {
        [Test]
        public void TestSetsCheckStatusToPaymentPending()
        {
            var checkID = Guid.NewGuid();

            var paymentID = Guid.NewGuid();
            var barTab = new RestaurantCheck
            {
                ID = checkID,
                PaymentStatus = CheckPaymentStatus.Closing,
            };
            barTab.AddOrderItem(new OrderItem
            {
                MenuItem = new MenuItem
                {
                    Price = new Money(10.0M)
                }
            });
            barTab.AddPayment(new CreditCardPayment
            {
                ID = paymentID,
                Card = new CreditCard
                {
                }
            });

            var addedEv = new CreditCardAuthorizationStartedEvent
            {
                EventOrderingID = new EventID { AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 1 },
                CheckID = checkID,
                EmployeeID = Guid.Empty,
                CreatedDate = DateTime.Now,
                DeviceID = Guid.Empty.ToString (),
                RestaurantID = Guid.Empty,
                CreditCard = MockingTools.GetCreditCard(),
                PaymentID = paymentID

            };

            //ACT
            barTab.When(addedEv);
            var res = barTab;

            //ASSERT
            Assert.IsNotNull(res);
            Assert.AreEqual(1, res.GetPayments().Count());
            Assert.AreEqual(CheckPaymentStatus.PaymentPending, res.PaymentStatus, "Check Payment status is in closing");

            var payment = res.GetPayments().First() as ICreditCardPayment;
            Assert.IsNotNull(payment, "payment");
            Assert.IsNotNull(payment.Card);

        }
    }
}
