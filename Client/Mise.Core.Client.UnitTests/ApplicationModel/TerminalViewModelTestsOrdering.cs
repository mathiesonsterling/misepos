using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Client.Repositories;
using Mise.Core.Client.UnitTests.Tools;
using Mise.Core.Common.Entities;
using Mise.Core.Common.Events;
using Mise.Core.Common.Events.Checks;
using Mise.Core.Common.UnitTests.Tools;
using Mise.Core.Entities;
using Mise.Core.Entities.Check;
using Mise.Core.Entities.Check.Events;
using Mise.Core.Entities.Menu;
using Mise.Core.Entities.People;
using Mise.Core.Entities.People.Events;
using Mise.Core.Repositories;
using Mise.Core.Services;
using Mise.Core.ValueItems;
using Mise.Core.Services.UtilityServices;
using Moq;
using NUnit.Framework;

namespace Mise.Core.Client.UnitTests.ApplicationModel
{
	[TestFixture]
	public class TerminalApplicationModelTestsOrdering
	{
        [TestCase(true)]
        [TestCase(false)]
		[Test]
		public async Task SendCheckPrintsDupesWhenTerminalIsSetTo(bool printDupes)
		{
            var cat = new Mock<MenuItemCategory>();
            var menu = new Mock<Menu>();
            menu.Setup(m => m.GetMenuItemCategories()).Returns(new List<MenuItemCategory> {
				cat.Object
			});
		    var service = MockingTools.GetTerminalService(printDupes);

			service.Setup(s => s.GetMenusAsync()).Returns(Task<IEnumerable<Menu>>.Factory.StartNew(() => new[]{menu.Object}));

		    service.Setup(s => s.GetEmployeesAsync())
		        .Returns(Task<IEnumerable<Employee>>.Factory.StartNew(() => new List<Employee>()));

			var sentDest = new List<OrderDestination>();
			var sentOI = new List<OrderItem >();
			service.Setup (s => s.SendOrderItemsToDestination (It.IsAny<OrderDestination>(), It.IsAny<OrderItem >()))
				.Callback<OrderDestination, OrderItem > ((dest, oi) => {sentDest.Add(dest);sentOI.Add(oi);});
			var orderItem = new OrderItem {
				Status = OrderItemStatus.Added
			};
            orderItem.AddDestination(new OrderDestination{Name="Kitchen"});
            orderItem.AddDestination(new OrderDestination{Name="Expiditer"});

            var fakeTab1 = new RestaurantCheck
            {
                OrderItems = new List<OrderItem> {orderItem},
                ID = Guid.NewGuid()
            };


		    service.Setup(s => s.GetChecksAsync())
		        .Returns(Task<IEnumerable<RestaurantCheck>>.Factory.StartNew(() => new[] {fakeTab1}));
		    

			service.Setup (s => s.SendEventsAsync (It.IsAny<RestaurantCheck> (), It.IsAny<IEnumerable<ICheckEvent>> ()))
				.Returns (Task.Factory.StartNew (()=> true));
			service.Setup(s => s.SendEventsAsync(It.IsAny<RestaurantCheck> (), It.IsAny<IEnumerable<ICheckEvent>>()))
		        .Returns(Task.Factory.StartNew(() => true));

			var logger = new Mock<ILogger> ();

            var checkRepos = new ClientCheckRepository(service.Object, logger.Object, MockingTools.GetResendEventsService().Object);
			await checkRepos.Load(MockingTools.RestaurantID);

            var empRepos = new ClientEmployeeRepository(service.Object, logger.Object, MockingTools.GetResendEventsService().Object);
            await empRepos.Load(MockingTools.RestaurantID);

			var empID = Guid.NewGuid ();
			var emp = new Employee {
				Name = PersonName.TestName,
				ID = empID
			};

		    var printerService = new Mock<IPrinterService>();
		    printerService.Setup(ps => ps.PrintDupeAsync(It.IsAny<ICheck>()))
                .Returns(Task.Factory.StartNew(() => true));

			var vm = ViewModelMockingTools.CreateViewModel (emp, checkRepos, empRepos, new Mock<ICashDrawerService>().Object, service.Object, printerService.Object);
			vm.SelectedEmployee = emp;

		    Assert.AreEqual(OrderItemStatus.Added, orderItem.Status, "Status starts at Added");

			//ACT
			vm.CheckClicked (fakeTab1);
			vm.SelectOrderItemToModify (orderItem);
			vm.SendSelectedCheck ();

			//ASSERT
            if (printDupes)
            {
                printerService.Verify(ps => ps.PrintDupeAsync(It.IsAny<ICheck>()), Times.Once());
            }
            else
            {
                printerService.Verify(ps => ps.PrintDupeAsync(It.IsAny<ICheck>()),Times.Never());
            }
		}

		[Test]
		public async Task ClosingACheckCreatesCorrectEvents()
		{
            var orderItem = new OrderItem
            {
                Status = OrderItemStatus.Added,
                MenuItem = new MenuItem { Price = new Money(10.0M) }
            };
            orderItem.AddDestination(new OrderDestination{Name="Kitchen"});
            orderItem.AddDestination(new OrderDestination{Name="Expiditer"});

            var checkID = Guid.NewGuid();
            var tab = new RestaurantCheck
            {
                ID = checkID,
                OrderItems = new List<OrderItem > { orderItem },
                Customer = new Customer { Name = PersonName.TestName }
            };

            var service = MockingTools.GetTerminalServiceWithChecks(new List<RestaurantCheck>{
				tab
			});

            var sentDest = new List<OrderDestination>();
            var sentOI = new List<OrderItem >();
            service.Setup(s => s.SendOrderItemsToDestination(It.IsAny<OrderDestination>(), It.IsAny<OrderItem >()))
                        .Callback<OrderDestination, OrderItem >((dest, oi) => { sentDest.Add(dest); sentOI.Add(oi); });
			var logger = new Mock<ILogger> ();


            var checkRepos = new ClientCheckRepository(service.Object,  logger.Object, MockingTools.GetResendEventsService().Object);
			await checkRepos.Load(MockingTools.RestaurantID);

            var empRepos = new ClientEmployeeRepository(service.Object, logger.Object, MockingTools.GetResendEventsService().Object);
		    await empRepos.Load(MockingTools.RestaurantID);

			var emp = new Employee {
				Name = PersonName.TestName,
				ID = Guid.NewGuid ()
			};

			var vm = ViewModelMockingTools.CreateViewModel (emp, checkRepos, empRepos, new Mock<ICashDrawerService>().Object,service.Object);


		    Assert.AreEqual(OrderItemStatus.Added, orderItem.Status, "Status starts at Added");

			//ACT
			vm.CheckClicked (tab);
			vm.SelectOrderItemToModify (orderItem);
			vm.CloseOrderButtonClicked ();

			Assert.AreEqual (CheckPaymentStatus.Closing, vm.SelectedCheck.PaymentStatus);

			//ASSERT
			Assert.AreEqual(OrderItemStatus.Sent, orderItem.Status);

		}

		[Test]
		public async Task DeleteAddedOrderItem()
		{
		    var service = MockingTools.GetTerminalServiceWithMenu();


		    var oiID = Guid.NewGuid();
		    var checkID = Guid.NewGuid();
			var oi = new OrderItem{ ID = oiID, Status = OrderItemStatus.Added, MenuItem = new MenuItem{ID = Guid.NewGuid()} };
			var tab = new RestaurantCheck {ID = checkID, OrderItems = new List<OrderItem > {oi}};

			var emp = new Employee{ Passcode = "1111", CurrentlyClockedInToPOS = false };
			/*service.Setup (s => s.GetEmployees ()).Returns (new List<IEmployee>{
				emp
			});*/
		    service.Setup(s => s.GetEmployeesAsync()).Returns(Task<IEnumerable<Employee>>.Factory.StartNew(() => new[] {emp}));

		    service.Setup(s => s.GetChecksAsync()).Returns(Task<IEnumerable<RestaurantCheck>>.Factory.StartNew(() => new[] {tab}));
		    
			var logger = new Mock<ILogger> ();

			var empRepos = new Mock<IEmployeeRepository> ();
			empRepos.Setup (er => er.GetAll ()).Returns (new List<IEmployee>{emp});

            var checkRepos = new ClientCheckRepository(service.Object, logger.Object, MockingTools.GetResendEventsService().Object);
			await checkRepos.Load(MockingTools.RestaurantID);

			var vm = ViewModelMockingTools.CreateViewModel (emp, checkRepos, empRepos.Object);

			//ACT
			vm.CheckClicked (tab);
			vm.SelectOrderItemToModify(oi);
			Assert.AreSame (oi, vm.SelectedOrderItem);

			vm.DeleteSelectedOrderItem ();

			//ASSERT
			Assert.IsNull (vm.SelectedOrderItem);
			Assert.AreEqual (0, tab.OrderItems.Count ());

            service.Verify(s => s.GetEmployeesAsync(), Times.Never);
            service.Verify(s => s.GetChecksAsync(), Times.Once, "GetOpenTabs called correctly");
		}

		[Test]
		public void DeleteAddedOrderItemAddsEvent()
		{
		    var service = MockingTools.GetTerminalServiceWithMenu();

			var emp = new Employee{ Passcode = "1111", CurrentlyClockedInToPOS = false };
			service.Setup (s => s.GetEmployeesAsync ()).Returns (Task.Factory.StartNew (() => new List<Employee>{
				emp}.AsEnumerable ()));
				
			var empRepos = new Mock<IEmployeeRepository> ();
			empRepos.Setup (er => er.GetAll ()).Returns (new List<IEmployee>{emp});

		    var oiID = Guid.NewGuid();
		    var checkID = Guid.NewGuid();
		    var miID = Guid.NewGuid();
			var oi = new OrderItem{ ID = oiID, Status = OrderItemStatus.Added, MenuItem = new MenuItem{ID = miID} };
			var tab = new RestaurantCheck {ID = checkID, OrderItems = new List<OrderItem > {oi}};
		    var checkRepos = new Mock<ICheckRepository> ();
			checkRepos.Setup (cr => cr.GetOpenChecks (null)).Returns (new List<ICheck>{ tab });
			checkRepos.Setup (cr => cr.ApplyEvent (It.IsAny<ICheckEvent> ())).Returns (tab);

			ICheckEvent checkEventSent = null;
			checkRepos.Setup (cr => cr.ApplyEvent (It.IsAny<ICheckEvent> ()))
				.Returns(tab)
				.Callback<ICheckEvent>(evs => checkEventSent = evs);
			var vm = ViewModelMockingTools.CreateViewModel (emp, checkRepos.Object, empRepos.Object);

			//ACT
		    vm.CheckClicked (tab);
			vm.SelectOrderItemToModify(oi);
			Assert.AreSame (oi, vm.SelectedOrderItem);

			vm.DeleteSelectedOrderItem ();

			//ASSERT
			Assert.IsNotNull (checkEventSent);
			var delEvent = checkEventSent as OrderItemDeletedEvent;
            Assert.IsNotNull(delEvent);
			Assert.AreEqual (checkID, delEvent.CheckID);
			Assert.AreEqual (oiID, delEvent.OrderItemID);
		}

		[Test]
		public void VoidSentItem()
		{
		    var service = MockingTools.GetTerminalServiceWithMenu();

		    var serverID = Guid.NewGuid();
		    var mgrID = Guid.NewGuid();
			var emp = new Employee{ID=serverID, Passcode = "1111", CurrentlyClockedInToPOS = false };
			var manager = new Employee{ID = mgrID, Passcode = "2222", CurrentlyClockedInToPOS = false, WhenICanVoid = new List<OrderItemStatus>{OrderItemStatus.Added, OrderItemStatus.Sent}};
			service.Setup (s => s.GetEmployeesAsync ()).Returns (Task.Factory.StartNew (() => new List<Employee>{emp}.AsEnumerable ()));
			service.Setup (s => s.NotifyDestinationOfVoid(It.IsAny<OrderDestination>(), It.IsAny<OrderItem >()));

			var empRepos = new Mock<IEmployeeRepository> ();
			empRepos.Setup (er => er.GetAll ()).Returns (new List<IEmployee>{emp, manager});

            var oiID = Guid.NewGuid();
		    var checkID = Guid.NewGuid();
			var oi = new OrderItem{ ID = oiID, Status = OrderItemStatus.Sent, PlacedByID = emp.ID}; 
			var tab = new RestaurantCheck {ID = checkID, OrderItems = new List<OrderItem > {oi}};

		    ICheckEvent checkEventsSent = null;
			var checkRepos = new Mock<ICheckRepository> ();
			checkRepos.Setup (cr => cr.GetOpenChecks (null)).Returns (new List<ICheck>{ tab });
			checkRepos.Setup (cr => cr.ApplyEvent (It.IsAny<ICheckEvent> ()))
				.Returns (tab)
				.Callback<ICheckEvent>(evs => checkEventsSent = evs);

			var vm = ViewModelMockingTools.CreateViewModel (emp, checkRepos.Object, empRepos.Object);

			//ACT
		    vm.CheckClicked (tab);
			vm.SelectOrderItemToModify(oi);
			Assert.AreSame (oi, vm.SelectedOrderItem);

			vm.VoidSelectedOrderItem ("2222", "for a test", false);

			//ASSERT
			Assert.IsNull (vm.SelectedOrderItem);
			Assert.AreEqual (0, tab.OrderItems.Count ());

			//check our event got added to the repos
			Assert.IsNotNull (checkEventsSent);
			var eventSent = checkEventsSent as OrderItemVoidedEvent;
			Assert.IsNotNull (eventSent, "Event is OrderItem VoidedEvent");
			Assert.AreEqual (MiseEventTypes.OrderItemVoided, eventSent.EventType, "has correct eventType");
			Assert.AreEqual(checkID, eventSent.CheckID);

			var eventOI = eventSent.OrderItemToVoid;
			Assert.IsNotNull (eventOI, "has order item on event");
			Assert.AreEqual (oiID, eventOI.ID);
			Assert.AreEqual(OrderItemStatus.Sent, eventOI.Status);
		}

		[Test]
		public void VoidSentItemFailsWithoutManagerPassword()
		{
		    var service = MockingTools.GetTerminalServiceWithMenu();

			var emp = new Employee{ Passcode = "1111", CurrentlyClockedInToPOS = false, WhenICanVoid = new List<OrderItemStatus>() };
			var manager = new Employee{Passcode = "2222", CurrentlyClockedInToPOS = false, WhenICanVoid = new List<OrderItemStatus>{OrderItemStatus.Added, OrderItemStatus.Sent}};
			service.Setup (s => s.GetEmployeesAsync ()).Returns (Task.Factory.StartNew (() => new List<Employee>{
				emp
			}.AsEnumerable ()));
				
			var empRepos = new Mock<IEmployeeRepository> ();
			empRepos.Setup (er => er.GetAll ()).Returns (new List<IEmployee>{emp, manager});
			IList<IEmployeeEvent> eventsSent = null;
			empRepos.Setup(er => er.ApplyEvents(It.IsAny<IEnumerable<IEmployeeEvent>>())).Callback<IEnumerable<IEmployeeEvent>>(l => eventsSent = l.ToList());


            var oiID = Guid.NewGuid();
            var checkID = Guid.NewGuid();
			var oi = new OrderItem{ ID = oiID, Status = OrderItemStatus.Sent };
			var tab = new RestaurantCheck {ID = checkID, OrderItems = new List<OrderItem > {oi}};
		    var checkRepos = new Mock<ICheckRepository> ();
			checkRepos.Setup (cr => cr.GetOpenChecks (null)).Returns (new List<ICheck>{ tab });

			var vm = ViewModelMockingTools.CreateViewModel (emp, checkRepos.Object, empRepos.Object);

			//ACT
		    vm.CheckClicked (tab);
			vm.SelectOrderItemToModify(oi);
			Assert.AreSame (oi, vm.SelectedOrderItem);

			vm.VoidSelectedOrderItem ("1111", "for a test", false);

			//ASSERT
			Assert.IsNotNull(vm.SelectedOrderItem);
			Assert.AreEqual (1, tab.OrderItems.Count ());
			//check our failed login event got added to the repos
			Assert.IsNotNull(eventsSent, "Events sent to employee repository");
			Assert.AreEqual(1, eventsSent.Count(), "one event exists");

			var evSent = eventsSent.First() as InsufficientPermissionsEvent;
			Assert.IsNotNull (evSent, "Event as InsufficientPermissions is null");
			Assert.AreEqual("void", evSent.FunctionAttempted);
			Assert.IsNull(evSent.DeviceID);
		}

        //[Ignore("Still flaky, investigate why!")]
		[Test]
		public async Task CancelCheckDumpsAllUnsentEvents()
		{

		    var miID = Guid.NewGuid();
			var menuItem = new MenuItem{ID=miID, ButtonName = "test"};
		

			var emp = new Employee {
				Name = PersonName.TestName,
				ID = Guid.NewGuid ()
			};
			var vm = (await ViewModelMockingTools.CreateViewModel (emp)).Item1;


		    //ACT
            var check = vm.CreateNewCheck(PersonName.TestName);
			vm.SendSelectedCheck ();

			Assert.IsNotNull (check);
			//TODO add order items to existing check, then cancel
			vm.CheckClicked (check);
			vm.DrinkClicked (menuItem);

			//check our tabs are in here!
			vm.CancelOrdering ();

			//assert tab is null
			Assert.IsNull (vm.SelectedCheck, "Selected tab set to null");

			var checks = vm.OpenChecks.ToList();
			Assert.IsNotNull (checks, "checks not null");
			Assert.AreEqual (0, checks.Count (), "no check was created");
		}

		[Test]
		public async Task CancelCheckOnCreateHasTabNotCreated()
		{
			var emp = new Employee {
				Name = PersonName.TestName,
				ID = Guid.NewGuid ()
			};
			var vm = (await ViewModelMockingTools.CreateViewModel (emp)).Item1;


		    //ACT
            var check = vm.CreateNewCheck(PersonName.TestName);

			Assert.IsNotNull (check);

			vm.CancelOrdering ();

			//assert tab is null
			Assert.IsNull (vm.SelectedCheck, "Selected tab set to null");

			var checks = vm.OpenChecks;
			Assert.IsNotNull (checks, "checks not null");
			Assert.AreEqual (0, checks.Count ());

		}

		[Test]
		public async Task CancelCheckMarksCategoryAsUnSelected()
		{
			var emp = new Employee {
				Name = PersonName.TestName,
				ID = Guid.NewGuid ()
			};
			var vm = (await ViewModelMockingTools.CreateViewModel (emp)).Item1;
			vm.SelectedCategory = new MenuItemCategory{
				Name = "TestCategory"
			};

			//ACT
			vm.CancelOrdering ();

			//ASSERT
			Assert.IsNull (vm.SelectedCategory, "Selected category set to null");

		}

		[Test]
		public async Task SendingACheckMarksAllOrderItems()
		{
			var orderItem = new OrderItem {Status = OrderItemStatus.Added};
		    var barTab = new RestaurantCheck
		    {
                ID = Guid.Empty,
		        OrderItems = new List<OrderItem> {orderItem}
		    };
				
			var emp = new Employee {
				Name = PersonName.TestName,
				ID = Guid.NewGuid ()
			};

			var service = MockingTools.GetTerminalServiceWithMenu();

		    service.Setup(s => s.GetChecksAsync())
		        .Returns(Task<IEnumerable<RestaurantCheck>>.Factory.StartNew(() => new[] {barTab}));
				

			var logger = new Mock<ILogger> ();
			var checkRepos = new ClientCheckRepository (service.Object, logger.Object, MockingTools.GetResendEventsService().Object);
			await checkRepos.Load(MockingTools.RestaurantID);

            var empRepos = new ClientEmployeeRepository(service.Object, logger.Object, MockingTools.GetResendEventsService().Object);
			await empRepos.Load(MockingTools.RestaurantID);

			var vm = ViewModelMockingTools.CreateViewModel (emp, checkRepos, empRepos, null, service.Object);

		    Assert.AreEqual(OrderItemStatus.Added, orderItem.Status, "Status starts at Added");

			//ACT
			vm.CheckClicked (barTab);
			vm.SelectOrderItemToModify (orderItem);
			vm.SendSelectedCheck ();

			//ASSERT
			Assert.AreEqual (OrderItemStatus.Sent, orderItem.Status, "Status changed to Sent");

			Assert.IsNull (vm.SelectedCategory);
			Assert.IsNull (vm.SelectedCategoryParent);
		}

        [Test]
        public async Task ItemCompReducesEmployeeCompBudget()
        {
            var emp = new Employee
            {
                ID = Guid.Empty,
                CompBudget = new Money(20.0M)
            };
            var items = await ViewModelMockingTools.CreateViewModel(emp);
            var vm = items.Item1;
            

            var tab = vm.CreateNewCheck(PersonName.TestName);

            Assert.IsNotNull(tab, "Tab created");
            Assert.AreEqual(CheckPaymentStatus.Open, vm.SelectedCheck.PaymentStatus);

            vm.DrinkClicked(
                new MenuItem
                {
                    ID = Guid.NewGuid(),
                    Name = "TestItem",
                    Price = new Money(10.0M)
                }
            );
            vm.SendSelectedCheck();

            //ACT
            vm.CheckClicked(tab);

            //add a cash payment
            vm.SelectOrderItemToModify(vm.SelectedCheck.OrderItems.First());
            var paid = vm.CompSelectedItem();
            Assert.True(paid, "Paid");

            Assert.AreEqual(1, vm.SelectedCheck.GetPayments().Count(), "has payment");

            //check that our comp amount is updated
            Assert.AreEqual(10.0M, emp.CompBudget.Dollars, "Comp amount was decremented");
        }
	}
}

