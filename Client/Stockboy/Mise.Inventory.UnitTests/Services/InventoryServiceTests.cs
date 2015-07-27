using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Mise.Core.Entities.Restaurant;
using Mise.Inventory.Services;
using NUnit.Framework;
using Moq;

using Mise.Core.Entities.Inventory;
using Mise.Core.Entities.Base;
using Mise.Inventory.Services.Implementation;
using Mise.Core.Client.Repositories;
using Mise.Core.Repositories;
using Mise.Core.Services;
using Mise.Core.Services.WebServices;
using Mise.Core.Common.Entities;
using Mise.Core.Common.Events;
using Mise.Core.Common.Services;
using Mise.Core.Entities.People;
using Mise.Core.ValueItems;
using Mise.Core.Entities;
using Mise.Core.Entities.Restaurant.Events;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Entities.Inventory.Events;
using Mise.Core.ValueItems.Inventory;
using Mise.Core.Common.Entities.Vendors;
using Mise.Core.Common;


namespace Mise.Inventory.UnitTests.Services
{
	[TestFixture]
	public class InventoryServiceTests
	{
		[Test]
		public async Task StartInventoryWithPreviousInventoryWithoutAnyItemsShouldStillCreate(){
			var sectionID = Guid.NewGuid ();
			var restaurant = new Restaurant {
				ID = Guid.NewGuid (),
				InventorySections = new List<RestaurantInventorySection>{
					new RestaurantInventorySection {
						Name = "testSec",
						ID = sectionID
					}
				}
			};

			var emp = new Employee {
				ID = Guid.NewGuid ()
			};

			var loginService = new Mock<ILoginService> ();
			loginService.Setup (ls => ls.GetCurrentRestaurant ())
				.Returns (Task.FromResult (restaurant as IRestaurant));
			loginService.Setup (ls => ls.GetCurrentEmployee ())
				.Returns (Task.FromResult (emp as IEmployee));

			var logger = new Mock<ILogger> ();

			var currentInventory = new Core.Common.Entities.Inventory.Inventory {
				RestaurantID = restaurant.ID,
				IsCurrent = true,
				CreatedDate = DateTime.UtcNow,
				Sections = new List<InventorySection>{
					new InventorySection {
						Name = "testSec",
						RestaurantInventorySectionID = sectionID,
						ID = Guid.NewGuid (),
						LineItems = new List<InventoryBeverageLineItem>()
					}
				}
			};
			var ws = new Mock<IInventoryWebService> ();
			ws.Setup(w => w.GetInventoriesForRestaurant(It.IsAny<Guid>()))
				.Returns(Task.FromResult(new List<IInventory>{currentInventory}.AsEnumerable()));
			ws.Setup (w => w.SendEventsAsync (It.IsAny<IInventory> (), It.IsAny<IEnumerable<IInventoryEvent>> ()))
				.Returns (Task.FromResult (true));

			var dal = new Mock<IClientDAL> ();
			dal.Setup (d => d.UpsertEntitiesAsync (It.IsAny<IEnumerable<IEntityBase>> ()))
				.Returns (Task.FromResult (true));
			var invRepository = new ClientInventoryRepository (logger.Object, dal.Object, ws.Object);

			var eventFact = new InventoryAppEventFactory ("test", MiseAppTypes.UnitTests);
			eventFact.SetRestaurant (restaurant);

			var insights = new Mock<IInsightsService>();

			var underTest = new InventoryService (logger.Object, loginService.Object, invRepository, eventFact, insights.Object);


			//ACT
			await invRepository.Load (restaurant.ID);

			await underTest.StartNewInventory ();

			var res = await underTest.GetSelectedInventory ();

			//ASSERT
			Assert.NotNull (res);
			Assert.AreEqual (1, res.GetSections ().Count());
			Assert.AreEqual (0, res.GetBeverageLineItems ().Count());
		}

		[Test]
		public async Task StartInventoryWithPreviousInventoryWithItemsShouldAddThoseItems(){
			var sectionID = Guid.NewGuid ();
			var restaurant = new Restaurant {
				ID = Guid.NewGuid (),
				InventorySections = new List<RestaurantInventorySection>{
					new RestaurantInventorySection {
						Name = "testSec",
						ID = sectionID
					}
				}
			};

			var emp = new Employee {
				ID = Guid.NewGuid ()
			};

			var loginService = new Mock<ILoginService> ();
			loginService.Setup (ls => ls.GetCurrentRestaurant ())
				.Returns (Task.FromResult (restaurant as IRestaurant));
			loginService.Setup (ls => ls.GetCurrentEmployee ())
				.Returns (Task.FromResult (emp as IEmployee));

			var logger = new Mock<ILogger> ();

			var currentInventory = new Core.Common.Entities.Inventory.Inventory {
				RestaurantID = restaurant.ID,
				IsCurrent = true,
				CreatedDate = DateTime.UtcNow,
				Sections = new List<InventorySection>{
					new InventorySection {
						Name = "testSec",
						RestaurantInventorySectionID = sectionID,
						ID = Guid.NewGuid (),
						LineItems = new List<InventoryBeverageLineItem>{
							new InventoryBeverageLineItem {
								DisplayName = "Item",
								MiseName = "MiseItem",
								Container = LiquidContainer.Bottle12oz,
								InventoryPosition = 3,
								NumFullBottles = 10
							}
						}
					}
				}
			};
			var ws = new Mock<IInventoryWebService> ();
			ws.Setup(w => w.GetInventoriesForRestaurant(It.IsAny<Guid>()))
				.Returns(Task.FromResult(new List<IInventory>{currentInventory}.AsEnumerable()));
			ws.Setup (w => w.SendEventsAsync (It.IsAny<IInventory> (), It.IsAny<IEnumerable<IInventoryEvent>> ()))
				.Returns (Task.FromResult (true));

			var dal = new Mock<IClientDAL> ();
			dal.Setup (d => d.UpsertEntitiesAsync (It.IsAny<IEnumerable<IEntityBase>> ()))
				.Returns (Task.FromResult (true));
			var invRepository = new ClientInventoryRepository (logger.Object, dal.Object, ws.Object);

			var eventFact = new InventoryAppEventFactory ("test", MiseAppTypes.UnitTests);
			eventFact.SetRestaurant (restaurant);

			var insights = new Mock<IInsightsService>();

			var underTest = new InventoryService (logger.Object, loginService.Object, invRepository, eventFact, insights.Object);


			//ACT
			await invRepository.Load (restaurant.ID);

			await underTest.StartNewInventory ();

			var res = await underTest.GetSelectedInventory ();

			//ASSERT
			Assert.NotNull (res);
			Assert.AreEqual (1, res.GetSections ().Count());
			Assert.AreEqual (1, res.GetBeverageLineItems ().Count());

			var li = res.GetBeverageLineItems ().First ();
			Assert.AreEqual ("Item", li.DisplayName);
			Assert.AreSame ("MiseItem", li.MiseName);
			Assert.NotNull (li.Container);
			Assert.True (li.Container.Equals (LiquidContainer.Bottle12oz), "Container equals");
			Assert.AreEqual (3, li.InventoryPosition, "Inventory position");
			Assert.AreEqual (0, li.Quantity, "Quantity is zero");
		}

	    [Test]
	    public async Task ItemsWithZeroQuantityShouldNotBeCopied()
	    {
            var sectionID = Guid.NewGuid();
            var restaurant = new Restaurant
            {
                ID = Guid.NewGuid(),
                InventorySections = new List<RestaurantInventorySection>{
					new RestaurantInventorySection {
						Name = "testSec",
						ID = sectionID
					}
				}
            };

            var emp = new Employee
            {
                ID = Guid.NewGuid()
            };

            var loginService = new Mock<ILoginService>();
            loginService.Setup(ls => ls.GetCurrentRestaurant())
                .Returns(Task.FromResult(restaurant as IRestaurant));
            loginService.Setup(ls => ls.GetCurrentEmployee())
                .Returns(Task.FromResult(emp as IEmployee));

            var logger = new Mock<ILogger>();

            var currentInventory = new Core.Common.Entities.Inventory.Inventory
            {
                RestaurantID = restaurant.ID,
                IsCurrent = true,
                CreatedDate = DateTime.UtcNow,
                Sections = new List<InventorySection>{
					new InventorySection {
						Name = "testSec",
						RestaurantInventorySectionID = sectionID,
						ID = Guid.NewGuid (),
						LineItems = new List<InventoryBeverageLineItem>{
							new InventoryBeverageLineItem {
								DisplayName = "Item",
								MiseName = "MiseItem",
								Container = LiquidContainer.Bottle12oz,
								InventoryPosition = 3,
								NumFullBottles = 10
							},
                            new InventoryBeverageLineItem
                            {
                                DisplayName = "EmptyItem",
                                MiseName = "EmptyItem",
                                Container = LiquidContainer.Bottle750ML,
                                InventoryPosition = 2,
                                NumFullBottles = 0
                            }
						}
					}
				}
            };
            var ws = new Mock<IInventoryWebService>();
            ws.Setup(w => w.GetInventoriesForRestaurant(It.IsAny<Guid>()))
                .Returns(Task.FromResult(new List<IInventory> { currentInventory }.AsEnumerable()));
            ws.Setup(w => w.SendEventsAsync(It.IsAny<IInventory>(), It.IsAny<IEnumerable<IInventoryEvent>>()))
                .Returns(Task.FromResult(true));

            var dal = new Mock<IClientDAL>();
            dal.Setup(d => d.UpsertEntitiesAsync(It.IsAny<IEnumerable<IEntityBase>>()))
                .Returns(Task.FromResult(true));
            var invRepository = new ClientInventoryRepository(logger.Object, dal.Object, ws.Object);

            var eventFact = new InventoryAppEventFactory("test", MiseAppTypes.UnitTests);
            eventFact.SetRestaurant(restaurant);

            var insights = new Mock<IInsightsService>();

            var underTest = new InventoryService(logger.Object, loginService.Object, invRepository, eventFact, insights.Object);


            //ACT
            await invRepository.Load(restaurant.ID);

            await underTest.StartNewInventory();

            var res = await underTest.GetSelectedInventory();

            //ASSERT
            Assert.NotNull(res);
            Assert.AreEqual(1, res.GetSections().Count());
            Assert.AreEqual(1, res.GetBeverageLineItems().Count());

            var li = res.GetBeverageLineItems().First();
            Assert.AreEqual("Item", li.DisplayName);
            Assert.AreSame("MiseItem", li.MiseName);
            Assert.NotNull(li.Container);
            Assert.True(li.Container.Equals(LiquidContainer.Bottle12oz), "Container equals");
            Assert.AreEqual(3, li.InventoryPosition, "Inventory position");
            Assert.AreEqual(0, li.Quantity, "Quantity is zero");
	    }

		/// <summary>
		/// If an inventory already exists and is current,
		/// and we add a new section, it should be added as well
		/// </summary>
		[Test]
		public async Task AddSectionShouldAddToCurrentlyExistingInventory(){
			var rest = new Restaurant {
				ID = Guid.NewGuid ()
			};

			var currentInventory = new Core.Common.Entities.Inventory.Inventory {
				RestaurantID = rest.ID,
                IsCurrent = true
			};

			var currentEmp = new Employee {
				RestaurantsAndAppsAllowed = new Dictionary<Guid, IList<MiseAppTypes>>{
					{rest.ID, new List<MiseAppTypes>()}
				}
			};

			var logger = new Mock<ILogger> ();
			var ws = new Mock<IInventoryWebService> ();
			ws.Setup(w => w.GetInventoriesForRestaurant(It.IsAny<Guid>()))
				.Returns(Task.FromResult(new List<IInventory>{currentInventory}.AsEnumerable()));
					
			var dal = new Mock<IClientDAL> ();
			var inventoryRepos = new ClientInventoryRepository (logger.Object, dal.Object, ws.Object);
			var eventFact = new InventoryAppEventFactory ("test", MiseAppTypes.UnitTests);
			eventFact.SetRestaurant (rest);

			var empRepos = new Mock<IEmployeeRepository> ();
			empRepos.Setup (er => er.GetByEmailAndPassword (It.IsAny<EmailAddress> (), It.IsAny<Password> ()))
				.Returns (Task.FromResult (currentEmp as IEmployee));
			empRepos.Setup (er => er.Commit (It.IsAny<Guid> ()))
				.Returns (Task.FromResult (CommitResult.SentToServer));

		    var restaurantWs = new Mock<IInventoryRestaurantWebService>();
		    restaurantWs.Setup(rws => rws.GetRestaurant(It.IsAny<Guid>()))
		        .Returns(Task.FromResult(rest as IRestaurant));
			restaurantWs.Setup (rws => rws.SendEventsAsync (It.IsAny<IRestaurant> (), It.IsAny<IEnumerable<IRestaurantEvent>> ()))
				.Returns(Task.FromResult(true));

		    var restRepos = new ClientRestaurantRepository(logger.Object, dal.Object, restaurantWs.Object);
            await restRepos.Load(rest.ID);
            
			var inviteRepos = new Mock<IApplicationInvitationRepository> ();
            var reposLoader = new Mock<IRepositoryLoader>();
            reposLoader.Setup(rl => rl.LoadRepositories(It.IsAny<Guid?>())).Returns(Task.FromResult(false));
            var loginService = new LoginService(empRepos.Object, restRepos, inviteRepos.Object, null, 
				eventFact, null, logger.Object, reposLoader.Object);
			loginService.SetCurrentEmployee (currentEmp);
			loginService.SetCurrentRestaurant (rest);

		    var insights = new Mock<IInsightsService>();
            await inventoryRepos.Load(rest.ID);

			var underTest = new InventoryService (logger.Object, loginService, inventoryRepos, eventFact, insights.Object);

			//ACT
			await underTest.AddNewSection("testSection", false, false);

			//ASSERT
			var sections = currentInventory.GetSections ().ToList();
			Assert.AreEqual (1, sections.Count);
		}

		[Test]
		public async Task AddItemShouldGiveItemPositionInOrderFromVendorAndPar(){
			var rest = new Restaurant {
				ID = Guid.NewGuid ()
			};

			var section = new RestaurantInventorySection{
				ID = Guid.NewGuid (),
				Name = "TestSection"
			};
			var currentInventory = new Core.Common.Entities.Inventory.Inventory {
				RestaurantID = rest.ID,
				IsCurrent = true,
				Sections = new List<InventorySection>{
					new InventorySection {
						ID = Guid.NewGuid (),
						RestaurantInventorySectionID = section.ID
					}
				}
			};

			var currentEmp = new Employee {
				RestaurantsAndAppsAllowed = new Dictionary<Guid, IList<MiseAppTypes>>{
					{rest.ID, new List<MiseAppTypes>()}
				}
			};

			var logger = new Mock<ILogger> ();
			var ws = new Mock<IInventoryWebService> ();
			ws.Setup(w => w.GetInventoriesForRestaurant(It.IsAny<Guid>()))
				.Returns(Task.FromResult(new List<IInventory>{currentInventory}.AsEnumerable()));

			var dal = new Mock<IClientDAL> ();
			var inventoryRepos = new ClientInventoryRepository (logger.Object, dal.Object, ws.Object);
			var eventFact = new InventoryAppEventFactory ("test", MiseAppTypes.UnitTests);
			eventFact.SetRestaurant (rest);


			var reposLoader = new Mock<IRepositoryLoader>();
			reposLoader.Setup(rl => rl.LoadRepositories(It.IsAny<Guid?>())).Returns(Task.FromResult(false));

			var loginService = new Mock<ILoginService> ();
			loginService.Setup (ls => ls.GetCurrentEmployee ())
				.Returns (Task.FromResult (currentEmp as IEmployee));
			loginService.Setup (ls => ls.GetCurrentSection ())
				.Returns (Task.FromResult (section as IRestaurantInventorySection));

			var insights = new Mock<IInsightsService>();
			await inventoryRepos.Load(rest.ID);

			var underTest = new InventoryService (logger.Object, loginService.Object, inventoryRepos, eventFact, insights.Object);

			//ACT
		
			var vendorItem1 = new VendorBeverageLineItem {
				ID = Guid.NewGuid (),
				Categories = new List<ItemCategory>{
					CategoriesService.Brandy
				},
				DisplayName = "TestBrandy",
				Container = LiquidContainer.Bottle750ML
			};
			await underTest.AddLineItemToCurrentInventory (vendorItem1, 10, Money.None);

			var vendorItem2 = new VendorBeverageLineItem {
				ID = Guid.NewGuid (),
				Categories = new List<ItemCategory>{CategoriesService.Rum},
				DisplayName = "TestRum",
				Container = LiquidContainer.Bottle375ML
			};
			await underTest.AddLineItemToCurrentInventory (vendorItem2, 1, Money.None);

			await underTest.AddLineItemToCurrentInventory ("TestRaw", CategoriesService.AgaveMezcal, "", 10, 10, 
				LiquidContainer.Bottle1L, Money.None);


			var parItem1 = new PARBeverageLineItem {
				ID = Guid.NewGuid (),
				Categories = new List<ItemCategory>{ CategoriesService.WhiskeyScotch },
				DisplayName = "TestScotch",
				Container = LiquidContainer.Bottle750ML
			};
			await underTest.AddLineItemToCurrentInventory (parItem1, 19, Money.None);

			var inventory = await underTest.GetCurrentInventory ();

			//ASSERT
			var sections = inventory.GetSections ().ToList();
			Assert.AreEqual (1, sections.Count, "num sections");
			var retSection = sections.FirstOrDefault ();
			var testRum = retSection.GetInventoryBeverageLineItemsInSection ()
				.FirstOrDefault (li => li.DisplayName == "TestRum");
			var testRaw = retSection.GetInventoryBeverageLineItemsInSection ()
				.FirstOrDefault (li => li.DisplayName == "TestRaw");
			var testBrandy = retSection.GetInventoryBeverageLineItemsInSection ()
				.FirstOrDefault (li => li.DisplayName == "TestBrandy");
			var testScotch = retSection.GetInventoryBeverageLineItemsInSection ()
				.FirstOrDefault (li => li.DisplayName == "TestScotch");
			
			Assert.NotNull (testBrandy);
			Assert.NotNull (testRum);
			Assert.NotNull (testRaw);
			Assert.NotNull (testScotch);

			Assert.GreaterOrEqual (testRum.InventoryPosition, testBrandy.InventoryPosition, "Rum after brandy");
			Assert.GreaterOrEqual (testRaw.InventoryPosition, testRum.InventoryPosition, "Raw after rum");
			Assert.GreaterOrEqual (testScotch.InventoryPosition, testRaw.InventoryPosition, "Scotch after raw");
		}
	}
}

