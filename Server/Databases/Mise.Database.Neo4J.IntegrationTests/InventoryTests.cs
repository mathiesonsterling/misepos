using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Common;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Entities;
using Mise.Core.Services;
using Mise.Core.ValueItems;
using Mise.Core.ValueItems.Inventory;
using Mise.Neo4J;
using Mise.Neo4J.Neo4JDAL;
using Moq;
using NUnit.Framework;

namespace Mise.Database.Neo4J.IntegrationTests
{
    /// <summary>
    /// Tests the items we'll use for inventory app
    /// </summary>
    [TestFixture]
    public class InventoryTests
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
        public async Task InventoryShouldCRUD()
        {
            await TestUtilities.LoadCategories(_underTest);

            var rest = TestUtilities.CreateRestaurant();
            await _underTest.AddRestaurantAsync(rest);

            var emp = TestUtilities.CreateEmployee();
            await _underTest.AddEmployeeAsync(emp);

            var invID = Guid.NewGuid();
            var sectionID = Guid.NewGuid();
            var liID = Guid.NewGuid();
            //CREATE
            var inventory = new Inventory
            {
                CreatedByEmployeeID = emp.ID,
                CreatedDate = DateTime.Now.AddDays(-1),
                ID = invID,
                DateCompleted = null,
                LastUpdatedDate = DateTime.Now,
                RestaurantID = rest.ID,
                IsCurrent = false,
                Revision = new EventID { AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 100 },
                Sections = new List<InventorySection>{
                    new InventorySection{
                        Name = "sectionMain",
                        ID = sectionID,
                        Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 1000},
                        LineItems = new List<InventoryBeverageLineItem>
                        {
                            new InventoryBeverageLineItem
                            {
                                Container = new LiquidContainer
                                {
                                    AmountContained = new LiquidAmount {Milliliters = 750},
                                    WeightEmpty = new Weight{Grams = 201}
                                },
                                CreatedDate = DateTime.UtcNow.AddDays(-1),
                                CurrentAmount = new LiquidAmount
                                {
                                    Milliliters = 375
                                },
                                ID = liID,
                                LastUpdatedDate = DateTime.UtcNow.AddDays(-2),
                                MethodsMeasuredLast = MeasurementMethods.VisualEstimate,
                                MiseName = "testItem",
                                DisplayName = "testEntered",
                                PricePaid = new Money(1.0M),
                                RestaurantID = rest.ID,
                                Revision = new EventID {AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 1001},
                                UPC = "1111",
                                NumFullBottles = 1,
                                PartialBottleListing = new List<decimal>{.75M},
                                Categories = new List<ItemCategory>
                                {
                                    CategoriesService.Agave,
                                    CategoriesService.Gin
                                }
                            }
                        }

                    }
                }
            };

            await _underTest.AddInventoryAsync(inventory);

            //RETRIEVE
            var getResults = (await _underTest.GetInventoriesAsync(rest.ID)).ToList();

            Assert.NotNull(getResults);
            Assert.AreEqual(1, getResults.Count(), "first retrieve count");

            var gotInvFirst = getResults.First();
            //check IDs and other fields
            Assert.AreEqual(inventory.ID, gotInvFirst.ID);
            Assert.False(gotInvFirst.IsCurrent, "IsCurrent");
            var lineItems = gotInvFirst.GetBeverageLineItems().ToList();
            Assert.AreEqual(1, lineItems.Count());
            var firstLineItem = lineItems.First();
            Assert.AreEqual("1111", firstLineItem.UPC);
            Assert.AreEqual(1, firstLineItem.NumFullBottles, "Bottle Quantity");
            Assert.NotNull(firstLineItem.CurrentAmount);
            Assert.AreEqual(375, firstLineItem.CurrentAmount.Milliliters);
            Assert.NotNull(firstLineItem.Container);
            Assert.AreEqual(750, firstLineItem.Container.AmountContained.Milliliters, "Amount contained");
            Assert.AreEqual(201, firstLineItem.Container.WeightEmpty.Grams, "Weight Empty");
            Assert.IsNull(firstLineItem.Container.WeightFull, "weight full not null");

            var cats = firstLineItem.GetCategories().ToList();
            Assert.AreEqual(2, cats.Count);
            Assert.IsTrue(cats.Select(c => c.ID).Contains(CategoriesService.Gin.ID));
            Assert.IsTrue(cats.Select(c => c.ID).Contains(CategoriesService.Agave.ID));

            Assert.AreEqual(1, firstLineItem.NumFullBottles);
            Assert.AreEqual(1, firstLineItem.PartialBottlePercentages.Count());
            Assert.AreEqual(.75M, firstLineItem.PartialBottlePercentages.First());


            //UPDATE
            var newInv = new Inventory
            {
                CreatedByEmployeeID = emp.ID,
                CreatedDate = DateTime.Now,
                ID = invID,
                IsCurrent = true,
                LastUpdatedDate = DateTime.Now,
                RestaurantID = rest.ID,
                Revision = new EventID { AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 1040 },
                DateCompleted = DateTime.UtcNow,
                Sections = new List<InventorySection>{
                    new InventorySection{
                        Name = "sectionMain",
                        ID = sectionID,
                        Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 1000},
                        LineItems = new List<InventoryBeverageLineItem>
                        {
                            new InventoryBeverageLineItem
                            {
                                Container = new LiquidContainer
                                {
                                    AmountContained = new LiquidAmount {Milliliters = 750},
                                    WeightEmpty = new Weight {Grams = 201}
                                },
                                CreatedDate = DateTime.UtcNow,
                                CurrentAmount = new LiquidAmount
                                {
                                    Milliliters = 375
                                },
                                ID = liID,
                                LastUpdatedDate = DateTime.UtcNow,
                                MethodsMeasuredLast = MeasurementMethods.VisualEstimate,
                                MiseName = "testItemUpdated",
                                DisplayName = "testEntered",
                                PricePaid = new Money(1.0M),
                                RestaurantID = rest.ID,
                                Revision = new EventID {AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 10012},
                                UPC = "2222",
                                NumFullBottles = 10,
                                PartialBottleListing = new List<decimal>{.5M, .25M}
                            },
                            new InventoryBeverageLineItem
                            {
                                Container = new LiquidContainer
                                {
                                    AmountContained = new LiquidAmount{Milliliters = 1000}
                                },
                                CreatedDate = DateTime.UtcNow,
                                CurrentAmount = new LiquidAmount{Milliliters = 10000},
                                NumFullBottles = 11,
                                ID = Guid.NewGuid(),
                                LastUpdatedDate = DateTime.UtcNow,
                                MiseName = "anotherTestItem",
                                MethodsMeasuredLast = MeasurementMethods.Weight,
                                DisplayName = "blergo",
                                PricePaid = new Money(1012.0M),
                                RestaurantID = rest.ID,
                                Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 102012},
                            }
                        }
                    }
                }
            };

            await _underTest.UpdateInventoryAsync(newInv);

            //RETRIEVE (by date)
            var inventoriesByDate = (await _underTest.GetInventoriesAsync(DateTime.Now.AddYears(-1))).ToList();

            Assert.NotNull(inventoriesByDate);
            Assert.AreEqual(1, inventoriesByDate.Count());

            var updatedInv = inventoriesByDate.First();
            Assert.True(updatedInv.IsCurrent, "Updated iscurrent");
            var lis = updatedInv.GetBeverageLineItems().ToList();
            Assert.AreEqual(2, lis.Count());
            Assert.True(lis.Select(li => li.NumFullBottles).Contains(11));
            Assert.True(lis.Select(li => li.UPC).Contains("2222"));

            var updatedLineItem = lis.First(l => l.ID == liID);
            Assert.AreEqual(10.75M, updatedLineItem.Quantity);
            Assert.AreEqual(10, updatedLineItem.NumFullBottles);
            Assert.AreEqual(2, updatedLineItem.PartialBottlePercentages.Count());
            Assert.IsTrue(updatedLineItem.PartialBottlePercentages.Contains(.25M));
            Assert.IsTrue(updatedLineItem.PartialBottlePercentages.Contains(.5M));
        }



        [Test]
        public async Task TwoInventoryLineItemsWithSameContainerShouldCreateOnlyOneContainer()
        {
            var rest = TestUtilities.CreateRestaurant();

            await _underTest.AddRestaurantAsync(rest);

            var emp = TestUtilities.CreateEmployee();
            await _underTest.AddEmployeeAsync(emp);

            var sectionID = Guid.NewGuid();
            var newInv = new Inventory
            {
                CreatedByEmployeeID = emp.ID,
                CreatedDate = DateTime.Now,
                ID = Guid.NewGuid(),
                LastUpdatedDate = DateTime.Now,
                RestaurantID = rest.ID,
                Revision = new EventID { AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 1040 },
                DateCompleted = DateTime.UtcNow,
                Sections = new List<InventorySection>{
                    new InventorySection{
                        Name = "sectionMain",
                        ID = sectionID,
                        Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 1000},
                        LineItems = new List<InventoryBeverageLineItem>
                        {
                            new InventoryBeverageLineItem
                            {
                                Container = new LiquidContainer
                                {
                                    AmountContained = new LiquidAmount {Milliliters = 750},
                                    WeightEmpty = new Weight {Grams = 201}
                                },
                                CreatedDate = DateTime.UtcNow,
                                CurrentAmount = new LiquidAmount
                                {
                                    Milliliters = 375
                                },
                                ID = Guid.NewGuid(),
                                LastUpdatedDate = DateTime.UtcNow,
                                MethodsMeasuredLast = MeasurementMethods.VisualEstimate,
                                MiseName = "testItemUpdated",
                                DisplayName = "testEntered",
                                PricePaid = new Money(1.0M),
                                RestaurantID = rest.ID,
                                Revision = new EventID {AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 10012},
                                UPC = "2222"
                            },
                            new InventoryBeverageLineItem
                            {
                                Container = new LiquidContainer
                                {
                                    AmountContained = new LiquidAmount {Milliliters = 750},
                                    WeightEmpty = new Weight {Grams = 201}
                                },
                                CreatedDate = DateTime.UtcNow,
                                CurrentAmount = new LiquidAmount{Milliliters = 10000},
                                NumFullBottles = 11,
                                ID = Guid.NewGuid(),
                                LastUpdatedDate = DateTime.UtcNow,
                                MiseName = "anotherTestItem",
                                MethodsMeasuredLast = MeasurementMethods.Weight,
                                DisplayName = "blergo",
                                PricePaid = new Money(1012.0M),
                                RestaurantID = rest.ID,
                                Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 102012},
                            }
                        }
                    }
                }
            };

            //ACT
            await _underTest.AddInventoryAsync(newInv);

            var containers = await _underTest.GetAllLiquidContainersAsync();

            //ASSERT
            Assert.NotNull(containers);
            Assert.AreEqual(1, containers.Count());
        }


        [Test]
        public async Task InventorySectionsShouldLinkToRestaurantSections()
        {
            var sectionID = Guid.NewGuid();
            var sectionID2 = Guid.NewGuid();
            const string SECTION_NAME = "testSection";
            var restaurant = TestUtilities.CreateRestaurant();
            restaurant.InventorySections = new List<RestaurantInventorySection>
            {
                new RestaurantInventorySection
                {
                    CreatedDate = DateTimeOffset.UtcNow,
                    ID = sectionID,
                    Name = "testSec1",
                    RestaurantID = restaurant.ID,
                    Revision = TestUtilities.GetEventID()
                },
                new RestaurantInventorySection
                {
                    CreatedDate = DateTimeOffset.UtcNow,
                    ID = sectionID2,
                    Name = "testSec2",
                    RestaurantID = restaurant.ID,
                    Revision = TestUtilities.GetEventID()
                }
            };
            await _underTest.AddRestaurantAsync(restaurant);

            var inventory = new Inventory
            {
                ID = Guid.NewGuid(),
                IsCurrent = false,
                CreatedDate = DateTime.UtcNow,
                DateCompleted = DateTime.UtcNow,
                RestaurantID = restaurant.ID,
                Revision = TestUtilities.GetEventID(),
                Sections = new List<InventorySection>
                {
                    new InventorySection
                    {
                        Revision = TestUtilities.GetEventID(),
                        ID = Guid.NewGuid(),
                        RestaurantInventorySectionID = sectionID,
                        Name = SECTION_NAME,
                        RestaurantID = restaurant.ID
                    },
                    new InventorySection
                    {
                        Revision = TestUtilities.GetEventID(),
                        ID = Guid.NewGuid(),
                        RestaurantInventorySectionID = sectionID2,
                        Name = "testSec2",
                        RestaurantID = restaurant.ID
                    }
                }
            };

            await _underTest.AddInventoryAsync(inventory);
            var getRes = (await _underTest.GetInventoriesAsync(DateTimeOffset.MinValue)).ToList();

            //ASSERT
            Assert.NotNull(getRes);
            Assert.AreEqual(1, getRes.Count(), "num in res");
            var gotInv = getRes.First();
            Assert.NotNull(gotInv);
            var sections = gotInv.GetSections().ToList();
            Assert.AreEqual(2, sections.Count(), "numberof sections");

            Assert.True(sections.Select(s => s.RestaurantInventorySectionID).Contains(sectionID));
            Assert.True(sections.Select(s => s.RestaurantInventorySectionID).Contains(sectionID2));

            Assert.True(sections.Select(s => s.Name).Contains(SECTION_NAME));
            Assert.AreNotEqual(sectionID, sections.First().ID, "Id equal to section ID, error");
            Assert.AreNotEqual(sectionID, sections.Last().ID, "Id equal to section ID, error");
        }

        [Test]
        public async Task MultipleSectionsShouldBeReturnedUniquely()
        {
            var section1 = Guid.NewGuid();
            var section2 = Guid.NewGuid();
            var restID = Guid.NewGuid();
            var container = new LiquidContainer
            {
                AmountContained = new LiquidAmount { Milliliters = 750 }
            };

            var inventory = new Inventory
            {
                ID = Guid.NewGuid(),
                Revision = TestUtilities.GetEventID(),
                RestaurantID = restID,
                Sections = new List<InventorySection>
                {
                    new InventorySection
                    {
                        ID = section1,
                        Revision = TestUtilities.GetEventID(),
                        RestaurantID = restID,
                        Name = "section1",
                        LastCompletedBy = Guid.NewGuid(),
                        CreatedDate = DateTime.UtcNow,
                        LineItems = new List<InventoryBeverageLineItem>
                        {
                            new InventoryBeverageLineItem
                            {
                                ID = Guid.NewGuid(),
                                Revision = TestUtilities.GetEventID(),
                                RestaurantID = restID,
                                NumFullBottles = 1,
                                CaseSize = 5,
                                Container = container,
                                MiseName = "li1",
                                CurrentAmount = new LiquidAmount{Milliliters = 83}
                            },
                            new InventoryBeverageLineItem
                            {
                                ID = Guid.NewGuid(),
                                Revision = TestUtilities.GetEventID(),
                                RestaurantID = restID,
                                NumFullBottles = 2,
                                Container = container,
                                MiseName = "li2",
                                CurrentAmount = new LiquidAmount{Milliliters = 100}
                            }
                        }
                    },
                    new InventorySection
                    {
                        ID = section2,
                        Revision = TestUtilities.GetEventID(),
                        LastCompletedBy = null,
                        Name = "section2",
                        RestaurantID = restID,
                        LineItems = new List<InventoryBeverageLineItem>
                        {
                            new InventoryBeverageLineItem
                            {
                                ID = Guid.NewGuid(),
                                Revision = TestUtilities.GetEventID(),
                                RestaurantID = restID,
                                NumFullBottles = 3,
                                Container = container,
                                MiseName = "liSec2First",
                                CurrentAmount = new LiquidAmount{Milliliters = 200}
                            }
                        }
                    }
                }
            };

            //ACT
            await _underTest.AddInventoryAsync(inventory);

            var res = (await _underTest.GetInventoriesAsync(restID)).ToList();

            //ASSERT
            Assert.AreEqual(1, res.Count());

            var sections = res.First().GetSections().ToList();
            Assert.AreEqual(2, sections.Count);

            var firstSec = sections.FirstOrDefault(s => s.ID == section1);
            Assert.NotNull(firstSec);
            Assert.AreEqual(2, firstSec.GetInventoryBeverageLineItemsInSection().Count());

            var secondSec = sections.FirstOrDefault(s => s.ID == section2);
            Assert.NotNull(secondSec);
            Assert.AreEqual(1, secondSec.GetInventoryBeverageLineItemsInSection().Count());

        }

        [Test]
        public async Task AddTwoCurrentInventoriesShouldMakeFirstNotCurrent()
        {
            var rest = TestUtilities.CreateRestaurant();

            await _underTest.AddRestaurantAsync(rest);

            var emp = TestUtilities.CreateEmployee();
            await _underTest.AddEmployeeAsync(emp);

            var firstInv = new Inventory
            {
                CreatedByEmployeeID = emp.ID,
                CreatedDate = DateTime.Now,
                ID = Guid.NewGuid(),
                LastUpdatedDate = DateTime.Now,
                RestaurantID = rest.ID,
                Revision = new EventID { AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 1040 },
                DateCompleted = DateTime.UtcNow,
                IsCurrent = true,
                Sections = new List<InventorySection>()
            };

            await _underTest.AddInventoryAsync(firstInv);

            var checkFirstAdd = await _underTest.GetInventoriesAsync(rest.ID);

            Assert.NotNull(checkFirstAdd);
            var inv = checkFirstAdd.First();
            Assert.IsTrue(inv.IsCurrent);

            //add another current one
            var secondInv = new Inventory
            {
                CreatedByEmployeeID = emp.ID,
                CreatedDate = DateTime.Now,
                ID = Guid.NewGuid(),
                LastUpdatedDate = DateTime.Now,
                RestaurantID = rest.ID,
                Revision = new EventID { AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 1040 },
                DateCompleted = DateTime.UtcNow,
                IsCurrent = true,
                Sections = new List<InventorySection>()
            };

            await _underTest.AddInventoryAsync(secondInv);
            var invsAfter = (await _underTest.GetInventoriesAsync(rest.ID)).ToList();
            var firstAfter = invsAfter.First(i => i.ID == firstInv.ID);
            var secondAfter = invsAfter.First(i => i.ID == secondInv.ID);

            Assert.IsFalse(firstAfter.IsCurrent, "first no longer current");
            Assert.IsTrue(secondAfter.IsCurrent, "second is current");
        }

        [Test]
        public async Task CategoryThatDoesntYetExistShouldBeCreatedAndRetrieved()
        {
            var catID = Guid.NewGuid();
            await TestUtilities.LoadCategories(_underTest);

            var rest = TestUtilities.CreateRestaurant();
            await _underTest.AddRestaurantAsync(rest);

            var emp = TestUtilities.CreateEmployee();
            await _underTest.AddEmployeeAsync(emp);

            var invID = Guid.NewGuid();
            var sectionID = Guid.NewGuid();
            var liID = Guid.NewGuid();
            //CREATE
            var inventory = new Inventory
            {
                CreatedByEmployeeID = emp.ID,
                CreatedDate = DateTime.Now.AddDays(-1),
                ID = invID,
                DateCompleted = null,
                LastUpdatedDate = DateTime.Now,
                RestaurantID = rest.ID,
                IsCurrent = false,
                Revision = new EventID { AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 100 },
                Sections = new List<InventorySection>{
                    new InventorySection{
                        Name = "sectionMain",
                        ID = sectionID,
                        Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 1000},
                        LineItems = new List<InventoryBeverageLineItem>
                        {
                            new InventoryBeverageLineItem
                            {
                                Container = new LiquidContainer
                                {
                                    AmountContained = new LiquidAmount {Milliliters = 750},
                                    WeightEmpty = new Weight{Grams = 201}
                                },
                                CreatedDate = DateTime.UtcNow.AddDays(-1),
                                CurrentAmount = new LiquidAmount
                                {
                                    Milliliters = 375
                                },
                                ID = liID,
                                LastUpdatedDate = DateTime.UtcNow.AddDays(-2),
                                MethodsMeasuredLast = MeasurementMethods.VisualEstimate,
                                MiseName = "testItem",
                                DisplayName = "testEntered",
                                PricePaid = new Money(1.0M),
                                RestaurantID = rest.ID,
                                Revision = new EventID {AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 1001},
                                UPC = "1111",
                                NumFullBottles = 1,
                                PartialBottleListing = new List<decimal>{.75M},
                                Categories = new List<ItemCategory>
                                {
                                    CategoriesService.Agave,
                                    new ItemCategory
                                    {
                                        ID = catID,
                                        CreatedDate = DateTime.UtcNow,
                                        LastUpdatedDate = DateTime.UtcNow,
                                        IsCustomCategory = true,
                                        Name = "testCustom",
                                        ParentCategoryID = null,        
                                    }
                                }
                            }
                        }

                    }
                }
            };

            await _underTest.AddInventoryAsync(inventory);

            //RETRIEVE
            var getResults = (await _underTest.GetInventoriesAsync(rest.ID)).ToList();

            Assert.NotNull(getResults);
            Assert.AreEqual(1, getResults.Count(), "first retrieve count");

            var gotInvFirst = getResults.First();
            //check IDs and other fields
            Assert.AreEqual(inventory.ID, gotInvFirst.ID);
            Assert.False(gotInvFirst.IsCurrent, "IsCurrent");
            var lineItems = gotInvFirst.GetBeverageLineItems().ToList();
            Assert.AreEqual(1, lineItems.Count());
            var firstLineItem = lineItems.First();

            var cats = firstLineItem.GetCategories().ToList();
            Assert.AreEqual(2, cats.Count);
            Assert.IsTrue(cats.Select(c => c.ID).Contains(CategoriesService.Agave.ID), "has agave");

            var customCat = cats.FirstOrDefault(c => c.ID == catID);
            
            Assert.NotNull(customCat);
            Assert.IsTrue(customCat.IsCustomCategory);
            Assert.False(customCat.ParentCategoryID.HasValue);
            Assert.AreEqual("testCustom", customCat.Name);
        }
    }
}
