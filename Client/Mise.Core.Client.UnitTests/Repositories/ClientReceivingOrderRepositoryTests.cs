using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Client.Repositories;
using Mise.Core.Common.Services;
using Mise.Core.Entities.Vendors.Events;
using Mise.Core.Services;
using Mise.Core.Services.WebServices;
using Mise.Core.ValueItems;
using Moq;
using NUnit.Framework;
using Mise.Core.Common.Events.Inventory;
using Mise.Core.Entities;
using Mise.Core.Entities.Inventory;


namespace Mise.Core.Client.UnitTests.Repositories
{
    [TestFixture]
    public class ClientReceivingOrderRepositoryTests
    {
        [Test]
        public void CreateEventShouldCreateWithCommit()
        {
            var logger = new Mock<ILogger>();
            var dal = new Mock<IClientDAL>();

            var inventoryEventsPassed = new List<IReceivingOrderEvent>();
            var service = new Mock<IReceivingOrderWebService>();
			service.Setup(s => s.SendEventsAsync(It.IsAny<IReceivingOrder>(), It.IsAny<IEnumerable<IReceivingOrderEvent>>()))
                .Callback<IReceivingOrder, IEnumerable<IReceivingOrderEvent>>((ro, evs) => inventoryEventsPassed.AddRange(evs))
                .Returns(Task.Factory.StartNew(() => true));

            var underTest = new ClientReceivingOrderRepository(logger.Object, dal.Object, service.Object);

            var entID = Guid.NewGuid();
            var creation = new ReceivingOrderCreatedEvent
            {
                ReceivingOrderID = entID,
                CreatedDate = DateTime.UtcNow,
                EventOrderingID = new EventID { AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 1 },
                RestaurantID = Guid.NewGuid()
            };

            //ACT
            var res = underTest.ApplyEvent(creation);
            var commitRes = underTest.Commit(entID).Result;
            var got = underTest.GetAll().ToList();

            //ASSERT
            Assert.NotNull(res);
            Assert.AreEqual(1, got.Count);
            Assert.AreEqual(entID, got.First().ID);

            Assert.AreEqual(CommitResult.SentToServer, commitRes);

        }
    }
}
