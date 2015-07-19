using System;
using System.Collections.Generic;
using System.Linq;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Common.Events.Inventory;
using Mise.Core.ValueItems;
using Mise.Core.ValueItems.Inventory;
using NUnit.Framework;
using Mise.Core.Entities;
namespace Mise.Core.Common.UnitTests.Entities.Inventory
{
    [TestFixture]
    public class InventoryTests
    {
        [Test]
        public void ShouldGetBeverageItems()
        {
            var underTest = new Common.Entities.Inventory.Inventory
            {
                Sections = new List<InventorySection>{
                    new InventorySection{
                        Name = "sectionName",
                        LineItems = new List<InventoryBeverageLineItem>
                        {
                            new InventoryBeverageLineItem
                            {
                                ID = Guid.NewGuid(),
                                Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 1},
                                VendorBoughtFrom = Guid.NewGuid(),
                                PricePaid = new Money(9.00M),
                                UPC = "11111"
                            },
                            new InventoryBeverageLineItem()
                        }
                    }
                }
            };

            //ACT
            var res = underTest.GetBeverageLineItems().ToList();

            //ASSERT
            Assert.AreEqual(2, res.Count());

            var li = underTest.Sections.First().GetInventoryBeverageLineItemsInSection().First();
            Assert.AreEqual(res.First().ID, li.ID);
            Assert.IsTrue(res.First().Revision.Equals(li.Revision));

            Assert.AreEqual(9.00M, res.First().PricePaid.Dollars);
        }


        [Test]
        public void CompletedShouldSetTime()
        {
            var underTest = new Common.Entities.Inventory.Inventory();

            var completedEvent = new InventoryCompletedEvent
            {
                CausedByID = Guid.NewGuid(),
                CreatedDate = DateTimeOffset.UtcNow,
            };

            //ACT
            underTest.When(completedEvent);

            //ASSERT
            Assert.AreEqual(completedEvent.CreatedDate, underTest.LastUpdatedDate);
            Assert.AreEqual(completedEvent.CreatedDate, underTest.DateCompleted, "Completed set to event time");
        }

        [Test]
        public void CreateAddsRestaurantSections()
        {
            var underTest = new Common.Entities.Inventory.Inventory();

            var id1 = Guid.NewGuid();
            var id2 = Guid.NewGuid();
            var restID = Guid.NewGuid();
            var create = new InventoryCreatedEvent
            {
                CausedByID = Guid.NewGuid(),
                CreatedDate = DateTimeOffset.UtcNow,
                RestaurantSectionsAndSectionIDs = new List<Tuple<RestaurantInventorySection, Guid>>
                {
                    new Tuple<RestaurantInventorySection, Guid>(
                    new RestaurantInventorySection
                    {
                        ID = id1,
                        Name = "section1"
                    }, Guid.NewGuid()),
                    new Tuple<RestaurantInventorySection, Guid>(
                    new RestaurantInventorySection
                    {
                        ID = id2,
                        Name = "section2"
                    }, Guid.NewGuid())
                },
                InventoryID = Guid.NewGuid(),
                RestaurantID = restID,
            };

            //ACT
            underTest.When(create);

            //ASSERT
            Assert.AreEqual(underTest.ID, create.InventoryID);
            Assert.AreEqual(underTest.CreatedDate, create.CreatedDate);
            Assert.AreEqual(underTest.LastUpdatedDate, create.CreatedDate);
            Assert.AreEqual(restID, underTest.RestaurantID, "RestaurantID");
            Assert.AreEqual(create.CreatedDate, underTest.CreatedDate);
            Assert.AreEqual(create.CreatedDate, underTest.LastUpdatedDate, "last updated");


            var sections = underTest.GetSections().ToList();
            Assert.AreEqual(2, sections.Count, "has 2 sections");

            var firstSec = sections.First();
            Assert.AreEqual("section1", firstSec.Name);
            Assert.AreEqual(id1, firstSec.RestaurantInventorySectionID);
            Assert.AreEqual(restID, firstSec.RestaurantID);

            var secondSec = sections.Last();
            Assert.AreEqual("section2", secondSec.Name);
            Assert.AreEqual(id2, secondSec.RestaurantInventorySectionID);
            Assert.AreEqual(restID, secondSec.RestaurantID);

        }

        [Test]
        public void InventoryMeasuredShouldAddInventoryItemToExistingSection()
        {
            var sectionID = Guid.NewGuid();
            var underTest = new Common.Entities.Inventory.Inventory
            {
                Sections = new List<InventorySection>
                {
                    new InventorySection
                    {
                        ID = Guid.NewGuid(),
                        RestaurantInventorySectionID = sectionID,
                        Name = "sectionMain"
                    }
                }
            };

            var inventoryEvent = new InventoryLiquidItemMeasuredEvent
            {
                CausedByID = Guid.NewGuid(),
                CreatedDate = DateTime.UtcNow,
                InventoryID = Guid.NewGuid(),
				AmountMeasured = new LiquidAmount{Milliliters = 375},
                RestaurantInventorySectionID = sectionID,
                BeverageLineItem = new InventoryBeverageLineItem
                {
                    Container = new LiquidContainer
                    {
                        AmountContained = new LiquidAmount
                        {
                            Milliliters = 750
                        }
                    },
                    CreatedDate = DateTime.UtcNow.AddDays(-1),
					NumFullBottles = 10,
                    PricePaid = new Money(10.0M),
                    MethodsMeasuredLast = MeasurementMethods.VisualEstimate
                }
            };

            //ACT
            underTest.When(inventoryEvent);
            var items = underTest.GetBeverageLineItems().ToList();

            //ASSERT
            Assert.NotNull(items);
            Assert.AreEqual(1, items.Count);

            var item = items.First();
            Assert.IsNotNull(item);
            Assert.AreEqual(375, item.CurrentAmount.Milliliters);
            Assert.AreEqual(MeasurementMethods.VisualEstimate, item.MethodsMeasuredLast);
        }

        [Test]
        public void InventoryMeasuredInNonExistentSectionShouldThrow()
        {
            var sectionID = Guid.NewGuid();
            var underTest = new Common.Entities.Inventory.Inventory();

            var inventoryEvent = new InventoryLiquidItemMeasuredEvent
            {
                CausedByID = Guid.NewGuid(),
                CreatedDate = DateTime.UtcNow,
                InventoryID = Guid.NewGuid(),
				AmountMeasured = new LiquidAmount (){Milliliters = 375},
                RestaurantInventorySectionID = sectionID,
                BeverageLineItem = new InventoryBeverageLineItem
                {
                    Container = new LiquidContainer
                    {
                        AmountContained = new LiquidAmount
                        {
                            Milliliters = 750
                        }
                    },
                    CreatedDate = DateTime.UtcNow.AddDays(-1),
                    NumFullBottles = 10,
                    PricePaid = new Money(10.0M),
                    MethodsMeasuredLast = MeasurementMethods.VisualEstimate
                }
            };

            //ACT
            var threw = false;
            try
            {
                underTest.When(inventoryEvent);
            }
            catch (ArgumentException)
            {
                threw = true;
            }

            //ASSERT
            Assert.True(threw);
        }

        /// <summary>
        /// Checks that a full bottle is OK
        /// </summary>
        [Test]
        public void InventoryMeasuredFullByVisualShouldAddInventoryItem()
        {
            var restSectionID = Guid.NewGuid();

            var underTest = new Common.Entities.Inventory.Inventory
            {
                Sections = new List<InventorySection>
                {
                    new InventorySection
                    {
                        RestaurantInventorySectionID = restSectionID,
                        Name = "newSection"
                    }
                }
            };

            var inventoryEvent = new InventoryLiquidItemMeasuredEvent
            {
                CausedByID = Guid.NewGuid(),
                CreatedDate = DateTime.UtcNow,
                InventoryID = Guid.NewGuid(),
				AmountMeasured = new LiquidAmount (){Milliliters = 750},
                RestaurantInventorySectionID = restSectionID,
                NumFullBottlesMeasured = 10,
                BeverageLineItem = new InventoryBeverageLineItem
                {
                    Container = new LiquidContainer
                    {
                        AmountContained = new LiquidAmount
                        {
                            Milliliters = 750
                        }
                    },
                    CreatedDate = DateTime.UtcNow.AddDays(-1),
                    NumFullBottles = 0,
                    PricePaid = new Money(10.0M),
                    MethodsMeasuredLast = MeasurementMethods.VisualEstimate
                }
            };

            //ACT
            underTest.When(inventoryEvent);
            var items = underTest.GetBeverageLineItems().ToList();

            //ASSERT
            Assert.NotNull(items);
            Assert.AreEqual(1, items.Count);

            var item = items.First();
            Assert.IsNotNull(item);
            Assert.AreEqual(750, item.CurrentAmount.Milliliters);
            Assert.AreEqual(MeasurementMethods.VisualEstimate, item.MethodsMeasuredLast);
            Assert.AreEqual(10, item.NumFullBottles, "bottleQuantity");
            Assert.AreEqual(10.0M, item.PricePaid.Dollars);
        }

        [Test]
        public void InventoryMeasuredWithoutVendorItemThrows()
        {
            var underTest = new Common.Entities.Inventory.Inventory();

            var invEvent = new InventoryLiquidItemMeasuredEvent
            {
				AmountMeasured = new LiquidAmount (){Milliliters = 150},
                BeverageLineItem = null
            };

            //ACT
            var threw = false;
            try
            {
                underTest.When(invEvent);
            }
            catch (ArgumentException)
            {
                threw = true;
            }

            //ASSERT
            Assert.IsTrue(threw);
        }
        [Test]
        public void InventoryMeasuredWithInvalidPercentageThrows()
        {
            var underTest = new Common.Entities.Inventory.Inventory();

            var invEvent = new InventoryLiquidItemMeasuredEvent
            {
				AmountMeasured = new LiquidAmount (){Milliliters = 1000},
                BeverageLineItem = new InventoryBeverageLineItem()
            };

            //ACT
            var threw = false;
            try
            {
                underTest.When(invEvent);
            }
            catch (ArgumentException)
            {
                threw = true;
            }

            //ASSERT
            Assert.IsTrue(threw);
        }

        [Test]
        public void TestClone()
        {
            var underTest = new Common.Entities.Inventory.Inventory
            {
                Revision = new EventID { AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 0 },
                CreatedByEmployeeID = Guid.NewGuid(),
                CreatedDate = DateTimeOffset.UtcNow.AddDays(-1),
                LastUpdatedDate = DateTimeOffset.UtcNow,
                ID = Guid.NewGuid(),
                DateCompleted = DateTimeOffset.UtcNow,
                RestaurantID = Guid.NewGuid(),
                IsCurrent = true,
                Sections = new List<InventorySection>{
                    new InventorySection{
                        Name = "merg",
                        LineItems = new List<InventoryBeverageLineItem>
                        {
                            new InventoryBeverageLineItem
                            {
                                ID = Guid.NewGuid(),
                                LastUpdatedDate = DateTime.UtcNow,
                                Revision = new EventID {AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 1},
                                CreatedDate = DateTime.UtcNow,
                                CurrentAmount = new LiquidAmount
                                {
                                    Milliliters = 10.0M,
                                    SpecificGravity = 1.0M
                                },
                                NumFullBottles = 13
                            }
                        }
                    }
                }
            };

            //ACT
            var clone = underTest.Clone() as Common.Entities.Inventory.Inventory;

            //ASSERT
            Assert.NotNull(clone);
            Assert.AreEqual(underTest.Revision, clone.Revision);
            Assert.AreEqual(underTest.CreatedByEmployeeID, clone.CreatedByEmployeeID);
            Assert.AreEqual(underTest.CreatedDate, clone.CreatedDate);
            Assert.AreEqual(underTest.IsCurrent, clone.IsCurrent);
            Assert.AreEqual(underTest.ID, clone.ID);
            Assert.AreEqual(underTest.DateCompleted, clone.DateCompleted);
            Assert.AreEqual(underTest.LastUpdatedDate, clone.LastUpdatedDate);
            Assert.AreEqual(underTest.RestaurantID, clone.RestaurantID);

            var clonedBIs = clone.GetBeverageLineItems().ToList();
            foreach (var item in underTest.GetBeverageLineItems())
            {
                var clonedItem = clonedBIs.FirstOrDefault(bi => bi.ID == item.ID);
                Assert.NotNull(clonedItem);

                Assert.AreEqual(item.CurrentAmount, clonedItem.CurrentAmount);
                Assert.AreEqual(item.CreatedDate, clonedItem.CreatedDate);
                Assert.AreEqual(item.LastUpdatedDate, clonedItem.LastUpdatedDate);
                Assert.AreEqual(item.NumFullBottles, clonedItem.NumFullBottles);
            }

            Assert.AreEqual(underTest.Sections.First().Name, clone.Sections.First().Name);
            Assert.AreEqual(underTest.GetSections().First().ID, clone.GetSections().First().ID, "Section ID");
        }

        [Test]
        public void AddLineItemWithoutSectionAddsToMostPopulated()
        {
            var underTest = new Common.Entities.Inventory.Inventory
            {
                Sections = new List<InventorySection>
                {
                    new InventorySection
                    {
                        Name = "storeRoom",
                        LineItems = new List<InventoryBeverageLineItem>
                        {
                            new InventoryBeverageLineItem
                            {
                                MiseName = "testalreadythere"
                            }
                        }
                    },
                    new InventorySection
                    {
                        Name = "emptySec",
                        LineItems = new List<InventoryBeverageLineItem>()
                    }
                }
            };

            var addEvent = new InventoryLineItemAddedEvent
            {
                CausedByID = Guid.NewGuid(),
                EventOrderingID = new EventID { AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 1 },
                DisplayName = "addedItem",
                InventoryID = Guid.NewGuid(),
                Quantity = 1,
                Categories = new List<ItemCategory>
                {
                    new ItemCategory
                    {
                        ID = Guid.NewGuid(),
                        Name ="testCat"
                    }
                }
            };

            //ACT
            underTest.When(addEvent);
            var sections = underTest.GetSections().ToList();

            //ASSERT
            Assert.AreEqual(2, sections.Count());

            var storeRoom = sections.FirstOrDefault(s => s.Name == "storeRoom");
            Assert.NotNull(storeRoom);
            Assert.AreEqual(2, storeRoom.GetInventoryBeverageLineItemsInSection().Count());

            var otherSec = sections.FirstOrDefault(s => s.Name != "storeRoom");
            Assert.NotNull(otherSec);
            Assert.False(otherSec.GetInventoryBeverageLineItemsInSection().Any());
        }
    }
}
