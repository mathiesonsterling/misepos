using System;
using System.Collections.Generic;
using System.Linq;
using Mise.Core.Common.Entities;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Common.Services;
using Mise.Core.ValueItems.Inventory;
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
	    private IClientDAL _underTest;

	    [SetUp]
	    public async void Setup()
	    {
	        var tryAgain = false;
	        try
	        {
	            _underTest = TestUtilities.GetTestSQLDB();
	        }
	        catch (Exception)
	        {
	            tryAgain = true;
	        }

	        if (tryAgain)
	        {
                await Task.Delay(1000);
	            _underTest = TestUtilities.GetTestSQLDB();
	        }
	    }

		[Test]
		public async Task StoreNotSentEventAndGet(){

			var ev = new InventoryCreatedEvent {
				Id = Guid.NewGuid (),
				CausedById = Guid.NewGuid (),
				CreatedDate = DateTime.UtcNow,
				DeviceId = "testDevID",
				EventOrder = new EventID{ AppInstanceCode = MiseAppTypes.UnitTests },
				InventoryID = Guid.NewGuid (),
				RestaurantId = Guid.NewGuid (),
				
			};

			var evFactory = new EventDataTransportObjectFactory (new JsonNetSerializer ());


			//ACT
			await _underTest.AddEventsThatFailedToSend (new []{ev});

			var pulled = (await _underTest.GetUnsentEvents ()).ToList();

			//ASSERT
			Assert.NotNull (pulled);
			Assert.AreEqual (1, pulled.Count);

			var first = pulled.First();
			Assert.AreEqual (ev.Id, first.Id, "ID");
			Assert.AreEqual (ev.CreatedDate, first.CreatedDate, "CreatedDate");
			Assert.AreEqual (first.CausedById, ev.CausedById, "CausedBy");
			Assert.AreEqual (ev.DeviceId, first.DeviceId, "DeviceID");
			Assert.True (ev.EventOrder.Equals (first.EventOrder), "EventOrderingID");

			var actualItem = evFactory.ToInventoryEvent (first);

			Assert.NotNull (actualItem);
			Assert.AreEqual (ev.InventoryID, actualItem.InventoryID);
			Assert.AreEqual (ev.RestaurantId, actualItem.RestaurantId);
			Assert.AreEqual (MiseEventTypes.InventoryCreated, actualItem.EventType);

            Assert.AreEqual(ev.Id, actualItem.Id, "ID");
            Assert.AreEqual(ev.CreatedDate, actualItem.CreatedDate, "CreatedDate");
            Assert.AreEqual(actualItem.CausedById, ev.CausedById, "CausedBy");
            Assert.AreEqual(ev.DeviceId, actualItem.DeviceId, "DeviceID");
            Assert.True(ev.EventOrder.Equals(actualItem.EventOrder), "EventOrderingID");
		}

	    [Test]
	    public async Task StoreAndRetrieveEmployee()
	    {

	        var emp = new Employee
	        {
	            Id = Guid.NewGuid(),
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
	        await _underTest.UpsertEntitiesAsync(new[] {emp});
	        var returned = (await _underTest.GetEntitiesAsync<Employee>()).ToList();

            //ASSERT
            Assert.NotNull(returned);
            Assert.AreEqual(1, returned.Count());

	        var first = returned.First();

            Assert.AreEqual(emp.Id, first.Id, "ID");
            Assert.True(emp.PrimaryEmail.Equals(first.PrimaryEmail), "Primary email");

            Assert.AreEqual(emp.CurrentlyLoggedIntoInventoryApp, first.CurrentlyLoggedIntoInventoryApp);
            Assert.AreEqual(emp.Id, first.Id);
            Assert.AreEqual(emp.LastTimeLoggedIntoInventoryApp, first.LastTimeLoggedIntoInventoryApp);
            Assert.AreEqual(emp.Password, first.Password);
	    }

	    [Test]
	    public async Task StoreAndRetrieveInventory()
	    {

            var inventory = new Core.Common.Entities.Inventory.Inventory
            {
                Revision = new EventID { AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 0 },
                CreatedByEmployeeID = Guid.NewGuid(),
                CreatedDate = DateTimeOffset.UtcNow.AddDays(-1),
                LastUpdatedDate = DateTimeOffset.UtcNow,
                Id = Guid.NewGuid(),
                DateCompleted = DateTimeOffset.UtcNow,
                RestaurantID = Guid.NewGuid(),
                IsCurrent = true,
                Sections = new List<InventorySection>{
                    new InventorySection{
                        Id = Guid.NewGuid(),
                        RestaurantInventorySectionID = Guid.NewGuid(),
                        Name = "merg",
                        LineItems = new List<InventoryBeverageLineItem>
                        {
                            new InventoryBeverageLineItem
                            {
                                Id = Guid.NewGuid(),
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
                        LineItems = new List<InventoryBeverageLineItem>(),
                        Id = Guid.NewGuid()
                    }
                }
            };

            //ACT
	        await _underTest.UpsertEntitiesAsync(new[] {inventory});
	        var results = (await _underTest.GetEntitiesAsync<Core.Common.Entities.Inventory.Inventory>()).ToList();

            //ASSERT
            Assert.AreEqual(1, results.Count);

	        var retInv = results.First();

            Assert.AreEqual(inventory.Id, retInv.Id, "ID");

	        var retSections = retInv.GetSections().ToList();
            Assert.AreEqual(2, retSections.Count, "num returned sections");

            Assert.AreEqual(inventory.Revision, retInv.Revision);
            Assert.AreEqual(inventory.CreatedByEmployeeID, retInv.CreatedByEmployeeID);
            Assert.AreEqual(inventory.CreatedDate, retInv.CreatedDate);
            Assert.AreEqual(inventory.IsCurrent, retInv.IsCurrent);
            Assert.AreEqual(inventory.Id, retInv.Id);
            Assert.AreEqual(inventory.DateCompleted, retInv.DateCompleted);
            Assert.AreEqual(inventory.LastUpdatedDate, retInv.LastUpdatedDate);
            Assert.AreEqual(inventory.RestaurantID, retInv.RestaurantID);

            var retInvdBIs = retInv.GetBeverageLineItems().ToList();
            foreach (var item in inventory.GetBeverageLineItems())
            {
                var retInvdItem = retInvdBIs.FirstOrDefault(bi => bi.Id == item.Id);
                Assert.NotNull(retInvdItem);

                Assert.AreEqual(item.CurrentAmount, retInvdItem.CurrentAmount);
                Assert.AreEqual(item.CreatedDate, retInvdItem.CreatedDate);
                Assert.AreEqual(item.LastUpdatedDate, retInvdItem.LastUpdatedDate);
                Assert.AreEqual(item.NumFullBottles, retInvdItem.NumFullBottles);
            }

            Assert.AreEqual(inventory.Sections.First().Name, retInv.Sections.First().Name);
            Assert.AreEqual(inventory.GetSections().First().Id, retInv.GetSections().First().Id, "Section ID");
	    }

	    [Test]
	    public async Task StoreAndRetrieveRestaurant()
	    {


            var restID = Guid.NewGuid();
            var rest = new Restaurant
            {
                AccountID = Guid.NewGuid(),
                CreatedDate = DateTimeOffset.UtcNow,
                Id = restID,
                Name = new RestaurantName("Test Restaurant"),
                Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 191},
                InventorySections = new List<RestaurantInventorySection>
                {
                    new RestaurantInventorySection
                    {
                        AllowsPartialBottles = true,
                        CreatedDate = DateTimeOffset.UtcNow.AddDays(-1),
                        Id = Guid.NewGuid(),
                        RestaurantID = restID,
                        LastUpdatedDate = DateTimeOffset.UtcNow,
                        Name = "testSec",
                        Revision = new EventID {AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 10101}
                    }
                }
            };

            //ACT
	        await _underTest.UpsertEntitiesAsync(new[] {rest});
	        var results = (await _underTest.GetEntitiesAsync<Restaurant>()).ToList();

            //ASSERT
            Assert.AreEqual(1, results.Count);

	        var first = results.First();

            Assert.NotNull(first);
            Assert.AreEqual(rest.Id, first.Id);
            Assert.IsTrue(rest.Name.Equals(first.Name), "restaurant name");
            Assert.AreEqual(rest.AccountID, first.AccountID, "account ID");
            Assert.IsTrue(rest.Revision.Equals(first.Revision));

            Assert.AreEqual(1, first.InventorySections.Count());
            Assert.AreEqual("testSec", first.InventorySections.First().Name, "Section name");
            Assert.AreEqual(rest.InventorySections.First().AllowsPartialBottles,
                first.InventorySections.First().AllowsPartialBottles, "Allows partial bottles");
            Assert.AreEqual(rest.InventorySections.First().Id,
                first.InventorySections.First().Id, "Section ID");
	    }
	}
}

