using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Client.Repositories;
using Mise.Core.Common;
using Mise.Core.Common.Events.Inventory;
using Mise.Core.Common.Services;
using Mise.Core.Common.Services.Implementation.DAL;
using Mise.Core.Common.Services.Implementation.Serialization;
using Mise.Core.Common.UnitTests.Tools;
using Mise.Core.Entities.Inventory;
using Mise.Core.Entities;
using Mise.Core.Entities.Inventory.Events;
using Mise.Core.Services.UtilityServices;
using Mise.Core.ValueItems;
using Mise.Core.ValueItems.Inventory;
using Moq;
using NUnit.Framework;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Common.Services.WebServices;

namespace Mise.Core.Client.UnitTests.Repositories
{
    [TestFixture]
    public class InventoryRepositoryTests
    {
        [Test]
        public async Task TwoEventsBothCommitedShouldReflectInGet()
        {
            var logger = new Mock<ILogger>();

            var inventoryEventsPassed = new List<IInventoryEvent>();
            var service = new Mock<IInventoryWebService> ();
			service.Setup(s => s.SendEventsAsync(It.IsAny<Inventory> (), It.IsAny<IEnumerable<IInventoryEvent>>()))
                .Callback<IInventory, IEnumerable<IInventoryEvent>>((inv, events) => inventoryEventsPassed.AddRange(events))
                .Returns(Task.FromResult(true));

            var underTest = new ClientInventoryRepository(logger.Object, service.Object, MockingTools.GetResendEventsService().Object);

            var entID = Guid.NewGuid();
            var createEvent = new InventoryCreatedEvent
            {
                CausedByID = Guid.NewGuid(),
                CreatedDate = DateTime.UtcNow,
                EventOrderingID = new EventID {AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 0},
                InventoryID = entID,
            };
     
            //Create
            underTest.ApplyEvent(createEvent);
            var comRes1 = await underTest.Commit(entID);

            Assert.AreEqual(CommitResult.SentToServer, comRes1);
            //Retrieve
            var res = underTest.GetAll().ToList();
            Assert.AreEqual(1, res.Count(), "not 1 item gotten");

            var item = res.First();
            Assert.NotNull(item);

            Assert.AreEqual(1, inventoryEventsPassed.Count);
            //Update
            var completed = new InventoryCompletedEvent
            {
                CausedByID = Guid.NewGuid(),
                CreatedDate = DateTime.UtcNow,
                EventOrderingID = new EventID {AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 0},
                InventoryID = entID
            };

            var updatedItem = underTest.ApplyEvent(completed);
            var comRes2 = await underTest.Commit(entID);
            Assert.AreEqual(CommitResult.SentToServer, comRes2, "second commit");

            Assert.NotNull(updatedItem);

            //Get again
            var secondGet = underTest.GetByID(entID);
            Assert.NotNull(secondGet);
            Assert.AreEqual(entID, secondGet.ID);

            //check our WS called out correctly!
            Assert.AreEqual(2, inventoryEventsPassed.Count);
        }

        [Test]
        public async Task TwoEventsSecondCanceledShouldNotReflectSecondInGet()
        {
            var logger = new Mock<ILogger>();

            var inventoryEventsPassed = new List<IInventoryEvent>();
            var service = new Mock<IInventoryWebService>();
			service.Setup(s => s.SendEventsAsync(It.IsAny<Inventory> (), It.IsAny<IEnumerable<IInventoryEvent>>()))
                .Callback<IInventory, IEnumerable<IInventoryEvent>>((inv, events)=>inventoryEventsPassed.AddRange(events))
                .Returns(Task.FromResult(true));

            var underTest = new ClientInventoryRepository(logger.Object, service.Object, MockingTools.GetResendEventsService().Object);

            var entID = Guid.NewGuid();
            var createEvent = new InventoryCreatedEvent
            {
                CausedByID = Guid.NewGuid(),
                CreatedDate = DateTime.UtcNow,
                EventOrderingID = new EventID { AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 0 },
                InventoryID = entID,
            };

            //Create
            underTest.ApplyEvent(createEvent);
            var comRes1 = await underTest.Commit(entID);

            Assert.AreEqual(CommitResult.SentToServer, comRes1);
            //Retrieve
            var res = underTest.GetAll().ToList();
            Assert.AreEqual(1, res.Count(), "not 1 item gotten");

            var item = res.First();
            Assert.NotNull(item);

            Assert.AreEqual(1, inventoryEventsPassed.Count);
            //Update
            var completed = new InventoryCompletedEvent
            {
                CausedByID = Guid.NewGuid(),
                CreatedDate = DateTime.UtcNow,
                EventOrderingID = new EventID { AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 0 },
                InventoryID = entID
            };

            var updatedItem = underTest.ApplyEvent(completed);
            underTest.CancelTransaction(entID);

            Assert.NotNull(updatedItem);

            //Get again
            var secondGet = underTest.GetByID(entID);
            Assert.NotNull(secondGet);
            Assert.AreEqual(entID, secondGet.ID);

            //check our WS called out correctly!
            Assert.AreEqual(1, inventoryEventsPassed.Count, "Only one event should have been sent");
        }
    }
}
