using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mise.Core.Entities.Restaurant;
using Mise.Core.Entities.Restaurant.Events;
using Mise.Core.Services.UtilityServices;
using Moq;
using NUnit.Framework;

using Mise.Inventory.Services.Implementation;
using Mise.Core.Entities.People;
using Mise.Core.Repositories;
using Mise.Core.Services;
using Mise.Core.Common.Events;
using Mise.Core.Entities.People.Events;
using Mise.Core.ValueItems;
using Mise.Core.Common.Entities;
using Mise.Core.Entities;
using Mise.Core.Client.Services;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Common.Services.WebServices;
namespace Mise.Inventory.UnitTests.Services
{
	[TestFixture]
	public class LoginServiceTests
	{
		[Test]
		public void LoginWithInvalidEmailAndPasswordReturnsNull(){
			var moqRepos = new Mock<IEmployeeRepository> ();
			moqRepos.Setup(r => r.GetByEmailAndPassword(It.IsAny<EmailAddress>(), It.IsAny<Password>()))
				.Returns(() => Task<IEmployee>.Factory.StartNew (() => null));

			var moqRestRepos = new Mock<IRestaurantRepository> ();

			var inviteRepos = new Mock<IApplicationInvitationRepository> ();
			var moqLogger = new Mock<ILogger> ();

			var accountRepos = new Mock<IAccountRepository> ();
			var restaurant = new Restaurant {
				Id = Guid.NewGuid()
			};
			var evFactory = new InventoryAppEventFactory ("testDEvice", MiseAppTypes.UnitTests);
			evFactory.SetRestaurant (restaurant);

			var mockKeyClient = new Mock<IClientKeyValueStorage> ();

		    var reposLoader = new Mock<IRepositoryLoader>();
		    reposLoader.Setup(rl => rl.LoadRepositories(It.IsAny<Guid?>())).Returns(Task.FromResult(false));
			var underTest = new LoginService (moqRepos.Object, moqRestRepos.Object, inviteRepos.Object, accountRepos.Object, 
				evFactory, mockKeyClient.Object, moqLogger.Object, reposLoader.Object, null);

			var badEmail = new EmailAddress{ Value = "bob@Qfuck.com" };
			var password = new Password ();
			//ACT
			var task = underTest.LoginAsync (badEmail, password);

			//ASSERT
			Assert.IsNotNull(task);
			Assert.IsNull(task.Result);
		}

		[Test]
		public void LoginWithValidEmailAndPasswordReturnsEmployee(){
			var moqRepos = new Mock<IEmployeeRepository> ();
			var emp = new Employee{ Id = Guid.NewGuid () };
			moqRepos.Setup(r => r.GetByEmailAndPassword(It.IsAny<EmailAddress>(), It.IsAny<Password>()))
				.Returns(() => Task<IEmployee>.Factory.StartNew (()=> emp));
			moqRepos.Setup (r => r.Commit (emp.Id)).Returns (Task.Factory.StartNew (() => CommitResult.SentToServer));
			moqRepos.Setup (r => r.ApplyEvent (It.IsAny<IEmployeeEvent> ())).Returns (emp);
			var moqLogger = new Mock<ILogger> ();

			var restaurant = new Restaurant {
				Id = Guid.NewGuid()
			};
			var evFactory = new InventoryAppEventFactory ("testDevice", MiseAppTypes.UnitTests);
			evFactory.SetRestaurant (restaurant);
			var moqRestRepos = new Mock<IRestaurantRepository> ();

			var inviteRepos = new Mock<IApplicationInvitationRepository> ();
            var reposLoader = new Mock<IRepositoryLoader>();
            reposLoader.Setup(rl => rl.LoadRepositories(It.IsAny<Guid?>())).Returns(Task.FromResult(false));

			var underTest = new LoginService (moqRepos.Object, moqRestRepos.Object,inviteRepos.Object, null, 
				evFactory, null, moqLogger.Object, reposLoader.Object, null);

			var badEmail = new EmailAddress{ Value = "bob@Qfuck.com" };
			var password = new Password ();
			//ACT
			var task = underTest.LoginAsync (badEmail, password);

			//ASSERT
			Assert.IsNotNull(task);
			var res = task.Result;
			Assert.IsNotNull(res);
			Assert.AreEqual (emp.Id, res.Id);
		}

		[Test]
		public async void GetInvitesReturnsStockboyInvitesOnly(){
			var moqRepos = new Mock<IEmployeeRepository> ();
            var email = new EmailAddress { Value = "test@test.com" };
			var emp = new Employee{ Id = Guid.NewGuid (), Emails = new List<EmailAddress>{email}};

			moqRepos.Setup(r => r.GetByEmailAndPassword(It.IsAny<EmailAddress>(), It.IsAny<Password>()))
				.Returns(() => Task<IEmployee>.Factory.StartNew (()=> emp));
			moqRepos.Setup (r => r.Commit (emp.Id)).Returns (Task.Factory.StartNew (() => CommitResult.SentToServer));
			moqRepos.Setup (r => r.ApplyEvent (It.IsAny<IEmployeeEvent> ())).Returns (emp);
			var moqLogger = new Mock<ILogger> ();

			var restaurant = new Restaurant {
				Id = Guid.NewGuid()
			};
			var evFactory = new InventoryAppEventFactory ("testDevice", MiseAppTypes.UnitTests);
			evFactory.SetRestaurant (restaurant);
			var moqRestRepos = new Mock<IRestaurantRepository> ();

			var inviteRepos = new Mock<IApplicationInvitationRepository> ();
			var goodID = Guid.NewGuid ();
			inviteRepos.Setup (ir => ir.GetAll ())
				.Returns (
					new List<IApplicationInvitation>{
						new ApplicationInvitation {
							Id = goodID,
							DestinationEmail = email,
							Application = MiseAppTypes.StockboyMobile
						},
						new ApplicationInvitation{
							Id = Guid.NewGuid (),
							DestinationEmail = email,
							Application = MiseAppTypes.UnitTests
						}
					}
			);
            var reposLoader = new Mock<IRepositoryLoader>();
            reposLoader.Setup(rl => rl.LoadRepositories(It.IsAny<Guid?>())).Returns(Task.FromResult(false));

			var underTest = new LoginService (moqRepos.Object, moqRestRepos.Object,inviteRepos.Object, null, 
				evFactory, null, moqLogger.Object, reposLoader.Object, null);

			//ACT
			var loginRes = await underTest.LoginAsync (new EmailAddress{Value="actor@test.com"}, 
				new Password("test"));
			Assert.NotNull (loginRes);
			var inviteRes = (await underTest.GetInvitationsForCurrentEmployee ()).ToList();

			Assert.NotNull (inviteRes);
			Assert.AreEqual (1, inviteRes.Count());

			var invite = inviteRes.FirstOrDefault ();
			Assert.NotNull (invite);
			Assert.AreEqual (goodID, invite.Id);
			Assert.AreEqual (MiseAppTypes.StockboyMobile, invite.Application);
		}

	    [Test]
	    public async Task LoadRestaurantSetsForEventFactoryAndReloadsRepositories()
	    {
	        var restID = Guid.NewGuid();
	        var moqRestaurantRepos = new Mock<IRestaurantRepository>();

	        var myRest = new Restaurant
	        {
	            Id = restID
	        };
	        moqRestaurantRepos.Setup(mr => mr.GetByID(restID)).Returns(myRest);
	        moqRestaurantRepos.Setup(mr => mr.ApplyEvent(It.IsAny<IRestaurantEvent>())).Returns(myRest);

	        var moqEventFactory = new Mock<IInventoryAppEventFactory>();
	        IRestaurant givenRest = null;
	        moqEventFactory.Setup(ef => ef.SetRestaurant(myRest))
	            .Callback<IRestaurant>(r => givenRest = r);
            var reposLoader = new Mock<IRepositoryLoader>();
            reposLoader.Setup(rl => rl.LoadRepositories(It.IsAny<Guid?>())).Returns(Task.FromResult(false));

	        var logger = new Mock<ILogger>();

			var keyValStorage = new Mock<IClientKeyValueStorage> ();
			keyValStorage.Setup (s => s.SetValue (It.IsAny<string> (), It.IsAny<LoginService.RestaurantSelectRecord> ()))
				.Returns (Task.FromResult (true));
	        var underTest = new LoginService(null, moqRestaurantRepos.Object, null, null, moqEventFactory.Object, 
				keyValStorage.Object, logger.Object, reposLoader.Object, null);

            //ACT
	        await underTest.SelectRestaurantForLoggedInEmployee(restID);
            await underTest.LoadSelectedRestaurant();

            //ASSERT
            moqEventFactory.Verify(ef => ef.SetRestaurant(myRest), Times.Once);
            reposLoader.Verify(rl => rl.LoadRepositories(restID), Times.Once);
            Assert.NotNull(givenRest);
            Assert.AreEqual(myRest, givenRest);
	    }

		[Test]
		public async Task AddNewSectionWithSameNameThrowsCaseInsensitive(){
			var restID = Guid.NewGuid ();
			var restaurant = new Restaurant {
				Id = restID,
				InventorySections = new List<RestaurantInventorySection>{
					new RestaurantInventorySection {
						Id = Guid.NewGuid (),
						Name = "TestSection"
					}
				}
			};
			var restRepos = new Mock<IRestaurantRepository> ();
			restRepos.Setup (rr => rr.GetByID (restID))
				.Returns (restaurant);
			
			var evFactory = new InventoryAppEventFactory ("testDevice", MiseAppTypes.UnitTests);
			evFactory.SetRestaurant (restaurant);

			var logger = new Mock<ILogger> ();

			var reposLoader = new Mock<IRepositoryLoader> ();
			reposLoader.Setup (rl => rl.LoadRepositories (restID))
				.Returns (Task.FromResult (true));
			
			var underTest = new LoginService (null, restRepos.Object, null, null, evFactory, null, 
				logger.Object, reposLoader.Object, null);
			underTest.SetCurrentRestaurant (restaurant);

			//ACT
			var threw = false;
			try{
				await underTest.AddNewSectionToRestaurant ("TESTSECTION", true, false);
			} catch(ArgumentException){
				threw = true;
			}

			var selRest = await underTest.GetCurrentRestaurant ();
			//ASSERT
			Assert.True (threw);

			Assert.NotNull (selRest);
			Assert.AreEqual (1, selRest.GetInventorySections ().Count(), "count is still 1");
		}

		[Test]
		public async Task RegisteringAlreadyHeldEmailThrows(){
			var webService = new Mock<IInventoryEmployeeWebService> ();
			webService.Setup (ws => ws.IsEmailRegistered (It.IsAny<EmailAddress> ())).ReturnsAsync (false);

			var underTest = new LoginService (null, null, null, null, null, null, null, null, webService.Object);

			//ACT
			Exception exception = null;
			try{
				var res = await underTest.RegisterEmployee(EmailAddress.TestEmail, Password.TestPassword, PersonName.TestName);
			} catch(Exception e){
				exception = e;
			}

			//ASSERT
			Assert.NotNull(exception);
			webService.Verify(ws => ws.IsEmailRegistered(It.IsAny<EmailAddress>()), Times.Once());
		}
	}
}

