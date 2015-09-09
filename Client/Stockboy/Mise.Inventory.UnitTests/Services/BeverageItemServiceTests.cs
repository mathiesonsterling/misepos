using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Mise.Core.Services.UtilityServices;
using NUnit.Framework;
using Moq;

using Mise.Core.Services;
using Mise.Core.Repositories;
using Mise.Core.Entities.Inventory;
using Mise.Core.Entities.Vendors;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Common.Entities.Vendors;
using Mise.Core.ValueItems;
using Mise.Core.ValueItems.Inventory;
using Mise.Inventory.Services;
using Mise.Core.Common.Entities;
using Mise.Core.Entities.Restaurant;
using Mise.Inventory.Services.Implementation;


namespace Mise.Inventory.UnitTests.Services
{
	[TestFixture]
	public class BeverageItemServiceTests
	{
		[Test]
		public void GetPossibleItemsShouldGetDistinctItems()
		{
			var restaurantID = Guid.NewGuid ();

			var mockLogger = new Mock<ILogger> ();
			var mockDevLoc = new DummyDeviceLocationService ();

			//basic test - one item exists in all 3, one exists only in 1, one exists in 1 and 2, one exists in 3
			// one and 2 are associated with our restaurant
			var allVendorItem = new VendorBeverageLineItem{
				MiseName = "allVendorsCarryThis",
				Container = LiquidContainer.Bottle750ML,
				DisplayName = "allVendors"
			};
			var oneOnly = new VendorBeverageLineItem{
				MiseName = "vendor1Exclusive",
				UPC = "1111",
				Container = new LiquidContainer{AmountContained = new LiquidAmount{Milliliters = 245}},
				DisplayName = "vendor1Only"
			};
			var threeOnly = new VendorBeverageLineItem {
				MiseName = "vendor3CurrentInv",
				Container = LiquidContainer.Bottle1L,
				DisplayName = "vendor3Only"
			};
			var oneAndTwo = new VendorBeverageLineItem {
				MiseName = "par2Vendors",
				UPC = "2222fd",
				Container = new LiquidContainer{ AmountContained = new LiquidAmount{ Milliliters = 22 } },
				DisplayName = "par2Vendors"
			};

			//we'll clone our items to ensure they're different objects!
			var vendor1 = new Mock<IVendor> ();
			vendor1.Setup (v => v.GetItemsVendorSells ()).Returns (new List<IVendorBeverageLineItem>{ 
				allVendorItem.Clone() as VendorBeverageLineItem,
				oneOnly.Clone() as VendorBeverageLineItem, 
				oneAndTwo.Clone() as VendorBeverageLineItem});

			var vendor2 = new Mock<IVendor> ();
			vendor2.Setup (v => v.GetItemsVendorSells ()).Returns (new List<IVendorBeverageLineItem> {
				allVendorItem.Clone () as VendorBeverageLineItem,
				oneAndTwo.Clone () as VendorBeverageLineItem
			});


			var vendor3 = new Mock<IVendor> ();
			vendor3.Setup (v => v.GetItemsVendorSells ()).Returns (new List<IVendorBeverageLineItem> {
				allVendorItem.Clone() as VendorBeverageLineItem,
				threeOnly
			});

			var mockVendorRepos = new Mock<IVendorRepository> ();
			mockVendorRepos.Setup (r => r.GetAll())
				.Returns (new List<IVendor>{vendor1.Object, vendor2.Object }.AsEnumerable());
			mockVendorRepos.Setup(r => r.GetVendorsNotAssociatedWithRestaurantWithinRadius(It.IsAny<Guid>(), It.IsAny<Location>(), It.IsAny<int>()))
				.Returns(Task.FromResult(new List<IVendor>{vendor3.Object}.AsEnumerable()));

			//one item in inventory will match the item in the vendor 3
			var currentInv = new Mock<IInventory> ();
			currentInv.Setup (i => i.IsCurrent).Returns (true);
			currentInv.Setup (i => i.RestaurantID).Returns (restaurantID);
			currentInv.Setup(i => i.GetBeverageLineItems()).Returns(new List<IInventoryBeverageLineItem>{
				new InventoryBeverageLineItem{
					MiseName = "currentInv",
					DisplayName = "currentInv",
					Container = new LiquidContainer{AmountContained = new LiquidAmount{Milliliters = 1000}}
				}
			});

			var oldInv = new Mock<IInventory> ();
			oldInv.Setup (i => i.IsCurrent).Returns (false);
			oldInv.Setup (i => i.RestaurantID).Returns (restaurantID);
			oldInv.Setup (i => i.GetBeverageLineItems ()).Returns (new List<IInventoryBeverageLineItem> {
				new InventoryBeverageLineItem{
					MiseName = "oldInv",
					DisplayName = "oldInv",
					Container = new LiquidContainer{AmountContained = new LiquidAmount{Milliliters = 1000}},
					UPC = "3333"
				}
			});

			var mockInventoryRepos = new Mock<IInventoryRepository> ();
			mockInventoryRepos.Setup (r => r.GetCurrentInventory (restaurantID))
				.Returns (Task.Run(() => currentInv.Object));
			mockInventoryRepos.Setup (r => r.GetAll ())
				.Returns (new List<IInventory>{ currentInv.Object, oldInv.Object });

			IPar currentPar = new Par{ 
				IsCurrent = true, 
				RestaurantID = restaurantID,
				ParLineItems = new List<ParBeverageLineItem>{
					new ParBeverageLineItem{
						RestaurantID = restaurantID,
						MiseName = "par2Vendors",
						DisplayName = "par2Vendors",
						UPC = "2222fd",
						Container = new LiquidContainer{ AmountContained = new LiquidAmount{ Milliliters = 22 } },
					}
				}
			};
			var mockPARRepos = new Mock<IParRepository> ();
			mockPARRepos.Setup (r => r.GetCurrentPAR (restaurantID))
				.Returns (Task.FromResult (currentPar));

			var mockRORepos = new Mock<IReceivingOrderRepository> ();
			mockRORepos.Setup (r => r.GetAll ())
				.Returns (new List<IReceivingOrder> ());
			var loginService = new Mock<ILoginService> ();
			loginService.Setup (ls => ls.GetCurrentRestaurant ()).Returns (() => Task.FromResult (new Restaurant{ID = restaurantID} as IRestaurant));
			var underTest = new BeverageItemService (mockLogger.Object, mockDevLoc, mockVendorRepos.Object, 
				mockPARRepos.Object, mockInventoryRepos.Object, mockRORepos.Object, loginService.Object) 
			{
			};

			//ACT
			var res = underTest.GetPossibleItems ().Result.ToList();

			//ASSERT
			Assert.NotNull(res);

			//check we got distinct results
			Assert.AreEqual (5, res.Count);
			Assert.AreEqual ("allVendorsCarryThis", res[0].MiseName);
			Assert.AreEqual ("currentInv", res[1].MiseName);
			Assert.AreEqual ("oldInv", res[2].MiseName);
			Assert.AreEqual ("par2Vendors", res [3].MiseName);
			Assert.AreEqual ("vendor1Exclusive", res[4].MiseName);
		}

		[Test]
		public void GetPossibleItemsShouldGetItemsInOrder()
		{
			var restaurantID = Guid.NewGuid ();

			var mockLogger = new Mock<ILogger> ();
			var mockDevLoc = new DummyDeviceLocationService ();

			//basic test - one item exists in all 3, one exists only in 1, one exists in 1 and 2, one exists in 3
			// one and 2 are associated with our restaurant
			var oneOnly = new VendorBeverageLineItem{
				MiseName = "vendor1Exclusive",
				UPC = "1111",
				Container = new LiquidContainer{AmountContained = new LiquidAmount{Milliliters = 245}},
			};
			var threeOnly = new VendorBeverageLineItem {
				MiseName = "vendor3Exclusive",
				Container = new LiquidContainer{AmountContained = new LiquidAmount{Milliliters = 1000}},
			};

			//we'll clone our items to ensure they're different objects!
			var vendor1 = new Mock<IVendor> ();
			vendor1.Setup (v => v.GetItemsVendorSells ()).Returns (new List<IVendorBeverageLineItem>{
				oneOnly});
				


			var vendor3 = new Mock<IVendor> ();
			vendor3.Setup (v => v.GetItemsVendorSells ()).Returns (new List<IVendorBeverageLineItem> {
				threeOnly
			});

			var mockVendorRepos = new Mock<IVendorRepository> ();
			mockVendorRepos.Setup (r => r.GetAll())
				.Returns (new List<IVendor>{vendor1.Object}.AsEnumerable());
			mockVendorRepos.Setup(r => r.GetVendorsNotAssociatedWithRestaurantWithinRadius(It.IsAny<Guid>(), It.IsAny<Location>(), It.IsAny<int>()))
				.Returns(Task.FromResult(new List<IVendor>{vendor3.Object}.AsEnumerable()));

			//one item in inventory will match the item in the vendor 3
			var currentInv = new Mock<IInventory> ();
			currentInv.Setup (i => i.IsCurrent).Returns (true);
			currentInv.Setup (i => i.RestaurantID).Returns (restaurantID);
			currentInv.Setup(i => i.GetBeverageLineItems()).Returns(new List<IInventoryBeverageLineItem>{
				new InventoryBeverageLineItem{
					MiseName = "currentInv",
					Container = new LiquidContainer{AmountContained = new LiquidAmount{Milliliters = 1000}},
					RestaurantID = restaurantID
				}
			});

			var oldInv = new Mock<IInventory> ();
			oldInv.Setup (i => i.IsCurrent).Returns (false);
			oldInv.Setup (i => i.RestaurantID).Returns (restaurantID);
			oldInv.Setup (i => i.GetBeverageLineItems ()).Returns (new List<IInventoryBeverageLineItem> {
				new InventoryBeverageLineItem{
					MiseName = "oldInv",
					Container = new LiquidContainer{AmountContained = new LiquidAmount{Milliliters = 1000}},
					UPC = "3333", 
					RestaurantID = restaurantID
				}
			});

			var mockInventoryRepos = new Mock<IInventoryRepository> ();
			mockInventoryRepos.Setup (r => r.GetAll ())
				.Returns (new List<IInventory>{ currentInv.Object, oldInv.Object });

			IPar currentPar = new Par{ IsCurrent = true, ParLineItems = new List<ParBeverageLineItem> {
					new ParBeverageLineItem {
						MiseName = "currentPar",
						Container = new LiquidContainer{AmountContained = new LiquidAmount{Milliliters = 1000}}
					}
				}};
			var mockPARRepos = new Mock<IParRepository> ();
			mockPARRepos.Setup (r => r.GetAll())
				.Returns (new []{currentPar});

			var mockLoginService = new Mock<ILoginService> ();
			mockLoginService.Setup (mls => mls.GetCurrentRestaurant ())
				.Returns (Task.FromResult (new Restaurant{RestaurantID = restaurantID, ID = restaurantID} as IRestaurant));

			var mockRORepos = new Mock<IReceivingOrderRepository> ();
			mockRORepos.Setup (r => r.GetAll ())
				.Returns (new List<IReceivingOrder> ());
			
			var underTest = new BeverageItemService (mockLogger.Object, mockDevLoc, mockVendorRepos.Object, 
				mockPARRepos.Object, mockInventoryRepos.Object, mockRORepos.Object, mockLoginService.Object);

			//ACT
			var res = underTest.GetPossibleItems ().Result.ToList();

			//ASSERT
			Assert.NotNull(res);

			//check we got distinct results
			Assert.AreEqual (4, res.Count, "num items");

			//check the order
			Assert.AreEqual ("currentInv", res[0].MiseName);
			Assert.AreEqual ("currentPar", res[1].MiseName);
			Assert.AreEqual ("oldInv", res[2].MiseName);
			Assert.AreEqual ("vendor1Exclusive", res[3].MiseName);
		//	Assert.AreEqual ("vendor3Exclusive", res[4].MiseName);
		}

		[Test]
		public void FindItemsShouldGetMatchedItemsInOrder()
		{
			var restaurantID = Guid.NewGuid ();

			var mockLogger = new Mock<ILogger> ();
			var mockDevLoc = new DummyDeviceLocationService ();

			//basic test - one item exists in all 3, one exists only in 1, one exists in 1 and 2, one exists in 3
			// one and 2 are associated with our restaurant
			var oneOnly = new VendorBeverageLineItem{
				MiseName = "inSearch",
				UPC = "1111",
				Container = new LiquidContainer{AmountContained = new LiquidAmount{Milliliters = 245}},
				DisplayName = "yup"
			};
			var threeOnly = new VendorBeverageLineItem {
				MiseName = "outOfSearch",
				Container = new LiquidContainer{AmountContained = new LiquidAmount{Milliliters = 1000}},
				DisplayName = "faraway"
			};

			//we'll clone our items to ensure they're different objects!
			var vendor1 = new Mock<IVendor> ();
			vendor1.Setup (v => v.GetItemsVendorSells ()).Returns (new List<IVendorBeverageLineItem>{
				oneOnly});



			var vendor3 = new Mock<IVendor> ();
			vendor3.Setup (v => v.GetItemsVendorSells ()).Returns (new List<IVendorBeverageLineItem> {
				threeOnly
			});

			var mockVendorRepos = new Mock<IVendorRepository> ();
			mockVendorRepos.Setup (r => r.GetAll())
				.Returns (new List<IVendor>{vendor1.Object}.AsEnumerable());
			mockVendorRepos.Setup(r => r.GetVendorsNotAssociatedWithRestaurantWithinRadius(It.IsAny<Guid>(), It.IsAny<Location>(), It.IsAny<int>()))
				.Returns(Task.FromResult(new List<IVendor>{vendor3.Object}.AsEnumerable()));

			//one item in inventory will match the item in the vendor 3
			var currentInv = new Mock<IInventory> ();
			currentInv.Setup (i => i.IsCurrent).Returns (true);
			currentInv.Setup (i => i.RestaurantID).Returns (restaurantID);
			currentInv.Setup(i => i.GetBeverageLineItems()).Returns(new List<IInventoryBeverageLineItem>{
				new InventoryBeverageLineItem{
					MiseName = "miseNameOldInv",
					Container = new LiquidContainer{AmountContained = new LiquidAmount{Milliliters = 1000}},
					DisplayName = "inSearch",
					RestaurantID = restaurantID
				}
			});

			var oldInv = new Mock<IInventory> ();
			oldInv.Setup (i => i.IsCurrent).Returns (false);
			oldInv.Setup (i => i.RestaurantID).Returns (restaurantID);
			oldInv.Setup (i => i.GetBeverageLineItems ()).Returns (new List<IInventoryBeverageLineItem> {
				new InventoryBeverageLineItem{
					MiseName = "inSearch3",
					Container = new LiquidContainer{AmountContained = new LiquidAmount{Milliliters = 1000}},
					UPC = "3333", 
					RestaurantID = restaurantID
				}
			});

			var mockInventoryRepos = new Mock<IInventoryRepository> ();
			mockInventoryRepos.Setup (r => r.GetCurrentInventory (restaurantID))
				.Returns (Task.FromResult(currentInv.Object));
			mockInventoryRepos.Setup (r => r.GetAll ())
				.Returns (new List<IInventory>{ currentInv.Object, oldInv.Object });

			IPar currentPar = new Par{ IsCurrent = true, ParLineItems = new List<ParBeverageLineItem> {
					new ParBeverageLineItem {
						MiseName = "currentPar",
						Container = new LiquidContainer{AmountContained = new LiquidAmount{Milliliters = 1000}}
					}
				}};
			var mockPARRepos = new Mock<IParRepository> ();
			mockPARRepos.Setup (r => r.GetCurrentPAR (restaurantID))
				.Returns (Task.FromResult(currentPar));

			var mockLoginService = new Mock<ILoginService> ();
			mockLoginService.Setup (mls => mls.GetCurrentRestaurant ())
				.Returns (Task.FromResult (new Restaurant{RestaurantID = restaurantID, ID = restaurantID} as IRestaurant));

			var mockRORepos = new Mock<IReceivingOrderRepository> ();
			mockRORepos.Setup (r => r.GetAll ())
				.Returns (new List<IReceivingOrder> ());
			var underTest = new BeverageItemService (mockLogger.Object, mockDevLoc, mockVendorRepos.Object, 
				mockPARRepos.Object, mockInventoryRepos.Object, mockRORepos.Object, mockLoginService.Object) {
		};

			//ACT
			var res = underTest.FindItem ("inSearch").Result.ToList();

			//ASSERT
			Assert.NotNull(res);

			//check we got distinct results
			Assert.AreEqual (3, res.Count, "num items");

			//check the order
			Assert.AreEqual ("inSearch", res[0].DisplayName);
			Assert.AreEqual ("miseNameOldInv", res[0].MiseName);

			Assert.AreEqual ("inSearch3", res[1].MiseName);
			Assert.AreEqual ("inSearch3", res[1].DisplayName);

			Assert.AreEqual ("yup", res[2].DisplayName);
		}

		/// <summary>
		/// Test calling the repositories, but they have no items
		/// </summary>
		[Test]
		public void GetPossibleItemsWithNoItemsForRestaurant()
		{
			var mockLogger = new Mock<ILogger> ();
			var mockDevLoc = new DummyDeviceLocationService ();

			var mockVendorRepos = new Mock<IVendorRepository> ();
			mockVendorRepos.Setup (r => r.GetVendorsAssociatedWithRestaurant (It.IsAny<Guid> ()))
				.Returns (Task.FromResult(new List<IVendor> ().AsEnumerable()));
			mockVendorRepos.Setup(r => r.GetVendorsNotAssociatedWithRestaurantWithinRadius(It.IsAny<Guid>(), It.IsAny<Location>(), It.IsAny<int>()))
				.Returns(Task.FromResult(new List<IVendor>().AsEnumerable()));

			IInventory currentInv = new Mise.Core.Common.Entities.Inventory.Inventory {IsCurrent = true};
			var mockInventoryRepos = new Mock<IInventoryRepository> ();
			mockInventoryRepos.Setup (r => r.GetCurrentInventory (It.IsAny<Guid>()))
				.Returns (Task.FromResult(currentInv));

			IPar currentPar = new Par{ IsCurrent = true };
			var mockPARRepos = new Mock<IParRepository> ();
			mockPARRepos.Setup (r => r.GetCurrentPAR (It.IsAny<Guid>()))
				.Returns (Task.FromResult(currentPar));

			var restaurantID = Guid.NewGuid ();
			var mockLoginService = new Mock<ILoginService> ();
			mockLoginService.Setup (mls => mls.GetCurrentRestaurant ())
				.Returns (Task.FromResult (new Restaurant{RestaurantID = restaurantID, ID = restaurantID} as IRestaurant));

			var mockRORepos = new Mock<IReceivingOrderRepository> ();
			mockRORepos.Setup (r => r.GetAll ())
				.Returns (new List<IReceivingOrder> ());
			
			var underTest = new BeverageItemService (mockLogger.Object, mockDevLoc, mockVendorRepos.Object, 
			mockPARRepos.Object, mockInventoryRepos.Object, mockRORepos.Object, mockLoginService.Object);

			//ACT
			var res = underTest.GetPossibleItems ().Result;

			//ASSERT
			Assert.NotNull(res);
			Assert.IsFalse (res.Any ());
		}

		[Test]
		public async void GetPossibleItemsWithoutRestaurantIDShouldNotCallRepositories(){
			var mockLogger = new Mock<ILogger> ();
			var mockDevLoc = new DummyDeviceLocationService ();

			var mockVendorRepos = new Mock<IVendorRepository> ();
		    var mockInventoryRepos = new Mock<IInventoryRepository> ();
				

			var mockPARRepos = new Mock<IParRepository> ();

			var restaurantID = Guid.Empty;
			var mockLoginService = new Mock<ILoginService> ();
			mockLoginService.Setup (mls => mls.GetCurrentRestaurant ())
				.Returns (Task.FromResult (new Restaurant{RestaurantID = restaurantID, ID = restaurantID} as IRestaurant));

			var mockRORepos = new Mock<IReceivingOrderRepository> ();
			mockRORepos.Setup (r => r.GetAll ())
				.Returns (new List<IReceivingOrder> ());
			
			var underTest = new BeverageItemService (mockLogger.Object, mockDevLoc, mockVendorRepos.Object, 
				mockPARRepos.Object, mockInventoryRepos.Object, mockRORepos.Object, mockLoginService.Object);

			//ACT
			var res = await underTest.GetPossibleItems ();

			//ASSERT
			Assert.NotNull(res);
			Assert.IsFalse (res.Any ());

			mockVendorRepos.Verify (r => r.GetAll(), Times.Once());
			//mockVendorRepos.Verify(r => r.GetVendorsNotAssociatedWithRestaurantWithinRadius(It.IsAny<Guid>(), It.IsAny<Location>()), Times.Once());
			mockInventoryRepos.Verify (r => r.GetAll(), Times.Once());
			mockPARRepos.Verify(r => r.GetAll(), Times.Once());
		}
	}
}

