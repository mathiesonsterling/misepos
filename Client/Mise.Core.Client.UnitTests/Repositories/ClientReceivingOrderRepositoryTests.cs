using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Client.Repositories;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Common.Services;
using Mise.Core.Common.UnitTests.Tools;
using Mise.Core.Entities.Vendors.Events;
using Mise.Core.Services;
using Mise.Core.ValueItems;
using Moq;
using NUnit.Framework;
using Mise.Core.Common.Events.Inventory;
using Mise.Core.Common.Services.WebServices;
using Mise.Core.Entities;
using Mise.Core.Entities.Inventory;
using Mise.Core.Services.UtilityServices;

namespace Mise.Core.Client.UnitTests.Repositories
{
    [TestFixture]
    public class ClientReceivingOrderRepositoryTests
    {
        [Test]
        public void CreateEventShouldCreateWithCommit()
        {
            var logger = new Mock<ILogger>();

            var inventoryEventsPassed = new List<IReceivingOrderEvent>();
            var service = new Mock<IReceivingOrderWebService>();
			service.Setup(s => s.SendEventsAsync(It.IsAny<ReceivingOrder>(), It.IsAny<IEnumerable<IReceivingOrderEvent>>()))
                .Callback<IReceivingOrder, IEnumerable<IReceivingOrderEvent>>((ro, evs) => inventoryEventsPassed.AddRange(evs))
                .Returns(Task.Factory.StartNew(() => true));

            var underTest = new ClientReceivingOrderRepository(logger.Object, service.Object);

            var entID = Guid.NewGuid();
            var creation = new ReceivingOrderCreatedEvent
            {
                ReceivingOrderID = entID,
                CreatedDate = DateTime.UtcNow,
                EventOrder = new EventID { AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 1 },
                RestaurantId = Guid.NewGuid()
            };

            //ACT
            var res = underTest.ApplyEvent(creation);
            var commitRes = underTest.Commit(entID).Result;
            var got = underTest.GetAll().ToList();

            //ASSERT
            Assert.NotNull(res);
            Assert.AreEqual(1, got.Count);
            Assert.AreEqual(entID, got.First().Id);

            Assert.AreEqual(CommitResult.SentToServer, commitRes);

        }
    }
}
