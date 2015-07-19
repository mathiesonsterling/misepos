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
    public class PurchaseOrderTests
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
        public async Task PurchaseOrderBasicCRUD()
        {
            await TestUtilities.LoadCategories(_underTest);

            var rest = TestUtilities.CreateRestaurant();
            await _underTest.AddRestaurantAsync(rest);

            var emp = TestUtilities.CreateEmployee();
            await _underTest.AddEmployeeAsync(emp);

            var vendor = TestUtilities.CreateVendor();
            await _underTest.AddVendorAsync(vendor);

            var poID = Guid.NewGuid();
            var liID = Guid.NewGuid();
            var pvID = Guid.NewGuid();
            var po = new PurchaseOrder
            {
                CreatedDate = DateTimeOffset.UtcNow,
                CreatedByEmployeeID = emp.ID,
                ID = poID,
                LastUpdatedDate = DateTimeOffset.UtcNow,
                RestaurantID = rest.ID,
                Revision = TestUtilities.GetEventID(),
                CreatedByName = "Test Employee",
                PurchaseOrdersPerVendor = new List<PurchaseOrderPerVendor>
                { 
                    new PurchaseOrderPerVendor{
                        ID = pvID,
                        Status = PurchaseOrderStatus.Created,
                        Revision = TestUtilities.GetEventID(),
                        VendorID = vendor.ID,
                        LineItems = new List<PurchaseOrderLineItem>
                        {
                            new PurchaseOrderLineItem
                            {
                                ID = liID,
                                CreatedDate = DateTimeOffset.UtcNow,
                                LastUpdatedDate = DateTimeOffset.UtcNow,
                                MiseName = "lineItem",
                                Quantity = 10,
                                VendorID = vendor.ID,
                                RestaurantID = rest.ID,
                                Revision = TestUtilities.GetEventID(),
                                UPC = "upc",
                                Container = new LiquidContainer
                                {
                                    AmountContained = new LiquidAmount
                                    {
                                        Milliliters = 300,
                                        SpecificGravity = .8M
                                    }
                                },
                                Categories = new List<ItemCategory>
                                {
                                    CategoriesService.LiquerAmaro,
                                    CategoriesService.NonAlcoholic
                                }
                            },
                            new PurchaseOrderLineItem
                            {
                                ID = Guid.NewGuid(),
                                CreatedDate = DateTimeOffset.UtcNow,
                                LastUpdatedDate = DateTimeOffset.UtcNow,
                                MiseName = "secondItem",
                                Quantity = 1,
                                RestaurantID = rest.ID,
                                Revision = TestUtilities.GetEventID(),
                                UPC= "nanan",
                                Container = new LiquidContainer
                                {
                                    AmountContained = new LiquidAmount
                                    {
                                        Milliliters = 750
                                    }
                                }                
                            }
                         }
                    }
                }
            };

            await _underTest.AddPurchaseOrderAsync(po);

            var getRes = (await _underTest.GetPurchaseOrdersAsync(DateTimeOffset.MinValue)).ToList();

            Assert.NotNull(getRes);
            Assert.AreEqual(1, getRes.Count);
            var gotPO = getRes.First();
            Assert.NotNull(gotPO);
            Assert.AreEqual(po.CreatedByEmployeeID, gotPO.CreatedByEmployeeID, "created by emp");
            Assert.AreEqual(po.CreatedDate, gotPO.CreatedDate, "created date");
            Assert.AreEqual(po.CreatedByName, "Test Employee");
            Assert.AreEqual(po.ID, gotPO.ID);
            Assert.AreEqual(po.LastUpdatedDate, gotPO.LastUpdatedDate, "last updated");
            Assert.AreEqual(po.RestaurantID, gotPO.RestaurantID);

            var gotPOVs = gotPO.GetPurchaseOrderPerVendors().ToList();
            Assert.NotNull(gotPOVs, "Got PurchaseOrderPerVendor");
            Assert.AreEqual(1, gotPOVs.Count(), "Number PurchaseOrderPerVendor");
            Assert.AreEqual(po.PurchaseOrdersPerVendor.First().Status, gotPOVs.First().Status, "PurchaseOrderPerVendorStatus");
            Assert.AreEqual(po.Revision.OrderingID, gotPO.Revision.OrderingID, "orderingID");


            var gotLineItems = gotPO.GetPurchaseOrderLineItems().ToList();
            Assert.AreEqual(2, gotLineItems.Count());
            var gotLineItem = gotLineItems.First(li => li.ID == liID);
            Assert.AreEqual("upc", gotLineItem.UPC);
            Assert.AreEqual("lineItem", gotLineItem.MiseName);
            Assert.AreEqual(10, gotLineItem.Quantity);
            Assert.NotNull(gotLineItem.Container);
            Assert.AreEqual(300, gotLineItem.Container.AmountContained.Milliliters);
            Assert.AreEqual(.8M, gotLineItem.Container.AmountContained.SpecificGravity);
            var firstCategories = gotLineItem.GetCategories().ToList();
            Assert.AreEqual(2, firstCategories.Count, "num categories");
            Assert.IsTrue(firstCategories.Select(c => c.ID).Contains(CategoriesService.LiquerAmaro.ID), "contains amaro");
            Assert.IsTrue(firstCategories.Select(c => c.ID).Contains(CategoriesService.NonAlcoholic.ID), "contains non-alcoholic");

            var updatedPO = new PurchaseOrder
            {
                CreatedDate = DateTimeOffset.UtcNow,
                CreatedByEmployeeID = emp.ID,
                ID = poID,
                LastUpdatedDate = DateTimeOffset.UtcNow,
                RestaurantID = rest.ID,
                Revision = TestUtilities.GetEventID(),
                PurchaseOrdersPerVendor = new List<PurchaseOrderPerVendor>{
                    new PurchaseOrderPerVendor
                    {
                        ID = pvID,
                        Status = PurchaseOrderStatus.SentToVendor,
                        Revision = TestUtilities.GetEventID(),
                        VendorID = vendor.ID,
                        LineItems =  new List<PurchaseOrderLineItem>
                        {
                            new PurchaseOrderLineItem
                            {
                                ID = liID,
                                CreatedDate = DateTimeOffset.UtcNow,
                                LastUpdatedDate = DateTimeOffset.UtcNow,
                                MiseName = "lineItemUpdated",
                                Quantity = 15,
                                RestaurantID = rest.ID,
                                Revision = TestUtilities.GetEventID(),
                                UPC = "upcUpdated",
                                                VendorID = vendor.ID,
                                Container = new LiquidContainer
                                {
                                    AmountContained = new LiquidAmount
                                    {
                                        Milliliters = 325,
                                        SpecificGravity = .8M
                                    }
                                }
                            }
                        }
                    }
                }
            };

            await _underTest.UpdatePurchaseOrderAsync(updatedPO);
            var auRes = (await _underTest.GetPurchaseOrdersAsync(DateTimeOffset.MinValue)).ToList();

            Assert.NotNull(auRes);
        }
    }
}
