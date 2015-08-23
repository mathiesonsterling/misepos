using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Entities;
using Mise.Core.Common.Events.Checks;
using Mise.Core.Entities.Check.Events;
using Mise.Core.Entities.People;
using Mise.Core.ValueItems;
using NUnit.Framework;
using Mise.Core.Common.UnitTests.Tools;
using Moq;
using Mise.Core.Entities.Check;
using Mise.Core.Services.UtilityServices;
using Mise.Core.Client.Repositories;

namespace Mise.Core.Client.UnitTests.Repositories
{
	[TestFixture]
	public class ClientCheckRepositoryTests
	{
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



            var repos = new ClientCheckRepository(service.Object, logger.Object);

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
            var repos = new ClientCheckRepository(service.Object, logger.Object);

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
            var repos = new ClientCheckRepository(service.Object, logger.Object);

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

            var repos = new ClientCheckRepository(service.Object, logger.Object);

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

            var repos = new ClientCheckRepository(service.Object, logger.Object);

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

            var repos = new ClientCheckRepository(service.Object, logger.Object);

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

	}
}

