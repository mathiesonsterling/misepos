using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Common.Entities;
using Mise.Core.Entities;
using Mise.Core.Services;
using Mise.Core.ValueItems;
using Mise.Neo4J.Neo4JDAL;
using Moq;
using NUnit.Framework;

namespace Mise.Database.Neo4J.IntegrationTests
{
    [TestFixture]
    public class EmployeeTests
    {
        private Neo4JEntityDAL _underTest;

        [SetUp]
        public void Setup()
        {
            var logger = new Mock<ILogger>();
            _underTest = new Neo4JEntityDAL(TestUtilities.GetConfig(), logger.Object);

            _underTest.ResetDatabase();
        }

        [Test]
        public async Task EmployeeWithoutRestaurantShouldCRUD()
        {

            var empID = Guid.NewGuid();
            var employee = new Employee
            {
                CanCompAmount = true,
                CompBudget = new Money(1.0M),
                CreatedDate = DateTime.UtcNow.AddDays(-1),
                CurrentlyClockedInToPOS = true,
                CurrentlyLoggedIntoInventoryApp = true,
                DisplayName = "testEmployee",
                Name = PersonName.TestName,
                HiredDate = DateTime.UtcNow.AddMonths(-1),
                ID = empID,
                LastTimeLoggedIntoInventoryApp = DateTime.UtcNow.AddDays(-1),
                LastUpdatedDate = DateTime.UtcNow.AddDays(-1),
                Passcode = "1111",
                Password = new Password { HashValue = "firstPass" },
                PreferredColorName = "blue",
                PrimaryEmail = new EmailAddress { Value = "prim1@test.com" },
                Revision = new EventID { AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 1 },
                Emails = new List<EmailAddress>
                {
                    new EmailAddress{Value = "prim1@test.com"},
                    new EmailAddress{Value = "other1@test.com"}
                }
            };

            //ACT
            //create
            await _underTest.AddEmployeeAsync(employee);


            //retrieve
            var getFirstRes = (await _underTest.GetInventoryAppUsingEmployeesAsync()).ToList();

            Assert.NotNull(getFirstRes);
            Assert.AreEqual(1, getFirstRes.Count());
            var retEmp = getFirstRes.First();

            Assert.AreEqual(employee.CanCompAmount, retEmp.CanCompAmount);
            Assert.AreEqual(employee.CompBudget, retEmp.CompBudget, "CompBudget");
            Assert.AreEqual(employee.CreatedDate, retEmp.CreatedDate, "CreatedDate");
            Assert.AreEqual(employee.CurrentlyLoggedIntoInventoryApp, retEmp.CurrentlyLoggedIntoInventoryApp, "CurrentlyLoggedIntoInventoryApp");
            Assert.AreEqual(employee.DisplayName, retEmp.DisplayName, "Display name");

            Assert.AreEqual(employee.Name, retEmp.Name);
            Assert.AreEqual(employee.ID, retEmp.ID);
            Assert.AreEqual(employee.LastTimeLoggedIntoInventoryApp, retEmp.LastTimeLoggedIntoInventoryApp);
            Assert.AreEqual(employee.Password, retEmp.Password);

            //update
            var empForUpdate = new Employee
            {
                CanCompAmount = false,
                CompBudget = new Money(2.0M),
                CreatedDate = DateTime.UtcNow,
                CurrentlyClockedInToPOS = false,
                CurrentlyLoggedIntoInventoryApp = false,
                DisplayName = "testEmployeeUpdated",
                Name = new PersonName("Updated", "Jumpy", "Vienn"),
                HiredDate = DateTime.UtcNow,
                ID = empID,
                LastTimeLoggedIntoInventoryApp = DateTime.UtcNow,
                LastUpdatedDate = DateTime.UtcNow,
                Passcode = "2222",
                Password = new Password { HashValue = "secondPass" },
                PreferredColorName = "pink",
                PrimaryEmail = new EmailAddress { Value = "primUpdated@test.com" },
                Revision = new EventID { AppInstanceCode = MiseAppTypes.DummyData, OrderingID = 100 },
                Emails = new List<EmailAddress>
                {
                    new EmailAddress{Value = "primUpdated@test.com"},
                    new EmailAddress{Value = "other1@test.com"}
                }
            };

            await _underTest.UpdateEmployeeAsync(empForUpdate);

            var updateRes = (await _underTest.GetInventoryAppUsingEmployeesAsync()).ToList();

            Assert.NotNull(updateRes);
            Assert.AreEqual(1, updateRes.Count());
            var updatedEmp = updateRes.FirstOrDefault();
            Assert.NotNull(updatedEmp);

            Assert.AreEqual(empForUpdate.CanCompAmount, updatedEmp.CanCompAmount);
            Assert.AreEqual(empForUpdate.CompBudget, updatedEmp.CompBudget, "CompBudget");
            Assert.AreEqual(empForUpdate.CreatedDate, updatedEmp.CreatedDate, "CreatedDate");
            Assert.AreEqual(empForUpdate.CurrentlyLoggedIntoInventoryApp, updatedEmp.CurrentlyLoggedIntoInventoryApp, "CurrentlyLoggedIntoInventoryApp");
            Assert.AreEqual(empForUpdate.DisplayName, updatedEmp.DisplayName, "Display name");

            Assert.AreEqual(empForUpdate.Name, updatedEmp.Name);
            Assert.AreEqual(empForUpdate.ID, updatedEmp.ID);
            Assert.AreEqual(empForUpdate.LastTimeLoggedIntoInventoryApp, updatedEmp.LastTimeLoggedIntoInventoryApp);
            Assert.AreEqual(empForUpdate.Password, updatedEmp.Password);

            //check our emails got properly associated as well
            Assert.AreEqual(empForUpdate.Emails.Count(), updatedEmp.GetEmailAddresses().Count(), "Count of emails");
            Assert.True(updatedEmp.GetEmailAddresses().Select(e => e.Value).Contains("primUpdated@test.com"), "updated email");
            Assert.False(updatedEmp.GetEmailAddresses().Select(e => e.Value).Contains("prim1@test.com"), "has original email");
        }

        [Test]
        public async Task EmployeeWithRestaurantShouldAssociate()
        {
            var emp = TestUtilities.CreateEmployee();

            var rest = TestUtilities.CreateRestaurant();

            await _underTest.AddRestaurantAsync(rest);

            emp.RestaurantsAndAppsAllowed.Add(rest.ID, new List<MiseAppTypes>{MiseAppTypes.UnitTests});
            emp.HiredDate = DateTime.Now;

            await _underTest.AddEmployeeAsync(emp);

            var getRes = await _underTest.GetEmployeeByIDAsync(emp.ID);

            Assert.NotNull(getRes);
            Assert.AreEqual(1, getRes.GetRestaurantIDs(MiseAppTypes.UnitTests).Count());
            Assert.AreEqual(rest.ID, getRes.GetRestaurantIDs(MiseAppTypes.UnitTests).First());
        }

        [Test]
        public async Task EmployeeWithMultipleAppsShouldAssociateAllApps()
        {
            var rest = TestUtilities.CreateRestaurant();
            var emp = TestUtilities.CreateEmployee();
            emp.RestaurantsAndAppsAllowed = new Dictionary<Guid, IList<MiseAppTypes>>
            {
                {rest.ID, new []{MiseAppTypes.UnitTests, MiseAppTypes.DummyData}}
            };

            await _underTest.AddRestaurantAsync(rest);

            emp.HiredDate = DateTime.Now;

            await _underTest.AddEmployeeAsync(emp);

            var getRes = await _underTest.GetEmployeeByIDAsync(emp.ID);

            Assert.NotNull(getRes);
            Assert.AreEqual(1, getRes.GetRestaurantIDs(MiseAppTypes.UnitTests).Count());
            Assert.AreEqual(rest.ID, getRes.GetRestaurantIDs(MiseAppTypes.UnitTests).First());

            var apps = getRes.GetAppsEmployeeCanUse(rest.ID).ToList();
            Assert.AreEqual(2, apps.Count);
            Assert.True(apps.Contains(MiseAppTypes.UnitTests));
            Assert.True(apps.Contains(MiseAppTypes.DummyData));
        }
    }
}
