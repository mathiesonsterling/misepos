using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Common.Events.Inventory;
using Mise.Core.Entities.Accounts;
using Mise.Core.Entities.Inventory;
using Mise.Core.Entities.Inventory.Events;
using Mise.Core.Entities.Menu;
using Mise.Core.Entities.People;
using Mise.Core.Entities.Restaurant;
using Mise.Core.Entities.Vendors;
using Mise.Core.Server.Services;
using Mise.Core.Server.Services.DAL;
using Mise.Core.Services;
using Mise.Core.ValueItems;
using Mise.Core.ValueItems.Inventory;
using MiseInventoryService.Repositories;
using Moq;
using NUnit.Framework;
namespace Mise.InventoryService.Tests.Repositories
{
    [TestFixture]
    public class InventoryServerRepositoryTests
    {
        /*
        [Test]
        public async Task InventoryShouldMarkAllInventoriesInCacheAsNotCurrentWhenNewCurrentComesIn()
        {
            var oldInvs = new List<Mise.Core.Entities.Inventory.IInventory>
            {
                new Inventory
                {
                    ID = Guid.NewGuid(),
                    IsCurrent = true
                },
                new Inventory
                {
                    ID = Guid.NewGuid(),
                    IsCurrent = false
                }
            };

            var logger = new Mock<ILogger>();
            var dal = new TestEntityDAL(oldInvs);

            var webHos = new Mock<IWebHostingEnvironment>();

            var underTest = new InventoryServerRepository(logger.Object, dal, webHos.Object);

            var invID = Guid.NewGuid();
            var newEvents = new List<IInventoryEvent>
            {
                new InventoryCreatedEvent
                {
                    InventoryID = invID,
                    CreatedDate = DateTime.UtcNow.AddMinutes(-1)
                },
                new InventoryMadeCurrentEvent
                {
                    InventoryID = invID,
                    CreatedDate = DateTime.UtcNow
                }
            };


            //ACT
            await underTest.Load(Guid.Empty);
            var updated = underTest.ApplyEvents(newEvents);
            var allInvs = underTest.GetAll().ToList();

            //ASSERT
            Assert.IsTrue(updated.IsCurrent);

            Assert.AreEqual(3, allInvs.Count);
            var currents = allInvs.Where(i => i.IsCurrent).ToList();

            Assert.AreEqual(1, currents.Count);
            Assert.AreEqual(invID, currents.First().ID);
        }*/
    }
}
