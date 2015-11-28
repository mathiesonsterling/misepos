using System;
using System.Collections.Generic;
using System.Linq;
using Mise.Core.Client.UnitTests.Tools;
using Mise.Core.Client.ApplicationModel;
using Mise.Core.Entities.Menu;
using Mise.Core.Repositories;
using Mise.Core.Services.UtilityServices;
using Mise.Core.Entities.People;
using Mise.Core.Client.Repositories;
using Mise.Core.Common.Entities;
using Mise.Core.Entities.Check;
using Mise.Core.ValueItems;
using Mise.Core.Common.UnitTests.Tools;
using NUnit.Framework;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Mise.Core.Entities.Payments;

namespace Mise.Core.Client.UnitTests.ApplicationModel
{
    [Ignore("Waiting for POS")]
	[TestFixture]
	public class TerminalApplicationModelTestsPayment
	{
        [Test]
        public void ClickingOnClosingCheckGoesToPayment()
        {
            var service = MockingTools.GetTerminalServiceWithMenu();

            var checkID = Guid.NewGuid();
            var closingCheck = new RestaurantCheck
            {
                Id = checkID,
                PaymentStatus = CheckPaymentStatus.Closing,
                Customer = new Customer { Name = PersonName.TestName },
            };
			service.Setup(s => s.GetChecksAsync()).Returns(Task.Factory.StartNew(() =>new List<RestaurantCheck> { closingCheck }.AsEnumerable ()));

            var logger = new Mock<ILogger>();

			var checkRepos = new ClientCheckRepository(service.Object, logger.Object);

            var empRepos = new Mock<IEmployeeRepository>();
            empRepos.Setup(r => r.GetAll()).Returns(new List<IEmployee>());
			empRepos.Setup (r => r.StartTransaction (It.IsAny<Guid> ())).Returns (true);

			var emp = new Employee { Name = new PersonName("testEmp", "last"), Id = Guid.NewGuid () };
			var vm = ViewModelMockingTools.CreateViewModel (emp, checkRepos, empRepos.Object);

            //ACT
            vm.CheckClicked(closingCheck);

            Assert.AreEqual(TerminalViewTypes.PaymentScreen, vm.CurrentTerminalViewTypeToDisplay);
        }

		[Test]
		public async Task ReopenCheck()
		{
            var empID = Guid.NewGuid();
            var emp = new Employee
            {
                Name = PersonName.TestName,
                Id = empID
            };
		    var items = await ViewModelMockingTools.CreateViewModel(emp);
		    var vm = items.Item1;


		    vm.SelectedEmployee = emp;

			//ACT
			vm.CreateNewCheck (PersonName.TestName);
            vm.DrinkClicked(
                new MenuItem
                {
                    Id = Guid.NewGuid(),
                    Name = "TestItem",
                    Price = new Money(10.0M)
                }
            );
			vm.CloseOrderButtonClicked ();

			Assert.AreEqual (CheckPaymentStatus.Closing, vm.SelectedCheck.PaymentStatus);

			var res = vm.ReopenSelectedCheck ();

			Assert.IsNotNull (res);
			Assert.AreEqual (CheckPaymentStatus.Open, res.PaymentStatus);
            Assert.AreEqual(empID, res.LastTouchedServerID);

		}
			

        [Test]
        public async Task CancelPaymentsClearsRepository()
        {
            var emp = new Employee
            {
                Id = Guid.Empty,
                CompBudget = new Money(10.0M)
            };

            var items = await ViewModelMockingTools.CreateViewModel(emp);
            ICheckRepository checkRepos = items.Item2;
            var vm = items.Item1;

            var check = vm.CreateNewCheck(PersonName.TestName);

            Assert.IsNotNull(check, "Check created");
            Assert.AreEqual(CheckPaymentStatus.Open, vm.SelectedCheck.PaymentStatus);

            vm.DrinkClicked(
                new MenuItem
                {
                    Id = Guid.NewGuid(),
                    Name = "TestItem",
                    Price = new Money(10.0M)
                }
                );

            vm.CloseOrderButtonClicked();

            //ACT
            vm.CheckClicked(check);
            Assert.AreEqual(CheckPaymentStatus.Closing, vm.SelectedCheck.PaymentStatus);

            //add a cash payment
            var paid = vm.PayCheckWithAmountComp(new Money(1.0M));
            Assert.IsTrue(paid);
            Assert.AreEqual(1, vm.SelectedCheck.GetPayments().Count(), "has payment");
            vm.CancelPayments();
            Assert.AreEqual(0, vm.SelectedCheck.GetPayments().Count());
            Assert.IsFalse(checkRepos.Dirty);

            vm.CheckClicked(vm.SelectedCheck);
            Assert.AreEqual(CheckPaymentStatus.Closing, vm.SelectedCheck.PaymentStatus);
            //assert our repos is dirty
            Assert.IsTrue(checkRepos.Dirty);
            Assert.AreEqual(0, vm.SelectedCheck.GetPayments().Count());

            //assert the money didn't come out of the comp budget now
            Assert.AreEqual(10.0M, vm.SelectedEmployee.CompBudget.Dollars, "comp budget was not decremented");
        }

		[Test]
		public async Task CancelPaymentsWithoutTotalGoesToClosed()
        {
			var emp = new Employee {
				Id = Guid.Empty,
				CompBudget = new Money (10.0M)
			};

		    var items = await ViewModelMockingTools.CreateViewModel(emp);
		    ICheckRepository checkRepos = items.Item2;
		    var vm = items.Item1;
 
		    var tab = vm.CreateNewCheck (PersonName.TestName);

            Assert.IsNotNull(tab, "Tab created");
            Assert.AreEqual(CheckPaymentStatus.Open, vm.SelectedCheck.PaymentStatus);

            vm.CloseOrderButtonClicked();

			//ACT
			vm.CheckClicked (tab);
            Assert.AreEqual(CheckPaymentStatus.Closed, vm.SelectedCheck.PaymentStatus);

            //add a cash payment
		    var paid = vm.PayCheckWithAmountComp(new Money(1.0M));
            Assert.IsTrue(paid);
            Assert.AreEqual(1, vm.SelectedCheck.GetPayments().Count(), "has payment");
			vm.CancelPayments ();
            Assert.AreEqual(0, vm.SelectedCheck.GetPayments().Count());
			Assert.IsFalse (checkRepos.Dirty);

			vm.CheckClicked (vm.SelectedCheck);
			Assert.AreEqual (CheckPaymentStatus.Closed, vm.SelectedCheck.PaymentStatus);
			//assert our repos is dirty
            Assert.IsTrue(checkRepos.Dirty);
            Assert.AreEqual(0, vm.SelectedCheck.GetPayments().Count());

			//assert the money was not removed from the comp budget
			Assert.AreEqual (10.0M, vm.SelectedEmployee.CompBudget.Dollars, "comp budget was decremented");
		}


        [Test]
		public async Task SavePaymentSetsOnCheckAndEmployee()
        {
			var emp = new Employee {
				Id = Guid.Empty,
				CompBudget = new Money (10.0M)
			};

            var items = await ViewModelMockingTools.CreateViewModel(emp);
            ICheckRepository checkRepos = items.Item2;
            var vm = items.Item1;

            var tab = vm.CreateNewCheck(PersonName.TestName);

            Assert.IsNotNull(tab, "Tab created");
            Assert.AreEqual(CheckPaymentStatus.Open, vm.SelectedCheck.PaymentStatus);

            vm.DrinkClicked(
                new MenuItem
                {
                    Id = Guid.NewGuid(),
                    Name = "TestItem",
                    Price = new Money(10.0M)
                }
            );
            vm.CloseOrderButtonClicked();

            //ACT
            vm.CheckClicked(tab);
            Assert.AreEqual(CheckPaymentStatus.Closing, vm.SelectedCheck.PaymentStatus);

            //add a cash payment
            var paid = vm.PayCheckWithAmountComp(new Money(1.0M));
            Assert.IsTrue(paid);
            Assert.AreEqual(1, vm.SelectedCheck.GetPayments().Count(), "has payment");
			vm.SavePaymentsClicked ();
            Assert.AreEqual(1, vm.SelectedCheck.GetPayments().Count());
            Assert.IsFalse(checkRepos.Dirty);

            //check that our comp amount is updated
            Assert.AreEqual(9.0M, emp.CompBudget.Dollars, "Comp amount was decremented");
        }


        /// <summary>
        /// Tests scenario described in POS-126
        /// </summary>
        [Test]
        public async Task SavePaymentAfterChargingPercent()
        {
            var emp = new Employee
            {
                Id = Guid.Empty,
                CompBudget = new Money(10.0M)
            };

            var items = await ViewModelMockingTools.CreateViewModel(emp);
            ICheckRepository checkRepos = items.Item2;
            var vm = items.Item1;

            var tab = vm.CreateNewCheck(PersonName.TestName);

            Assert.IsNotNull(tab, "Tab created");
            Assert.AreEqual(CheckPaymentStatus.Open, vm.SelectedCheck.PaymentStatus);

            var mi = new MenuItem
            {
                Id = Guid.NewGuid(),
                Name = "TestItem",
                Price = new Money(10.0M)
            };
            for (var i = 0; i < 5; i++)
            {
                vm.DrinkClicked(mi);
            }
            vm.CloseOrderButtonClicked();

            //ACT
            vm.CheckClicked(tab);
            Assert.AreEqual(CheckPaymentStatus.Closing, vm.SelectedCheck.PaymentStatus);

            //add a partial
            var amt = vm.SelectedCheck.Total.Dollars*.1M;

            var paid = vm.PayCheckWithCreditCard(new CreditCard
            {
                ExpMonth = DateTime.Now.Month,
                ExpYear = DateTime.Now.Year
            }, new Money(amt));

            Assert.IsTrue(paid);
            Assert.AreEqual(1, vm.SelectedCheck.GetPayments().Count(), "has payment");
            vm.SavePaymentsClicked();
            Assert.AreEqual(1, vm.SelectedCheck.GetPayments().Count());
            Assert.IsFalse(checkRepos.Dirty, "repository is still dirty");
        }

		[Test]
		public async Task CreateAndPayCheckWithCreditCard(){
			var emp = new Employee {
				Id = Guid.Empty,
				CompBudget = new Money (10.0M)
			};

			var vm = (await ViewModelMockingTools.CreateViewModelWithEmployeeAndCreditCardProcessor (emp));

			var mi = new MenuItem {
				Id = Guid.NewGuid (),
				Price = new Money (100.0M),
				Name = "testMI"
			};

			var creditCard = new CreditCard {
				ExpMonth = DateTime.Now.Month,
				ExpYear = DateTime.Now.Year
			};
					
			ICheck passedCheck = null;
			vm.SetCreditCardProcessedCallback (check => {
				passedCheck = check;
			});

			//ACT
			var tab = vm.CreateNewCheck(PersonName.TestName);

			Assert.IsNotNull(tab, "Tab created");
			Assert.AreEqual(CheckPaymentStatus.Open, vm.SelectedCheck.PaymentStatus);


			vm.DrinkClicked (mi);
			vm.CloseOrderButtonClicked ();

			//pay the check
			var res = vm.PayCheckWithCreditCard (creditCard, new Money (108.88M));
			Assert.IsTrue (res);

			vm.MarkSelectedCheckAsPaid ();

			//we have to hold until the card comes back!
			var startTime = DateTime.Now;
			var maxTime = new TimeSpan (0, 0, 20);
			while((DateTime.Now - startTime) < maxTime && passedCheck == null){
				Thread.Sleep (100);
			}

			Assert.IsNotNull (passedCheck, "Credit card came back");
			Assert.AreEqual (TerminalViewTypes.ViewChecks, vm.CurrentTerminalViewTypeToDisplay, "Terminal is not at ViewTabs");
		
			vm.SelectedEmployee = emp;

			var payment = passedCheck.GetPayments().First () as IProcessingPayment;
			Assert.IsNotNull (payment);
			var closeRes = vm.AddTipToProcessingPayment (payment, new Money(10.0M));
			Assert.True (closeRes);
		}

		[Test]
		public async Task CantCloseCheckWithPaymentStillRemaining(){
			var emp = new Employee {
				Id = Guid.Empty,
				CompBudget = new Money (10.0M)
			};
			var vm = await ViewModelMockingTools.CreateViewModelWithEmployeeAndCreditCardProcessor (emp);
     

			var mi = new MenuItem {
				Id = Guid.NewGuid (),
				Price = new Money (100.0M),
				Name = "testMI"
			};

			var creditCard = new CreditCard {
				ExpMonth = DateTime.Now.Month,
				ExpYear = DateTime.Now.Year
			};

			//ACT
			var tab = vm.CreateNewCheck(PersonName.TestName);

			Assert.IsNotNull(tab, "Tab created");
			Assert.AreEqual(CheckPaymentStatus.Open, vm.SelectedCheck.PaymentStatus);


			vm.DrinkClicked (mi);
			vm.CloseOrderButtonClicked ();

			//pay the check
			var res = vm.PayCheckWithCreditCard (creditCard, new Money (1.00M));
			Assert.IsTrue (res);

		    var threw = false;
		    try
		    {
                vm.MarkSelectedCheckAsPaid();
		    }
		    catch (ArgumentException)
		    {
		        threw = true;
          
		    }

            Assert.True(threw, "Argument Exception was thrown!");
			Assert.AreEqual (TerminalViewTypes.PaymentScreen, vm.CurrentTerminalViewTypeToDisplay, "Terminal is not at ViewTabs");

		}

		[Test]
		public async Task CancelAuthOnCard(){
			var emp = new Employee {
				Id = Guid.Empty,
				CompBudget = new Money (10.0M)
			};
					
			var vm = await ViewModelMockingTools.CreateViewModelWithEmployeeAndCreditCardProcessor (emp);

			var mi = new MenuItem {
				Id = Guid.NewGuid (),
				Price = new Money (100.0M),
				Name = "testMI"
			};

			var creditCard = new CreditCard {
				ExpMonth = DateTime.Now.Month,
				ExpYear = DateTime.Now.Year
			};
					
			ICheck passedCheck = null;
			vm.SetCreditCardProcessedCallback (check => {
				passedCheck = check;
			});

			//ACT
			var tab = vm.CreateNewCheck(PersonName.TestName);

			Assert.IsNotNull(tab, "Tab created");
			Assert.AreEqual(CheckPaymentStatus.Open, vm.SelectedCheck.PaymentStatus);


			vm.DrinkClicked (mi);
			vm.CloseOrderButtonClicked ();

			//pay the check
			var res = vm.PayCheckWithCreditCard (creditCard, new Money (108.88M));
			Assert.IsTrue (res);

			var paidTab = vm.MarkSelectedCheckAsPaid ();
			Assert.IsNotNull (paidTab, "Paid selected tab");
			//we have to hold until the card comes back!
			var startTime = DateTime.Now;
			var maxTime = new TimeSpan (0, 0, 20);
			while((DateTime.Now - startTime) < maxTime && passedCheck == null){
				Thread.Sleep (100);
			}

			Assert.IsNotNull (passedCheck, "Credit card came back");
			Assert.AreEqual (TerminalViewTypes.ViewChecks, 
				vm.CurrentTerminalViewTypeToDisplay, 
				"Terminal is not at ViewTabs");

			vm.SelectedEmployee = emp;
			vm.CheckClicked (paidTab);

			Assert.AreEqual(1, paidTab.GetPayments().Count());

			var ccPayment = paidTab.GetPayments().First () as ICreditCardPayment;
			Assert.IsNotNull (ccPayment, "don't have credit card payment");
			var cancelRes = vm.VoidAuthorizedProcessingPayment (ccPayment);
			Assert.True (cancelRes, "cancelRes was false!");

			var vmCheck = vm.SelectedCheck;
			Assert.IsNotNull (vmCheck, "Check back");
			Assert.AreEqual (CheckPaymentStatus.Closing, vmCheck.PaymentStatus, "payment status");
		}

       // [Ignore("Flaky when run in sequence, fix that")]
	    [Test]
	    public async Task PayCashAfterCreditCardIsRejected()
	    {
            var emp = new Employee
            {
                Id = Guid.Empty,
                CompBudget = new Money(10.0M)
            };

	        var vm = await ViewModelMockingTools.CreateViewModelWithEmployeeAndCreditCardProcessor(emp);

            var mi = new MenuItem
            {
                Id = Guid.NewGuid(),
                Price = new Money(91.1M),
                Name = "testMI"
            };

            var creditCard = new CreditCard
            {
                ExpMonth = DateTime.Now.Month,
                ExpYear = DateTime.Now.Year
            };

            ICheck passedCheck = null;
            vm.SetCreditCardProcessedCallback(check =>
            {
                passedCheck = check;
            });

            //ACT
            var tab = vm.CreateNewCheck(PersonName.TestName);

            Assert.IsNotNull(tab, "Tab created");
            Assert.AreEqual(CheckPaymentStatus.Open, vm.SelectedCheck.PaymentStatus);


            vm.DrinkClicked(mi);
            vm.CloseOrderButtonClicked();

            //pay the check
            var res = vm.PayCheckWithCreditCard(creditCard, new Money(111.11M));
            Assert.True(res);

            Assert.IsNotNull(vm.SelectedEmployee, "Employee selected before marking as paid");
            //we'll need to apply the failed callback here I believe!
	        vm.MarkSelectedCheckAsPaid();

	        var maxTime = new TimeSpan(0, 1, 0);
	        var startTime = DateTime.Now;
	        while (DateTime.Now - startTime < maxTime && passedCheck == null)
	        {
	            Thread.Sleep(100);
	        }

            Assert.IsNotNull(passedCheck, "Have not been given a check by credit card callback");

            Assert.AreEqual(CheckPaymentStatus.PaymentRejected, passedCheck.PaymentStatus, "Check is in PaymentReject");
            Assert.AreEqual(passedCheck.Id, vm.SelectedCheck.Id);
            Assert.AreEqual(CheckPaymentStatus.PaymentRejected, vm.SelectedCheck.PaymentStatus);

	        var payments = passedCheck.GetPayments().ToList();
            Assert.AreEqual(1, payments.Count());
	        var ccPayment = payments.First() as CreditCardPayment;
            Assert.IsNotNull(ccPayment);
            Assert.AreEqual(PaymentProcessingStatus.BaseRejected, ccPayment.PaymentProcessingStatus, "Payment has correct rejected status");

            Assert.IsNotNull(vm.SelectedEmployee, "Employee selected after credit card failed");

            //now let's do the cash payment
            var cashRes = vm.PayCheckWithCash(new Money(111.11M));
            Assert.True(cashRes);

	        var cashCheck = vm.MarkSelectedCheckAsPaid();
            Assert.IsNotNull(cashCheck);
            startTime = DateTime.Now;
            while (DateTime.Now - startTime < maxTime && vm.SelectedCheck.PaymentStatus != CheckPaymentStatus.Closed)
            {
                Thread.Sleep(100);
            }
            Assert.IsNotNull(cashCheck);
            Assert.AreEqual(cashCheck.Id, vm.SelectedCheck.Id);
            Assert.AreEqual(CheckPaymentStatus.Closed, cashCheck.PaymentStatus, "Check given has closed payment status");
            Assert.AreEqual(CheckPaymentStatus.Closed, vm.SelectedCheck.PaymentStatus, "Selected check has closed payment status");
            //test it doesn't show up as an open check either
	        var openChecks = vm.OpenChecks;
            Assert.False(openChecks.Any(), "Checks are still open");
	    }

        /// <summary>
        /// This should be prevented by the UI, but added here for consistency
        /// </summary>
	    [Test]
	    public async Task TestCompAfterPayment()
	    {
            var emp = new Employee
            {
                Id = Guid.Empty,
                CompBudget = new Money(10.0M)
            };
	        var vm = (await ViewModelMockingTools.CreateViewModel(emp)).Item1;

            var tab = vm.CreateNewCheck(PersonName.TestName);

            Assert.IsNotNull(tab, "Tab created");
            Assert.AreEqual(CheckPaymentStatus.Open, vm.SelectedCheck.PaymentStatus);

            vm.DrinkClicked(
                new MenuItem
                {
                    Id = Guid.NewGuid(),
                    Name = "TestItem",
                    Price = new Money(10.0M)
                }
                );

            vm.CloseOrderButtonClicked();

            //pay some cash on it
	        var payRes = vm.PayCheckWithCash(new Money(8.00M));
            Assert.True(payRes);

	        vm.SavePaymentsClicked();

            //now comp the item
	        vm.CheckClicked(tab);

	        vm.SelectOrderItemToModify(vm.SelectedCheck.OrderItems.First());
	        var compRes = vm.CompSelectedItem();
            Assert.True(compRes, "CompRes is true");

            //what should our total be?
            Assert.True(vm.SelectedCheck.Total.Equals(Money.None));
            //Assert.True(vm.SelectedCheck.ChangeDue.Equals(new Money(8.0M)), "Amount paid should be in the change");
	    }
	}
}

