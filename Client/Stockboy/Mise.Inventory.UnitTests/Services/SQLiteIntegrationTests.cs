using System;
using System.Collections.Generic;
using System.Linq;
using Mise.Core.Common.Entities;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.ValueItems.Inventory;
using Mono.Security.Cryptography;
using NUnit.Framework;
using System.Threading.Tasks;
using Mise.Core.Common.Events.Inventory;
using Mise.Core.ValueItems;
using Mise.Core.Entities;
using Mise.Core.Common.Events.DTOs;
using Mise.Core.Common.Services.Implementation.Serialization;

namespace Mise.Inventory.UnitTests.Services
{
	[TestFixture]
	public class SQLiteIntegrationTests
	{
		[Test]
		public async Task StoreNotSentEventAndGet(){
			var underTest = TestUtilities.GetTestSQLDB ();

			var ev = new InventoryCreatedEvent {
				ID = Guid.NewGuid (),
				CausedByID = Guid.NewGuid (),
				CreatedDate = DateTime.UtcNow,
				DeviceID = "testDevID",
				EventOrderingID = new EventID{ AppInstanceCode = MiseAppTypes.UnitTests },
				InventoryID = Guid.NewGuid (),
				RestaurantID = Guid.NewGuid (),
				
			};

			var evFactory = new EventDataTransportObjectFactory (new JsonNetSerializer ());


			//ACT
		    var dto = evFactory.ToDataTransportObject(ev);
			await underTest.AddEventsThatFailedToSend (new []{dto});

			var pulled = (await underTest.GetUnsentEvents ()).ToList();

			//ASSERT
			Assert.NotNull (pulled);
			Assert.AreEqual (1, pulled.Count);

			var first = pulled.First();
			Assert.AreEqual (ev.ID, first.ID, "ID");
			Assert.AreEqual (ev.CreatedDate, first.CreatedDate, "CreatedDate");
			Assert.AreEqual (first.CausedByID, ev.CausedByID, "CausedBy");
			Assert.AreEqual (ev.DeviceID, first.DeviceID, "DeviceID");
			Assert.True (ev.EventOrderingID.Equals (first.EventOrderingID), "EventOrderingID");

			//TODO transform back to event to check inventory and restaurant
			var actualItem = evFactory.ToInventoryEvent (first);

			Assert.NotNull (actualItem);
			Assert.AreEqual (ev.InventoryID, actualItem.InventoryID);
			Assert.AreEqual (ev.RestaurantID, actualItem.RestaurantID);
			Assert.AreEqual (MiseEventTypes.InventoryCreated, actualItem.EventType);

            Assert.AreEqual(ev.ID, actualItem.ID, "ID");
            Assert.AreEqual(ev.CreatedDate, actualItem.CreatedDate, "CreatedDate");
            Assert.AreEqual(actualItem.CausedByID, ev.CausedByID, "CausedBy");
            Assert.AreEqual(ev.DeviceID, actualItem.DeviceID, "DeviceID");
            Assert.True(ev.EventOrderingID.Equals(actualItem.EventOrderingID), "EventOrderingID");
		}

	    [Test]
	    public async Task StoreAndRetrieveEmployee()
	    {
	        var underTest = TestUtilities.GetTestSQLDB();

	        var emp = new Employee
	        {
	            ID = Guid.NewGuid(),
	            CanCompAmount = true,
	            CompBudget = Money.MiseMonthlyFee,
	            CreatedDate = DateTime.UtcNow,
	            CurrentlyLoggedIntoInventoryApp = true,
	            CurrentlyClockedInToPOS = true,
	            DisplayName = "testEmp",
	            Emails = new List<EmailAddress>
	            {
	                EmailAddress.TestEmail,
	                new EmailAddress("another@test.com")
	            },
	            EmployeeIconUri = new Uri("http://mise.in/logo.png"),
	            HiredDate = DateTimeOffset.UtcNow,
	            LastDeviceIDLoggedInWith = "testDevice",
	            LastTimeLoggedIntoInventoryApp = DateTime.UtcNow,
	            Name = PersonName.TestName,
	            LastUpdatedDate = DateTime.UtcNow,
	            Passcode = "1111",
	            Password = Password.TestPassword,
	            PreferredColorName = "blue",
	            PrimaryEmail = EmailAddress.TestEmail,
	            RestaurantsAndAppsAllowed = new Dictionary<Guid, IList<MiseAppTypes>>
	            {
	                {Guid.NewGuid(), new List<MiseAppTypes> {MiseAppTypes.UnitTests, MiseAppTypes.DummyData}}
	            },
	            Revision = new EventID {AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 10011},
	        };

            //ACT
	        await underTest.UpsertEntitiesAsync(new[] {emp});
	        var returned = (await underTest.GetEntitiesAsync<Employee>()).ToList();

            //ASSERT
            Assert.NotNull(returned);
            Assert.AreEqual(1, returned.Count());

	        var first = returned.First();

            Assert.AreEqual(emp.ID, first.ID, "ID");
            Assert.True(emp.PrimaryEmail.Equals(first.PrimaryEmail), "Primary email");

            Assert.AreEqual(emp.CurrentlyLoggedIntoInventoryApp, first.CurrentlyLoggedIntoInventoryApp);
            Assert.AreEqual(emp.ID, first.ID);
            Assert.AreEqual(emp.LastTimeLoggedIntoInventoryApp, first.LastTimeLoggedIntoInventoryApp);
            Assert.AreEqual(emp.Password, first.Password);
	    }

	    [Test]
	    public async Task StoreAndRetrieveInventory()
	    {
	        var underTest = TestUtilities.GetTestSQLDB();

            var inventory = new Core.Common.Entities.Inventory.Inventory
            {
                Revision = new EventID { AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 0 },
                CreatedByEmployeeID = Guid.NewGuid(),
                CreatedDate = DateTimeOffset.UtcNow.AddDays(-1),
                LastUpdatedDate = DateTimeOffset.UtcNow,
                ID = Guid.NewGuid(),
                DateCompleted = DateTimeOffset.UtcNow,
                RestaurantID = Guid.NewGuid(),
                IsCurrent = true,
                Sections = new List<InventorySection>{
                    new InventorySection{
                        ID = Guid.NewGuid(),
                        RestaurantInventorySectionID = Guid.NewGuid(),
                        Name = "merg",
                        LineItems = new List<InventoryBeverageLineItem>
                        {
                            new InventoryBeverageLineItem
                            {
                                ID = Guid.NewGuid(),
                                LastUpdatedDate = DateTime.UtcNow,
                                Revision = new EventID {AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 1},
                                CreatedDate = DateTime.UtcNow,
                                CurrentAmount = new LiquidAmount
                                {
                                    Milliliters = 10.0M,
                                    SpecificGravity = 1.0M
                                },
                                NumFullBottles = 13
                            }
                        }
                    },
                    new InventorySection
                    {
                        Name = "emptySec",
                        LineItems = new List<InventoryBeverageLineItem>{},
                        ID = Guid.NewGuid()
                    }
                }
            };

            //ACT
	        await underTest.UpsertEntitiesAsync(new[] {inventory});
	        var results = (await underTest.GetEntitiesAsync<Core.Common.Entities.Inventory.Inventory>()).ToList();

            //ASSERT
            Assert.AreEqual(1, results.Count);

	        var retInv = results.First();

            Assert.AreEqual(inventory.ID, retInv.ID, "ID");

	        var retSections = retInv.GetSections().ToList();
            Assert.AreEqual(2, retSections.Count, "num returned sections");

            Assert.AreEqual(inventory.Revision, retInv.Revision);
            Assert.AreEqual(inventory.CreatedByEmployeeID, retInv.CreatedByEmployeeID);
            Assert.AreEqual(inventory.CreatedDate, retInv.CreatedDate);
            Assert.AreEqual(inventory.IsCurrent, retInv.IsCurrent);
            Assert.AreEqual(inventory.ID, retInv.ID);
            Assert.AreEqual(inventory.DateCompleted, retInv.DateCompleted);
            Assert.AreEqual(inventory.LastUpdatedDate, retInv.LastUpdatedDate);
            Assert.AreEqual(inventory.RestaurantID, retInv.RestaurantID);

            var retInvdBIs = retInv.GetBeverageLineItems().ToList();
            foreach (var item in inventory.GetBeverageLineItems())
            {
                var retInvdItem = retInvdBIs.FirstOrDefault(bi => bi.ID == item.ID);
                Assert.NotNull(retInvdItem);

                Assert.AreEqual(item.CurrentAmount, retInvdItem.CurrentAmount);
                Assert.AreEqual(item.CreatedDate, retInvdItem.CreatedDate);
                Assert.AreEqual(item.LastUpdatedDate, retInvdItem.LastUpdatedDate);
                Assert.AreEqual(item.NumFullBottles, retInvdItem.NumFullBottles);
            }

            Assert.AreEqual(inventory.Sections.First().Name, retInv.Sections.First().Name);
            Assert.AreEqual(inventory.GetSections().First().ID, retInv.GetSections().First().ID, "Section ID");
	    }

	    [Test]
	    public async Task StoreAndRetrieveRestaurant()
	    {

	        var underTest = TestUtilities.GetTestSQLDB();

            var restID = Guid.NewGuid();
            var rest = new Restaurant
            {
                AccountID = Guid.NewGuid(),
                CreatedDate = DateTimeOffset.UtcNow,
                ID = restID,
                Name = new RestaurantName("Test Restaurant"),
                Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 191},
                InventorySections = new List<RestaurantInventorySection>
                {
                    new RestaurantInventorySection
                    {
                        AllowsPartialBottles = true,
                        CreatedDate = DateTimeOffset.UtcNow.AddDays(-1),
                        ID = Guid.NewGuid(),
                        RestaurantID = restID,
                        LastUpdatedDate = DateTimeOffset.UtcNow,
                        Name = "testSec",
                        Revision = new EventID {AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 10101}
                    }
                }
            };

            //ACT
	        await underTest.UpsertEntitiesAsync(new[] {rest});
	        var results = (await underTest.GetEntitiesAsync<Restaurant>()).ToList();

            //ASSERT
            Assert.AreEqual(1, results.Count);

	        var first = results.First();

            Assert.NotNull(first);
            Assert.AreEqual(rest.ID, first.ID);
            Assert.IsTrue(rest.Name.Equals(first.Name), "restaurant name");
            Assert.AreEqual(rest.AccountID, first.AccountID, "account ID");
            Assert.IsTrue(rest.Revision.Equals(first.Revision));

            Assert.AreEqual(1, first.InventorySections.Count());
            Assert.AreEqual("testSec", first.InventorySections.First().Name, "Section name");
            Assert.AreEqual(rest.InventorySections.First().AllowsPartialBottles,
                first.InventorySections.First().AllowsPartialBottles, "Allows partial bottles");
            Assert.AreEqual(rest.InventorySections.First().ID,
                first.InventorySections.First().ID, "Section ID");
	    }
	}
}

