using System;

using NUnit.Framework;
using Mise.Core.Common.Entities.Inventory;

using Mise.Core.ValueItems.Inventory;
namespace Mise.Core.Common.UnitTests
{
	[TestFixture]
	public class InventoryBeverageLineItemTests
	{
		[Test]
		public void SearchShouldNotThrowOnEmpty(){
			var underTest = new InventoryBeverageLineItem ();

			//ACT
			var res = underTest.ContainsSearchString ("test");

			//ASSERT
			Assert.IsFalse (res);
		}

		[Test]
		public void SearchShouldFindFromContainer(){
			var underTest = new InventoryBeverageLineItem () {
				MiseName = "testPL",
				Container = new LiquidContainer{ AmountContained = new LiquidAmount{ Milliliters = 100 } }
			};

			//ACT
			var res = underTest.ContainsSearchString ("100");
			var badRes = underTest.ContainsSearchString ("750");

			//ASSERT
			Assert.IsTrue (res);
			Assert.IsFalse (badRes, "False positive");
		}
	}
}

