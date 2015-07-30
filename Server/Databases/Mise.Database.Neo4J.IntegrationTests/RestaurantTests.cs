using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Common.Entities;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Entities;
using Mise.Core.Services;
using Mise.Core.Services.UtilityServices;
using Mise.Core.ValueItems;
using Mise.Neo4J;
using Mise.Neo4J.Neo4JDAL;
using Moq;
using NUnit.Framework;

namespace Mise.Database.Neo4J.IntegrationTests
{
    [TestFixture]
    public class RestaurantTests
    {
        private Neo4JEntityDAL _underTest;

        [SetUp]
        public void Setup()
        {
            var logger = new Mock<ILogger>();
            _underTest = new Neo4JEntityDAL(TestUtilities.GetConfig(), logger.Object);

            _underTest.ResetDatabase();
        }

        [Test]
        public async Task RestaurantShouldStoreAndRetrieveSections()
        {
            var restID = Guid.NewGuid();
            var restaurant = new Restaurant
            {
                AccountID = null,
                CreatedDate = DateTime.Now,
                ID = restID,
                Revision = new EventID {AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
                Name = RestaurantName.TestName,
                InventorySections = new List<RestaurantInventorySection>
                {
                    new RestaurantInventorySection
                    {
                        ID = Guid.NewGuid(),
                        Revision = new EventID {AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 1},
                        Name = "mainBar"
                    },
                    new RestaurantInventorySection
                    {
                        ID = Guid.NewGuid(),
                        Revision = new EventID {AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 10},
                        Name = "stockRoom",
                        Beacon = new Beacon
                        {
                            UUID = "aaaa",
                            LocationPlaced = new Location
                            {
                                Longitude = 10,
                                Latitude = 1.5
                            }
                        }
                    }
                }
            };

            await _underTest.AddRestaurantAsync(restaurant);

            var got = await _underTest.GetRestaurantAsync(restID);
            Assert.NotNull(got);
            Assert.AreEqual(restID, got.ID);

            var sections = got.GetInventorySections().ToList();
            Assert.AreEqual(2, sections.Count(), "num sections");

            var firstSec = sections.FirstOrDefault(s => s.Name == "mainBar");
            Assert.NotNull(firstSec);
            Assert.IsNull(firstSec.Beacon);

            var secondSec = sections.FirstOrDefault(s => s.Name == "stockRoom");
            Assert.NotNull(secondSec);
            var beacon = secondSec.Beacon;
            Assert.IsNotNull(beacon);
            Assert.AreEqual("aaaa", beacon.UUID);
            Assert.NotNull(beacon.LocationPlaced);
            Assert.AreEqual(10, beacon.LocationPlaced.Longitude);
            Assert.AreEqual(1.5, beacon.LocationPlaced.Latitude);
        }
    }
}
