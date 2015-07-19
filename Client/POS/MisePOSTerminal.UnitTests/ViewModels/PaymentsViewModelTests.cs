using System;
using System.Collections.Generic;
using System.Linq;
using Mise.Core.Client.ApplicationModel;
using Mise.Core.Common.Entities;
using Mise.Core.Entities.Check;
using Mise.Core.Entities.Menu;
using Mise.Core.Entities.Payments;
using Mise.Core.Services;
using Mise.Core.ValueItems;
using MisePOSTerminal.ViewModels;
using Moq;
using NUnit.Framework;
using System.ComponentModel;

namespace MisePOSTerminal.UnitTests.ViewModels
{
    [TestFixture]
    public class PaymentsViewModelTests
    {
        [TestCase(false)]
        [TestCase(true)]
        [Test]
        public void EmployeeCanCompAmountsValueShowsInViewModel(bool value)
        {
            var app = new Mock<ITerminalApplicationModel>();
            app.Setup(a => a.SelectedEmployee).Returns(new Employee
            {
                CanCompAmount = value
            });
            app.Setup(a => a.GetTotalWithSalesTaxForSelectedCheck()).Returns(Money.None);

            var logger = new Mock<ILogger>();
            //ACT
            var vm = new PaymentsViewModel(logger.Object, app.Object);

            //ASSERT
            Assert.AreEqual(value, vm.CanCompAmounts);
        }


        [Test]
        public void GetAmountsOver()
        {

            var app = new Mock<ITerminalApplicationModel>();
            app.Setup(a => a.GetRemainingAmountToBePaidOnCheck()).Returns(new Money(7.11M));
            app.Setup(a => a.GetTotalWithSalesTaxForSelectedCheck()).Returns(new Money(7.11M));

            var logger = new Mock<ILogger>();
            var vm = new PaymentsViewModel(logger.Object, app.Object);

            //ACT
            var res = vm.PossibleAmountsAboveAmount.Take(6).ToList();

            //ASSERT
            Assert.IsNotNull(res);
            Assert.AreEqual(6, res.Count, "Has 6 items");
            Assert.IsTrue(res.Contains(8.00M));
            Assert.IsTrue(res.Contains(10.00M));
            Assert.IsTrue(res.Contains(20.00M));
            Assert.IsTrue(res.Contains(40.00M));
            Assert.IsTrue(res.Contains(50.00M));
            Assert.IsTrue(res.Contains(60.00M));
        }

        [Test]
        public void GetAmountsUnder()
        {

            var app = new Mock<ITerminalApplicationModel>();
            app.Setup(a => a.GetRemainingAmountToBePaidOnCheck()).Returns(new Money(52.11M));
            app.Setup(a => a.GetTotalWithSalesTaxForSelectedCheck()).Returns(new Money(52.11M));

            var logger = new Mock<ILogger>();
            var vm = new PaymentsViewModel(logger.Object, app.Object);

            //ACT
            var res = vm.PossibleAmountsUnderAmount.Take(6).ToList();

            //ASSERT
            Assert.IsNotNull(res);
            Assert.AreEqual(5, res.Count, "Has 5 items");
            Assert.IsFalse(res.Contains(1.00M), "Should never do single dollars");
            Assert.IsTrue(res.Contains(5.00M));
            Assert.IsTrue(res.Contains(10.00M));
            Assert.IsTrue(res.Contains(20.00M));
            Assert.IsTrue(res.Contains(40.00M));
            Assert.IsTrue(res.Contains(50.00M));
        }

		[Test]
		public void CheckTotalIncludesSalesTax()
		{
		    var mockCheck = new RestaurantCheck
		    {
		        OrderItems = new List<OrderItem>
		        {
		            new OrderItem
		            {
		                MenuItem = new MenuItem
		                {
		                    Price = new Money(25.00M)
		                }
		            }
		        }
		    };

			var app = new Mock<ITerminalApplicationModel>();
		    app.Setup(a => a.GetTotalWithSalesTaxForSelectedCheck()).Returns(new Money(100.0M));
            
			var logger = new Mock<ILogger>();

            //ACT
			var vm = new PaymentsViewModel(logger.Object, app.Object);

            //ASSERT
            Assert.AreEqual(100.0M, vm.CheckTotal);
            app.Verify(a => a.GetTotalWithSalesTaxForSelectedCheck(), Times.Once());
		}

        [Test]
        public void FirstPaymentIsCashOverAmountDueClosesAutomatically()
        {
            var amtPaid = new Money(30.00M);
            var checkAfter = new RestaurantCheck{PaymentStatus = CheckPaymentStatus.Closed};
            checkAfter.AddPayment(new CashPayment
            {
                AmountPaid = new Money(22.22M),
                AmountTendered = amtPaid
            });

            var app = new Mock<ITerminalApplicationModel>();
            //aount we'll have after paying the tab
            app.Setup(a => a.GetRemainingAmountToBePaidOnCheck()).Returns(Money.None);

            app.Setup(a => a.PayCheckWithCash(It.IsAny<Money>())).Returns(true);
            app.Setup(a => a.SelectedCheck).Returns(checkAfter);
            app.Setup(a => a.MarkSelectedCheckAsPaid()).Returns(checkAfter);
            app.Setup(a => a.CurrentTerminalViewTypeToDisplay).Returns(TerminalViewTypes.DisplayChange);
            app.Setup(a => a.GetTotalWithSalesTaxForSelectedCheck()).Returns(new Money(22.22M));

            var logger = new Mock<ILogger>();
            var vm = new PaymentsViewModel(logger.Object, app.Object);

            var destinationView = TerminalViewTypes.PaymentScreen;
            vm.OnMoveToView += calling => destinationView = calling.DestinationView;

            var calledUpdate = false;
            vm.OnUpdatePayments += calling => calledUpdate = true;

            //ACT
            vm.MakeCashPayment.Execute(amtPaid);

            //ASSERT
            Assert.AreEqual(TerminalViewTypes.DisplayChange, destinationView, "View is at Payment Screen still");
            Assert.False(calledUpdate);
        }

        [Test]
        public void FirstPaymentIsCreditOverAmountDueDoesntCloseAutomatically()
        {
            var amtPaid = new Money(30.00M);
            var checkAfter = new RestaurantCheck { PaymentStatus = CheckPaymentStatus.Closed };
            checkAfter.AddPayment(new CreditCardPayment
            {
                AmountCharged = new Money(30.00M),
            });

            var app = new Mock<ITerminalApplicationModel>();
            //aount we'll have after paying the tab
            app.Setup(a => a.GetRemainingAmountToBePaidOnCheck()).Returns(Money.None);

            app.Setup(a => a.PayCheckWithCreditCard(It.IsAny<CreditCard>(), It.IsAny<Money>())).Returns(true);
            app.Setup(a => a.SelectedCheck).Returns(checkAfter);
            app.Setup(a => a.MarkSelectedCheckAsPaid()).Returns(checkAfter);
            app.Setup(a => a.CurrentTerminalViewTypeToDisplay).Returns(TerminalViewTypes.DisplayChange);
            app.Setup(a => a.GetTotalWithSalesTaxForSelectedCheck()).Returns(new Money(22.22M));

            var logger = new Mock<ILogger>();
            var vm = new PaymentsViewModel(logger.Object, app.Object);

            var destinationView = TerminalViewTypes.PaymentScreen;
            vm.OnMoveToView += calling => destinationView = calling.DestinationView;

            var calledUpdate = false;
            vm.OnUpdatePayments += calling => calledUpdate = true;
            vm.CreditCardYear = DateTime.UtcNow.Year;
            vm.CreditCardMonth = DateTime.UtcNow.Month;
            vm.CreditCardNumber = "1111222233334444";
            //ACT
            vm.MakeInternalCreditPayment.Execute(amtPaid);

            //ASSERT
            Assert.AreEqual(TerminalViewTypes.PaymentScreen, destinationView, "View is at Payment Screen still");
            Assert.True(calledUpdate);
        }

        /// <summary>
        /// Tests that when we have an item with sales tax added, and hit our '25%' button 4 times we get it
        /// </summary>
        [TestCase(111.1, 2)]
        [TestCase(16.33, 4)]
        [Test]
        public void PercentagePaymentsHitTotalWithSalesTaxCompletely(decimal checkTotal, int numTouches)
        {
            var runningTotal =  checkTotal;
            var percentage = 1.0M/numTouches;
            var app = new Mock<ITerminalApplicationModel>();
            app.Setup(a => a.GetRemainingAmountToBePaidOnCheck()).Returns(new Money(runningTotal));
            app.Setup(a => a.GetTotalWithSalesTaxForSelectedCheck()).Returns(new Money(checkTotal));

            var logger = new Mock<ILogger>();
            var vm = new PaymentsViewModel(logger.Object, app.Object);

            //ACT
            for (var i = 0; i < numTouches; i++)
            {
                var res = vm.GetPercentageOfCheckTotal(percentage);
                runningTotal -= res;
            }

            Assert.LessOrEqual(runningTotal, 0.0M);
            //Assert we don't round over 1 cent per card over
            Assert.GreaterOrEqual(runningTotal, 0 - numTouches/100M);

        }

        [Test]
        public void FirstPaymentIsCashUnderAmountDueDoesntCloseAutomatically()
        {
            var amtPaid = new Money(3.00M);
            var checkAfter = new RestaurantCheck();
            checkAfter.AddPayment(new CashPayment
                    {
                        AmountPaid = amtPaid,
                        AmountTendered = amtPaid
                    });

            var app = new Mock<ITerminalApplicationModel>();
            app.Setup(a => a.GetRemainingAmountToBePaidOnCheck())
                .Returns(new Money(10.00M));
            app.Setup(a => a.PayCheckWithCash(It.IsAny<Money>())).Returns(true);
            app.Setup(a => a.SelectedCheck).Returns(checkAfter);
            app.Setup(a => a.GetTotalWithSalesTaxForSelectedCheck()).Returns(new Money(13.00M));

            var logger = new Mock<ILogger>();
            var vm = new PaymentsViewModel(logger.Object, app.Object);

            var destinationView = TerminalViewTypes.PaymentScreen;
            vm.OnMoveToView += calling => destinationView = calling.DestinationView;

            var calledUpdate = false;
            vm.OnUpdatePayments += calling => calledUpdate = true;

            //ACT
            vm.MakeCashPayment.Execute(amtPaid);

            //ASSERT
            Assert.AreEqual(TerminalViewTypes.PaymentScreen, destinationView, "View is at Payment Screen still");
            Assert.True(calledUpdate);
        }

		[Test]
		public void CreditCardSwipeAddsCreditCardPayment(){
			//ASSEMBLE
			//setup check to not have our card
			var check = new RestaurantCheck {
				PaymentStatus = CheckPaymentStatus.Closing
			};

			var appModel = new Mock<ITerminalApplicationModel> ();
			appModel.Setup (am => am.SelectedCheck).Returns (check);
			appModel.Setup (am => am.GetTotalWithSalesTaxForSelectedCheck ()).Returns (new Money (10.0M));

			var card = new CreditCard {
				CardNumber = "1111222233334444",
				FirstName = "fName",
				LastName = "lName",
				ExpMonth = 11,
				ExpYear = 2020
			};
			var logger = new Mock<ILogger>();
			var vm = new PaymentsViewModel (logger.Object, appModel.Object);

			//ACT
			vm.CreditCardSwiped (card);

			//ASSERT
			Assert.AreEqual (vm.CreditCardNumber, card.CardNumber);
			Assert.AreEqual (vm.CreditCardMonth, card.ExpMonth);
			Assert.AreEqual (vm.CreditCardYear, card.ExpYear);
		}

		[Test]
		public void CreditCardSwipeSelectsPaymentForTip(){
			//ASSEMBLE
			var card = new CreditCard {
				CardNumber = "1111222233334444",
				FirstName = "fName",
				LastName = "lName",
				ExpMonth = 11,
				ExpYear = 2020
			};

			var check = new RestaurantCheck {
				PaymentStatus = CheckPaymentStatus.PaymentApprovedWithoutTip,
				CreditCardPayments = new List<CreditCardPayment> {
					new CreditCardPayment {
						AmountCharged = new Money (10.0M),
						Card = card,
					},
					new CreditCardPayment {
						AmountCharged = new Money(5.0M),
						Card = new CreditCard{
							CardNumber= "4444555566667777"
						}
					}
				}
			};

			var appModel = new Mock<ITerminalApplicationModel> ();
			appModel.Setup (am => am.SelectedCheck).Returns (check);
			appModel.Setup (am => am.GetTotalWithSalesTaxForSelectedCheck ()).Returns (new Money (10.0M));

			var logger = new Mock<ILogger>();
			var vm = new PaymentsViewModel (logger.Object, appModel.Object);

			//ACT
			//assert what our current selected is
			vm.CreditCardSwiped (card);

			//ASSERT
			Assert.IsNotNull (vm.SelectedPayment);
			var ccPayment = vm.SelectedPayment as CreditCardPayment;
			Assert.IsNotNull (ccPayment);
			Assert.IsNotNull (ccPayment.Card);
			Assert.AreEqual (ccPayment.Card, card);
		}
    }
}
