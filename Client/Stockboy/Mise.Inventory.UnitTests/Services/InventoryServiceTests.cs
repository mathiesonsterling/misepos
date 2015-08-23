﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Client.Repositories;
using Mise.Core.Client.Services;
using Mise.Core.Common.Entities;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Common.Events;
using Mise.Core.Common.Services.WebServices;
using Mise.Core.Entities;
using Mise.Core.Entities.Inventory.Events;
using Mise.Core.Entities.People;
using Mise.Core.Entities.Restaurant;
using Mise.Core.Entities.Restaurant.Events;
using Mise.Core.Repositories;
using Mise.Core.Services.UtilityServices;
using Mise.Core.ValueItems;
using Mise.Core.ValueItems.Inventory;
using Mise.Inventory.Services;
using Mise.Inventory.Services.Implementation;
using Moq;
using NUnit.Framework;

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
				.Returns(Task.FromResult(new List<Core.Common.Entities.Inventory.Inventory>{currentInventory}.AsEnumerable()));
			ws.Setup (w => w.SendEventsAsync (It.IsAny<Core.Common.Entities.Inventory.Inventory> (), It.IsAny<IEnumerable<IInventoryEvent>> ()))
				.Returns (Task.FromResult (true));
			
			var invRepository = new ClientInventoryRepository (logger.Object, ws.Object);

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
			var restSectionID = Guid.NewGuid ();
		    var invSectionID = Guid.NewGuid();
			var restaurant = new Restaurant {
				ID = Guid.NewGuid (),
				InventorySections = new List<RestaurantInventorySection>{
					new RestaurantInventorySection {
						Name = "testSec",
						ID = restSectionID
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
                DateCompleted = DateTime.UtcNow,
				Sections = new List<InventorySection>{
					new InventorySection {
						Name = "testSec",
						RestaurantInventorySectionID = restSectionID,
						ID = invSectionID,
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
				.Returns(Task.FromResult(new List<Core.Common.Entities.Inventory.Inventory>{currentInventory}.AsEnumerable()));
			ws.Setup (w => w.SendEventsAsync (It.IsAny<Core.Common.Entities.Inventory.Inventory> (), It.IsAny<IEnumerable<IInventoryEvent>> ()))
				.Returns (Task.FromResult (true));
			
            var invRepository = new ClientInventoryRepository(logger.Object, ws.Object);

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
            var restSectionID = Guid.NewGuid();
	        var invSectionID = Guid.NewGuid();
            var restaurant = new Restaurant
            {
                ID = Guid.NewGuid(),
                InventorySections = new List<RestaurantInventorySection>{
					new RestaurantInventorySection {
						Name = "testSec",
						ID = restSectionID
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
                DateCompleted = DateTime.UtcNow,
                Sections = new List<InventorySection>{
					new InventorySection {
						Name = "testSec",
						RestaurantInventorySectionID = restSectionID,
						ID = invSectionID,
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
                .Returns(Task.FromResult(new List<Core.Common.Entities.Inventory.Inventory> { currentInventory }.AsEnumerable()));
            ws.Setup(w => w.SendEventsAsync(It.IsAny<Core.Common.Entities.Inventory.Inventory>(), It.IsAny<IEnumerable<IInventoryEvent>>()))
                .Returns(Task.FromResult(true));
			
            var invRepository = new ClientInventoryRepository(logger.Object, ws.Object);

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
				.Returns(Task.FromResult(new List<Core.Common.Entities.Inventory.Inventory>{currentInventory}.AsEnumerable()));
					
            var inventoryRepos = new ClientInventoryRepository(logger.Object, ws.Object);
			var eventFact = new InventoryAppEventFactory ("test", MiseAppTypes.UnitTests);
			eventFact.SetRestaurant (rest);

			var empRepos = new Mock<IEmployeeRepository> ();
			empRepos.Setup (er => er.GetByEmailAndPassword (It.IsAny<EmailAddress> (), It.IsAny<Password> ()))
				.Returns (Task.FromResult (currentEmp as IEmployee));
			empRepos.Setup (er => er.Commit (It.IsAny<Guid> ()))
				.Returns (Task.FromResult (CommitResult.SentToServer));

		    var restaurantWs = new Mock<IInventoryRestaurantWebService>();
		    restaurantWs.Setup(rws => rws.GetRestaurant(It.IsAny<Guid>()))
		        .Returns(Task.FromResult(rest as Restaurant));
			restaurantWs.Setup (rws => rws.SendEventsAsync (It.IsAny<Restaurant> (), It.IsAny<IEnumerable<IRestaurantEvent>> ()))
				.Returns(Task.FromResult(true));

		    var loc = new Mock<IDeviceLocationService>();
            var restRepos = new ClientRestaurantRepository(logger.Object, restaurantWs.Object, loc.Object);
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
	}
}

