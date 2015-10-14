using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using Moq;

using Mise.Core.Common.Entities.Reports;
using Mise.Core.Entities.Inventory;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.ValueItems.Inventory;
namespace Mise.Core.Common.UnitTests
{
	[TestFixture]
	public class COGReportTests
	{
		[Test]
		public async Task TwoInventoriesWithoutPricing(){
			var testCon = LiquidContainer.Bottle750ML;

			var inventories = new List<IInventory>{
				new Inventory{
					DateCompleted = DateTimeOffset.UtcNow.AddDays(-7),
					Sections = new List<InventorySection>{
						new InventorySection{
							LineItems = new List<IInventoryBeverageLineItem>{
								new InventoryBeverageLineItem{
									DisplayName = "TestItem",
									Container = testCon,
									NumFullBottles = 99,
									PartialBottleListing = new List<decimal>{.5M, .5M}
								}
							}
						}
					}
				}
			};

			var receivingOrders = new List<IReceivingOrder>();
			var underTest = new COGsReport(inventories, receivingOrders);

			//ACT
			var res = await underTest.RunReportAsync();
		}
	}
}

