using System;
using System.Collections.Generic;
using Mise.Core.Client.ApplicationModel;
using Mise.Core.Common.Entities;
using Mise.Core.Entities.Check;
using Mise.Core.Entities.Menu;
using Mise.Core.Entities.Payments;
using Mise.Core.ValueItems;
using Mise.Core.Services;
using MisePOSTerminal.ViewModels;
using Moq;
using NUnit.Framework;

namespace MisePOSTerminal.UnitTests.ViewModels
{
    [TestFixture]
    public class OrderOnTabViewModelTests
    {
        [Test]
        public void TestCloseButtonNotEnabledIfNoOrderedItems()
        {
            var check = new Mock<ICheck>();
            check.Setup(c => c.OrderItems).Returns(new List<OrderItem >());

			var logger = new Mock<ILogger> ();
            var appModel = new Mock<ITerminalApplicationModel>();

            appModel.Setup(a => a.SelectedCheck).Returns(check.Object);
            var vm = new OrderOnCheckViewModel(logger.Object, appModel.Object);
            //ACT
            var res = vm.Close.CanExecute(null);
            //ASSERT
            Assert.IsFalse(res);
        }

        [Test]
        public void TestCloseButtonEnabledIfOrderedItems()
        {
            var check = new Mock<ICheck>();
            check.Setup(c => c.OrderItems).Returns(new List<OrderItem >
            {
                new OrderItem
                {
                    MenuItem = new MenuItem
                    {
                        Price = new Money(1.0M)
                    }
                }
            });

            var appModel = new Mock<ITerminalApplicationModel>();

            appModel.Setup(a => a.SelectedCheck).Returns(check.Object);
			var logger = new Mock<ILogger> ();
            var vm = new OrderOnCheckViewModel(logger.Object, appModel.Object);
            //ACT
            var res = vm.Close.CanExecute(null);
            //ASSERT
            Assert.IsTrue(res);
        }
    
		[Test]
		public void TestFastCashAndCloseEnabledWithTotal(){
			var oi = new OrderItem {
				MenuItem = new MenuItem {
					Price = new Money (1.0M)
				}
			};
			var check = new RestaurantCheck {
				ID = Guid.NewGuid (),
				OrderItems = new List<OrderItem > {
					oi
				}
			};

			var appModel = new Mock<ITerminalApplicationModel>();
			appModel.Setup(a => a.SelectedCheck).Returns(check);
			appModel.Setup (a => a.SelectedOrderItem).Returns (oi);

			var logger = new Mock<ILogger> ();
			//ACT
			var vm = new OrderOnCheckViewModel(logger.Object, appModel.Object);

			//ASSERT
			var fastCash = vm.FastCash.CanExecute (null);
			Assert.IsTrue (fastCash, "Fast cash enabled");
			Assert.IsTrue (vm.Close.CanExecute (null), "Close enabled");
		}

		[Test]
		public void TestFastCashAndCloseDisabledWithEmptyTab(){

			var check = new RestaurantCheck {
				ID = Guid.NewGuid (),
				OrderItems = new List<OrderItem >()
			};

			var appModel = new Mock<ITerminalApplicationModel>();
			appModel.Setup(a => a.SelectedCheck).Returns(check);

			var logger = new Mock<ILogger> ();
			//ACT
			var vm = new OrderOnCheckViewModel(logger.Object, appModel.Object);

			//ASSERT
			var fastCash = vm.FastCash.CanExecute (null);
			Assert.IsFalse (fastCash, "Fast cash disabled");
			Assert.IsFalse (vm.Close.CanExecute (null), "Close disabled");
		}

		[Test]
		public void TestDeletingOrderItemCallsUpdateCommands(){
			var oi = new OrderItem {
				MenuItem = new MenuItem {
					Price = new Money (1.0M)
				},
                Status = OrderItemStatus.Added
			};
			var check = new RestaurantCheck {
				ID = Guid.NewGuid (),
				OrderItems = new List<OrderItem > {
					oi
				}
			};

			var appModel = new Mock<ITerminalApplicationModel>();
			appModel.Setup(a => a.SelectedCheck).Returns(check);
			appModel.Setup (a => a.SelectedOrderItem).Returns (oi);
			appModel.Setup (a => a.DeleteSelectedOrderItem()).Returns(true);

			var logger = new Mock<ILogger> ();
			var vm = new OrderOnCheckViewModel(logger.Object, appModel.Object);

			bool fired = false;
			vm.OnUpdateCommands += calling => fired = true;

			//ACT
			Assert.IsTrue (vm.DeleteOrderItem.CanExecute (null), "DeleteOrderItem enabled");
			vm.DeleteOrderItem.Execute (null);

			//ASSERT 
			Assert.IsTrue(fired, "Update was fired");

		}


        [Test]
        public void TestCategoryHomeFiresUpdateEvent()
        {
            var check = new RestaurantCheck
            {
                ID = Guid.NewGuid(),
                OrderItems = new List<OrderItem >()
            };

            var appModel = new Mock<ITerminalApplicationModel>();
            appModel.Setup(a => a.SelectedCheck).Returns(check);
            appModel.SetupSet(a => a.SelectedCategory = It.IsAny<MenuItemCategory>()).Verifiable();
          
            var logger = new Mock<ILogger>();
            var vm = new OrderOnCheckViewModel(logger.Object, appModel.Object);

            var updateCommandsFired = false;
            vm.OnUpdateCommands += calling => updateCommandsFired = true;
            //ACT
            vm.CategoryHomeClicked.Execute(null);

            //ASSERT
            Assert.IsTrue(updateCommandsFired, "OnUpdateCommands fired");
            appModel.VerifySet(a => a.SelectedCategory=null, "Category was not set!");
        }

        [Test]
        public void TestCategoryClickedFiresUpdateEvent()
        {
            var check = new RestaurantCheck
            {
                ID = Guid.NewGuid(),
                OrderItems = new List<OrderItem >()
            };

            var appModel = new Mock<ITerminalApplicationModel>();
            appModel.Setup(a => a.SelectedCheck).Returns(check);
            appModel.SetupSet(a => a.SelectedCategory = It.IsAny<MenuItemCategory>()).Verifiable();

            var logger = new Mock<ILogger>();
            var vm = new OrderOnCheckViewModel(logger.Object, appModel.Object);

            var updateCommandsFired = false;
            vm.OnUpdateCommands += calling => updateCommandsFired = true;

            var category = new Mock<MenuItemCategory>();

            //ACT
            vm.CategoryClicked.Execute(category.Object);

            //ASSERT
            Assert.IsTrue(updateCommandsFired, "OnUpdateCommands fired");
            appModel.VerifySet(a => a.SelectedCategory = category.Object, "Category was not set!");
        }

		/// <summary>
		/// Fix for added bug.  Checks our vm canExecutes don't throw when the SelectedOrderItem is null
		/// </summary>
		[Test]
		public void TestNoSelectedOrderItemDoesntThrow(){
			//ASSEMBLE
			var appModel = new Mock<ITerminalApplicationModel> ();
			appModel.Setup (a => a.SelectedOrderItem).Returns<OrderItem> (null);

			var logger = new Mock<ILogger> ();
			var vm = new OrderOnCheckViewModel (logger.Object, appModel.Object);

			//ACT
			var undoRes = vm.UndoCompedSelectedOrderItem.CanExecute (null);
			Assert.False (undoRes);

			var compRes = vm.CompSelectedOrderItem.CanExecute (null);
			Assert.False (compRes);

			var delRes = vm.DeleteOrderItem.CanExecute (null);
			Assert.False (delRes);
		}

        [Test]
        public void TestCheckWithPaymentsCantCompItems()
        {
            var oi = new OrderItem
            {
                MenuItem = new MenuItem
                {
                    Name = "testItem",
                    Price = new Money(10.0M)
                }
            };

            var selectedTab = new RestaurantCheck
            {
                OrderItems = new List<OrderItem>
                {
                    oi
                }
            };

            selectedTab.CashPayments.Add(new CashPayment
            {
                AmountPaid = new Money(10.0M),
                AmountTendered = new Money(10.0M)
            });


            var app = new Mock<ITerminalApplicationModel>();
            app.Setup(a => a.SelectedCheck).Returns(selectedTab);

            var logger = new Mock<ILogger>();

            //ACT
            var vm = new OrderOnCheckViewModel(logger.Object, app.Object);

            //ASSERT

        }
	}
}
