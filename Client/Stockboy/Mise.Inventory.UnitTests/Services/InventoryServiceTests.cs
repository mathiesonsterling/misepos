using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Mise.Core.Entities.Restaurant;
using Mise.Inventory.Services;
using NUnit.Framework;
using Moq;

using Mise.Core.Entities.Inventory;
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
namespace Mise.Inventory.UnitTests.Services
{
	public class InventoryServiceTests
	{
		/// <summary>
		/// If an inventory already exists and is current,
		/// and we add a new section, it should be added as well
		/// </summary>
		[Test]
		public async void AddSectionShouldAddToCurrentlyExistingInventory(){
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

		    var insights = new Mock<IInsightsService>();
            await inventoryRepos.Load(rest.ID);

			var underTest = new InventoryService (logger.Object, loginService, inventoryRepos, eventFact, insights.Object);

			//ACT
			await loginService.LoginAsync(new EmailAddress(), new Password());
			await loginService.SelectRestaurantForLoggedInEmployee (rest.ID);

			var res = await underTest.AddNewSection("testSection", false, false);

			//ASSERT
			Assert.IsTrue(res);
			var sections = currentInventory.GetSections ().ToList();
			Assert.AreEqual (1, sections.Count);
		}
	}
}

