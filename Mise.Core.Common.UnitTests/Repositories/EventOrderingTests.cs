using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Common.Entities;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Common.Events;
using Mise.Core.Common.Events.Inventory;
using Mise.Core.Common.Repositories.Base;
using Mise.Core.Entities;
using Mise.Core.Entities.Inventory;
using Mise.Core.Entities.Inventory.Events;
using Mise.Core.ValueItems;
using Mise.Core.ValueItems.Inventory;
using NUnit.Framework;

namespace Mise.Core.Common.UnitTests.Repositories
{
    [TestFixture]
    public class EventOrderingTests
    {
        [Test]
        public void DifferentAppsAndOrderingsShouldComeInOrderWithSameDate()
        {
            var sameDate = DateTime.UtcNow;
            var completeMainEvent = new InventoryCompletedEvent
            {
                ID = Guid.NewGuid(),
                CreatedDate = sameDate,
                EventOrderingID = new EventID(MiseAppTypes.UnitTests, 10),
                DeviceID = "first"
            };
            var startEvent = new InventoryCreatedEvent
            {
                ID = Guid.NewGuid(),
                CreatedDate = sameDate,
                EventOrderingID = new EventID(MiseAppTypes.UnitTests, 1),
                DeviceID = "first"
            };

            var measureEvent = new InventoryLiquidItemMeasuredEvent
            {
                ID = Guid.NewGuid(),
                CreatedDate = sameDate,
                EventOrderingID = new EventID(MiseAppTypes.UnitTests, 7),
                DeviceID = "first"
            };

            var addLI = new InventoryLineItemAddedEvent
            {
                ID = Guid.NewGuid(),
                CreatedDate = sameDate,
                EventOrderingID = new EventID(MiseAppTypes.UnitTests, 6),
                DeviceID = "first"
            };

            var addSection = new InventoryNewSectionAddedEvent
            {
                ID = Guid.NewGuid(),
                CreatedDate = sameDate,
                EventOrderingID = new EventID(MiseAppTypes.UnitTests, 2),
                DeviceID = "first"
            };

            var sectionEvent = new InventorySectionCompletedEvent
            {
                ID = Guid.NewGuid(),
                CreatedDate = sameDate,
                EventOrderingID = new EventID(MiseAppTypes.UnitTests, 8),
                DeviceID = "first"
            };

            var otherAppCreate = new InventoryCreatedEvent
            {
                ID = Guid.NewGuid(),
                CreatedDate = sameDate,
                EventOrderingID = new EventID(MiseAppTypes.DummyData, 2)
            };
            var otherDeviceAddSec = new InventoryNewSectionAddedEvent
            {
                ID = Guid.NewGuid(),
                CreatedDate = sameDate.AddDays(-1),
                EventOrderingID = new EventID(MiseAppTypes.UnitTests, 2),
                DeviceID = "yetanother"
            };
            var events = new List<IInventoryEvent>
            {
                completeMainEvent,
                measureEvent,
                startEvent,
                addLI,
                otherDeviceAddSec,
                otherAppCreate,
                sectionEvent,
                addSection,
            };

            //ACT
            var res = BaseEventSourcedRepository<IInventory, IInventoryEvent>.OrderEvents(events).ToList();

            //ASSERT
            Assert.AreEqual(startEvent, res[0]);
            Assert.AreEqual(addSection, res[1]);
            Assert.AreEqual(addLI, res[2]);
            Assert.AreEqual(measureEvent, res[3]);
            Assert.AreEqual(sectionEvent, res[4]);
            Assert.AreEqual(completeMainEvent, res[5]);
            Assert.AreEqual(otherDeviceAddSec, res[6]);
            Assert.AreEqual(otherAppCreate, res[7]);
        }

        [Test]
        public void EventFactoryCreatesItemsWithOrder()
        {
            var underTest = new InventoryAppEventFactory("unitTest", MiseAppTypes.UnitTests);
            var emp = new Employee {ID = Guid.NewGuid()};

            //ACT
            underTest.SetRestaurant(new Restaurant{ID = Guid.NewGuid()});
            var create = underTest.CreateInventoryCreatedEvent(emp);

            
            var inv = new Inventory
            {
                ID = create.InventoryID
            };

            var section = new InventorySection
            {
                ID = Guid.NewGuid(),
                RestaurantInventorySectionID = Guid.NewGuid()
            };

            var addSection = underTest.CreateInventoryNewSectionAddedEvent(emp, inv,
                new RestaurantInventorySection {ID = section.RestaurantInventorySectionID});

            var addLI = underTest.CreateInventoryLineItemAddedEvent(emp, "test", "", new List<ItemCategory>(), 0,
                LiquidContainer.Bottle12oz, 0, null, null, section, 1, inv);

            var measure = underTest.CreateInventoryLiquidItemMeasuredEvent(emp, inv, section,
                new InventoryBeverageLineItem(), 1, new List<decimal>(), LiquidAmount.Liter);

            var secDone = underTest.CreateInventorySectionCompletedEvent(emp, inv, section);

            var invDone = underTest.CreateInventoryCompletedEvent(emp, inv);

            //ASSERT
            Assert.Greater(invDone.EventOrderingID.OrderingID, secDone.EventOrderingID.OrderingID, "done > sec");
            Assert.Greater(secDone.EventOrderingID.OrderingID, measure.EventOrderingID.OrderingID, "sec > measure");
            Assert.Greater(measure.EventOrderingID.OrderingID, addLI.EventOrderingID.OrderingID, "measure > addLI");
            Assert.Greater(addLI.EventOrderingID.OrderingID, create.EventOrderingID.OrderingID, "addLI > create");

            Assert.Greater(addSection.EventOrderingID.OrderingID, create.EventOrderingID.OrderingID, "add sec > create");

            var all = new List<IInventoryEvent> {invDone, secDone, measure, addLI, create, addSection};
            Assert.IsTrue(all.All(e => e.EventOrderingID.AppInstanceCode == MiseAppTypes.UnitTests));

            Assert.IsTrue(all.All(e => e.ID != Guid.Empty), "Got ids");
            Assert.IsTrue(all.All(e => e.CausedByID == emp.ID), "caused by");
        }
    }
}
