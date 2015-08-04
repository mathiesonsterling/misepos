using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Client.ApplicationModel.Implementation;
using Mise.Core.Common.Events;
using Mise.Core.Common.Events.Employee;
using Mise.Core.Entities;
using Mise.Core.Entities.Menu;
using Mise.Core.Entities.People;
using Mise.Core.Entities.Restaurant;
using Mise.Core.Repositories;
using Mise.Core.Common.Entities;
using Mise.Core.Entities.Check;
using Mise.Core.Entities.People.Events;
using Mise.Core.Services;
using Mise.Core.Client.ApplicationModel;
using Mise.Core.ValueItems;
using NUnit.Framework;
using Mise.Core.Services.UtilityServices;
using Moq;
using Mise.Core.Client.UnitTests.Tools;
using Mise.Core.Client.Services;
using Mise.Core.Common.Services.WebServices;


namespace Mise.Core.Client.UnitTests.ApplicationModel
{
    [Ignore]
	[TestFixture]
	public class TerminalApplicationModelTests
	{
        [Test]
        public void GuidForTopCatLoadsCorrectCat()
        {
            var uow = new Mock<IUnitOfWork>();
            var webService = new Mock<IRestaurantTerminalService>();
            var logger = new Mock<ILogger>();
            var cashDrawer = new Mock<ICashDrawerService>();
            var creditHardware = new Mock<ICreditCardReaderService>();
            var creditProcesser = new Mock<ICreditCardProcessorService>();
            var salesTax = new Mock<ISalesTaxCalculatorService>();

            var checkRepos = new Mock<ICheckRepository>();
            var employeeRepos = new Mock<IEmployeeRepository>();
            var menuRepos = new Mock<IMenuRepository>();

            var catID = Guid.NewGuid();
            menuRepos.Setup(mr => mr.GetCurrentMenu())
                .Returns(new Menu
                {
                    Categories = new List<MenuItemCategory>
                    {
                        new MenuItemCategory
                        {
                            ID = Guid.NewGuid(),
                            Name= "first"
                        },
                        new MenuItemCategory
                        {
                            ID = catID,
                            Name = "second"
                        }
                    }
                });
            var devSettings = new Mock<IMiseTerminalDevice>();
            devSettings.Setup(d => d.TopLevelCategoryID).Returns(catID);

            var restaurant = new Mock<IRestaurant>();

            //ACT
            var vt = new TerminalApplicationModel(uow.Object, webService.Object, logger.Object, cashDrawer.Object,
                creditHardware.Object, creditProcesser.Object,
                salesTax.Object, null, checkRepos.Object, employeeRepos.Object, menuRepos.Object, devSettings.Object,
                restaurant.Object);

            //assert it went over
            Assert.IsNotNull(vt.TopCat);
            Assert.AreEqual("second", vt.TopCat.Name);
        }

	    [Test]
	    public void EmptyGuidForTopCatLoadsFirstCatGiven()
	    {
	        var uow = new Mock<IUnitOfWork>();
	        var webService = new Mock<IRestaurantTerminalService>();
	        var logger = new Mock<ILogger>();
	        var cashDrawer = new Mock<ICashDrawerService>();
	        var creditHardware = new Mock<ICreditCardReaderService>();
	        var creditProcesser = new Mock<ICreditCardProcessorService>();
	        var salesTax = new Mock<ISalesTaxCalculatorService>();

	        var checkRepos = new Mock<ICheckRepository>();
	        var employeeRepos = new Mock<IEmployeeRepository>();
	        var menuRepos = new Mock<IMenuRepository>();
	        menuRepos.Setup(mr => mr.GetCurrentMenu())
	            .Returns(new Menu
	            {
                    Categories = new List<MenuItemCategory>
                    {
                        new MenuItemCategory
                        {
                            ID = Guid.NewGuid(),
                            Name= "first"
                        },
                        new MenuItemCategory
                        {
                            ID = Guid.NewGuid(),
                            Name = "second"
                        }
                    }
	            });
	        var devSettings = new Mock<IMiseTerminalDevice>();
	        devSettings.Setup(d => d.TopLevelCategoryID).Returns(new Guid(Guid.Empty.ToString()));

	        var restaurant = new Mock<IRestaurant>();

            //ACT
	        var vt = new TerminalApplicationModel(uow.Object, webService.Object, logger.Object, cashDrawer.Object,
	            creditHardware.Object, creditProcesser.Object,
	            salesTax.Object, null, checkRepos.Object, employeeRepos.Object, menuRepos.Object, devSettings.Object,
	            restaurant.Object);

            //assert it went over
            Assert.IsNotNull(vt.TopCat);
            Assert.AreEqual("first", vt.TopCat.Name);
	    }

		[Test]
		public async Task SetCustomerSetsNameCorrectly()
		{
			var empID = Guid.NewGuid ();
			var emp = new Employee{ Name = PersonName.TestName, ID = empID };

			var vm = (await ViewModelMockingTools.CreateViewModel (emp)).Item1;
			vm.SelectedEmployee = emp;

			//ACT
			var check = vm.CreateNewCheck (PersonName.TestName);

			Assert.IsNotNull (check);
			Assert.AreEqual (check, vm.SelectedCheck, "created tab was set as selected");
			Assert.IsNotNull (check.Customer, "customer assigned");
			Assert.AreEqual("test ", check.GetTopOfCheck(), "top of check");
		}

		[Test]
		public async Task CreateNewTabCreatesWithServerID(){
			var empID = Guid.NewGuid ();
			var emp = new Employee{ Name = PersonName.TestName, ID = empID };
			var vm = (await ViewModelMockingTools.CreateViewModel (emp)).Item1;
			vm.SelectedEmployee = emp;

			//ACT
			var check = vm.CreateNewCheck (PersonName.TestName);

			Assert.IsNotNull (check);
			Assert.AreEqual (check, vm.SelectedCheck, "created tab was set as selected");
			Assert.AreEqual (empID, check.CreatedByServerID);
			Assert.AreEqual (empID, check.LastTouchedServerID);
		}

        [Test]
        public async Task CreateNewTabWithCreditCardPutsCardOnCheck()
        {
            var empID = Guid.NewGuid();
            var emp = new Employee { Name = PersonName.TestName, ID = empID };
            var vm = (await ViewModelMockingTools.CreateViewModel(emp)).Item1;
            vm.SelectedEmployee = emp;

            var creditCard = new CreditCard
            {
                Name = PersonName.TestName,
                ExpMonth = 11,
                ExpYear = 2014
            };
            //ACT
            var check = vm.CreateNewCheck(creditCard);

            Assert.IsNotNull(check);
            Assert.AreEqual(check, vm.SelectedCheck, "created tab was set as selected");
            Assert.AreEqual(empID, check.CreatedByServerID);
            Assert.AreEqual(empID, check.LastTouchedServerID);

            Assert.AreEqual(1, check.CreditCards.Count());
            var cc = check.CreditCards.FirstOrDefault();
            Assert.IsNotNull(cc);
        }

	    [Test]
		public async Task AddCheckAndRetrieve()
		{
			var empID = Guid.NewGuid ();
            var emp = new Employee { Name = PersonName.TestName, ID = empID };
			var vm = (await ViewModelMockingTools.CreateViewModel (emp)).Item1;

			//ACT
			var check = vm.CreateNewCheck (PersonName.TestName);

			Assert.IsNotNull (check);
			Assert.AreEqual (check, vm.SelectedCheck, "created tab was set as selected");
			Assert.AreEqual (PersonName.TestName.ToSingleString(), check.GetTopOfCheck());
			Assert.AreEqual (empID, check.CreatedByServerID);

			vm.SendSelectedCheck ();

			var tabs = vm.OpenChecks.ToList();
			Assert.IsNotNull (tabs);
			Assert.AreEqual (1, tabs.Count());
		}



		[Test]
		public async Task AddTwoTabsAndCheckTheyAreStored()
		{
		    var emp = new Employee {
		        Name = PersonName.TestName,
		        ID = Guid.Empty
		    };
		    var vm = (await ViewModelMockingTools.CreateViewModel(emp)).Item1;
		    vm.SelectedEmployee = emp;

			//ACT
			var check = vm.CreateNewCheck (PersonName.TestName);

			Assert.IsNotNull (check);
			Assert.AreEqual (check, vm.SelectedCheck, "created tab was set as selected");
			Assert.AreEqual ("test ", check.GetTopOfCheck());
			Assert.AreEqual (Guid.Empty, check.CreatedByServerID);
			vm.SendSelectedCheck ();

			var check2 = vm.CreateNewCheck (new PersonName("another", "name"));
			Assert.IsNotNull (check2);
			Assert.AreEqual (check2, vm.SelectedCheck, "second set as selected");
			Assert.AreEqual ("another ", check2.GetTopOfCheck());
			Assert.AreEqual (Guid.Empty, check2.CreatedByServerID);
			vm.SendSelectedCheck();

			var got = vm.OpenChecks;
			Assert.IsNotNull (got);
			Assert.AreEqual (2, got.Count ());
		}




		[Test]
		public async Task ClockinAndClockout()
		{
			var emp = new Employee{ Passcode = "1111", CurrentlyClockedInToPOS = false };

			var vm = (await ViewModelMockingTools.CreateViewModel (emp)).Item1;
			Assert.IsFalse (emp.CurrentlyClockedInToPOS);

			//ACT
			Assert.AreEqual (emp, vm.SelectedEmployee);
			var clockinRes = vm.EmployeeClockin ("1111");

			//ASSERT
			Assert.IsTrue (clockinRes);
			Assert.IsTrue (emp.CurrentlyClockedInToPOS, "Clocked in");

			//ACT AGAIN
			var clockoutRes = vm.EmployeeClockout ("1111", vm.SelectedEmployee);

			Assert.True (clockoutRes);
			Assert.IsFalse (emp.CurrentlyClockedInToPOS, "Clocked out");
		}

        /// <summary>
        /// POS-102
        /// </summary>
        [Test]
        public async Task ClockinIsNotResetByCancel()
        {
            var emp = new Employee { Passcode = "1111", CurrentlyClockedInToPOS = false };

            var vm = (await ViewModelMockingTools.CreateViewModel(emp)).Item1;
            Assert.IsFalse(emp.CurrentlyClockedInToPOS);

            Assert.AreEqual(emp, vm.SelectedEmployee);
            var clockinRes = vm.EmployeeClockin("1111");

            Assert.IsTrue(clockinRes);
            Assert.IsTrue(emp.CurrentlyClockedInToPOS, "Clocked in");

            //we'll want to fire a cancel on the repos
            var check = vm.CreateNewCheck(PersonName.TestName);
            Assert.IsNotNull(check);

            vm.CancelOrdering();

            //ASSERT
            Assert.IsNotNull(vm.SelectedEmployee);
            Assert.IsTrue(vm.SelectedEmployee.CurrentlyClockedInToPOS);
        }

		[Test]
		public void TestNoSale(){
			var empID = Guid.NewGuid ();
			var emp = new Employee{ Passcode = "1111", CurrentlyClockedInToPOS = false, ID=empID};

			var checkRepos = new Mock<ICheckRepository> ();
			checkRepos.Setup (cr => cr.GetOpenChecks (null)).Returns (new List<ICheck>());

			var empRepos = new Mock<IEmployeeRepository> ();
		    empRepos.Setup(er => er.GetByPasscode("1111")).Returns(emp);

			IList<IEmployeeEvent> givenEvents = null;
			empRepos.Setup (er => er.ApplyEvents (It.IsAny<IEnumerable<IEmployeeEvent>> ()))
				.Callback<IEnumerable<IEmployeeEvent>> (evs => givenEvents = evs.ToList ());

			var vm = ViewModelMockingTools.CreateViewModel (emp, checkRepos.Object, empRepos.Object);

			//ACT
			var res = vm.NoSale ("1111");

			//ASSERT
			Assert.IsTrue (res);
			Assert.AreEqual (vm.CurrentTerminalViewTypeToDisplay, TerminalViewTypes.NoSale);
			Assert.IsNotNull (givenEvents);
			Assert.AreEqual (1, givenEvents.Count ());

			var firstEvent = givenEvents.First () as NoSaleEvent;
			Assert.IsNotNull (firstEvent);
			Assert.IsNotNull (firstEvent.CreatedDate);
			Assert.AreEqual(empID, firstEvent.EmployeeID);

            empRepos.Verify(er => er.GetAll(), Times.Never);
            empRepos.Verify(er => er.GetByPasscode("1111"), Times.Once);
		}

		[Test]
		public void TestNoSaleFailsWithBadLogin(){
			var emp = new Employee{ Passcode = "1111", CurrentlyClockedInToPOS = false };

			var cashDrawerService = new Mock<ICashDrawerService> ();

			var checkRepos = new Mock<ICheckRepository> ();
			checkRepos.Setup (cr => cr.GetOpenChecks (null)).Returns (new List<ICheck>());
			var empRepos = new Mock<IEmployeeRepository> ();
			empRepos.Setup (er => er.GetAll ()).Returns (new List<IEmployee>{emp});
			IList<IEmployeeEvent> givenEvents = null;
			empRepos.Setup (er => er.ApplyEvents (It.IsAny<IEnumerable<IEmployeeEvent>> ()))
				.Callback<IEnumerable<IEmployeeEvent>> (evs => givenEvents = evs.ToList ());

			var vm = ViewModelMockingTools.CreateViewModel (emp, checkRepos.Object, empRepos.Object, cashDrawerService.Object);

			//ACT
			var res = vm.NoSale ("1929");

			//ASSERT
			Assert.IsFalse (res);
			cashDrawerService.Verify (m => m.OpenDrawer (), Times.Never (), "Open drawer was called");

			Assert.IsNotNull (givenEvents);
			Assert.AreEqual (1, givenEvents.Count ());

			var firstEvent = givenEvents.First () as BadLoginAttemptEvent;
			Assert.IsNotNull (firstEvent);
			Assert.IsNotNull (firstEvent.CreatedDate);
			Assert.AreEqual("NoSale", firstEvent.FunctionAttempted);
			Assert.AreEqual ("1929", firstEvent.PasscodeGiven);
		}
	}
}

