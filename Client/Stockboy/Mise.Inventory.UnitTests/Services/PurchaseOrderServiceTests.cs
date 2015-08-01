using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Mise.Core.Entities.Restaurant;
using Mise.Core.Repositories;
using Mise.Core.Services.UtilityServices;
using NUnit.Framework;
using Moq;
using Mise.Inventory.Services;
using Mise.Inventory.Services.Implementation;
using Mise.Core.Common.Events;
using Mise.Core.Common.Entities;
using Mise.Core.Entities.People;
using Mise.Core.Entities.Inventory;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.ValueItems.Inventory;
using Mise.Core.Client.Repositories;
using Mise.Core.Services;
using Mise.Core.Common.Services;
using Mise.Core.Services.WebServices;
using Mise.Core.Entities.Vendors;
using Mise.Core.Entities;

namespace Mise.Inventory.UnitTests.Services
{
    [TestFixture]
    public class PurchaseOrderServiceTests
    {
        [Test]
        public void GeneratePOWithoutInventoryShouldThrowException()
        {
            var restID = Guid.NewGuid();
            var rest = new Restaurant { ID = restID };

            var emp = new Employee
            {
                ID = Guid.NewGuid(),
                RestaurantsAndAppsAllowed = new Dictionary<Guid, IList<MiseAppTypes>>{
					{restID, new List<MiseAppTypes>()}
				}
            };
            var loginService = new Mock<ILoginService>();
            loginService.Setup(ls => ls.GetCurrentEmployee())
                .Returns(Task.FromResult(emp as IEmployee));
            loginService.Setup(ls => ls.GetCurrentRestaurant())
                .Returns(Task.FromResult(rest as IRestaurant));

            var container = new LiquidContainer
            {
                AmountContained = new LiquidAmount { Milliliters = 100 },
                DisplayName = "testCont"
            };
            var par = new Par
            {
                ID = Guid.NewGuid(),
                RestaurantID = rest.ID,
                ParLineItems = new List<PARBeverageLineItem>{
					new PARBeverageLineItem{
						ID = Guid.NewGuid(),
						RestaurantID = restID,
						CaseSize = 1,
						Container = container,
						MiseName = "testItem",
						DisplayName = "testPARItem",
						Quantity = 12
					},
					new PARBeverageLineItem{
						RestaurantID = restID,
						Container = container,
						MiseName="itemOnlyInPar",
						Quantity = 981
					}
				}
            };
            var parService = new Mock<IPARService>();
            parService.Setup(ps => ps.GetCurrentPAR())
                .Returns(Task.FromResult(par as IPAR));

            var inventoryService = new Mock<IInventoryService>();
            IInventory inv = null;
            inventoryService.Setup(ins => ins.GetLastCompletedInventory())
                .Returns(Task.FromResult(inv));

            var vendorService = new Mock<IVendorService>();
            IVendor vendor = null;
            vendorService.Setup(vs => vs.GetVendorWithLowestPriceForItem(It.IsAny<IBaseBeverageLineItem>(),
                It.IsAny<int>(), It.IsAny<Guid?>()))
                .Returns(Task.FromResult(vendor));

            //use a real repos for full test
            var logger = new Mock<ILogger>();
            var dal = TestUtilities.GetDALWithoutResends();
            var ws = new Mock<IPurchaseOrderWebService>();
            var roWS = new Mock<IReceivingOrderWebService>();
            var poRepos = new ClientPurchaseOrderRepository(logger.Object, dal.Object, ws.Object, TestUtilities.GetResendService().Object);
            var roRepos = new ClientReceivingOrderRepository(logger.Object, dal.Object, roWS.Object, TestUtilities.GetResendService().Object);
            var eventFactory = new InventoryAppEventFactory("test", MiseAppTypes.UnitTests);

            eventFactory.SetRestaurant(rest);
            var underTest = new PurchaseOrderService(loginService.Object, inventoryService.Object,
                                parService.Object, vendorService.Object, poRepos, roRepos, eventFactory);

            //ACT
            Assert.Throws<InvalidOperationException>(async () => await underTest.CreatePurchaseOrder());

        }

        [Test]
        public async Task GeneratePOWithInventoryShouldMakeANewPOBasedOnDifferences()
        {
            var restID = Guid.NewGuid();
            var rest = new Restaurant { ID = restID };

            var emp = new Employee
            {
                ID = Guid.NewGuid(),
                RestaurantsAndAppsAllowed = new Dictionary<Guid, IList<MiseAppTypes>>{
					{restID, new List<MiseAppTypes>()}
				}
            };
            var loginService = new Mock<ILoginService>();
            loginService.Setup(ls => ls.GetCurrentEmployee())
                .Returns(Task.FromResult(emp as IEmployee));
            loginService.Setup(ls => ls.GetCurrentRestaurant())
                .Returns(Task.FromResult(rest as IRestaurant));

            var container = new LiquidContainer
            {
                AmountContained = new LiquidAmount { Milliliters = 100 },
                DisplayName = "testCont"
            };
            var par = new Par
            {
                ID = Guid.NewGuid(),
                RestaurantID = rest.ID,
                ParLineItems = new List<PARBeverageLineItem>{
					new PARBeverageLineItem{
						ID = Guid.NewGuid(),
						RestaurantID = restID,
						CaseSize = 1,
						Container = container,
						MiseName = "testItem",
						DisplayName = "testPARItem",
						Quantity = 12
					},
					new PARBeverageLineItem{
						RestaurantID = restID,
						Container = container,
						MiseName="itemOnlyInPar",
						Quantity = 981
					},
					new PARBeverageLineItem{
						RestaurantID = restID,
						Container = container,
						MiseName = "fullyStocked",
						Quantity = 1
					}
				}
            };
            var parService = new Mock<IPARService>();
            parService.Setup(ps => ps.GetCurrentPAR())
                .Returns(Task.FromResult(par as IPAR));

            var inventoryService = new Mock<IInventoryService>();
            IInventory inv = new Mise.Core.Common.Entities.Inventory.Inventory
            {
                RestaurantID = restID,
                IsCurrent = false,
                DateCompleted = DateTime.UtcNow,
                Sections = new List<InventorySection>{
					new InventorySection{
						RestaurantID = restID,
						LastCompletedBy = null,
						LineItems = new List<InventoryBeverageLineItem>{
							//partially held item
							new InventoryBeverageLineItem{
								ID = Guid.NewGuid(),
								RestaurantID = restID,
								CaseSize = 1,
								Container = container,
								MiseName = "testItem",
								DisplayName = "testPARItem",
								NumFullBottles = 10
							},
							//fully held
							new InventoryBeverageLineItem{
								RestaurantID = restID,
								Container = container,
								MiseName = "fullyStocked",
								NumFullBottles = 1
							},
						}
					}
				}
            };
            inventoryService.Setup(ins => ins.GetLastCompletedInventory())
                .Returns(Task.FromResult(inv));

            var vendorService = new Mock<IVendorService>();
            IVendor vendor = null;
            vendorService.Setup(vs => vs.GetVendorWithLowestPriceForItem(It.IsAny<IBaseBeverageLineItem>(),
                It.IsAny<int>(), It.IsAny<Guid?>()))
                .Returns(Task.FromResult(vendor));

            //use a real repos for full test
            var logger = new Mock<ILogger>();
            var dal = TestUtilities.GetDALWithoutResends();
            var ws = new Mock<IPurchaseOrderWebService>();
            var roWS = new Mock<IReceivingOrderWebService>();
            var poRepos = new ClientPurchaseOrderRepository(logger.Object, dal.Object, ws.Object, TestUtilities.GetResendService().Object);
            var roRepos = new ClientReceivingOrderRepository(logger.Object, dal.Object, roWS.Object, TestUtilities.GetResendService().Object);
            var eventFactory = new InventoryAppEventFactory("test", MiseAppTypes.UnitTests);

            eventFactory.SetRestaurant(rest);
            var underTest = new PurchaseOrderService(loginService.Object, inventoryService.Object,
                parService.Object, vendorService.Object, poRepos, roRepos, eventFactory);

            //ACT
            var res = await underTest.CreatePurchaseOrder();

            //ASSERT
            Assert.NotNull(res);
            //check all our items came through
            Assert.AreEqual(restID, res.RestaurantID);
            Assert.AreEqual(emp.ID, res.CreatedByEmployeeID, "created by");

            var lines = res.GetPurchaseOrderLineItems().ToList();
            Assert.AreEqual(2, lines.Count());

            var testItem = lines.FirstOrDefault(li => li.MiseName == "testItem");
            Assert.NotNull(testItem);
            Assert.AreEqual(2, testItem.Quantity);

            var fullSTock = lines.FirstOrDefault(li => li.MiseName == "fullyStocked");
            Assert.Null(fullSTock);

            var parOnly = lines.FirstOrDefault(li => li.MiseName == "itemOnlyInPar");
            Assert.NotNull(parOnly);
            Assert.AreEqual(981, parOnly.Quantity);
        }

        [Test]
        public async Task InventoryInDifferentSectionsAddsUpToBeSubtractedFromPar()
        {
            var restID = Guid.NewGuid();
            var rest = new Restaurant { ID = restID };

            var emp = new Employee
            {
                ID = Guid.NewGuid(),
                RestaurantsAndAppsAllowed = new Dictionary<Guid, IList<MiseAppTypes>>{
					{restID, new List<MiseAppTypes>()}
				}
            };
            var loginService = new Mock<ILoginService>();
            loginService.Setup(ls => ls.GetCurrentEmployee())
                .Returns(Task.FromResult(emp as IEmployee));
            loginService.Setup(ls => ls.GetCurrentRestaurant())
                .Returns(Task.FromResult(rest as IRestaurant));

            var container = new LiquidContainer
            {
                AmountContained = new LiquidAmount { Milliliters = 100 },
                DisplayName = "testCont"
            };
            var par = new Par
            {
                ID = Guid.NewGuid(),
                RestaurantID = rest.ID,
                ParLineItems = new List<PARBeverageLineItem>{
					new PARBeverageLineItem{
						ID = Guid.NewGuid(),
						RestaurantID = restID,
						CaseSize = 1,
						Container = container,
						MiseName = "testItem",
						DisplayName = "testPARItem",
						Quantity = 12
					}
				}
            };
            var parService = new Mock<IPARService>();
            parService.Setup(ps => ps.GetCurrentPAR())
                .Returns(Task.FromResult(par as IPAR));

            var inventoryService = new Mock<IInventoryService>();
            IInventory inv = new Core.Common.Entities.Inventory.Inventory
            {
                RestaurantID = restID,
                IsCurrent = false,
                DateCompleted = DateTime.UtcNow,
                Sections = new List<InventorySection>{
					new InventorySection{
						RestaurantID = restID,
						LastCompletedBy = null,
						LineItems = new List<InventoryBeverageLineItem>{
							//partially held item
							new InventoryBeverageLineItem{
								ID = Guid.NewGuid(),
								RestaurantID = restID,
								CaseSize = 1,
								Container = container,
								MiseName = "testItem",
								DisplayName = "testPARItem",
								NumFullBottles = 10
							},
                        }
                    },
                    new InventorySection{
                        RestaurantID = restID,
                        LastCompletedBy = null,
                        LineItems = new List<InventoryBeverageLineItem>{
							        //fully held
							        new InventoryBeverageLineItem{
								        RestaurantID = restID,
								        Container = container,
								        MiseName = "testItem",
								        NumFullBottles = 1
							        },
						        }
					        }
				        }
            };
            inventoryService.Setup(ins => ins.GetLastCompletedInventory())
                .Returns(Task.FromResult(inv));

            var vendorService = new Mock<IVendorService>();
            IVendor vendor = null;
            vendorService.Setup(vs => vs.GetVendorWithLowestPriceForItem(It.IsAny<IBaseBeverageLineItem>(),
                It.IsAny<int>(), It.IsAny<Guid?>()))
                .Returns(Task.FromResult(vendor));

            //use a real repos for full test
            var logger = new Mock<ILogger>();
            var dal = new Mock<IClientDAL>();
            var ws = new Mock<IPurchaseOrderWebService>();
            var roWS = new Mock<IReceivingOrderWebService>();
            var poRepos = new ClientPurchaseOrderRepository(logger.Object, dal.Object, ws.Object, TestUtilities.GetResendService().Object);
            var roRepos = new ClientReceivingOrderRepository(logger.Object, dal.Object, roWS.Object, TestUtilities.GetResendService().Object);

            var eventFactory = new InventoryAppEventFactory("test", MiseAppTypes.UnitTests);

            eventFactory.SetRestaurant(rest);
            var underTest = new PurchaseOrderService(loginService.Object, inventoryService.Object,
                parService.Object, vendorService.Object, poRepos, roRepos, eventFactory);

            //ACT
            var res = await underTest.CreatePurchaseOrder();

            //ASSERT
            Assert.NotNull(res);
            //check all our items came through
            Assert.AreEqual(restID, res.RestaurantID);
            Assert.AreEqual(emp.ID, res.CreatedByEmployeeID, "created by");

            var lines = res.GetPurchaseOrderLineItems().ToList();
            Assert.AreEqual(1, lines.Count());

            var testItem = lines.FirstOrDefault(li => li.MiseName == "testItem");
            Assert.NotNull(testItem);
            Assert.AreEqual(1, testItem.Quantity);
        }

        [Test]
        public async Task ROsAreOnlyCountedAgainstPOIfReceviedAfterLastInventoryDate()
        {
            var restID = Guid.NewGuid();
            var rest = new Restaurant { ID = restID };

            var dateOfInventory = DateTime.UtcNow.AddDays(-2);
            var emp = new Employee
            {
                ID = Guid.NewGuid(),
                RestaurantsAndAppsAllowed = new Dictionary<Guid, IList<MiseAppTypes>>{
					{restID, new List<MiseAppTypes>()}
				}
            };
            var loginService = new Mock<ILoginService>();
            loginService.Setup(ls => ls.GetCurrentEmployee())
                .Returns(Task.FromResult(emp as IEmployee));
            loginService.Setup(ls => ls.GetCurrentRestaurant())
                .Returns(Task.FromResult(rest as IRestaurant));

            var container = new LiquidContainer
            {
                AmountContained = new LiquidAmount { Milliliters = 100 },
                DisplayName = "testCont"
            };
            var par = new Par
            {
                ID = Guid.NewGuid(),
                RestaurantID = rest.ID,
                ParLineItems = new List<PARBeverageLineItem>{
					new PARBeverageLineItem{
						ID = Guid.NewGuid(),
						RestaurantID = restID,
						CaseSize = 1,
						Container = container,
						MiseName = "testItem",
						DisplayName = "testPARItem",
						Quantity = 100
					}
				}
            };
            var parService = new Mock<IPARService>();
            parService.Setup(ps => ps.GetCurrentPAR())
                .Returns(Task.FromResult(par as IPAR));

            var inventoryService = new Mock<IInventoryService>();
            IInventory inv = new Core.Common.Entities.Inventory.Inventory
            {
                RestaurantID = restID,
                IsCurrent = false,
                DateCompleted = dateOfInventory,
                Sections = new List<InventorySection>{
					new InventorySection{
						RestaurantID = restID,
						LastCompletedBy = null,
						LineItems = new List<InventoryBeverageLineItem>{
							//partially held item
							new InventoryBeverageLineItem{
								ID = Guid.NewGuid(),
								RestaurantID = restID,
								CaseSize = 1,
								Container = container,
								MiseName = "testItem",
								DisplayName = "testPARItem",
								NumFullBottles = 10
							},
                        }
                    }
				}
            };
            inventoryService.Setup(ins => ins.GetLastCompletedInventory())
                .Returns(Task.FromResult(inv));

            var vendorService = new Mock<IVendorService>();
            IVendor vendor = null;
            vendorService.Setup(vs => vs.GetVendorWithLowestPriceForItem(It.IsAny<IBaseBeverageLineItem>(),
                It.IsAny<int>(), It.IsAny<Guid?>()))
                .Returns(Task.FromResult(vendor));

            //use a real repos for full test
            var logger = new Mock<ILogger>();
            var dal = TestUtilities.GetDALWithoutResends();
            var ws = new Mock<IPurchaseOrderWebService>();


            //create one RO in range, one not
            var roList = new List<IReceivingOrder>
		    {
		        //this one should count
		        new ReceivingOrder
		        {
		            DateReceived = dateOfInventory.AddDays(2),
		            LineItems = new List<ReceivingOrderLineItem>
		            {
		                new ReceivingOrderLineItem
		                {
		                    ID = Guid.NewGuid(),
		                    RestaurantID = restID,
		                    CaseSize = 1,
		                    Container = container,
		                    MiseName = "testItem",
		                    DisplayName = "testPARItem",
		                    Quantity = 30
		                }
		            }
		        },
		        new ReceivingOrder
		        {
		            DateReceived = dateOfInventory.AddDays(-1),
		            LineItems = new List<ReceivingOrderLineItem>
		            {
		                new ReceivingOrderLineItem
		                {
		                    ID = Guid.NewGuid(),
		                    RestaurantID = restID,
		                    CaseSize = 1,
		                    Container = container,
		                    MiseName = "testItem",
		                    DisplayName = "testPARItem",
		                    Quantity = 10
		                }
		            }
		        }
		    };

            var poRepos = new ClientPurchaseOrderRepository(logger.Object, dal.Object, ws.Object, TestUtilities.GetResendService().Object);
            var roRepos = new Mock<IReceivingOrderRepository>();
            roRepos.Setup(r => r.GetAll())
                .Returns(roList);

            var eventFactory = new InventoryAppEventFactory("test", MiseAppTypes.UnitTests);

            eventFactory.SetRestaurant(rest);
            var underTest = new PurchaseOrderService(loginService.Object, inventoryService.Object,
                parService.Object, vendorService.Object, poRepos, roRepos.Object, eventFactory);

            //ACT
            var res = await underTest.CreatePurchaseOrder();

            //ASSERT
            Assert.NotNull(res);
            //check all our items came through
            Assert.AreEqual(restID, res.RestaurantID);
            Assert.AreEqual(emp.ID, res.CreatedByEmployeeID, "created by");

            var lines = res.GetPurchaseOrderLineItems().ToList();
            Assert.AreEqual(1, lines.Count());

            var testItem = lines.FirstOrDefault(li => li.MiseName == "testItem");
            Assert.NotNull(testItem);
            //we should have 60, not 80 - we get the ROs AFTER the inventory
            Assert.AreEqual(60,testItem.Quantity);
        }
    }
}
