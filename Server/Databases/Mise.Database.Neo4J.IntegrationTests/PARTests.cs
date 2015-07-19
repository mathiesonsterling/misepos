using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Common;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Services;
using Mise.Core.ValueItems.Inventory;
using Mise.Neo4J;
using Mise.Neo4J.Neo4JDAL;
using Moq;
using NUnit.Framework;

namespace Mise.Database.Neo4J.IntegrationTests
{
    [TestFixture]
    public class PARTests
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
        public async Task PARShouldCRUD()
        {
            await TestUtilities.LoadCategories(_underTest);

            var rest = TestUtilities.CreateRestaurant();
            await _underTest.AddRestaurantAsync(rest);

            var emp = TestUtilities.CreateEmployee();
            await _underTest.AddEmployeeAsync(emp);

            var vendor = TestUtilities.CreateVendor();
            await _underTest.AddVendorAsync(vendor);

            var parID = Guid.NewGuid();
            var liID = Guid.NewGuid();
            var secondLIID = Guid.NewGuid();
            var par = new PAR
            {
                CreatedByEmployeeID = emp.ID,
                CreatedDate = DateTime.UtcNow,
                ID = parID,
                IsCurrent = true,
                LastUpdatedDate = DateTime.UtcNow,
                RestaurantID = rest.ID,
                Revision = TestUtilities.GetEventID(),
                ParLineItems = new List<PARBeverageLineItem>
                {
                    new PARBeverageLineItem
                    {
                        Container =
                            new LiquidContainer
                            {
                                AmountContained = new LiquidAmount {Milliliters = 750, SpecificGravity = .08M}
                            },
                        CreatedDate = DateTime.UtcNow,
                        ID = liID,
                        LastUpdatedDate = DateTime.UtcNow,
                        MiseName = "firstPARItem",
                        Quantity = 10,
                        RestaurantID = rest.ID,
                        Revision = TestUtilities.GetEventID(),
                        UPC = "1111",
                        Categories = new List<ItemCategory>
                        {
                            CategoriesService.LiquerAmaro,
                            CategoriesService.NonAlcoholic
                        }
                    },
                    new PARBeverageLineItem
                    {
                        Container =
                            new LiquidContainer
                            {
                                AmountContained = new LiquidAmount {Milliliters = 750, SpecificGravity = .08M}
                            },
                        CreatedDate = DateTime.UtcNow,
                        ID = secondLIID,
                        LastUpdatedDate = DateTime.UtcNow,
                        MiseName = "secondPARItem",
                        Quantity = 1,
                        RestaurantID = rest.ID,
                        Revision = TestUtilities.GetEventID(),
                        UPC = "2222",
                    }
                }
            };

            //CREATE
            await _underTest.AddPARAsync(par);
            var getRes = (await _underTest.GetPARsAsync()).ToList();

            Assert.AreEqual(1, getRes.Count);

            var got = getRes.First();
            Assert.NotNull(got);
            Assert.AreEqual(rest.ID, got.RestaurantID);
            Assert.AreEqual(parID, got.ID);

            var gotLineItems = got.GetBeverageLineItems().ToList();
            Assert.AreEqual(2, gotLineItems.Count());
            var firstLI = gotLineItems.FirstOrDefault(li => li.MiseName == "firstPARItem");
            Assert.NotNull(firstLI);
            Assert.NotNull(firstLI.Container, "container set");
            var firstCategories = firstLI.GetCategories().ToList();
            Assert.AreEqual(2, firstCategories.Count, "number of categories on first item");
            Assert.IsTrue(firstCategories.Select(c => c.ID).Contains(CategoriesService.LiquerAmaro.ID));
            Assert.IsTrue(firstCategories.Select(c => c.ID).Contains(CategoriesService.NonAlcoholic.ID));

            var secondLI = gotLineItems.FirstOrDefault(li => li.MiseName == "secondPARItem");
            Assert.NotNull(secondLI);
            Assert.AreEqual(0, secondLI.GetCategories().Count(), "second item has no categories");

            //update
            //removes a LI, changes IsCurrent
            var updatedPAR = new PAR
            {
                CreatedByEmployeeID = emp.ID,
                CreatedDate = DateTime.UtcNow,
                ID = parID,
                IsCurrent = false,
                LastUpdatedDate = DateTime.UtcNow,
                RestaurantID = rest.ID,
                Revision = TestUtilities.GetEventID(),
                ParLineItems = new List<PARBeverageLineItem>
                {
                    new PARBeverageLineItem
                    {
                        Container =
                            new LiquidContainer
                            {
                                AmountContained = new LiquidAmount {Milliliters = 750, SpecificGravity = 1.5M}
                            },
                        CreatedDate = DateTime.UtcNow,
                        ID = liID,
                        LastUpdatedDate = DateTime.UtcNow,
                        MiseName = "firstPARItem",
                        Quantity = 15,
                        RestaurantID = rest.ID,
                        Revision = TestUtilities.GetEventID(),
                        UPC = "1111"
                    },

                }
            };

            await _underTest.UpdatePARAsync(updatedPAR);
            var auRes = (await _underTest.GetPARsAsync(rest.ID)).ToList();

            Assert.AreEqual(auRes.Count, 1);
            var auPAR = auRes.First();
            Assert.AreEqual(parID, auPAR.ID);
            Assert.IsFalse(auPAR.IsCurrent);

            var auLineItems = auPAR.GetBeverageLineItems().ToList();
            Assert.AreEqual(1, auLineItems.Count);
            Assert.NotNull(auLineItems[0].Container);
            Assert.AreEqual(1.5M, auLineItems[0].Container.AmountContained.SpecificGravity);
            Assert.AreEqual(15, auLineItems[0].Quantity, "Updated Quantity");
        }
    }
}
