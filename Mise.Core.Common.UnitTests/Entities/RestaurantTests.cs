using System;
using System.Collections.Generic;
using System.Linq;
using Mise.Core.Common.Entities;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Common.Events.Restaurant;
using Mise.Core.ValueItems;
using NUnit.Framework;
using Mise.Core.Entities;

namespace Mise.Core.Common.UnitTests.Entities
{
    [TestFixture]
    public class RestaurantTests
    {
        [Test]
        public void TestCloneEmpty()
        {
            var empty = new Restaurant();

            var res = empty.Clone() as Restaurant;

            Assert.NotNull(res);
        }

        [Test]
        public void TestCloneForSections()
        {
            var restID = Guid.NewGuid();
            var underTest = new Restaurant
            {
                AccountID = Guid.NewGuid(),
                CreatedDate = DateTimeOffset.UtcNow,
                ID = restID,

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
            var res = underTest.Clone() as Restaurant;

            //ASSERT
            Assert.NotNull(res);
            Assert.AreEqual(1, res.InventorySections.Count());
            Assert.AreEqual("testSec", res.InventorySections.First().Name, "Section name");
            Assert.AreEqual(underTest.InventorySections.First().AllowsPartialBottles, 
                res.InventorySections.First().AllowsPartialBottles, "Allows partial bottles");
            Assert.AreEqual(underTest.InventorySections.First().ID,
                res.InventorySections.First().ID, "Section ID");
        }

        [Test]
        public void AddInventorySectionEventShouldAddInventorySection()
        {
            var underTest = new Restaurant();

            var date = DateTimeOffset.UtcNow;
            var ev = new InventorySectionAddedToRestaurantEvent
            {
                CreatedDate = date,
                DeviceID = "blerg",
                EventOrderingID = new EventID {AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
                SectionID = Guid.NewGuid(),
                SectionName = "testBar",
                AllowsPartialBottles = true
            };

            //ACT
            underTest.When(ev);
            var sections = underTest.GetInventorySections().ToList();

            //ASSERT
            Assert.AreEqual(1, sections.Count);
            var firstSec = sections.First();
            Assert.AreEqual("testBar", firstSec.Name);
            Assert.IsTrue(firstSec.AllowsPartialBottles);

            Assert.AreEqual(date, underTest.LastUpdatedDate);
        }

        [Test]
        public void AddInventorySectionEventOnExistingSectionShouldBeIgnored()
        {
            var sectionID = Guid.NewGuid();
            var underTest = new Restaurant
            {
                InventorySections = new List<RestaurantInventorySection>
                {
                    new RestaurantInventorySection
                    {
                        ID = sectionID,
                        Name = "mainBar"
                    }
                }
            };

            var date = DateTimeOffset.UtcNow;
            var ev = new InventorySectionAddedToRestaurantEvent
            {
                CreatedDate = date,
                DeviceID = "blerg",
                EventOrderingID = new EventID { AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101 },
                SectionID = sectionID,
                SectionName = "testBar",

            };

            //ACT
            underTest.When(ev);
            var sections = underTest.GetInventorySections().ToList();

            //ASSERT
            Assert.AreEqual(1, sections.Count);
            Assert.AreEqual("mainBar", sections.First().Name);

            Assert.AreEqual(date, underTest.LastUpdatedDate);
        }

        [Test]
        public void CreatePlaceholderEventShouldMakeAccountIDNullAndSetIDAndDatesOnEntity()
        {
            var underTest = new Restaurant();

            var date = DateTimeOffset.UtcNow;
            var ev = new PlaceholderRestaurantCreatedEvent
            {
                EventOrderingID = new EventID { AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
                CreatedDate = date,
                RestaurantID = Guid.NewGuid()
            };

            //ACT
            underTest.When(ev);

            //ASSERT
            Assert.IsNull(underTest.AccountID);
            Assert.AreEqual(date, underTest.CreatedDate);
            Assert.AreEqual(date, underTest.LastUpdatedDate);
            Assert.AreEqual(ev.EventOrderingID.OrderingID, underTest.Revision.OrderingID);
            Assert.AreEqual(ev.RestaurantID, underTest.ID);
            Assert.AreEqual(ev.RestaurantID, underTest.RestaurantID);
        }
    }
}
