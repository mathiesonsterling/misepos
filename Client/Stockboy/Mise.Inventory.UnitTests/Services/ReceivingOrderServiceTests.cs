using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Client.Repositories;
using Mise.Core.Common;
using Mise.Core.Common.Entities;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Common.Entities.Vendors;
using Mise.Core.Common.Events;
using Mise.Core.Common.Events.Inventory;
using Mise.Core.Common.Services;
using Mise.Core.Common.Services.WebServices;
using Mise.Core.Entities;
using Mise.Core.Entities.Inventory;
using Mise.Core.Entities.People;
using Mise.Core.Entities.Vendors;
using Mise.Core.Entities.Vendors.Events;
using Mise.Core.Repositories;
using Mise.Core.Services;
using Mise.Core.Services.UtilityServices;
using Mise.Core.ValueItems.Inventory;
using Mise.Inventory.Services;
using Mise.Inventory.Services.Implementation;
using Moq;
using NUnit.Framework;
namespace Mise.Inventory.UnitTests.Services
{
    [TestFixture]
    class ReceivingOrderServiceTests
    {
        [Test]
        public async Task CreateShouldCreateIfNonePresent()
        {
            var vendorID = Guid.NewGuid();
            var roID = Guid.NewGuid();
            var otherID = Guid.NewGuid();
            var restID = Guid.NewGuid();

            var mockRORepos = new Mock<IReceivingOrderRepository>();
            mockRORepos.Setup(r => r.GetAll())
                .Returns(new List<IReceivingOrder>());

            var newRO = new ReceivingOrder
            {
                ID = Guid.NewGuid(),
                Status = ReceivingOrderStatus.Created,
                RestaurantID = restID,
                VendorID = vendorID
            };
            mockRORepos.Setup(r => r.ApplyEvents(It.IsAny<IEnumerable<IReceivingOrderEvent>>()))
                .Returns(newRO);
            mockRORepos.Setup(r => r.ApplyEvent(It.IsAny<IReceivingOrderEvent>()))
                .Returns(newRO);

            var logger = new Mock<ILogger>();
            var invService = new Mock<IInventoryService>();

            var evFac = new InventoryAppEventFactory("test", MiseAppTypes.UnitTests);
            evFac.SetRestaurant(new Restaurant
            {
                ID = restID
            });

            var loginServ = new Mock<ILoginService>();
            loginServ.Setup(ls => ls.GetCurrentEmployee())
                .Returns(Task.FromResult(new Employee() as IEmployee));

            var poServ = new Mock<IPurchaseOrderService>();
            poServ.Setup(ps => ps.GetPurchaseOrdersWaitingForVendor(It.IsAny<Vendor>()))
                .Returns(Task.FromResult(new List<IPurchaseOrder>().AsEnumerable()));

            var vendServ = new Mock<IVendorService>();
            vendServ.Setup(vs => vs.GetSelectedVendor())
                .Returns(Task.FromResult(new Vendor { ID = vendorID } as IVendor));

			var insights = new Mock<IInsightsService> ();
            var underTest = new ReceivingOrderService(logger.Object, mockRORepos.Object, invService.Object, evFac,
				loginServ.Object, vendServ.Object, poServ.Object, insights.Object);

            //ACT
			var res = await underTest.StartReceivingOrderForSelectedVendor();

            //ASSERT
            Assert.NotNull(res);
            Assert.AreNotEqual(roID, res.ID);
            Assert.AreNotEqual(otherID, res.ID);
            Assert.AreEqual(ReceivingOrderStatus.Created, res.Status);
            Assert.AreEqual(restID, res.RestaurantID);
            Assert.AreEqual(newRO, res);
        }

      /*  [Test]
        public async void CreateOrGetShouldCreateIfOnlyCompletedOrCancelledPresent()
        {
            var vendorID = Guid.NewGuid();
            var roID = Guid.NewGuid();
            var otherID = Guid.NewGuid();
            var restID = Guid.NewGuid();

            var mockRORepos = new Mock<IReceivingOrderRepository>();
            mockRORepos.Setup(r => r.GetAll())
                .Returns(new List<IReceivingOrder>
                {
                    new ReceivingOrder
                    {
                        ID = roID,
                        Status = ReceivingOrderStatus.Completed,
                        VendorID = vendorID
                    },
                    new ReceivingOrder
                    {
                        ID = otherID,
                        Status = ReceivingOrderStatus.Cancelled,
                        VendorID = vendorID
                    }
                });

            var newRO = new ReceivingOrder
            {
                ID = Guid.NewGuid(),
                Status = ReceivingOrderStatus.Created,
                RestaurantID = restID,
                VendorID = vendorID
            };
            mockRORepos.Setup(r => r.ApplyEvents(It.IsAny<IEnumerable<IReceivingOrderEvent>>()))
                .Returns(newRO);
            mockRORepos.Setup(r => r.ApplyEvent(It.IsAny<IReceivingOrderEvent>()))
                .Returns(newRO);

            var logger = new Mock<ILogger>();
            var invService = new Mock<IInventoryService>();

            var evFac = new InventoryAppEventFactory("test", MiseAppTypes.UnitTests);
            evFac.SetRestaurant(new Restaurant
            {
                ID = restID
            });

            var loginServ = new Mock<ILoginService>();
            loginServ.Setup(ls => ls.GetCurrentEmployee())
                .Returns(Task.FromResult(new Employee() as IEmployee));

            var poServ = new Mock<IPurchaseOrderService>();
            poServ.Setup(ps => ps.GetPurchaseOrdersWaitingForVendor(It.IsAny<Vendor>()))
                .Returns(Task.FromResult(new List<IPurchaseOrder>().AsEnumerable()));

            var vendServ = new Mock<IVendorService>();
            vendServ.Setup(vs => vs.GetSelectedVendor())
                .Returns(Task.FromResult(new Vendor { ID = vendorID } as IVendor));
            var underTest = new ReceivingOrderService(logger.Object, mockRORepos.Object, invService.Object, evFac,
                loginServ.Object, vendServ.Object, poServ.Object);

            //ACT
            var res = await underTest.StartOrContinueReceivingOrderForSelectedVendor();
			Assert.Fail();
            //ASSERT
            Assert.NotNull(res);
            Assert.AreNotEqual(roID, res.ID);
            Assert.AreNotEqual(otherID, res.ID);
            Assert.AreEqual(ReceivingOrderStatus.Created, res.Status);
            Assert.AreEqual(restID, res.RestaurantID);
            Assert.AreEqual(newRO, res);
        }*/

        [Test]
        public async Task CreateWithMismatchedPOVendorShouldThrow()
        {
            var vendorID = Guid.NewGuid();
            var otherVendorID = Guid.NewGuid();
            var roID = Guid.NewGuid();
            var restID = Guid.NewGuid();

            var mockRORepos = new Mock<IReceivingOrderRepository>();

            var logger = new Mock<ILogger>();
            var invService = new Mock<IInventoryService>();

            var evFac = new Mock<IInventoryAppEventFactory>();

            var loginServ = new Mock<ILoginService>();
            loginServ.Setup(ls => ls.GetCurrentEmployee())
                .Returns(Task.FromResult(new Employee() as IEmployee));

            var poServ = new Mock<IPurchaseOrderService>();

            var vendServ = new Mock<IVendorService>();
            vendServ.Setup(vs => vs.GetSelectedVendor())
                .Returns(Task.FromResult(new Vendor { ID = vendorID } as IVendor));
			var insights = new Mock<IInsightsService> ();
            var underTest = new ReceivingOrderService(logger.Object, mockRORepos.Object, invService.Object, 
				evFac.Object, loginServ.Object, vendServ.Object, poServ.Object, insights.Object);

            var po = CreatePurchaseOrder(otherVendorID);
            var threw = false;
            IReceivingOrder res = null;
            //ACT
            try
            {
                res = await underTest.StartReceivingOrder(po);
            }
            catch (InvalidOperationException)
            {
                threw = true;
            }

            //ASSERT
            Assert.True(threw);
            Assert.Null(res);
        }

        [Test]
        public async Task CreateWithPurchaseOrderShouldAddLineItems()
        {
            var vendorID = Guid.NewGuid();
            var roID = Guid.NewGuid();
            var otherID = Guid.NewGuid();
            var restID = Guid.NewGuid();

            var logger = new Mock<ILogger>();

            //we'll want a real Repository for this test
            var ws = new Mock<IReceivingOrderWebService>();
            var roRepos = new ClientReceivingOrderRepository(logger.Object, ws.Object);

            var invService = new Mock<IInventoryService>();

            var evFac = new InventoryAppEventFactory("test", MiseAppTypes.UnitTests);
            evFac.SetRestaurant(new Restaurant
            {
                ID = restID
            });

            var poServ = new Mock<IPurchaseOrderService>();

            var loginServ = new Mock<ILoginService>();
            loginServ.Setup(ls => ls.GetCurrentEmployee())
                .Returns(Task.FromResult(new Employee() as IEmployee));



            var vendServ = new Mock<IVendorService>();
            vendServ.Setup(vs => vs.GetSelectedVendor())
                .Returns(Task.FromResult(new Vendor { ID = vendorID } as IVendor));
			var insights = new Mock<IInsightsService> ();
            var underTest = new ReceivingOrderService(logger.Object, roRepos, invService.Object, evFac,
				loginServ.Object, vendServ.Object, poServ.Object, insights.Object);

            var po = CreatePurchaseOrder(vendorID);

            //ACT
            var res = await underTest.StartReceivingOrder(po);

            //ASSERT
            Assert.NotNull(res);
            Assert.AreNotEqual(roID, res.ID);
            Assert.AreNotEqual(otherID, res.ID);
            Assert.AreEqual(ReceivingOrderStatus.Created, res.Status);
            Assert.AreEqual(restID, res.RestaurantID);

            var items = res.GetBeverageLineItems().ToList();
            Assert.NotNull(items);
            Assert.AreEqual(1, items.Count());
            Assert.AreEqual("testItem", items.First().DisplayName);
        }



        [Test]
        public async Task CompleteShouldSetStatusToCompleteAndNullCurrentOrderWhenNotLinkedWithPO()
        {
            var vendorID = Guid.NewGuid();
            var roID = Guid.NewGuid();
            var restID = Guid.NewGuid();

            var logger = new Mock<ILogger>();

            //we'll want a real Repository for this test, since it uses events
            var ws = new Mock<IReceivingOrderWebService>();
            ws.Setup(s => s.GetReceivingOrdersForRestaurant(It.IsAny<Guid>()))
                .Returns(
                    Task.FromResult(
                        new List<ReceivingOrder>
                        {
                            new ReceivingOrder
                            {
                                ID = roID,
                                Status = ReceivingOrderStatus.Created,
                                VendorID = vendorID,
                                RestaurantID = restID
                            }
                        }.AsEnumerable()
                    )
                );
            var roRepos = new ClientReceivingOrderRepository(logger.Object,  ws.Object);
            await roRepos.Load(Guid.NewGuid());


            var invService = new Mock<IInventoryService>();

            var evFac = new InventoryAppEventFactory("test", MiseAppTypes.UnitTests);
            evFac.SetRestaurant(new Restaurant{ID = restID});

            var loginServ = new Mock<ILoginService>();
            loginServ.Setup(ls => ls.GetCurrentEmployee())
                .Returns(Task.FromResult(new Employee() as IEmployee));

            var poServ = new Mock<IPurchaseOrderService>();
            var vendServ = new Mock<IVendorService>();
            vendServ.Setup(vs => vs.GetSelectedVendor())
                .Returns(Task.FromResult(new Vendor { ID = vendorID } as IVendor));
            vendServ.Setup(
                vs => vs.AddLineItemsToVendorIfDontExist(vendorID, It.IsAny<IEnumerable<IReceivingOrderLineItem>>()))
                .Returns(Task.FromResult(true));

			var insights = new Mock<IInsightsService> ();
            var underTest = new ReceivingOrderService(logger.Object, roRepos, invService.Object, evFac,
				loginServ.Object, vendServ.Object, poServ.Object, insights.Object);

            //ACT
			var poRes = await underTest.StartReceivingOrderForSelectedVendor ();
			var completeRes = await underTest.CompleteReceivingOrderForSelectedVendor(DateTimeOffset.UtcNow, string.Empty, string.Empty);

            var current = await underTest.GetCurrentReceivingOrder();

            //ASSERT
            Assert.NotNull(poRes);
            Assert.True(completeRes);
            Assert.Null(current);
            poServ.Verify(ps => ps.IsPurchaseOrderTotallyFufilledByReceivingOrder(It.IsAny<IReceivingOrder>()), Times.Never());
        }

        /// <summary>
        /// Shows what happens when a PO is associated with an RO, but doesn't have all the items
        /// </summary>
        [TestCase(true)]
        [TestCase(false)]
        [Test]
        public async Task CompleteShouldReturnFalseWhenPONotTotallyFilled(bool isTotallyFilled)
        {
            var vendorID = Guid.NewGuid();
            var restID = Guid.NewGuid();

            var logger = new Mock<ILogger>();

            //we'll want a real Repository for this test, since it uses events
            var ws = new Mock<IReceivingOrderWebService>();
            ws.Setup(s => s.GetReceivingOrdersForRestaurant(It.IsAny<Guid>()))
                .Returns(
                    Task.FromResult(
                        new List<ReceivingOrder>().AsEnumerable()
                    )
                );
            var roRepos = new ClientReceivingOrderRepository(logger.Object,  ws.Object);
            await roRepos.Load(Guid.NewGuid());


            var invService = new Mock<IInventoryService>();

            var evFac = new InventoryAppEventFactory("test", MiseAppTypes.UnitTests);
            evFac.SetRestaurant(new Restaurant { ID = restID });

            var loginServ = new Mock<ILoginService>();
            loginServ.Setup(ls => ls.GetCurrentEmployee())
                .Returns(Task.FromResult(new Employee() as IEmployee));

            var po = CreatePurchaseOrder(vendorID);
            var poServ = new Mock<IPurchaseOrderService>();
            poServ.Setup(ps => ps.IsPurchaseOrderTotallyFufilledByReceivingOrder(It.IsAny<IReceivingOrder>()))
                .Returns(Task.FromResult(isTotallyFilled));

            var vendServ = new Mock<IVendorService>();
            vendServ.Setup(vs => vs.GetSelectedVendor())
                .Returns(Task.FromResult(new Vendor { ID = vendorID } as IVendor));
            vendServ.Setup(
				vs => vs.AddLineItemsToVendorIfDontExist(vendorID, It.IsAny<IEnumerable<IReceivingOrderLineItem>>()))
                .Returns(Task.FromResult(true));
			var insights = new Mock<IInsightsService> ();
            var underTest = new ReceivingOrderService(logger.Object, roRepos, invService.Object, evFac,
				loginServ.Object, vendServ.Object, poServ.Object, insights.Object);

            //ACT
            var poRes = await underTest.StartReceivingOrder(po);
			var completeRes = await underTest.CompleteReceivingOrderForSelectedVendor(DateTimeOffset.UtcNow, string.Empty, string.Empty);

            var current = await underTest.GetCurrentReceivingOrder();

            //ASSERT
            Assert.NotNull(poRes);
            Assert.AreEqual(isTotallyFilled, completeRes);
            Assert.Null(current);
            poServ.Verify(ps => ps.IsPurchaseOrderTotallyFufilledByReceivingOrder(It.IsAny<IReceivingOrder>()), Times.Once());
		}

        [TestCase(PurchaseOrderStatus.ReceivedTotally)]
        [TestCase(PurchaseOrderStatus.ReceivedWithAlterations)]
        [TestCase(PurchaseOrderStatus.RejectedByRestaurant)]
        [Test]
        public async void CommitShouldMarkItemsAsReceivedIfAssociatedWithAPurchaseOrder(PurchaseOrderStatus status)
        {
            var vendorID = Guid.NewGuid();
            var restID = Guid.NewGuid();

            var logger = new Mock<ILogger>();

            //we'll want a real Repository for this test, since it uses events
            var ws = new Mock<IReceivingOrderWebService>();
            ws.Setup(s => s.GetReceivingOrdersForRestaurant(It.IsAny<Guid>()))
                .Returns(
                    Task.FromResult(
                        new List<ReceivingOrder>().AsEnumerable()
                    )
                );
            var roRepos = new ClientReceivingOrderRepository(logger.Object, ws.Object);
            await roRepos.Load(Guid.NewGuid());


            var invService = new Mock<IInventoryService>();

            var evFac = new InventoryAppEventFactory("test", MiseAppTypes.UnitTests);
            evFac.SetRestaurant(new Restaurant { ID = restID });

            var loginServ = new Mock<ILoginService>();
            loginServ.Setup(ls => ls.GetCurrentEmployee())
                .Returns(Task.FromResult(new Employee() as IEmployee));

            var po = CreatePurchaseOrder(vendorID);
            var poServ = new Mock<IPurchaseOrderService>();
            poServ.Setup(ps => ps.IsPurchaseOrderTotallyFufilledByReceivingOrder(It.IsAny<IReceivingOrder>()))
                .Returns(Task.FromResult(true));
            poServ.Setup(ps => ps.MarkItemsAsReceivedForPurchaseOrder(It.IsAny<IReceivingOrder>(), status))
                .Returns(Task.FromResult(true));

            var vendServ = new Mock<IVendorService>();
            vendServ.Setup(vs => vs.GetSelectedVendor())
                .Returns(Task.FromResult(new Vendor { ID = vendorID } as IVendor));
            vendServ.Setup(
				vs => vs.AddLineItemsToVendorIfDontExist(vendorID, It.IsAny<IEnumerable<IReceivingOrderLineItem>>()))
                .Returns(Task.FromResult(true));
			var insights = new Mock<IInsightsService> ();
            var underTest = new ReceivingOrderService(logger.Object, roRepos, invService.Object, evFac,
				loginServ.Object, vendServ.Object, poServ.Object, insights.Object);

            //ACT
            var poRes = await underTest.StartReceivingOrder(po);
			var completeRes = await underTest.CompleteReceivingOrderForSelectedVendor(DateTimeOffset.UtcNow, string.Empty, string.Empty);

            var current = await underTest.GetCurrentReceivingOrder();
            await underTest.CommitCompletedOrder(status);
			
            //ASSERT
            Assert.NotNull(poRes);
            Assert.AreEqual(true, completeRes);
            Assert.Null(current);
            poServ.Verify(ps => ps.IsPurchaseOrderTotallyFufilledByReceivingOrder(It.IsAny<IReceivingOrder>()), Times.Once());
            poServ.Verify(ps => ps.MarkItemsAsReceivedForPurchaseOrder(It.IsAny<IReceivingOrder>(), status), Times.Once);
		}

		[Test]
		public async void CommitWithNotesShouldAssignThemToEntity(){
            var vendorID = Guid.NewGuid();
            var restID = Guid.NewGuid();
		    var roID = Guid.NewGuid();
            var logger = new Mock<ILogger>();

		    var ro = new ReceivingOrder
		    {
		        ID = roID,
                VendorID = vendorID,
                Status = ReceivingOrderStatus.Created,
		        Notes = null
		    };

		    var completedRO = ro.Clone() as ReceivingOrder;
            Assert.NotNull(completedRO);
		    completedRO.Status = ReceivingOrderStatus.Completed;

		    ReceivingOrderCompletedEvent givenEvent = null;
		    var roRepos = new Mock<IReceivingOrderRepository>();
		    roRepos.Setup(rr => rr.GetAll()).Returns(new List<IReceivingOrder> {ro});
		    roRepos.Setup(rr => rr.ApplyEvent(It.IsAny<IReceivingOrderEvent>()))
		        .Returns(completedRO)
		        .Callback<IReceivingOrderEvent>(ev => givenEvent = ev as ReceivingOrderCompletedEvent);

            var invService = new Mock<IInventoryService>();

            var evFac = new InventoryAppEventFactory("test", MiseAppTypes.UnitTests);
            evFac.SetRestaurant(new Restaurant { ID = restID });

            var loginServ = new Mock<ILoginService>();
            loginServ.Setup(ls => ls.GetCurrentEmployee())
                .Returns(Task.FromResult(new Employee() as IEmployee));

            var poServ = new Mock<IPurchaseOrderService>();
            poServ.Setup(ps => ps.IsPurchaseOrderTotallyFufilledByReceivingOrder(It.IsAny<IReceivingOrder>()))
                .Returns(Task.FromResult(true));

            //need to give the PO so it's associated
            poServ.Setup(ps => ps.GetPurchaseOrdersWaitingForVendor(It.IsAny<IVendor>()))
                .Returns(Task.FromResult(new List<IPurchaseOrder>().AsEnumerable()));
            poServ.Setup(ps => ps.MarkItemsAsReceivedForPurchaseOrder(It.IsAny<IReceivingOrder>(), PurchaseOrderStatus.ReceivedTotally))
                .Returns(Task.FromResult(true));

            var vendServ = new Mock<IVendorService>();
            vendServ.Setup(vs => vs.GetSelectedVendor())
                .Returns(Task.FromResult(new Vendor { ID = vendorID } as IVendor));
            vendServ.Setup(
				vs => vs.AddLineItemsToVendorIfDontExist(vendorID, It.IsAny<IEnumerable<IReceivingOrderLineItem>>()))
                .Returns(Task.FromResult(true));
			var insights = new Mock<IInsightsService> ();
            var underTest = new ReceivingOrderService(logger.Object, roRepos.Object, invService.Object, evFac,
				loginServ.Object, vendServ.Object, poServ.Object, insights.Object);

            //ACT
			var poRes = await underTest.StartReceivingOrderForSelectedVendor();
			var completeRes = await underTest.CompleteReceivingOrderForSelectedVendor(DateTimeOffset.UtcNow, "this is a test note", string.Empty);

            var current = await underTest.GetCurrentReceivingOrder();

            //ASSERT
            Assert.NotNull(poRes);
            Assert.True(completeRes);
            Assert.NotNull(givenEvent);
            Assert.AreEqual("this is a test note", givenEvent.Notes);
            Assert.Null(current);
		}

        private static PurchaseOrder CreatePurchaseOrder(Guid vendorID)
        {
            //create a PO for the same vendor
            var po = new PurchaseOrder
            {
                ID = Guid.NewGuid(),
                PurchaseOrdersPerVendor = new List<PurchaseOrderPerVendor>
                {
                    new PurchaseOrderPerVendor
                    {
                        VendorID = vendorID,
                        LineItems = new List<PurchaseOrderLineItem>
                        {
                            new PurchaseOrderLineItem
                            {
                                ID = Guid.NewGuid(),
                                DisplayName = "testItem",
                                Container =
                                    new LiquidContainer
                                    {
                                        AmountContained = new LiquidAmount {Milliliters = 10},
                                        DisplayName = "testContainer"
                                    }
                            }
                        }
                    }
                }
            };
            return po;
        }
    }


}
