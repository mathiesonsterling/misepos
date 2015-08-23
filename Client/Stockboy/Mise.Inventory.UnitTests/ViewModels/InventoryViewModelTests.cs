using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Common;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Entities.Inventory;
using Mise.Core.Services.UtilityServices;
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
                    DisplayName = "secondItem"
                },
                new InventoryBeverageLineItem
                {
                    ID = Guid.NewGuid(),
                    InventoryPosition = 1,
                    DisplayName = "firstItem"
                },
				new InventoryBeverageLineItem {
					ID = Guid.NewGuid (),
					InventoryPosition = 4,
					DisplayName = "fourthItem"
				},
				new InventoryBeverageLineItem {
					ID = Guid.NewGuid (),
					InventoryPosition = 3,
					DisplayName = "thirdItem"
				}
            };
            invService.Setup(inv => inv.GetLineItemsForCurrentSection())
                .Returns(Task.FromResult(invItems.AsEnumerable()));

            var underTest = new InventoryViewModel(appNav.Object, invService.Object, null);

            //ACT
            await underTest.OnAppearing();

            //ASSERT
			var lis = underTest.LineItems.ToList ();
            Assert.AreEqual(4, lis.Count());
			Assert.AreEqual("firstItem", lis[0].DisplayName);
			Assert.AreEqual("secondItem", lis[1].DisplayName);
			Assert.AreEqual("thirdItem", lis[2].DisplayName);
			Assert.AreEqual("fourthItem", lis[3].DisplayName);
        }

		[Test]
		public async Task InventoryLineItemsShouldBeOrderedByInventoryPositionAfterSearch()
		{
			var sectionID = Guid.NewGuid();
			var appNav = new Mock<IAppNavigation>();

			var invService = new Mock<IInventoryService>();

			//return objects out of order
			var invItems = new List<IInventoryBeverageLineItem>
			{
				new InventoryBeverageLineItem
				{
					ID = Guid.NewGuid(),
					InventoryPosition = 2,
					DisplayName = "secondItem"
				},
				new InventoryBeverageLineItem
				{
					ID = Guid.NewGuid(),
					InventoryPosition = 1,
					DisplayName = "firstItem"
				},
				new InventoryBeverageLineItem {
					ID = Guid.NewGuid (),
					InventoryPosition = 4,
					DisplayName = "fourthItem"
				},
				new InventoryBeverageLineItem {
					ID = Guid.NewGuid (),
					InventoryPosition = 3,
					DisplayName = "thirdItem"
				}
			};
			invService.Setup(inv => inv.GetLineItemsForCurrentSection())
				.Returns(Task.FromResult(invItems.AsEnumerable()));

			var underTest = new InventoryViewModel(appNav.Object, invService.Object, null);

			//ACT
			await underTest.OnAppearing();
			underTest.SearchString = "Item";

			//ASSERT
			var lis = underTest.LineItems.ToList ();
			Assert.AreEqual(4, lis.Count());
			Assert.AreEqual("firstItem", lis[0].DisplayName);
			Assert.AreEqual("secondItem", lis[1].DisplayName);
			Assert.AreEqual("thirdItem", lis[2].DisplayName);
			Assert.AreEqual("fourthItem", lis[3].DisplayName);
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


            var underTest = new InventoryViewModel(appNav.Object, invService.Object, logger.Object);

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


            var underTest = new InventoryViewModel(appNav.Object, invService.Object, logger.Object);

            //ACT
            underTest.SearchString = "beer";

            //ASSERT
            var shownLineItems = underTest.LineItems.ToList();

            Assert.AreEqual(1, shownLineItems.Count);
            Assert.AreEqual("Budweiser", shownLineItems.First().DisplayName);
        }
    }
}
 