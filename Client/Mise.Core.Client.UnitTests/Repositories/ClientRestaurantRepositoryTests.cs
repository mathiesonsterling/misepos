using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Client.Repositories;
using Mise.Core.Client.Services;
using Mise.Core.Common.Entities;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Common.Services.Implementation.DAL;
using Mise.Core.Common.Services.Implementation.Serialization;
using Mise.Core.Common.Services.WebServices;
using Mise.Core.Services.UtilityServices;
using Mise.Core.ValueItems;
using Moq;
using NUnit.Framework;

namespace Mise.Core.Client.UnitTests.Repositories
{
    [TestFixture]
    public class ClientRestaurantRepositoryTests
    {
        [Test]
        public async Task ShouldLoadFromDBWhenCannotConnect()
        {
            var webService = new Mock<IInventoryRestaurantWebService>();
            var restID = Guid.NewGuid();
            webService.Setup(ws => ws.GetRestaurant(restID))
                .Throws<Exception>();

            var logger = new Mock<ILogger>();

            var dal = new MemoryClientDAL(logger.Object, new JsonNetSerializer());
            var resendService = new Mock<IResendEventsWebService>();

            var loc = new Mock<IDeviceLocationService>();
            var underTest = new ClientRestaurantRepository(logger.Object, dal, webService.Object, resendService.Object, loc.Object);

            var rest = new Restaurant
            {
                ID = restID,
                StreetAddress = StreetAddress.TestStreetAddress,
                RestaurantID = restID,
                AccountID = Guid.NewGuid(),
                InventorySections = new List<RestaurantInventorySection>
                {
                    new RestaurantInventorySection
                    {
                        ID = Guid.NewGuid(),
                        Name = "testSection"
                    }
                }
            };

            //ACT
            var loadRes = await dal.UpsertEntitiesAsync(new[] {rest});
            Assert.True(loadRes);

            await underTest.Load(restID);
            var results = underTest.GetAll().ToList();

            //ASSERT
            Assert.AreEqual(1, results.Count);
            var retRest = results.First();

            Assert.AreEqual(rest.ID, retRest.ID);
            Assert.AreEqual(rest.AccountID, retRest.AccountID, "AccountID");
            Assert.IsTrue(rest.StreetAddress.Equals(retRest.StreetAddress), "address");

            var sections = retRest.GetInventorySections().ToList();
            Assert.AreEqual(1, sections.Count);
            Assert.AreEqual("testSection", sections.First().Name);
        }
    }
}
