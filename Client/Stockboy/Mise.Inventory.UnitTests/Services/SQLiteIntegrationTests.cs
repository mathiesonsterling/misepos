using System;
using System.Linq;
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
		}

	}
}

