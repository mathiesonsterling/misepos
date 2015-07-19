using System;
using Mise.Core.Common.Entities;
using Mise.Core.Common.Events;
using Mise.Core.Common.Events.Employee;
using Mise.Core.Entities.People;
using Mise.Core.ValueItems;
using NUnit.Framework;

namespace Mise.Core.Common.UnitTests.Entities
{
    [TestFixture]
    public class EmployeeTests
    {
        [Test]
        public void TestInsufficientPermissionsEvent()
        {
            var employee = new Employee();

            var ev = new InsufficientPermissionsEvent{EventOrderingID = new EventID()};

            //ACT
            employee.When(ev);

            //ASSERT
            Assert.IsTrue(true, "No exception thrown");
        }

        [Test]
        public void TestNoSaleEvent()
        {
            var employee = new Employee();

            var ev = new NoSaleEvent{EventOrderingID = new EventID()};

            //ACT
            employee.When(ev);

            //ASSERT
            Assert.IsTrue(true, "No exception thrown");
        }

        [Test]
        public void TestCloneEmpty()
        {
            var employee = new Employee();

            //ACT
            var res = employee.Clone();

            //Assert
            Assert.IsNotNull(res, "No exception thrown!");
        }

        [Test]
        public void TestCloneInventoryAppFields()
        {
            var employee = new Employee
            {
                ID = Guid.NewGuid(),
                LastTimeLoggedIntoInventoryApp = DateTime.Now,
                CurrentlyLoggedIntoInventoryApp = true,
                CurrentlyClockedInToPOS = true,
                Password = new Password { HashValue = "blabla"}
            };

            //ACT
            var res = employee.Clone() as IEmployee;

            //Assert
            Assert.IsNotNull(res, "No exception thrown!");
            Assert.AreEqual(employee.CurrentlyLoggedIntoInventoryApp, res.CurrentlyLoggedIntoInventoryApp);
            Assert.AreEqual(employee.ID, res.ID);
            Assert.AreEqual(employee.LastTimeLoggedIntoInventoryApp, res.LastTimeLoggedIntoInventoryApp);
            Assert.AreEqual(employee.Password, res.Password);
        }

        [Test]
        public void TestCompBudgetIsValueWithClone()
        {
            var employee = new Employee {CompBudget = new Money(10.0M)};

            //ACT
            var res = employee.Clone() as IEmployee;
            Assert.IsNotNull(res);
            var oldMon = res.CompBudget.Subtract(new Money(6.0M));

            //ASSERT
            Assert.AreEqual(4.0M, oldMon.Dollars);
            Assert.AreEqual(10.0M, employee.CompBudget.Dollars, "Original did not change comp budget");
        }

        [Test]
        public void TestInventoryAppLogin()
        {
            var emp = new Employee{CurrentlyLoggedIntoInventoryApp = false};

            var eventDate = DateTimeOffset.UtcNow.AddDays(-2);
            var ev = new EmployeeLoggedIntoInventoryAppEvent
            {
                CreatedDate = eventDate
            };

            //ACT
            emp.When(ev);

            //ASSERT
            Assert.IsTrue(emp.CurrentlyLoggedIntoInventoryApp);
            Assert.AreEqual(eventDate, emp.LastTimeLoggedIntoInventoryApp);
        }
    }
}
