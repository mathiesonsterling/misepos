using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Entities.Inventory;
using Mise.Core.Repositories;
using Mise.Core.Services.UtilityServices;
using Mise.Core.ValueItems;
using Mise.Core.ValueItems.Inventory;
using Mise.Core.ValueItems.Reports;
using Mise.Inventory.Services.Implementation;
using Moq;
using NUnit.Framework;

namespace Mise.Inventory.UnitTests.Services
{
    [TestFixture]
    public class ReportsServiceTests
    {
        #region COGs reports

        [Test]
        public async Task COGsReportsWithoutROs()
        {
            var logger = new Mock<ILogger>();
            var invRepos = new Mock<IInventoryRepository>();
            invRepos.Setup(i => i.GetAll())
                .Returns(new List<IInventory>
                {
                    new Core.Common.Entities.Inventory.Inventory
                    {
                        DateCompleted = DateTimeOffset.UtcNow.AddMonths(-1),
                        Sections = new List<InventorySection>
                        {
                            new InventorySection
                            {
                                LineItems = new List<InventoryBeverageLineItem> {
                                    new InventoryBeverageLineItem
                                    {
                                        Container = LiquidContainer.Bottle1L,
                                        PricePaid = new Money(10),
                                        NumFullBottles = 12
                                    }
                                }
                            }
                        }
                    },
                    new Core.Common.Entities.Inventory.Inventory
                    {
                        DateCompleted = DateTimeOffset.UtcNow,
                        Sections = new List<InventorySection>
                        {
                            new InventorySection
                            {
                                LineItems = new List<InventoryBeverageLineItem> {
                                    new InventoryBeverageLineItem
                                    {
                                        Container = LiquidContainer.Bottle1L,
                                        PricePaid = new Money(10),
                                        NumFullBottles = 2
                                    }
                                }
                            }
                        }
                    },
                });
            var roRepos = new Mock<IReceivingOrderRepository>();

            var request = new ReportRequest(ReportTypes.COGS, DateTimeOffset.MinValue, DateTimeOffset.MaxValue, null,
                null);
            var underTest = new ReportsService(logger.Object, invRepos.Object, roRepos.Object);

            //ACT
            await underTest.SetCurrentReportRequest(request);
            var res = await underTest.RunCurrentReport();

            //ASSERT
            Assert.NotNull(res);
        }
        #endregion
    }
}
