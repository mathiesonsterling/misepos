using System;
using System.Collections.Generic;
using System.Linq;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Common.Events.Inventory;
using Mise.Core.Entities.Inventory;
using Mise.Core.ValueItems;
using Mise.Core.ValueItems.Inventory;
using NUnit.Framework;
using Mise.Core.Entities;
using System.Runtime.InteropServices;
using Mise.Core.Common.Services;
using System.Reflection;


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
                                Id = Guid.NewGuid(),
                                Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 1},
                                VendorBoughtFrom = Guid.NewGuid(),
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
            Assert.AreEqual(res.First().Id, li.Id);
            Assert.IsTrue(res.First().Revision.Equals(li.Revision));

        }


        [Test]
        public void CompletedShouldSetTime()
        {
            var underTest = new Common.Entities.Inventory.Inventory();

            var completedEvent = new InventoryCompletedEvent
            {
                CausedById = Guid.NewGuid(),
                CreatedDate = DateTimeOffset.UtcNow,
            };

            //ACT
            underTest.When(completedEvent);

            //ASSERT
            Assert.AreEqual(completedEvent.CreatedDate, underTest.LastUpdatedDate);
            Assert.AreEqual(completedEvent.CreatedDate, underTest.DateCompleted, "Completed set to event time");
        }

        [Test]
        public void AddingExistingSectionIsIgnored()
        {
            var underTest = new Common.Entities.Inventory.Inventory
            {
                Id = Guid.NewGuid(),
                Sections = new List<InventorySection>
                {
                    new InventorySection
                    {
                        Id = Guid.NewGuid(),
                        Name = "Test section",
                        LineItems = new List<InventoryBeverageLineItem>()
                    }
                }
            };

            var addEvent = new InventoryNewSectionAddedEvent
            {
                Id = Guid.NewGuid(),
                CreatedDate = DateTime.UtcNow,
                InventoryID = underTest.Id,
                Name = "Updated section",
                SectionID = underTest.Sections.First().Id,
            };

            //ACT
            underTest.When(addEvent);

            //ASSERT
            var sections = underTest.Sections;
            Assert.AreEqual(1, sections.Count);
            Assert.AreEqual("Test section", underTest.Sections.First().Name);
        }

        [Test]
        public void CreateAddsRestaurantSections()
        {
            var underTest = new Common.Entities.Inventory.Inventory();
            var empID = Guid.NewGuid();
            var invID = Guid.NewGuid();
            var id1 = Guid.NewGuid();
            var id2 = Guid.NewGuid();
            var restID = Guid.NewGuid();

            var restSections = new List<IRestaurantInventorySection>{
                new RestaurantInventorySection
            {
                Id = id2,
                Name = "section2"
            },new RestaurantInventorySection
                    {
                        Id = id1,
                        Name = "section1"
                    }
            };
            
            var create = new InventoryCreatedEvent
            {
                CausedById = empID,
                CreatedDate = DateTimeOffset.UtcNow,
                InventoryID = invID,
                RestaurantId = restID,
            };

            var addSections = restSections.Select(rs => new InventoryNewSectionAddedEvent
            {
                CausedById = empID,
                CreatedDate = DateTimeOffset.UtcNow,
                InventoryID = invID,
                RestaurantId = restID,
                RestaurantSectionId = rs.Id,
                SectionID = Guid.NewGuid(),
                Name = rs.Name,
                Id = Guid.NewGuid()
            });
            //ACT
            underTest.When(create);

            foreach (var addSec in addSections)
            {
                underTest.When(addSec);
            }
            //ASSERT
            Assert.AreEqual(underTest.Id, create.InventoryID);
            Assert.AreEqual(underTest.CreatedDate, create.CreatedDate);
            Assert.AreEqual(restID, underTest.RestaurantID, "RestaurantID");
            Assert.AreEqual(create.CreatedDate, underTest.CreatedDate);


            var sections = underTest.GetSections().ToList();
            Assert.AreEqual(2, sections.Count, "has 2 sections");

            var firstSec = sections.First(s => s.Name == "section1");
            Assert.AreEqual(id1, firstSec.RestaurantInventorySectionID);
            Assert.AreEqual(restID, firstSec.RestaurantID);

            var secondSec = sections.First(s => s.Name == "section2");
            Assert.AreEqual(id2, secondSec.RestaurantInventorySectionID);
            Assert.AreEqual(restID, secondSec.RestaurantID);

        }


        [Test]
        public void InventoryMeasuredInNonExistentSectionShouldThrow()
        {
            var underTest = new Common.Entities.Inventory.Inventory();

            var inventoryEvent = new InventoryLiquidItemMeasuredEvent
            {
                CausedById = Guid.NewGuid(),
                CreatedDate = DateTime.UtcNow,
                InventoryID = Guid.NewGuid(),
				AmountMeasured = new LiquidAmount {Milliliters = 375},
                InventorySectionID = Guid.NewGuid(),
                BeverageLineItem = new InventoryBeverageLineItem
                {
                    Container = LiquidContainer.Bottle750ML,
                    CreatedDate = DateTime.UtcNow.AddDays(-1),
                    NumFullBottles = 10,
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
        public void InventoryMeasuredFullByVisualShouldUpdateItem()
        {
            var restSectionID = Guid.NewGuid();
            var invSectionID = Guid.NewGuid();
            var liID = Guid.NewGuid();
            var underTest = new Common.Entities.Inventory.Inventory
            {
                Sections = new List<InventorySection>
                {
                    new InventorySection
                    {
                        Id = invSectionID,
                        RestaurantInventorySectionID = restSectionID,
                        Name = "newSection",
                        LineItems = new List<InventoryBeverageLineItem>
                        {
                            new InventoryBeverageLineItem
                            {
                                Id = liID,
                                Container = LiquidContainer.Bottle750ML
                            }
                        }
                    }
                }
            };

            var inventoryEvent = new InventoryLiquidItemMeasuredEvent
            {
                CausedById = Guid.NewGuid(),
                CreatedDate = DateTime.UtcNow,
                InventoryID = Guid.NewGuid(),
				AmountMeasured = LiquidAmount.SevenFiftyMillilters,
                InventorySectionID = invSectionID,
                NumFullBottlesMeasured = 10,
                BeverageLineItem = new InventoryBeverageLineItem
                {
                    Id = liID,
                    Container = LiquidContainer.Bottle750ML,
                    CreatedDate = DateTime.UtcNow.AddDays(-1),
                    NumFullBottles = 0,
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
        }

        [Test]
        public void InventoryMeasuredWithoutVendorItemThrows()
        {
            var underTest = new Common.Entities.Inventory.Inventory();

            var invEvent = new InventoryLiquidItemMeasuredEvent
            {
				AmountMeasured = new LiquidAmount {Milliliters = 150},
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
        public void InventoryMeasuredWithNegativeAmountThrows()
        {
            var underTest = new Common.Entities.Inventory.Inventory();
            var liID = Guid.NewGuid();
            var lineItem = new InventoryBeverageLineItem
            {
                Id = liID,
                Container = LiquidContainer.Bottle330ML
            };

            var sectionID = Guid.NewGuid();
            underTest.Sections.Add(
                new InventorySection
                {
                    Id = sectionID,
                    LineItems = new List<InventoryBeverageLineItem> { lineItem}
                });

            var invEvent = new InventoryLiquidItemMeasuredEvent
            {
				AmountMeasured = new LiquidAmount {Milliliters = -10000},
                BeverageLineItem = new InventoryBeverageLineItem
                {
                    Id = liID,
                },
                InventorySectionID = sectionID
            };

            //ACT
            var threw = false;
            try
            {
                underTest.When(invEvent);
            }
            catch (ArgumentException ae)
            {
                Assert.IsTrue(ae.Message.Contains("Cannot measure a negative amount"));
                threw = true;
            }

            //ASSERT
            Assert.IsTrue(threw);
        }

        [Test]
        public void InventoryMeasuredWithNonExistentItemThrows()
        {
            var underTest = new Common.Entities.Inventory.Inventory();
            var sectionID = Guid.NewGuid();
            underTest.Sections.Add(
                new InventorySection
                {
                    Id = sectionID
                });
            var invEvent = new InventoryLiquidItemMeasuredEvent
            {
                AmountMeasured = new LiquidAmount { Milliliters = 1000 },
                BeverageLineItem = new InventoryBeverageLineItem
                {
                    Id = Guid.NewGuid(),
                    DisplayName = "testItem"
                },
                InventorySectionID = sectionID,
            };

            //ACT
            var threw = false;
            try
            {
                underTest.When(invEvent);
            }
            catch (ArgumentException ae)
            {
                Assert.True(ae.Message.Contains("No inventory item of testItem"));
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
                Id = Guid.NewGuid(),
                DateCompleted = DateTimeOffset.UtcNow,
                RestaurantID = Guid.NewGuid(),
                IsCurrent = true,
                Sections = new List<InventorySection>{
                    new InventorySection{
                        Id = Guid.NewGuid(),
                        RestaurantInventorySectionID = Guid.NewGuid(),
                        Name = "merg",
                        LineItems = new List<InventoryBeverageLineItem>
                        {
                            new InventoryBeverageLineItem
                            {
                                Id = Guid.NewGuid(),
                                LastUpdatedDate = DateTime.UtcNow,
                                Revision = new EventID {AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 1},
                                CreatedDate = DateTime.UtcNow,
                                CurrentAmount = new LiquidAmount
                                {
                                    Milliliters = 10.0M,
                                    SpecificGravity = 1.0M
                                },
                                NumFullBottles = 13,
                                PricePaid = new Money(11.11M)
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
            Assert.AreEqual(underTest.Id, clone.Id);
            Assert.AreEqual(underTest.DateCompleted, clone.DateCompleted);
            Assert.AreEqual(underTest.LastUpdatedDate, clone.LastUpdatedDate);
            Assert.AreEqual(underTest.RestaurantID, clone.RestaurantID);

            var clonedBIs = clone.GetBeverageLineItems().ToList();
            foreach (var item in underTest.GetBeverageLineItems())
            {
                var clonedItem = clonedBIs.FirstOrDefault(bi => bi.Id == item.Id);
                Assert.NotNull(clonedItem);

                Assert.AreEqual(item.CurrentAmount, clonedItem.CurrentAmount);
                Assert.AreEqual(item.CreatedDate, clonedItem.CreatedDate);
                Assert.AreEqual(item.LastUpdatedDate, clonedItem.LastUpdatedDate);
                Assert.AreEqual(item.NumFullBottles, clonedItem.NumFullBottles);
            }

            Assert.AreEqual(underTest.Sections.First().Name, clone.Sections.First().Name);
            Assert.AreEqual(underTest.GetSections().First().Id, clone.GetSections().First().Id, "Section ID");
            Assert.AreEqual(underTest.GetBeverageLineItems().First().PricePaid, clone.GetBeverageLineItems().First().PricePaid);
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
                CausedById = Guid.NewGuid(),
                EventOrder = new EventID { AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 1 },
                DisplayName = "addedItem",
                InventoryID = Guid.NewGuid(),
                Quantity = 1,
                Categories = new List<InventoryCategory>
                {
                    new InventoryCategory
                    {
                        Id = Guid.NewGuid(),
                        Name ="testCat"
                    }
                },
                PricePaid = new Money(11.11M)
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

		[Test]
		public void DeleteEventShouldRemoveLineItem(){
			var liID = Guid.NewGuid ();
			var underTest = new Common.Entities.Inventory.Inventory {
				Id = Guid.NewGuid (),
				Sections = new List<InventorySection> {
					new InventorySection {
						Id = Guid.NewGuid (),
						LineItems = new List<InventoryBeverageLineItem> {
							new InventoryBeverageLineItem {
								Id = liID,
								DisplayName = "testItem"
							},
							new InventoryBeverageLineItem {
								Id = Guid.NewGuid (),
								DisplayName = "notDel"
							}
						}
					}
				}
			};

			var delEv = new InventoryLineItemDeletedEvent {
				Id = Guid.NewGuid (),
				InventoryID = underTest.Id,
				InventorySectionID = underTest.Sections.First ().Id,
				InventoryLineItemID = liID
			};

			//ACT
			underTest.When (delEv);

			//ASSERT
			var lis = underTest.GetBeverageLineItems ().ToList ();
			Assert.AreEqual (1, lis.Count());
			Assert.AreEqual ("notDel", lis.First().DisplayName);
		}
    }
}
