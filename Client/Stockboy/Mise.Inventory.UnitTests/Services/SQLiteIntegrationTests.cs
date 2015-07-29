using System;
using System.Collections.Generic;
using System.Linq;
using Mise.Core.Common.Entities;
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
	    }
	}
}

