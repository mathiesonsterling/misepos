using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Mise.Core.Common.Entities;
using Mise.Core.Common.Events.Employee;
using Mise.Core.Entities.People.Events;
using Mise.Core.Services.WebServices;
using NUnit.Framework;
using Mise.Core.Common.UnitTests.Tools;
using Moq;
using Mise.Core.Common.Services;
using Mise.Core.Entities.Base;
using Mise.Core.Services;
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
		public void LoadRepositoryStoresWebServiceResultsInDB(){
			var service = MockingTools.GetTerminalService ();

			var emp = new Employee
			{
				ID = Guid.NewGuid ()
			};
			service.Setup (s => s.GetEmployeesAsync ()).Returns (Task<IEnumerable<IEmployee>>.Factory.StartNew(() => new List<IEmployee>{emp}));

			var dal = new Mock<IClientDAL> ();
			IList<IEntityBase> sentEnts = null;
			dal.Setup(d => d.UpsertEntitiesAsync (It.IsAny<IEnumerable<IEntityBase>> ()))
				.Callback<IEnumerable<IEntityBase>>(r => sentEnts = r.ToList())
                .Returns(Task.Factory.StartNew(() => true));

			var logger = new Mock<ILogger> ();
			var repos = new ClientEmployeeRepository (service.Object, dal.Object, logger.Object);

			//ACT
			repos.Load (MockingTools.RestaurantID).Wait();
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

            service.Verify(s => s.GetEmployeesAsync(), Times.Once);
		}

		[Test]
		public void LoadRepositoryLoadsFromDBWhenServiceThrows(){
			var service = MockingTools.GetTerminalService ();

			var emp = new Employee
			{
				ID = Guid.NewGuid ()
			};
			service.Setup (s => s.GetEmployeesAsync ()).Throws (new ArgumentException ());

			var dal = new Mock<IClientDAL> ();
			dal.Setup (d => d.GetEntitiesAsync<IEmployee> ()).Returns(Task.Factory.StartNew(() =>new List<IEmployee>{emp}.AsEnumerable()));

			var logger = new Mock<ILogger> ();
			logger.Setup (l => l.HandleException (It.IsAny<Exception>(), It.IsAny<LogLevel> ()));
			var repos = new ClientEmployeeRepository (service.Object, dal.Object, logger.Object);

			//ACT
            repos.Load(MockingTools.RestaurantID).Wait();

			var gotten = repos.GetAll ().ToList();
			//ASSERT
			dal.Verify (d => d.UpsertEntitiesAsync (It.IsAny<IEnumerable<IRestaurantEntityBase>> ()), Times.Never ());

			Assert.IsNotNull (gotten);
			Assert.AreEqual (1, gotten.Count());
			Assert.AreEqual (emp.ID, gotten.First ().ID);
		}

        [Test]
        public void LoadRepositoryLoadsFromDBWhenTaskIsFaulted()
        {
            var service = MockingTools.GetTerminalService();

            var emp = new Employee
            {
                ID = Guid.NewGuid()
            };
            service.Setup(s => s.GetEmployeesAsync())
                .Returns(Task<IEnumerable<IEmployee>>.Factory.StartNew(() => { throw new WebException(); }));

            var dal = new Mock<IClientDAL>();
            dal.Setup(d => d.GetEntitiesAsync<IEmployee> ()).Returns(Task.Factory.StartNew(() => new List<IEmployee> { emp }.AsEnumerable()));

            var logger = new Mock<ILogger>();
            logger.Setup(l => l.HandleException(It.IsAny<Exception>(), It.IsAny<LogLevel>()));
            var repos = new ClientEmployeeRepository(service.Object, dal.Object, logger.Object);

            //ACT
            repos.Load(MockingTools.RestaurantID).Wait();

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
			var repository = new ClientEmployeeRepository (service.Object, dal.Object, logger.Object);

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
            restaurantService.Setup(rs => rs.GetEmployeesAsync())
                .Returns(Task<IEnumerable<IEmployee>>.Factory.StartNew(() => new List<IEmployee> { emp }));


            var dal = new Mock<IClientDAL>();
            dal.Setup(d => d.StoreEventsAsync(It.IsAny<IEnumerable<IEmployeeEvent>>())).Returns(Task.Factory.StartNew(() => true));
            dal.Setup(d => d.UpsertEntitiesAsync(It.IsAny<IEnumerable<IRestaurantEntityBase>>()))
                .Returns(Task.Factory.StartNew(() => true));

            var logger = new Mock<ILogger>();

            var repository = new ClientEmployeeRepository(restaurantService.Object, dal.Object, logger.Object);

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
            dal.Verify(d => d.StoreEventsAsync(It.IsAny<IEnumerable<IEmployeeEvent>>()), Times.Once());
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
            restaurantService.Setup(rs => rs.GetEmployeesAsync())
                .Returns(Task<IEnumerable<IEmployee>>.Factory.StartNew(() => new List<IEmployee>{emp}));


            var dal = new Mock<IClientDAL>();
            dal.Setup(d => d.StoreEventsAsync(It.IsAny<IEnumerable<IEmployeeEvent>>())).Returns(Task.Factory.StartNew(() => true));
            dal.Setup(d => d.UpsertEntitiesAsync(It.IsAny<IEnumerable<IRestaurantEntityBase>>()))
                .Returns(Task.Factory.StartNew(() => true));

            var logger = new Mock<ILogger>();

            var repository = new ClientEmployeeRepository(restaurantService.Object, dal.Object, logger.Object);

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
	}
}

