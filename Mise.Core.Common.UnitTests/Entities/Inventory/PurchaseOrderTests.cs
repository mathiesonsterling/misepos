using System;
using System.Collections.Generic;
using System.Linq;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Common.Events.Inventory;
using Mise.Core.ValueItems;
using NUnit.Framework;
using Mise.Core.Entities;

namespace Mise.Core.Common.UnitTests.Entities.Inventory
{
    [TestFixture]
    public class PurchaseOrderTests
    {
        [Test]
        public void CloneShouldFillAllFields()
        {
            var underTest = new PurchaseOrder
            {
                CreatedByEmployeeID = Guid.NewGuid(),
                CreatedDate = DateTime.UtcNow.AddDays(-3),
                ID = Guid.NewGuid(),
                LastUpdatedDate = DateTime.UtcNow,
                RestaurantID = Guid.NewGuid(),
                Revision = new EventID {AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 1},
				PurchaseOrdersPerVendor = new List<PurchaseOrderPerVendor>{
					new PurchaseOrderPerVendor{
						LineItems = new List<PurchaseOrderLineItem>
                			{
			                    new PurchaseOrderLineItem
			                    {
			                        ID = Guid.NewGuid(),
			                        Quantity = 1,
			                        MiseName = "test1",
			                        UPC = "1111"
			                    },
			                    new PurchaseOrderLineItem
			                    {
			                        ID = Guid.NewGuid(),
			                        Quantity = 13,
			                        MiseName = "test2",
			                        UPC = "2222"
			                    }
						}
					}
				}
            };

            //ACT
            var clone = underTest.Clone() as PurchaseOrder;

            //ASSERT
            Assert.IsNotNull(clone);
            Assert.AreEqual(underTest.CreatedByEmployeeID, clone.CreatedByEmployeeID);
            Assert.AreEqual(underTest.CreatedDate, underTest.CreatedDate);
            Assert.AreEqual(underTest.ID, clone.ID);
            Assert.AreEqual(underTest.LastUpdatedDate, clone.LastUpdatedDate);
            Assert.AreEqual(underTest.RestaurantID, clone.RestaurantID);

            //line items
            var lineItems = clone.GetPurchaseOrderLineItems ().ToList ();
            Assert.NotNull(lineItems);
            Assert.AreEqual(2, lineItems.Count);

            Assert.AreEqual(1, lineItems.First().Quantity);
            Assert.AreEqual(13, lineItems.Last().Quantity);

            Assert.AreEqual("test1",lineItems.First().MiseName);
            Assert.AreEqual("2222", lineItems.Last().UPC, "last upc");
        }

        [Test]
        public void CreateShouldFillInIDAndDates()
        {
            var underTest = new PurchaseOrder();

            var create = new PurchaseOrderCreatedEvent
            {
                PurchaseOrderID = Guid.NewGuid(),
                CreatedDate = DateTime.UtcNow,
                CausedByID = Guid.NewGuid(),
                EventOrderingID = new EventID {AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 12},
                RestaurantID = Guid.NewGuid()
            };

            //ACT
            underTest.When(create);

            //ASSERT
            Assert.AreEqual(create.PurchaseOrderID, underTest.ID, "ID");
            Assert.AreEqual(create.CreatedDate, underTest.LastUpdatedDate, "Last Updated");
            Assert.AreEqual(create.CreatedDate, underTest.CreatedDate, "Created Date");
            Assert.AreEqual(create.RestaurantID, underTest.RestaurantID);
            Assert.AreEqual(create.CausedByID, underTest.CreatedByEmployeeID);
        }
		/*
        [Test]
        public void VendorItemsAddToRequisition()
        {
            var underTest = new PurchaseOrder();

            var addFirst = new VendorItemsAddedToPurchaseOrderEvent
            {
                VendorBeverageLineItem = new VendorBeverageLineItem
                {
                    ID = Guid.NewGuid(),
                    MiseName = "Powers"
                },
                Quantity = 2
            };
            var addSecond = new VendorItemsAddedToPurchaseOrderEvent
            {
                VendorBeverageLineItem = new VendorBeverageLineItem
                {
                    ID = Guid.NewGuid(),
                    MiseName = "Fernet"
                },
                Quantity = 3
            };

            //ACT
            underTest.When(addFirst);
            underTest.When(addSecond);

            var items = underTest.GetPurchaseOrderLineItems().ToList();

            //ASSERT
            Assert.NotNull(items);
            Assert.AreEqual(2, items.Count());

            Assert.AreEqual(2, items.First().Quantity);
            Assert.AreEqual("Powers", items.First().MiseName);

            Assert.AreEqual(3, items.Last().Quantity);
            Assert.AreEqual("Fernet", items.Last().MiseName);
        }

        [Test]
        public void VendorItemsAddIfAlreadyPresent()
        {
            var underTest = new PurchaseOrder();

            var vendorItemID = Guid.NewGuid();
            var addFirst = new VendorItemsAddedToPurchaseOrderEvent
            {
                VendorBeverageLineItem = new VendorBeverageLineItem
                {
                    ID = vendorItemID,
                    MiseName = "Powers750ml",
                    UPC = "1111"
                },
                Quantity = 2
            };
            var addSecond = new VendorItemsAddedToPurchaseOrderEvent
            {
                VendorBeverageLineItem = new VendorBeverageLineItem
                {
                    ID = vendorItemID,
                    MiseName = "Powers750ml",
                    UPC = "1111"
                },
                Quantity = 3
            };

            //ACT
            underTest.When(addFirst);
            underTest.When(addSecond);

            var items = underTest.GetPurchaseOrderLineItems().ToList();

            //ASSERT
            Assert.AreEqual(1, items.Count());
            Assert.AreEqual(5, items.First().Quantity);
        }*/
    }
}
