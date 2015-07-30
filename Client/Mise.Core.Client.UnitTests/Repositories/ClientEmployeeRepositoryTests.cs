using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Mise.Core.Common.Entities;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Common.Events.Employee;
using Mise.Core.Common.Services.Implementation.DAL;
using Mise.Core.Common.Services.Implementation.Serialization;
using Mise.Core.Entities.People.Events;
using Mise.Core.Services.WebServices;
using NUnit.Framework;
using Mise.Core.Common.UnitTests.Tools;
using Moq;
using Mise.Core.Common.Services;
using Mise.Core.Entities.Base;
using Mise.Core.Services.UtilityServices;
using Mise.Core.Entities.People;
using Mise.Core.Client.Repositories;
using Mise.Core.ValueItems;
using Mise.Core.Common.Events;
using Mise.Core.Entities;


namespace Mise.Core.Client.UnitTests.Repositories
{
	[TestFixture]
	public class ClientEmployeeRepositoryTests
	{
		[Test]
		public async Task LoadRepositoryStoresWebServiceResultsInDB(){
			var service = MockingTools.GetTerminalService ();

			var emp = new Employee
			{
				ID = Guid.NewGuid ()
			};
			service.Setup (s => s.GetEmployeesForRestaurant(It.IsAny<Guid>())).Returns (Task<IEnumerable<IEmployee>>.Factory.StartNew(() => new List<IEmployee>{emp}));

			var dal = new Mock<IClientDAL> ();
			IList<IEntityBase> sentEnts = null;
			dal.Setup(d => d.UpsertEntitiesAsync (It.IsAny<IEnumerable<IEntityBase>> ()))
				.Callback<IEnumerable<IEntityBase>>(r => sentEnts = r.ToList())
                .Returns(Task.Factory.StartNew(() => true));

			var logger = new Mock<ILogger> ();
            var repos = new ClientEmployeeRepository(service.Object, dal.Object, logger.Object, MockingTools.GetResendEventsService().Object);

			//ACT
			await repos.Load (MockingTools.RestaurantID);
			var gotten = repos.GetAll ().ToList();

			//ASSERT
			Assert.IsNotNull (sentEnts);
			Assert.AreEqual (1, sentEnts.Count());
			var myEnt = sentEnts.First () as IEmployee;
			Assert.IsNotNull (myEnt);
			Assert.AreEqual (emp.ID, myEnt.ID);

			Assert.IsNotNull (gotten);
			Assert.AreEqual (1, gotten.Count());
			Assert.AreEqual (emp.ID, gotten.First ().ID);

            service.Verify(s => s.GetEmployeesForRestaurant(MockingTools.RestaurantID), Times.Once);
		}

		[Test]
		public async Task LoadRepositoryLoadsFromDBWhenServiceThrows(){
			var service = MockingTools.GetTerminalService ();

			var emp = new Employee
			{
				ID = Guid.NewGuid ()
			};
			service.Setup (s => s.GetEmployeesForRestaurant(It.IsAny<Guid>())).Throws (new ArgumentException ());

			var dal = new Mock<IClientDAL> ();
			dal.Setup (d => d.GetEntitiesAsync<Employee> ()).Returns(Task.FromResult(new List<Employee>{emp}.AsEnumerable()));

			var logger = new Mock<ILogger> ();
			logger.Setup (l => l.HandleException (It.IsAny<Exception>(), It.IsAny<LogLevel> ()));
            var repos = new ClientEmployeeRepository(service.Object, dal.Object, logger.Object, MockingTools.GetResendEventsService().Object);

			//ACT
            await repos.Load(MockingTools.RestaurantID);

			var gotten = repos.GetAll ().ToList();
			//ASSERT
			dal.Verify (d => d.UpsertEntitiesAsync (It.IsAny<IEnumerable<IRestaurantEntityBase>> ()), Times.Never ());

			Assert.IsNotNull (gotten);
			Assert.AreEqual (1, gotten.Count());
			Assert.AreEqual (emp.ID, gotten.First ().ID);
		}

        [Test]
        public async Task LoadRepositoryLoadsFromDBWhenTaskIsFaulted()
        {
            var service = MockingTools.GetTerminalService();

            var emp = new Employee
            {
                ID = Guid.NewGuid()
            };
            service.Setup(s => s.GetEmployeesAsync())
                .Returns(Task<IEnumerable<IEmployee>>.Factory.StartNew(() => { throw new WebException(); }));

            var dal = new Mock<IClientDAL>();
            dal.Setup(d => d.GetEntitiesAsync<Employee> ()).Returns(Task.FromResult(new List<Employee> { emp }.AsEnumerable()));

            var logger = new Mock<ILogger>();
            logger.Setup(l => l.HandleException(It.IsAny<Exception>(), It.IsAny<LogLevel>()));
            var repos = new ClientEmployeeRepository(service.Object, dal.Object, logger.Object, MockingTools.GetResendEventsService().Object);

            //ACT
            await repos.Load(null);

            var gotten = repos.GetAll().ToList();
            //ASSERT
            dal.Verify(d => d.UpsertEntitiesAsync(It.IsAny<IEnumerable<IRestaurantEntityBase>>()), Times.Never());

            Assert.IsNotNull(gotten);
            Assert.AreEqual(1, gotten.Count());
            Assert.AreEqual(emp.ID, gotten.First().ID);
        }

		/// <summary>
		/// Since bad login might not have a selected event, we need to make sure the repository works with it
		/// </summary>
		[Test]
		public void EmployeeRepositoryHandlesBadLoginEvent(){
			var blEvent = new BadLoginAttemptEvent {
				EventOrderingID = new EventID (),
				EmployeeID = new Guid (),
				PasscodeGiven = "2293"
			};

			var dal = new Mock<IClientDAL> ();
			var service = MockingTools.GetTerminalService ();
			var logger = new Mock<ILogger> ();
            var repository = new ClientEmployeeRepository(service.Object, dal.Object, logger.Object, MockingTools.GetResendEventsService().Object);

			//ACT
			var res = repository.ApplyEvent (blEvent);

			//ASSERT
			Assert.Null (res, "Employee is null");
		}

        [Test]
        public async Task CommitStoresEventsInDALWhenUnableToSend()
        {
            var empID = Guid.NewGuid();
            var emp = new Employee
            {
                ID = empID
            };

            var restaurantService = new Mock<IRestaurantTerminalService>();
			restaurantService.Setup(rs => rs.SendEventsAsync(It.IsAny<IEmployee> (), It.IsAny<IEnumerable<IEmployeeEvent>>()))
                .Returns(Task<bool>.Factory.StartNew(() =>
                {
                    var thrower = new ClientCheckRepositoryTests.Thrower();
                    return thrower.DoIt();
                }));
            restaurantService.Setup(rs => rs.GetEmployeesForRestaurant(It.IsAny<Guid>()))
                .Returns(Task<IEnumerable<IEmployee>>.Factory.StartNew(() => new List<IEmployee> { emp }));


            var dal = new Mock<IClientDAL>();
            dal.Setup(d => d.StoreEventsAsync(It.IsAny<IEnumerable<IEmployeeEvent>>())).Returns(Task.Factory.StartNew(() => true));
            dal.Setup(d => d.UpsertEntitiesAsync(It.IsAny<IEnumerable<IRestaurantEntityBase>>()))
                .Returns(Task.Factory.StartNew(() => true));

            var logger = new Mock<ILogger>();

            var repository = new ClientEmployeeRepository(restaurantService.Object, dal.Object, logger.Object, MockingTools.GetResendEventsService().Object);

            //ACT
            await repository.Load(MockingTools.RestaurantID);

            repository.ApplyEvent(new EmployeeClockedInEvent
            {
                ID = Guid.NewGuid(),
                EmployeeID = empID,
                EventOrderingID = new EventID
                {
                    AppInstanceCode = MiseAppTypes.UnitTests,
                    OrderingID = 1
                }
            });

            var res = await repository.Commit(empID);

            //ASSERT
            Assert.AreEqual(CommitResult.StoredInDB,res, "res is stored in DB");
            Assert.IsFalse(repository.Dirty, "repository is dirty");
			restaurantService.Verify(r => r.SendEventsAsync(It.IsAny<IEmployee> (), It.IsAny<IEnumerable<IEmployeeEvent>>()), Times.Once());
            dal.Verify(d => d.AddEventsThatFailedToSend(It.IsAny<IEnumerable<IEntityEventBase>>()), Times.Once, "AddedEventsThat failed to send");
            dal.Verify(d => d.StoreEventsAsync(It.IsAny<IEnumerable<IEmployeeEvent>>()), Times.Never);

            //once when we loaded, once when we updated
            dal.Verify(d => d.UpsertEntitiesAsync(It.IsAny<IEnumerable<IEntityBase>>()), Times.Exactly(2));
        }

        [Test]
        public async Task CommitDoesntStoreEventsInDALWhenAbleToSend()
        {
            var empID = Guid.NewGuid();
            var emp = new Employee
            {
                ID = empID
            };

            var restaurantService = new Mock<IRestaurantTerminalService>();
			restaurantService.Setup(rs => rs.SendEventsAsync(It.IsAny<IEmployee> (), It.IsAny<IEnumerable<IEmployeeEvent>>()))
                .Returns(Task<bool>.Factory.StartNew(() => true));
            restaurantService.Setup(rs => rs.GetEmployeesForRestaurant(It.IsAny<Guid>()))
                .Returns(Task<IEnumerable<IEmployee>>.Factory.StartNew(() => new List<IEmployee>{emp}));


            var dal = new Mock<IClientDAL>();
            dal.Setup(d => d.StoreEventsAsync(It.IsAny<IEnumerable<IEmployeeEvent>>())).Returns(Task.Factory.StartNew(() => true));
            dal.Setup(d => d.UpsertEntitiesAsync(It.IsAny<IEnumerable<IRestaurantEntityBase>>()))
                .Returns(Task.Factory.StartNew(() => true));

            var logger = new Mock<ILogger>();

            var repository = new ClientEmployeeRepository(restaurantService.Object, dal.Object, logger.Object, MockingTools.GetResendEventsService().Object);

            //ACT
            await repository.Load(MockingTools.RestaurantID);

            repository.ApplyEvent(new EmployeeClockedInEvent
            {
                ID = Guid.NewGuid(),
                EmployeeID = empID,
                EventOrderingID = new EventID
                {
                    AppInstanceCode = MiseAppTypes.UnitTests,
                    OrderingID = 1
                }
            });

            var res = await repository.Commit(empID);

            //ASSERT
            Assert.AreEqual(CommitResult.SentToServer,res, "res");
            Assert.IsFalse(repository.Dirty, "repository is dirty");
			restaurantService.Verify(r => r.SendEventsAsync(It.IsAny<IEmployee> (), It.IsAny<IEnumerable<IEmployeeEvent>>()), Times.Once());
            dal.Verify(d => d.StoreEventsAsync(It.IsAny<IEnumerable<IEmployeeEvent>>()), Times.Never());
            dal.Verify(d => d.UpsertEntitiesAsync(It.IsAny<IEnumerable<IEntityBase>>()), Times.Exactly(2));
        }

	    [Test]
	    public async Task ShouldLoadFromDBWhenCannotConnect()
	    {
	        var webService = new Mock<IInventoryEmployeeWebService>();
	        var restID = Guid.NewGuid();
	        webService.Setup(ws => ws.GetEmployeesAsync())
	            .Throws<Exception>();

	        var logger = new Mock<ILogger>();

	        var dal = new MemoryClientDAL(logger.Object, new JsonNetSerializer());
	        var resendService = new Mock<IResendEventsWebService>();

	        var underTest = new ClientEmployeeRepository(webService.Object, dal, logger.Object, resendService.Object);

	        var emps = new List<IEmployee>
	        {
	            new Employee
	            {
	                ID = Guid.NewGuid(),
	                CanCompAmount = true,
	                CreatedDate = DateTime.UtcNow,
	                CurrentlyLoggedIntoInventoryApp = false,
	                DisplayName = "Test employee",
	                Name = PersonName.TestName,
	                Emails = new List<EmailAddress> {EmailAddress.TestEmail},
	                Password = Password.TestPassword,
	                RestaurantsAndAppsAllowed =
	                    new Dictionary<Guid, IList<MiseAppTypes>>
	                    {
	                        {restID, new[] {MiseAppTypes.StockboyMobile, MiseAppTypes.UnitTests}}
	                    },
	                Revision = new EventID {AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101}
	            }
	        };

	        //ACT
	        //load up the DB
	        var storeRes = await dal.UpsertEntitiesAsync(emps);
            Assert.True(storeRes);

	        await underTest.Load(null);
	        var items = underTest.GetAll().ToList();

            //ASSERT
            Assert.NotNull(items);
            Assert.AreEqual(1, items.Count());

	        var first = items.First();
            Assert.AreEqual(first.ID, emps.First().ID);
            Assert.True(first.CanCompAmount);
            Assert.False(first.CurrentlyLoggedIntoInventoryApp);
            Assert.AreEqual("Test employee", first.DisplayName);
            Assert.IsTrue(emps.First().Name.Equals(first.Name), "Name");
            Assert.AreEqual(1, first.GetEmailAddresses().Count());
	        Assert.IsTrue(first.GetEmailAddresses().First().Equals(EmailAddress.TestEmail), "Email");
            Assert.IsTrue(first.Password.Equals(Password.TestPassword));

	        var apps = first.GetAppsEmployeeCanUse(restID).ToList();
            Assert.AreEqual(2, apps.Count);
            Assert.IsTrue(apps.Contains(MiseAppTypes.StockboyMobile));
            Assert.IsTrue(apps.Contains(MiseAppTypes.UnitTests));

            Assert.NotNull(first.Revision);
	    }
	}
}

