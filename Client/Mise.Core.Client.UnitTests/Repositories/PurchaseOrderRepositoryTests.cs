using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Client.Repositories;
using Mise.Core.Common.Events.Inventory;
using Mise.Core.Common.UnitTests.Tools;
using Mise.Core.Entities.Inventory.Events;
using Mise.Core.ValueItems;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Common.Services.WebServices;
using Moq;
using NUnit.Framework;
using Mise.Core.Entities;
using Mise.Core.Entities.Inventory;
using Mise.Core.Services.UtilityServices;

namespace Mise.Core.Client.UnitTests.Repositories
{
    [TestFixture]
    public class PurchaseOrderRepositoryTests
    {
        [Test]
        public async Task CreateEventShouldCreateWithCommit()
        {
            var logger = new Mock<ILogger>();

            var inventoryEventsPassed = new List<IPurchaseOrderEvent>();
            var service = new Mock<IPurchaseOrderWebService>();
			service.Setup(s => s.SendEventsAsync(It.IsAny<PurchaseOrder> (), It.IsAny<IEnumerable<IPurchaseOrderEvent>>()))
                .Callback<IPurchaseOrder, IEnumerable<IPurchaseOrderEvent>>((po, events) => inventoryEventsPassed.AddRange(events))
                .Returns(Task.Factory.StartNew(() => true));

            var underTest = new ClientPurchaseOrderRepository(logger.Object, service.Object, MockingTools.GetResendEventsService().Object);

            var entID = Guid.NewGuid();
            var creation = new PurchaseOrderCreatedEvent
            {
                PurchaseOrderID = entID,
                CreatedDate = DateTime.UtcNow,
                EventOrderingID = new EventID {AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 1},
                RestaurantID = Guid.NewGuid()
            };

            //ACT
            var res = underTest.ApplyEvent(creation);
            var commitRes = await underTest.Commit(entID);
            var got = underTest.GetAll().ToList();

            //ASSERT
            Assert.NotNull(res);
            Assert.AreEqual(1, got.Count);
            Assert.AreEqual(entID, got.First().ID);

            Assert.AreEqual(CommitResult.SentToServer, commitRes);

        }
    }
}
