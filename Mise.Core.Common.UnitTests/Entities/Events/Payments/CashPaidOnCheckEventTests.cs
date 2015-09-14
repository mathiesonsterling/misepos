using System;
using System.Collections.Generic;
using System.Linq;
using Mise.Core.Common.Entities;
using Mise.Core.Common.Events.Payments;
using Mise.Core.Entities.Check;
using Mise.Core.Entities.Menu;
using Mise.Core.ValueItems;
using NUnit.Framework;

namespace Mise.Core.Common.UnitTests.Entities.Events.Payments
{
    [TestFixture]
    public class CashPaidOnCheckEventTests
    {
        static ICheck GetTestCheck(decimal total)
        {
            var check = new RestaurantCheck
            {
                PaymentStatus = CheckPaymentStatus.Closing,
                OrderItems = new List<OrderItem >
                {
                    new OrderItem
                    {
                        MenuItem = new MenuItem{Price = new Money(total)}
                    }
                }
            };
            return check;
        }

        [Test]
        public void TestExactAmountDoesntAddChangeOnePayments()
        {
			var empID = Guid.NewGuid ();
            var ev = new CashPaidOnCheckEvent
            {
                EventOrder = new EventID(),
                AmountPaid = new Money(100.0M),
                ChangeGiven = Money.None,
				EmployeeID = empID
            };

            var check = new RestaurantCheck();
            
            //ACT
            check.When(ev);

            Assert.AreEqual(1, check.GetPayments().Count());
            Assert.AreEqual(new Money(100.0M), check.GetPayments().First().AmountToApplyToCheckTotal);
			Assert.AreEqual(empID, check.LastTouchedServerID);
            Assert.AreEqual(Money.None, check.ChangeDue, "Change is correct");
        }

		[Test]
		public void TestPartialAmountDoesntAddChangeOnePayments()
		{
			var empID = Guid.NewGuid ();
			var ev = new CashPaidOnCheckEvent
			{
                EventOrder = new EventID(),
				AmountPaid = new Money(10.0M),
				ChangeGiven = Money.None,
				EmployeeID = empID
			};

		    var res = GetTestCheck(100.0M);
            res.When(ev);

			Assert.IsNotNull(res);
			Assert.AreEqual(1, res.GetPayments().Count());
			Assert.AreEqual(new Money(10.0M), res.GetPayments().First().AmountToApplyToCheckTotal);
			Assert.AreEqual(empID, res.LastTouchedServerID);
			Assert.AreEqual(Money.None, res.ChangeDue, "Change is correct");
		}

        [Test]
        public void TestChangeDueAdds()
        {
			var empID = Guid.NewGuid ();
            var ev = new CashPaidOnCheckEvent
            {
                EventOrder = new EventID(),
                AmountPaid = new Money(110.0M),
                ChangeGiven = new Money(10.0M),
				EmployeeID = empID
            };
            var res = GetTestCheck(100.0M);
            res.ChangeDue = new Money(10.0M);

            //ACT
            res.When(ev);

            //ASSERT
            Assert.IsNotNull(res);
			Assert.AreEqual(1, res.GetPayments().Count());
			Assert.AreEqual(empID, res.LastTouchedServerID);
            Assert.AreEqual(new Money(20.0M), res.ChangeDue, "Change is correct");
        }
    }
}
