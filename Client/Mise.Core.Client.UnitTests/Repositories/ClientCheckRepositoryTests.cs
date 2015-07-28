using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Mise.Core.Entities;
using Mise.Core.Common.Entities;
using Mise.Core.Common.Events.Checks;
using Mise.Core.Entities.Check.Events;
using Mise.Core.Entities.People;
using Mise.Core.Services.WebServices;
using Mise.Core.ValueItems;
using NUnit.Framework;
using Mise.Core.Common.UnitTests.Tools;
using Moq;
using Mise.Core.Common.Services;
using Mise.Core.Entities.Base;
using Mise.Core.Entities.Check;
using Mise.Core.Services;
using Mise.Core.Client.Repositories;

namespace Mise.Core.Client.UnitTests.Repositories
{
	[TestFixture]
	public class ClientCheckRepositoryTests
	{
		[Test]
		public async Task LoadRepositoryStoresWebServiceResultsInDB(){
			var service = MockingTools.GetTerminalService ();

			var barTab = new RestaurantCheck {
				ID = Guid.NewGuid ()
			};
		    service.Setup(s => s.GetChecksAsync())
		        .Returns(
		            Task<IEnumerable<ICheck>>.Factory.StartNew(() => new List<ICheck> {barTab})
		        );
			service.Setup (s => s.GetChecksAsync()).Returns (Task.FromResult(new List<ICheck>{
				barTab}.AsEnumerable ()
			));

			var dal = new Mock<IClientDAL> ();
			IList<IEntityBase> sentEnts = null;
		    dal.Setup(d => d.UpsertEntitiesAsync(It.IsAny<IEnumerable<IEntityBase>>()))
		        .Callback<IEnumerable<IEntityBase>>(r => sentEnts = r.ToList())
		        .Returns(Task.FromResult(true));

			var logger = new Mock<ILogger> ();
            var repos = new ClientCheckRepository(service.Object, dal.Object, logger.Object, MockingTools.GetResendEventsService().Object);

			//ACT
			await repos.Load (MockingTools.RestaurantID);
			var gotten = repos.GetAll ().ToList();

			//ASSERT
			Assert.IsNotNull (sentEnts);
			Assert.AreEqual (1, sentEnts.Count());
			var myEnt = sentEnts.First () as ICheck;
			Assert.IsNotNull (myEnt);
			Assert.AreEqual (barTab.ID, myEnt.ID);

			Assert.IsNotNull (gotten);
			Assert.AreEqual (1, gotten.Count());
			Assert.AreEqual (barTab.ID, gotten.First ().ID);
		}

		[Test]
		public async Task LoadRepositoryLoadsFromDBWhenExceptionThrownFromService(){
			var service = MockingTools.GetTerminalService ();

			var barTab = new RestaurantCheck {
				ID = Guid.NewGuid ()
			};
		    service.Setup(s => s.GetChecksAsync()).Throws(new WebException());

			var dal = new Mock<IClientDAL> ();
			dal.Setup (d => d.GetEntitiesAsync<RestaurantCheck> ()).Returns(Task.Factory.StartNew(() => new List<RestaurantCheck>{barTab}.AsEnumerable()));

			var logger = new Mock<ILogger> ();
            var repos = new ClientCheckRepository(service.Object, dal.Object, logger.Object, MockingTools.GetResendEventsService().Object);

			//ACT
			await repos.Load (MockingTools.RestaurantID);

			var gotten = repos.GetAll ().ToList();

			//ASSERT
			dal.Verify (d => d.UpsertEntitiesAsync (It.IsAny<IEnumerable<IEntityBase>> ()), Times.Never ());

			Assert.IsNotNull (gotten);
			Assert.AreEqual (1, gotten.Count());
			Assert.AreEqual (barTab.ID, gotten.First ().ID);
		}

        [Test]
        public async Task LoadRepositoryLoadsFromDBWhenTaskFaulted()
        {
            var service = MockingTools.GetTerminalService();

            var barTab = new RestaurantCheck
            {
                ID = Guid.NewGuid()
            };

            service.Setup(s => s.GetChecksAsync())
                .Returns(() =>
                    {
                        var task = new Task<IEnumerable<ICheck>>(() =>
                        {
                            throw new WebException();
                        });
                        task.Start();
                        return task;
                    }
                );
      

            var dal = new Mock<IClientDAL>();
            dal.Setup(d => d.GetEntitiesAsync<RestaurantCheck> ()).Returns(Task.FromResult(new List<RestaurantCheck> { barTab }.AsEnumerable()));

            var logger = new Mock<ILogger>();
            var repos = new ClientCheckRepository(service.Object, dal.Object, logger.Object, MockingTools.GetResendEventsService().Object);

            //ACT
            await repos.Load(MockingTools.RestaurantID);

            var gotten = repos.GetAll().ToList();
            //ASSERT
            dal.Verify(d => d.UpsertEntitiesAsync(It.IsAny<IEnumerable<IRestaurantEntityBase>>()), Times.Never());
            logger.Verify(l => l.HandleException(It.IsAny<WebException>(), It.IsAny<LogLevel>()), Times.Once());
            Assert.IsNotNull(gotten);
            Assert.AreEqual(1, gotten.Count());
            Assert.AreEqual(barTab.ID, gotten.First().ID);
        }

		[Test]
		public void AddEventsWithCreationAddsToRepository()
		{
		    var service = MockingTools.GetTerminalService();

			var checkID = Guid.NewGuid();
			var ccev = new CheckCreatedEvent{
                EventOrderingID = new EventID(),
				CheckID = checkID,
			};

			//create customer and add it!
			var custEv = new CustomerAssignedToCheckEvent
			{ 
                EventOrderingID = new EventID(),
				CheckID = checkID,
                Customer = new Customer { Name = PersonName.TestName } 
			};
				
			var events = new List<ICheckEvent>{ ccev, custEv};

			var logger = new Mock<ILogger> ();

		    var dal = MockingTools.GetClientDAL();


            var repos = new ClientCheckRepository(service.Object, dal.Object, logger.Object, MockingTools.GetResendEventsService().Object);

			//ACT
			var check = repos.ApplyEvents (events);
			var res = repos.Commit (ccev.CheckID).Result;
            Assert.AreEqual(CommitResult.SentToServer, res);
			Assert.IsNotNull (check);

			var got = repos.GetAll ().ToList();
			Assert.IsNotNull (got);
			Assert.AreEqual (1, got.Count ());
			Assert.AreEqual (check, got.FirstOrDefault ());
		}


		[Test]
		public async Task AddMemoEventSetsMemoOnOrderItem(){
            var service = MockingTools.GetTerminalService();

			var checkID =Guid.NewGuid();
            var ccev = new CheckCreatedEvent { CheckID = checkID, EventOrderingID = new EventID() };

            var custEv = new CustomerAssignedToCheckEvent { 
				EventOrderingID = new EventID(),
                Customer = new Customer { Name = PersonName.TestName }, 
				CheckID = checkID 
			};

		    var oiID = Guid.NewGuid();
			var orderItem = new OrderItem {
				ID = oiID,
				Memo = string.Empty,
			};

		    var empID = Guid.NewGuid();
			var orderEv = new OrderedOnCheckEvent
			{
			    CheckID = checkID,
                OrderItem = orderItem,
                EmployeeID = empID,
                EventOrderingID = new EventID()
			};

			var events = new List<ICheckEvent>{ ccev, custEv, orderEv };

			var logger = new Mock<ILogger> ();
		    var dal = MockingTools.GetClientDAL();
            var repos = new ClientCheckRepository(service.Object, dal.Object, logger.Object, MockingTools.GetResendEventsService().Object);

			var check = repos.ApplyEvents (events);
			await repos.Commit (ccev.CheckID);
			Assert.IsNotNull (check);

			//ACT
			var memoEvent = new OrderItemSetMemoEvent {
                EventOrderingID = new EventID(),
				CheckID = checkID,
				Memo = "memoTest",
				OrderItemID = oiID
			};
			repos.ApplyEvents (new List<ICheckEvent>{memoEvent});

			var got = repos.GetAll ().ToList();
			Assert.IsNotNull (got);
			Assert.AreEqual (1, got.Count ());
			Assert.AreEqual (check, got.FirstOrDefault ());

			var retCheck = got.First();
			Assert.AreEqual (1, retCheck.OrderItems.Count ());

			var retOI = retCheck.OrderItems.First();
			Assert.AreEqual ("memoTest", retOI.Memo);
		}

		[Test]
		public void CancelIgnoresUncommittedEvents()
		{
		    var service = MockingTools.GetTerminalService();

            var ccev = new CheckCreatedEvent { EventOrderingID = new EventID(), CheckID = Guid.NewGuid(), EmployeeID = Guid.NewGuid() };

			//create customer and add it!
            var custEv = new CustomerAssignedToCheckEvent { 
				EventOrderingID = new EventID(),
                Customer = new Customer { Name = PersonName.TestName } 
			};

			var events = new List<ICheckEvent>{ ccev, custEv};

			var logger = new Mock<ILogger> ();
            var repos = new ClientCheckRepository(service.Object, null, logger.Object, MockingTools.GetResendEventsService().Object);

			//ACT
			var check = repos.ApplyEvents (events);
			Assert.IsNotNull (check);

			repos.CancelTransaction (check.ID);

			var got = repos.GetAll ();
			Assert.IsNotNull (got);
			Assert.IsFalse (got.Any ());
		}

		[Test]
		public async Task CancelKeepsCommittedIgnoresUncommittedEvents()
		{
		    var service = MockingTools.GetTerminalService();

		    var checkID = Guid.NewGuid();
		    var empID = Guid.NewGuid();
			var ccev = new CheckCreatedEvent {
                EventOrderingID = new EventID(),
				CheckID = checkID,
				EmployeeID = empID
            };

			//create customer and add it!
			var custEv = new CustomerAssignedToCheckEvent {
                EventOrderingID = new EventID(),
				CheckID = checkID,
                Customer = new Customer { Name = PersonName.TestName },
				EmployeeID = empID
			};

			var events = new List<ICheckEvent>{ ccev, custEv};

			var logger = new Mock<ILogger> ();
		    var dal = MockingTools.GetClientDAL();

            var repos = new ClientCheckRepository(service.Object, dal.Object, logger.Object, MockingTools.GetResendEventsService().Object);

			//ACT
			var check = repos.ApplyEvents (events);
			Assert.IsNotNull (check);
			Assert.IsTrue (repos.Dirty, "repository is dirty");

			await repos.Commit (check.ID);
			Assert.IsFalse (repos.Dirty, "repository is clean");

			var got = repos.GetAll ().ToList();
			Assert.IsNotNull (got);
			Assert.AreEqual (1, got.Count ());
			Assert.AreEqual (check, got.FirstOrDefault ());
			Assert.AreEqual (empID, got.First ().LastTouchedServerID);

			repos.CancelTransaction (check.ID);

			var gotAgain = repos.GetAll ();
			Assert.IsNotNull (gotAgain);
			Assert.AreEqual (1, got.Count());
			Assert.AreEqual (empID, got.First ().LastTouchedServerID);
			Assert.IsFalse (repos.Dirty, "repository is clean");
		}

		[TestCase(10)]
		[TestCase(100)]
		[Test]
		public async Task AddEventsWithCreationAddsToRepositoryMultipleTimes(int numTimes)
		{
		    var service = MockingTools.GetTerminalService();

			var logger = new Mock<ILogger> ();

		    var dal = MockingTools.GetClientDAL();
            var repos = new ClientCheckRepository(service.Object, dal.Object, logger.Object, MockingTools.GetResendEventsService().Object);

			//ACT
			for(var i = 0; i < numTimes;i++){
                var ccev = new CheckCreatedEvent { EventOrderingID = new EventID(), CheckID = Guid.NewGuid() };

				//create customer and add it!
                var custEv = new CustomerAssignedToCheckEvent { EventOrderingID = new EventID(), Customer = new Customer { Name = new PersonName("test","TEST"+ i )} };

		        var events = new List<ICheckEvent>{ ccev, custEv};

				var check = repos.ApplyEvents (events);
				await repos.Commit (ccev.CheckID);
				Assert.IsNotNull (check);
			}

			var got = repos.GetAll ();
			Assert.IsNotNull (got);
			Assert.AreEqual (numTimes, got.Count ());
		}

		[Test]
		public async Task AddMultipleEventsWithCreationAddsToRepository()
		{
		    var service = MockingTools.GetTerminalService();
			var logger = new Mock<ILogger> ();
		    var dal = MockingTools.GetClientDAL();

            var repos = new ClientCheckRepository(service.Object, dal.Object, logger.Object, MockingTools.GetResendEventsService().Object);

		    var empID = Guid.NewGuid();
		    var checkID = Guid.NewGuid();
            var ccev = new CheckCreatedEvent { EventOrderingID = new EventID(), CheckID = checkID, EmployeeID = empID };
			var custEv = new CustomerAssignedToCheckEvent
			{
                EventOrderingID = new EventID(),
                CheckID = checkID,
				Customer = new Customer{Name = PersonName.TestName}, EmployeeID = empID 
            };
			var events = new List<ICheckEvent>{ ccev, custEv};


			//ACT
			var check = repos.ApplyEvents (events);
			await repos.Commit (ccev.CheckID);
			Assert.IsNotNull (check);

			var got = repos.GetAll ().ToList();
			Assert.IsNotNull (got);
			Assert.AreEqual (1, got.Count ());
			Assert.AreEqual (check, got.FirstOrDefault ());

		    var check2ID = Guid.NewGuid();
			var ccev2 = new CheckCreatedEvent{ CheckID = check2ID,
                EventOrderingID = new EventID { AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 1 }, 
				CreatedDate = DateTime.UtcNow,
				EmployeeID  = empID};
			var custEv2 = new CustomerAssignedToCheckEvent{
                CheckID = check2ID,
                Customer = new Customer { Name = PersonName.TestName },
                EventOrderingID = new EventID { AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 2 }, 
				CreatedDate = DateTime.UtcNow,
				EmployeeID = empID
			};
			var events2 = new List<ICheckEvent>{ccev2, custEv2};

			var check2 = repos.ApplyEvents (events2);
			await repos.Commit (ccev2.CheckID);
			Assert.IsNotNull (check2);
			Assert.AreNotEqual (check.ID, check2.ID);

			var got2 = repos.GetAll ().ToList();
			Assert.AreEqual (2, got2.Count ());
			Assert.True (got2.All (c => c.LastTouchedServerID == empID), "has serverID");
			Assert.True (got2.All (c => c.CreatedByServerID == empID), "created by serverIDs sent");
		}

	    public class Thrower
	    {
	        public bool DoIt()
	        {
	            throw new ArgumentException();
	        }
	    }

	    [Test]
	    public void CommitStoresEventsInDALWhenUnableToSend()
	    {
	        var checkID = Guid.NewGuid();

	        var restaurantService = new Mock<IRestaurantTerminalService>();
			restaurantService.Setup(rs => rs.SendEventsAsync(It.IsAny<ICheck> (), It.IsAny<IEnumerable<ICheckEvent>>()))
                .Returns(Task<bool>.Factory.StartNew(() =>
                {
                    var thrower = new Thrower();
                    return thrower.DoIt();
                }));


	        var dal = new Mock<IClientDAL>();
	        dal.Setup(d => d.StoreEventsAsync(It.IsAny<IEnumerable<ICheckEvent>>())).Returns(Task.Factory.StartNew(() => true));
	        dal.Setup(d => d.UpsertEntitiesAsync(It.IsAny<IEnumerable<IRestaurantEntityBase>>()))
	            .Returns(Task.Factory.StartNew(() => true));

	        var logger = new Mock<ILogger>();

            var repository = new ClientCheckRepository(restaurantService.Object, dal.Object, logger.Object, MockingTools.GetResendEventsService().Object);

            //ACT
	        repository.ApplyEvent(new CheckCreatedEvent
	        {
	            ID = Guid.NewGuid(),
	            CheckID = checkID,
                EventOrderingID = new EventID
                {
                    AppInstanceCode = MiseAppTypes.UnitTests,
                    OrderingID = 1
                }
	        });

	        var res = repository.Commit(checkID).Result;

            //ASSERT
            Assert.AreEqual(CommitResult.StoredInDB,res, "res is StoredInDB");
            Assert.IsFalse(repository.Dirty, "repository is dirty");
			restaurantService.Verify(r => r.SendEventsAsync(It.IsAny<ICheck> (), It.IsAny<IEnumerable<ICheckEvent>>()), Times.Once());
            dal.Verify(d => d.AddEventsThatFailedToSend(It.IsAny<IEnumerable<IEntityEventBase>>()), Times.Once);
            dal.Verify(d => d.StoreEventsAsync(It.IsAny<IEnumerable<ICheckEvent>>()), Times.Never);
            dal.Verify(d => d.UpsertEntitiesAsync(It.IsAny<IEnumerable<IEntityBase>>()), Times.Once());
	    }

        [Test]
        public void CommitDoesntStoreEventsInDALWhenAbleToSend()
        {
            var checkID = Guid.NewGuid();

            var restaurantService = new Mock<IRestaurantTerminalService>();
			restaurantService.Setup(rs => rs.SendEventsAsync(It.IsAny<ICheck> (), It.IsAny<IEnumerable<ICheckEvent>>()))
                .Returns(Task<bool>.Factory.StartNew(() => true));


            var dal = new Mock<IClientDAL>();
            dal.Setup(d => d.StoreEventsAsync(It.IsAny<IEnumerable<ICheckEvent>>())).Returns(Task.Factory.StartNew(() => true));
            dal.Setup(d => d.UpsertEntitiesAsync(It.IsAny<IEnumerable<IRestaurantEntityBase>>()))
                .Returns(Task.Factory.StartNew(() => true));

            var logger = new Mock<ILogger>();

            var repository = new ClientCheckRepository(restaurantService.Object, dal.Object, logger.Object, MockingTools.GetResendEventsService().Object);

            //ACT
            repository.ApplyEvent(new CheckCreatedEvent
            {
                ID = Guid.NewGuid(),
                CheckID = checkID,
                EventOrderingID = new EventID
                {
                    AppInstanceCode = MiseAppTypes.UnitTests,
                    OrderingID = 1
                }
            });

            var res = repository.Commit(checkID).Result;

            //ASSERT
            Assert.AreEqual(CommitResult.SentToServer, res, "res is SentToServer");
            Assert.IsFalse(repository.Dirty, "repository is dirty");
			restaurantService.Verify(r => r.SendEventsAsync(It.IsAny<ICheck> (), It.IsAny<IEnumerable<ICheckEvent>>()), Times.Once());
            dal.Verify(d => d.StoreEventsAsync(It.IsAny<IEnumerable<ICheckEvent>>()), Times.Never());
            dal.Verify(d => d.UpsertEntitiesAsync(It.IsAny<IEnumerable<IEntityBase>>()), Times.Once());
        }
	}
}

