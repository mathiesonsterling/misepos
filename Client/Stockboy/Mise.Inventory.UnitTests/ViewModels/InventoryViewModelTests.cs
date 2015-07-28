using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Common;
using Mise.Core.Common.Entities;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Entities.Inventory;
using Mise.Core.Entities.People;
using Mise.Core.Services;
using Mise.Core.ValueItems.Inventory;
using Mise.Inventory.Services;
using Mise.Inventory.ViewModels;
using Moq;
using NUnit.Framework;

namespace Mise.Inventory.UnitTests.ViewModels
{
    [TestFixture]
    public class InventoryViewModelTests
    {
        [Test]
        public async Task InventoryLineItemsShouldBeOrderedByInventoryPosition()
        {
            var sectionID = Guid.NewGuid();
            var loginService = new Mock<ILoginService>();
            loginService.Setup(s => s.GetCurrentEmployee()).Returns(Task.FromResult(new Employee() as IEmployee));

            var appNav = new Mock<IAppNavigation>();

            var invService = new Mock<IInventoryService>();
            invService.Setup(s => s.GetCurrentInventorySection())
                .Returns(
                    Task.FromResult(
                        new InventorySection {RestaurantInventorySectionID = sectionID, ID = Guid.NewGuid()} as
                            IInventorySection));

            //return objects out of order
            var invItems = new List<IInventoryBeverageLineItem>
            {
                new InventoryBeverageLineItem
                {
                    ID = Guid.NewGuid(),
                    InventoryPosition = 2,
                    DisplayName = "second"
                },
                new InventoryBeverageLineItem
                {
                    ID = Guid.NewGuid(),
                    InventoryPosition = 1,
                    DisplayName = "first"
                }
            };
            invService.Setup(inv => inv.GetLineItemsForCurrentSection())
                .Returns(Task.FromResult(invItems.AsEnumerable()));

            var underTest = new InventoryViewModel(appNav.Object, loginService.Object, invService.Object, null);

            //ACT
            await underTest.OnAppearing();

            //ASSERT
            Assert.AreEqual(2, underTest.LineItems.Count());
            Assert.AreEqual("first", underTest.LineItems.First().DisplayName);
            Assert.AreEqual("second", underTest.LineItems.Last().DisplayName);
        }

        [Test]
        public void SearchTermsShouldTreatASpaceAsAnd()
        {
            var lineItems = new List<IInventoryBeverageLineItem>
            {
                new InventoryBeverageLineItem
                {
                    ID = Guid.NewGuid(),
                    MiseName = "Budweiser",
                    DisplayName = "Budweiser",
                    Container = new LiquidContainer {DisplayName = "12oz Can", AmountContained = LiquidAmount.FromLiquidOunces(12.0M )}
                },
                new InventoryBeverageLineItem
                {
                    ID = Guid.NewGuid(),
                    DisplayName = "Powers Irish Whiskey",
                    Container = new LiquidContainer{AmountContained = new LiquidAmount{Milliliters = 750}}
                }
            };

            var appNav = new Mock<IAppNavigation>();
            var logger = new Mock<ILogger>();
            var invService = new Mock<IInventoryService>();
            invService.Setup(invS => invS.GetLineItemsForCurrentSection())
                .Returns(Task.FromResult(lineItems.AsEnumerable()));


            var underTest = new InventoryViewModel(appNav.Object, null, invService.Object, logger.Object);

            //ACT
            underTest.SearchString = "bu d";

            //ASSERT
            var shownLineItems = underTest.LineItems.ToList();

            Assert.AreEqual(1, shownLineItems.Count);
            Assert.AreEqual("Budweiser", shownLineItems.First().DisplayName);
        }

        [Test]
        public void SearchShouldWorkOnCategories()
        {
            var lineItems = new List<IInventoryBeverageLineItem>
            {
                new InventoryBeverageLineItem
                {
                    ID = Guid.NewGuid(),
                    MiseName = "Budweiser",
                    DisplayName = "Budweiser",
                    Container = new LiquidContainer {DisplayName = "12oz Can", AmountContained = LiquidAmount.FromLiquidOunces(12.0M )},
                    Categories = new List<ItemCategory>
                    {
                        CategoriesService.Beer
                    }
                },
                new InventoryBeverageLineItem
                {
                    ID = Guid.NewGuid(),
                    DisplayName = "Powers Irish Whiskey",
                    Container = LiquidContainer.Bottle750ML
                }
            };

            var appNav = new Mock<IAppNavigation>();
            var logger = new Mock<ILogger>();
            var invService = new Mock<IInventoryService>();
            invService.Setup(invS => invS.GetLineItemsForCurrentSection())
                .Returns(Task.FromResult(lineItems.AsEnumerable()));


            var underTest = new InventoryViewModel(appNav.Object, null, invService.Object, logger.Object);

            //ACT
            underTest.SearchString = "beer";

            //ASSERT
            var shownLineItems = underTest.LineItems.ToList();

            Assert.AreEqual(1, shownLineItems.Count);
            Assert.AreEqual("Budweiser", shownLineItems.First().DisplayName);
        }
    }
}
 