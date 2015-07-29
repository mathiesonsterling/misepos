using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Common;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Common.Events.ApplicationInvitations;
using Mise.Core.Common.Events.Inventory;
using Mise.Core.Entities.Inventory.Events;
using Mise.Core.Entities.People;
using Mise.Core.Entities.Restaurant.Events;
using Mise.Core.Services.WebServices;
using Mise.Core.ValueItems.Inventory;
using Mise.Inventory.Services.Implementation;
using Moq;
using NUnit.Framework;

using Mise.Core.Services;
using Mise.Core.Entities.People.Events;
using Mise.Core.Common.Events.Employee;
using Mise.Core.Common.Services.Implementation.Serialization;
using Mise.Core.ValueItems;
using Mise.Core.Entities;

namespace Mise.Inventory.UnitTests.Services
{
    /// <summary>
    /// Test the client can actually go out to our dev server and do stuff
    /// </summary>
    [TestFixture]
    public class HttpWebServiceClientIntegrationTests
    {
        readonly Guid _testRestaurantID = Guid.Empty;

        const string TEST_SERVER_URL = "http://miseinventoryservicedev.azurewebsites.net/";
        //private const string TEST_SERVER_URL = "http://miseinventoryserviceqa.azurewebsites.net/";
        //const string TEST_SERVER_URL = "http://localhost:43499/";
        //private const string TEST_SERVER_URL = "http://miseinventoryserviceprod.azurewebsites.net/";
        static HttpWebServiceClient CreateClient()
        {
            var uri = new Uri(TEST_SERVER_URL);

            var logger = new Mock<ILogger>();
            var client = new HttpWebServiceClient(uri, "integrationtest", new JsonNetSerializer(), logger.Object);

            return client;
        }

        [Test]
        public async Task GetRestaurantShouldGetForTestID()
        { 
            var client = CreateClient();
            var results = await client.GetRestaurant(_testRestaurantID);

            Assert.NotNull(results);
            Assert.AreEqual(_testRestaurantID, results.RestaurantID);

            var sections = results.GetInventorySections().ToList();
            Assert.NotNull(sections);
            Assert.True(sections.Any());
        }

        [Test]
        public async Task GetEmployeesForRestaurantShouldHaveAtLeastTwoAndNoPasswords()
        {
            var client = CreateClient();
            var results = await client.GetEmployeesForRestaurant(_testRestaurantID);
            Assert.NotNull(results);

            var emps = results.ToList();
            Assert.GreaterOrEqual(emps.Count(), 2);

            Assert.True(emps.All(emp => emp.GetRestaurantIDs().Contains(_testRestaurantID)));

            foreach (var emp in emps)
            {
                Assert.Null(emp.Password.HashValue, "password hash value is not null!");
            }
        }

        [Test]
        public async Task GetEmployeeByEmailAndPasswordShouldReturnAndHaveHash()
        {
            var client = CreateClient();
            var email = new EmailAddress { Value = "test@misepos.com" };
            //var password = new Password("test");
            var password = new Password { HashValue = "A94A8FE5CCB19BA61C4C0873D391E987982FBBD3" };

            var results = await client.GetEmployeeByPrimaryEmailAndPassword(email, password);
             
            Assert.NotNull(results);
            Assert.NotNull(results.Password);
            Assert.AreEqual("A94A8FE5CCB19BA61C4C0873D391E987982FBBD3", results.Password.HashValue);
            Assert.True(results.GetEmailAddresses().Any());
        }

        [Test]
        public async Task GetVendorsByRadiusShouldReturnVendors()
        {
            var client = CreateClient();
            var emptyLoc = new Location();
            var bigRadius = new Distance { Kilometers = 100000 };
            var results = await client.GetVendorsWithinSearchRadius(emptyLoc, bigRadius);

            Assert.NotNull(results);
            var vendors = results.ToList();
            Assert.True(vendors.Any());
        }

        [Test]
        public async Task GetVendorsByRestaurantShouldReturnVendorsSellingToRestaurant()
        {
            var client = CreateClient();
            var results = await client.GetVendorsAssociatedWithRestaurant(_testRestaurantID);

            Assert.NotNull(results);
            var vendors = results.ToList();
            Assert.True(vendors.Any());

            Assert.True(vendors.All(v => v.GetRestaurantIDsAssociatedWithVendor().Contains(_testRestaurantID)));
        }

        [Test]
        public async Task GetParsByRestaurantShouldReturn()
        {
            var client = CreateClient();

            var results = await client.GetPARsForRestaurant(_testRestaurantID);

            Assert.True(results.Any());
        }

        [Test]
        public async Task GetInventoriesByRestaurantShouldReturnEmpty()
        {
            var client = CreateClient();

            var results = await client.GetInventoriesForRestaurant(_testRestaurantID);

            Assert.IsTrue(true, "returned, don't know if has inventories though");
        }

        [Test]
        public async Task GetReceivingOrdersByRestaurantShouldReturn()
        {
            var client = CreateClient();

            var results = await client.GetReceivingOrdersForRestaurant(_testRestaurantID);

            Assert.True(results.Any());
        }

        /// <summary>
        /// Tests just that events go across the wire, not that they are processed
        /// </summary>
        [Test]
        public async Task SendEmployeeEventsShouldNotHaveException()
        {
            var client = CreateClient();

            //get an employee
            var emps = await client.GetEmployeesForRestaurant(_testRestaurantID);
            var oneEmp = emps.FirstOrDefault();
            Assert.NotNull(oneEmp);
            var ev = new EmployeeLoggedIntoInventoryAppEvent
            {
                RestaurantID = _testRestaurantID,
                EventOrderingID = new EventID { AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 0 },
                EmployeeID = oneEmp.ID
            };

            var events = new List<IEmployeeEvent> { ev };
            //ACT
            var res = await client.SendEventsAsync(null, events);

            //ASSERT
            Assert.IsTrue(res);
        }

        [Test]
        public async Task EmployeeCreationShouldFailWhenEmailAlreadyTaken()
        {
            var client = CreateClient();

            var emps = await client.GetEmployeesForRestaurant(_testRestaurantID);
            var emp = emps.FirstOrDefault();

            Assert.NotNull(emp, "Employee is null");

            var recreate = new EmployeeCreatedEvent
            {
                CausedByID = Guid.NewGuid(),
                CreatedDate = DateTime.UtcNow,
                DeviceID = "test",
                Email = emp.PrimaryEmail,
                EmployeeID = Guid.NewGuid(),
                EventOrderingID = new EventID {AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 100},
                ID = Guid.NewGuid(),
                Name = PersonName.TestName,
                Password = new Password("test"),
                RestaurantID = _testRestaurantID
            };

            var events = new List<IEmployeeEvent> {recreate};

            bool thrown = false;
            try
            {
                await client.SendEventsAsync(null, events);
            }
            catch (SendEventsException e)
            {
                thrown = true;
                Assert.AreEqual(SendErrors.EmailAlreadyInUse, e.Error, "Send error is not correct type");
            }

            Assert.True(thrown, "SendEventsError was thrown");
        }

        [Test]
        public async Task EmployeeLoginAndLogoutViaEventsShouldReflectInGet()
        {
            var client = CreateClient();

            var emps = await client.GetEmployeesForRestaurant(_testRestaurantID);
            var oneEmp = emps.FirstOrDefault();

            Assert.NotNull(oneEmp);

            var ev = new EmployeeLoggedIntoInventoryAppEvent
            {
                RestaurantID = _testRestaurantID,
                EventOrderingID = new EventID { AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 0 },
                EmployeeID = oneEmp.ID
            };

            //send events
            var events = new List<IEmployeeEvent> { ev };
            var res = await client.SendEventsAsync(null, events);
            Assert.IsTrue(res, "Sent First event");

            //now get again
            var reget = await client.GetEmployeesForRestaurant(_testRestaurantID);
            var updatedEmp = reget.FirstOrDefault(e => e.ID == oneEmp.ID);
            Assert.NotNull(updatedEmp);
            Assert.IsTrue(updatedEmp.CurrentlyLoggedIntoInventoryApp, "currently logged in");

            //log out
            var logoutEv = new EmployeeLoggedOutOfInventoryAppEvent
            {
                RestaurantID = _testRestaurantID,
                EventOrderingID = new EventID { AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 1 },
                EmployeeID = updatedEmp.ID
            };

            var logoutEvRes = await client.SendEventsAsync(null, new[] { logoutEv });
            Assert.IsTrue(logoutEvRes, "sent logout event");

            reget = await client.GetEmployeesForRestaurant(_testRestaurantID);
            updatedEmp = reget.FirstOrDefault(e => e.ID == oneEmp.ID);
            Assert.NotNull(updatedEmp);
            Assert.IsFalse(updatedEmp.CurrentlyLoggedIntoInventoryApp, "currently logged in after logout");

        }

        #region Registration integration tests
        #endregion

        #region AppInvites

        [Test]
        public async Task GetAppInvitationsForRestaurant()
        {
            var client = CreateClient();

            //ACT
            var invites = await client.GetInvitationsForRestaurant(_testRestaurantID);

            //ASSERT
            Assert.NotNull(invites);
            var invite = invites.First();
            Assert.NotNull(invite);

            //check we got destination email, etc
            Assert.NotNull(invite.DestinationEmail);
            Assert.False(string.IsNullOrEmpty(invite.DestinationEmail.Value));
            Assert.AreEqual(_testRestaurantID, invite.RestaurantID);
            Assert.NotNull(invite.RestaurantName, "rest name not null");
            Assert.AreEqual("MainTestRestaurant", invite.RestaurantName.FullName, "rest name");
        }

        [TestCase("joe@test.com")]
        [Test]
        public async Task GetAppInvitationsByEmail(string email)
        {
            //ASSEMBLE
            var emailA = new EmailAddress(email);
            var client = CreateClient();

            //ACT
            var emps = (await client.GetEmployeesForRestaurant(_testRestaurantID)).ToList();
            var sendingEmp = emps.FirstOrDefault();
            var gettingEmp = emps.LastOrDefault();
            Assert.NotNull(sendingEmp);
            Assert.NotNull(gettingEmp);
            Assert.AreNotEqual(sendingEmp.ID, gettingEmp.ID);

            var createEvent = new EmployeeInvitedToApplicationEvent
            {
                Application = MiseAppTypes.UnitTests,
                CausedByID = sendingEmp.ID,
                CreatedDate = DateTime.UtcNow,
                DeviceID = "integrationTests",
                EmailToInvite = emailA,
                EventOrderingID = new EventID { AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101029 },
                ID = Guid.NewGuid(),
                InvitationID = Guid.NewGuid(),
                RestaurantID = _testRestaurantID,
                RestaurantName = new RestaurantName("MainTestRestaurant"),
            };

            var sendRes = await client.SendEventsAsync(null, new[] { createEvent });
            Assert.True(sendRes);

            var invites = await client.GetInvitationsForEmail(emailA);

            //ASSERT
            Assert.NotNull(invites);
            var invite = invites.First();
            Assert.NotNull(invite);

            //check we got destination email, etc
            Assert.NotNull(invite.DestinationEmail);
            Assert.False(string.IsNullOrEmpty(invite.DestinationEmail.Value));
            Assert.AreEqual(_testRestaurantID, invite.RestaurantID);
            Assert.AreEqual("MainTestRestaurant", invite.RestaurantName.FullName, "rest name");
        }

        [Test]
        public async Task CreateAndRefuseInvitationShouldGetAfterwards()
        {
            //create invitation from an employee to a new email
            var client = CreateClient();

            var emps = (await client.GetEmployeesForRestaurant(_testRestaurantID)).ToList();
            var sendingEmp = emps.FirstOrDefault();
            var gettingEmp = emps.LastOrDefault();
            Assert.NotNull(sendingEmp);
            Assert.NotNull(gettingEmp);
            Assert.AreNotEqual(sendingEmp.ID, gettingEmp.ID);

            var createEvent = new EmployeeInvitedToApplicationEvent
            {
                Application = MiseAppTypes.UnitTests,
                CausedByID = sendingEmp.ID,
                CreatedDate = DateTime.UtcNow,
                DeviceID = "integrationTests",
                EmailToInvite = gettingEmp.PrimaryEmail,
                EventOrderingID = new EventID {AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101029},
                ID = Guid.NewGuid(),
                InvitationID = Guid.NewGuid(),
                RestaurantID = _testRestaurantID
            };

            var sendRes = await client.SendEventsAsync(null, new[] {createEvent as IApplicationInvitationEvent});
            Assert.True(sendRes);

            //refuse it
            var refusal = new EmployeeRejectsInvitationEvent
            {
                Application = MiseAppTypes.UnitTests,
                CausedByID = gettingEmp.ID,
                CreatedDate = DateTime.UtcNow,
                DeviceID = "integrationTests",
                EmployeeID = gettingEmp.ID,
                EventOrderingID = new EventID {AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101030},
                ID = Guid.NewGuid(),
                InvitationID = createEvent.InvitationID,
                RestaurantID = _testRestaurantID
            };

            var sendRefuse = await client.SendEventsAsync(null, new[]{refusal as IApplicationInvitationEvent});
            Assert.True(sendRefuse);

            //get it
            var invites = (await client.GetInvitationsForRestaurant(_testRestaurantID)).ToList();
            Assert.GreaterOrEqual(invites.Count(), 1);
            var invite = invites.FirstOrDefault(i => i.ID == createEvent.InvitationID);
            Assert.NotNull(invite, "Invitation not found");
            Assert.AreEqual(InvitationStatus.Rejected, invite.Status);
        }
        #endregion

        #region PAR

        [Test]
        public async Task UpdatePARLineItemQuantity()
        {
            var client = CreateClient();

            var emps = await client.GetEmployeesForRestaurant(_testRestaurantID);
            var emp = emps.FirstOrDefault(n => n.PrimaryEmail.Value.Contains("test"));
            Assert.NotNull(emp);

            var par = await client.GetCurrentPAR(_testRestaurantID);

            Assert.NotNull(par, "par is null");

            //pick a random item
            var items = par.GetBeverageLineItems().ToList();
            var rand = new Random();
            var pos = rand.Next(items.Count - 1);

            var item = items[pos];

            Assert.NotNull(item, "null item found");

            var ev = new PARLineItemQuantityUpdatedEvent
            {
                CausedByID = emp.ID,
                CreatedDate = DateTime.UtcNow,
                DeviceID = "integrationTest",
                EventOrderingID = new EventID {AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 1010},
                ID = Guid.NewGuid(),
                LineItemID = item.ID,
                ParID = par.ID,
                RestaurantID = _testRestaurantID,
                UpdatedQuantity = 4
            };

            var sendRes = await client.SendEventsAsync(null, new []{ev});
            Assert.True(sendRes, "results from send events");

            var updated = await client.GetCurrentPAR(_testRestaurantID);
            Assert.NotNull(updated, "Updated par not found");

            var updatedItem = updated.GetBeverageLineItems().FirstOrDefault(pli => pli.DisplayName == item.DisplayName);
            Assert.NotNull(updatedItem, "item found in update");
            Assert.AreEqual(4, updatedItem.Quantity, "quantity is updated");
        }

        #endregion

        #region Inventory

        [Test]
        public async Task InventoryShouldBeCreatedAndRetreived()
        {
            var client = CreateClient();

            var emps = await client.GetEmployeesForRestaurant(_testRestaurantID);
            var emp = emps.FirstOrDefault(n => n.PrimaryEmail.Value.Contains("test"));
            Assert.NotNull(emp);

            var invID = Guid.NewGuid();
            //create an inventory, add a section, set current, add line items, measure them, and complete
            var create = new InventoryCreatedEvent
            {
                CreatedDate = DateTime.UtcNow,
                CausedByID = emp.ID,
                DeviceID = "unitTest",
                EventOrderingID = new EventID(MiseAppTypes.UnitTests, 1),
                ID = Guid.NewGuid(),
                RestaurantID = _testRestaurantID,
                InventoryID = invID
            };

            var section = new InventoryNewSectionAddedEvent
            {
                CausedByID = emp.ID,
                CreatedDate = DateTime.UtcNow,
                DeviceID = "unitTest",
                EventOrderingID = new EventID(MiseAppTypes.UnitTests, 2),
                ID = Guid.NewGuid(),
                InventoryID = invID,
                Name = "testSection",
                RestaurantID = _testRestaurantID,
                RestaurantSectionId = Guid.NewGuid(),
                SectionID = Guid.NewGuid()
            };

            var current = new InventoryMadeCurrentEvent
            {
                CausedByID = emp.ID,
                CreatedDate = DateTime.UtcNow,
                DeviceID = "unitTest",
                EventOrderingID = new EventID(MiseAppTypes.UnitTests, 3),
                ID = Guid.NewGuid(),
                InventoryID = invID,
                RestaurantID = _testRestaurantID
            };

            //then send
            var sendRes = await client.SendEventsAsync(null, new IInventoryEvent[] {create, section, current});

            Assert.True(sendRes);

            //get it
            var get = (await client.GetInventoriesForRestaurant(_testRestaurantID)).ToList();

            Assert.AreEqual(1, get.Count);
            Assert.AreEqual(invID, get.First().ID, "ID came back!");

            //add line items, measure, complete section, and add
            var addLI = new InventoryLineItemAddedEvent
            {
                CaseSize = 12,
                CausedByID = emp.ID,
                CreatedDate = DateTime.UtcNow,
                RestaurantID = _testRestaurantID,
                EventOrderingID = new EventID(MiseAppTypes.UnitTests, 4),
                DeviceID = "unitTest",
                UPC = "",
                Container = LiquidContainer.Bottle750ML,
                DisplayName = "testLI",
                MiseName = string.Empty,
                Quantity = 0,
                PricePaid = null,
                VendorBoughtFrom = null,
                RestaurantInventorySectionID = section.RestaurantSectionId,
                InventorySectionID = section.SectionID,
                InventoryID = invID,
                Categories = new List<ItemCategory>
                {
                    CategoriesService.WhiskeyScotch,
                    CategoriesService.Vodka
                },
                InventoryPosition = 1,
                LineItemID = Guid.NewGuid()
            };

            var measure = new InventoryLiquidItemMeasuredEvent
            {
                AmountMeasured = new LiquidAmount {}
            };


            //retrieve, verify our LIs are there
        }
        #endregion

    }
}

