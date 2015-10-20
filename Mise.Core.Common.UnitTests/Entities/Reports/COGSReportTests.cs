using System;
using System.Linq;
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
	public class COGSReportTests
	{
		[Test]
		public async Task TwoInventoriesWithoutPricing(){
			var testCon = LiquidContainer.Bottle750ML;

			//we've got a difference of 9 items between the two
			var inventories = new List<IInventory>{
				new Inventory{
					DateCompleted = DateTimeOffset.UtcNow.AddDays(-7),
					Sections = new List<InventorySection>{
						new InventorySection{
							LineItems = new List<InventoryBeverageLineItem>{
								new InventoryBeverageLineItem{
									DisplayName = "TestItem",
									Container = testCon,
									NumFullBottles = 99,
									PartialBottleListing = new List<decimal>{.5M, .5M}
								}
							}
						}
					}
				},
				new Inventory{
					DateCompleted = DateTimeOffset.UtcNow,
					Sections = new List<InventorySection>{
						new InventorySection{
							LineItems = new List<InventoryBeverageLineItem>{
								new InventoryBeverageLineItem{
									DisplayName = "TestItem",
									Container = testCon,
									NumFullBottles = 90,
									PartialBottleListing = new List<decimal>{.5M, .5M}, 
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

			//ASSERT
			Assert.NotNull(res);
			Assert.AreEqual (0, res.Checksum);
			Assert.AreEqual (1, res.LineItems.Count ());

			var item = res.LineItems.First ();
			Assert.AreEqual ("TestItem", item.MainText);
			Assert.AreEqual (0, item.Quantity, "Quantity");
			Assert.AreEqual (LiquidContainer.Bottle750ML.DisplayName, item.DetailText, "Detail");
		}

		[Test]
		public async Task TwoInventoriesWithPricePaid(){
			var testCon = LiquidContainer.Bottle750ML;

			//we've got a difference of 9 items between the two
			var inventories = new List<IInventory>{
				new Inventory{
					DateCompleted = DateTimeOffset.UtcNow.AddDays(-7),
					Sections = new List<InventorySection>{
						new InventorySection{
							LineItems = new List<InventoryBeverageLineItem>{
								new InventoryBeverageLineItem{
									DisplayName = "TestItem",
									Container = testCon,
									NumFullBottles = 99,
									PricePaid = new Mise.Core.ValueItems.Money(10.0M),
									PartialBottleListing = new List<decimal>{.5M, .5M}
								}
							}
						}
					}
				},
				new Inventory{
					DateCompleted = DateTimeOffset.UtcNow,
					Sections = new List<InventorySection>{
						new InventorySection{
							LineItems = new List<InventoryBeverageLineItem>{
								new InventoryBeverageLineItem{
									DisplayName = "TestItem",
									Container = testCon,
									NumFullBottles = 89,
									PartialBottleListing = new List<decimal>{.5M, .5M}, 
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

			//ASSERT
			Assert.NotNull(res);
			Assert.AreEqual (100, res.Checksum, "Checksum");
			Assert.AreEqual (1, res.LineItems.Count ());

			var item = res.LineItems.First ();
			Assert.AreEqual ("TestItem", item.MainText);
			Assert.AreEqual (100.0M, item.Quantity, "Quantity");
			Assert.AreEqual (LiquidContainer.Bottle750ML.DisplayName, item.DetailText, "Detail");
		}

		[Test]
		public async Task TwoInventoriesWithPricePaidAndRoShouldUseROPrice(){
			var testCon = LiquidContainer.Bottle750ML;

			//we've got a difference of 9 items between the two
			var inventories = new List<IInventory>{
				new Inventory{
					DateCompleted = DateTimeOffset.UtcNow.AddDays(-7),
					Sections = new List<InventorySection>{
						new InventorySection{
							LineItems = new List<InventoryBeverageLineItem>{
								new InventoryBeverageLineItem{
									DisplayName = "TestItem",
									Container = testCon,
									NumFullBottles = 99,
									PricePaid = new Mise.Core.ValueItems.Money(10.0M),
									PartialBottleListing = new List<decimal>{.5M, .5M}
								}
							}
						}
					}
				},
				new Inventory{
					DateCompleted = DateTimeOffset.UtcNow,
					Sections = new List<InventorySection>{
						new InventorySection{
							LineItems = new List<InventoryBeverageLineItem>{
								new InventoryBeverageLineItem{
									DisplayName = "TestItem",
									Container = testCon,
									NumFullBottles = 89,
									PartialBottleListing = new List<decimal>{.5M, .5M}, 
								}
							}
						}
					}
				}
			};

			var receivingOrders = new List<IReceivingOrder> {
				new ReceivingOrder{
					DateReceived = DateTimeOffset.UtcNow.AddDays(-1),
					LineItems = new List<ReceivingOrderLineItem>{
						new ReceivingOrderLineItem{
							DisplayName = "TestItem",
							Container = testCon,
							Quantity = 10,
							UnitPrice = new Mise.Core.ValueItems.Money(20.0M)
						}
					}
				}
			};
			var underTest = new COGsReport(inventories, receivingOrders);

			//ACT
			var res = await underTest.RunReportAsync();

			//ASSERT
			Assert.NotNull(res);
			Assert.AreEqual (1, res.LineItems.Count ());

			var item = res.LineItems.First ();
			Assert.AreEqual ("TestItem", item.MainText);
			//TOTAL should be 20 dollars times 20 items (10 from inv, 10 from RO)
			Assert.AreEqual (400.0M, item.Quantity, "Quantity");
			Assert.AreEqual (400, res.Checksum, "Checksum");
			Assert.AreEqual (LiquidContainer.Bottle750ML.DisplayName, item.DetailText, "Detail");
		}
	}
}

