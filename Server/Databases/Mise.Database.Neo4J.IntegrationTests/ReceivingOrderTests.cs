using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Common;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Entities;
using Mise.Core.Services;
using Mise.Core.Services.UtilityServices;
using Mise.Core.ValueItems;
using Mise.Core.ValueItems.Inventory;
using Mise.Neo4J;
using Mise.Neo4J.Neo4JDAL;
using Moq;
using NUnit.Framework;

namespace Mise.Database.Neo4J.IntegrationTests
{
    [TestFixture]
    public class ReceivingOrderTests
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
        public async Task ReceivingOrderWithoutPurchaseOrderTieInShouldBasicCRUD()
        {
            var rest = TestUtilities.CreateRestaurant();
            await _underTest.AddRestaurantAsync(rest);

            var emp = TestUtilities.CreateEmployee();
            await _underTest.AddEmployeeAsync(emp);

            var vendor = TestUtilities.CreateVendor();
            await _underTest.AddVendorAsync(vendor);

            var roID = Guid.NewGuid();
            var liID = Guid.NewGuid();
            //CREATE
            var ro = new ReceivingOrder
            {
                ID = roID,
                CreatedDate = DateTime.UtcNow.AddDays(-1),
                Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 19191},
                Status = ReceivingOrderStatus.Created,
                LastUpdatedDate = DateTime.UtcNow.AddDays(-1),
                RestaurantID = rest.ID,
                ReceivedByEmployeeID = emp.ID,
                VendorID = vendor.ID,
                InvoiceID = "firstInvoiceID",
                Notes = "testing how we make notes!",
                DateReceived = new DateTime(2015, 01, 01),
                LineItems = new List<ReceivingOrderLineItem>
                {
                    new ReceivingOrderLineItem
                    {
                        Container = new LiquidContainer{AmountContained = new LiquidAmount{Milliliters = 375}},
                        ID = liID,
                        CreatedDate = DateTime.UtcNow.AddDays(-1),
                        MiseName = "testROLine",
                        UPC = "1111",
                        RestaurantID = rest.ID,
                        Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101011},
                        Quantity = 12,
                        LineItemPrice = new Money(12.99M),
                        UnitPrice = new Money(12.99M/12),
                        Categories = new List<ItemCategory>
                                {
                                    CategoriesService.Unknown,
                                    CategoriesService.NonAlcoholic
                                }
                    }
                }
            };

            await _underTest.AddReceivingOrderAsync(ro);

            var firstGetRes = (await _underTest.GetReceivingOrdersAsync(DateTime.Now.AddYears(-2))).ToList();

            Assert.NotNull(firstGetRes);
            Assert.AreEqual(1, firstGetRes.Count());

            var gotRes = firstGetRes.First();
            Assert.NotNull(gotRes);
            Assert.AreEqual(rest.ID, gotRes.RestaurantID, "RestaurantID");
            Assert.AreEqual(emp.ID, gotRes.ReceivedByEmployeeID, "Employee ID");
            Assert.AreEqual(ro.VendorID, gotRes.VendorID, "VendorID");
            Assert.AreEqual(ro.Notes, gotRes.Notes, "Notes");
            Assert.AreEqual(ro.InvoiceID, gotRes.InvoiceID, "InvoiceID");
            Assert.AreEqual(ro.DateReceived, new DateTimeOffset(new DateTime(2015, 01, 01)));
            //test line items and their container
            var lis = gotRes.GetBeverageLineItems().ToList();
            Assert.AreEqual(1, lis.Count);
            var li = lis.FirstOrDefault();
            Assert.NotNull(li);
            Assert.AreEqual(12, li.Quantity);
            Assert.NotNull(li.LineItemPrice, "Price is null");
            Assert.NotNull(li.UnitPrice, "Unit price is null");
            Assert.AreEqual(12.99M, li.LineItemPrice.Dollars);
            var aiContainer = li.Container;
            Assert.NotNull(aiContainer, "Container");
            Assert.NotNull(aiContainer.AmountContained, "AmountContainer is null");
            Assert.AreEqual(375, aiContainer.AmountContained.Milliliters);
          
            var firstCategories = li.GetCategories().ToList();
            Assert.IsTrue(firstCategories.Select(c => c.ID).Contains(CategoriesService.Unknown.ID));
            Assert.IsTrue(firstCategories.Select(c => c.ID).Contains(CategoriesService.NonAlcoholic.ID));

            //UPDATE
            var updatedReceivingOrder = new ReceivingOrder
            {
                ID = roID,
                CreatedDate = DateTime.UtcNow,
                Status = ReceivingOrderStatus.Completed,
                LastUpdatedDate = DateTime.UtcNow,
                Revision = new EventID { AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 1010102 },
                RestaurantID = rest.ID,
                ReceivedByEmployeeID = emp.ID,
                VendorID = vendor.ID,
                InvoiceID = "updatedInvoiceID",
                Notes = string.Empty,
                LineItems = new List<ReceivingOrderLineItem>
                {
                    new ReceivingOrderLineItem
                    {
                        Container = new LiquidContainer{AmountContained = new LiquidAmount{Milliliters = 750}},
                        ID = liID,
                        CreatedDate = DateTime.UtcNow.AddDays(-1),
                        MiseName = "testROLine",
                        UPC = "1111",
                        RestaurantID = rest.ID,
                        Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101011},
                        Quantity = 15,
                        LineItemPrice = new Money(20.0M),
                        UnitPrice = new Money(20.0M/15)
                    }
                }
            };

            await _underTest.UpdateReceivingOrderAsync(updatedReceivingOrder);

            var auRes = (await _underTest.GetReceivingOrdersAsync(DateTime.MinValue)).ToList();
            Assert.AreEqual(1, auRes.Count());

            var updatedRO = auRes.First();
            Assert.AreEqual(updatedReceivingOrder.InvoiceID, updatedRO.InvoiceID, "update InvoiceID");
            Assert.AreEqual(updatedReceivingOrder.Notes, updatedRO.Notes);

            var lineItems = updatedRO.GetBeverageLineItems().ToList();
            Assert.AreEqual(1, lineItems.Count());
            var gotLI = lineItems.First();
            Assert.AreEqual(750, gotLI.Container.AmountContained.Milliliters);
            Assert.AreEqual(gotLI.LineItemPrice.Dollars, 20);
        }

        [Test]
        public async Task TwoReceivingOrdersBetweenRestaurantAndVendorShouldHaveVendorAssociatedWithRestaurant()
        {
            var rest = TestUtilities.CreateRestaurant();
            await _underTest.AddRestaurantAsync(rest);

            var emp = TestUtilities.CreateEmployee();
            await _underTest.AddEmployeeAsync(emp);

            var vendor = TestUtilities.CreateVendor();
            await _underTest.AddVendorAsync(vendor);

            var ro1 = new ReceivingOrder
            {
                ID = Guid.NewGuid(),
                Revision = TestUtilities.GetEventID(),
                VendorID = vendor.ID,
                CreatedDate = DateTimeOffset.UtcNow,
                RestaurantID = rest.ID,
                ReceivedByEmployeeID = emp.ID
            };
            await _underTest.AddReceivingOrderAsync(ro1);

            var ro2 = new ReceivingOrder
            {
                ID = Guid.NewGuid(),
                Revision = TestUtilities.GetEventID(),
                VendorID = vendor.ID,
                CreatedDate = DateTimeOffset.UtcNow,
                RestaurantID = rest.ID,
                ReceivedByEmployeeID = emp.ID
            };
            await _underTest.AddReceivingOrderAsync(ro2);

            var res = (await _underTest.GetRestaurantIDsAssociatedWithVendor(vendor.ID)).ToList();

            //ASSERT
            Assert.AreEqual(1, res.Count());
            Assert.AreEqual(rest.ID, res.First());
        }

        [Test]
        public async Task UpdatingWithRemovedLineItemsShouldRemove()
        {
            var rest = TestUtilities.CreateRestaurant();
            await _underTest.AddRestaurantAsync(rest);

            var emp = TestUtilities.CreateEmployee();
            await _underTest.AddEmployeeAsync(emp);

            var vendor = TestUtilities.CreateVendor();
            await _underTest.AddVendorAsync(vendor);

            var roID = Guid.NewGuid();
            var liID = Guid.NewGuid();
            //CREATE
            var ro = new ReceivingOrder
            {
                ID = roID,
                CreatedDate = DateTime.UtcNow.AddDays(-1),
                Revision = new EventID { AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 19191 },
                Status = ReceivingOrderStatus.Created,
                LastUpdatedDate = DateTime.UtcNow.AddDays(-1),
                RestaurantID = rest.ID,
                ReceivedByEmployeeID = emp.ID,
                VendorID = vendor.ID,
                Notes = "testing how we make notes!",
                LineItems = new List<ReceivingOrderLineItem>
                {
                    new ReceivingOrderLineItem
                    {
                        Container = new LiquidContainer{AmountContained = new LiquidAmount{Milliliters = 375}},
                        ID = liID,
                        CreatedDate = DateTime.UtcNow.AddDays(-1),
                        MiseName = "testROLine",
                        UPC = "1111",
                        RestaurantID = rest.ID,
                        Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101011},
                        Quantity = 12,
                        LineItemPrice = new Money(12.99M),
                        UnitPrice = new Money(12.99M/12)
                    }
                }
            };

            await _underTest.AddReceivingOrderAsync(ro);

            var firstGetRes = (await _underTest.GetReceivingOrdersAsync(DateTime.Now.AddYears(-2))).ToList();

            Assert.NotNull(firstGetRes);
            Assert.AreEqual(1, firstGetRes.Count());

            var gotRes = firstGetRes.First();
            Assert.NotNull(gotRes);
            Assert.AreEqual(rest.ID, gotRes.RestaurantID, "RestaurantID");
            Assert.AreEqual(emp.ID, gotRes.ReceivedByEmployeeID, "Employee ID");
            Assert.AreEqual(ro.VendorID, gotRes.VendorID, "VendorID");
            Assert.AreEqual(ro.Notes, gotRes.Notes, "Notes");
            //test line items and their container
            var lis = gotRes.GetBeverageLineItems().ToList();
            Assert.AreEqual(1, lis.Count);
            var li = lis.FirstOrDefault();
            Assert.NotNull(li);
            Assert.AreEqual(12, li.Quantity);
            Assert.NotNull(li.LineItemPrice, "Price is null");
            Assert.AreEqual(12.99M, li.LineItemPrice.Dollars);
            Assert.NotNull(li.UnitPrice, "unit price is null");
            var aiContainer = li.Container;
            Assert.NotNull(aiContainer, "Container");
            Assert.NotNull(aiContainer.AmountContained, "AmountContainer is null");
            Assert.AreEqual(375, aiContainer.AmountContained.Milliliters);

            //UPDATE
            var updatedReceivingOrder = new ReceivingOrder
            {
                ID = roID,
                CreatedDate = DateTime.UtcNow,
                Status = ReceivingOrderStatus.Completed,
                LastUpdatedDate = DateTime.UtcNow,
                Revision = new EventID { AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 1010102 },
                RestaurantID = rest.ID,
                ReceivedByEmployeeID = emp.ID,
                VendorID = vendor.ID,
                LineItems = new List<ReceivingOrderLineItem>()
            };

            await _underTest.UpdateReceivingOrderAsync(updatedReceivingOrder);
            var auRes = await _underTest.GetReceivingOrdersAsync(DateTime.MinValue);

            //check our line items are gone
            var auLineItems = auRes.First().GetBeverageLineItems();
            Assert.AreEqual(0, auLineItems.Count());
        }
    }
}
