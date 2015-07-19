using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Common;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Entities.Inventory;
using Mise.Core.Services;
using Mise.Core.ValueItems.Inventory;
using Mise.Inventory.Services;
using Mise.Inventory.ViewModels;
using Moq;
using NUnit.Framework;

namespace Mise.Inventory.UnitTests.ViewModels
{
    [TestFixture]
    public class ReceivingOrderViewModelTests
    {
        [Test]
        public void SearchTermsShouldTreatASpaceAsAnd()
        {
            var lineItems = new List<ReceivingOrderLineItem>
            {
                new ReceivingOrderLineItem
                {
                    ID = Guid.NewGuid(),
                    MiseName = "Budweiser",
                    DisplayName = "Budweiser",
                    Container = new LiquidContainer {DisplayName = "12oz Can", AmountContained = LiquidAmount.FromLiquidOunces(12.0M )}
                },
                new ReceivingOrderLineItem
                {
                    ID = Guid.NewGuid(),
                    DisplayName = "Powers Irish Whiskey",
                    Container = new LiquidContainer{AmountContained = new LiquidAmount{Milliliters = 750}}
                }
            };

            var ro = new ReceivingOrder
            {
                LineItems = lineItems
            };

            var appNav = new Mock<IAppNavigation>();
            var logger = new Mock<ILogger>();
            var roService = new Mock<IReceivingOrderService>();
            roService.Setup(rs => rs.GetCurrentReceivingOrder())
                .Returns(Task.FromResult(ro as IReceivingOrder));


            var underTest = new ReceivingOrderViewModel(logger.Object, appNav.Object, roService.Object, null);

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
            var lineItems = new List<ReceivingOrderLineItem>
            {
                new ReceivingOrderLineItem
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
                new ReceivingOrderLineItem
                {
                    ID = Guid.NewGuid(),
                    DisplayName = "Powers Irish Whiskey",
                    Container = new LiquidContainer{AmountContained = new LiquidAmount{Milliliters = 750}},
                    Categories = new List<ItemCategory>
                    {
                        CategoriesService.WhiskeyWorld
                    }
                }
            };

            var ro = new ReceivingOrder
            {
                LineItems = lineItems
            };

            var appNav = new Mock<IAppNavigation>();
            var logger = new Mock<ILogger>();
            var roService = new Mock<IReceivingOrderService>();
            roService.Setup(rs => rs.GetCurrentReceivingOrder())
                .Returns(Task.FromResult(ro as IReceivingOrder));


            var underTest = new ReceivingOrderViewModel(logger.Object, appNav.Object, roService.Object, null);

            //ACT
            underTest.SearchString = "beer";

            //ASSERT
            var shownLineItems = underTest.LineItems.ToList();

            Assert.AreEqual(1, shownLineItems.Count);
            Assert.AreEqual("Budweiser", shownLineItems.First().DisplayName);
        }
    }
}
