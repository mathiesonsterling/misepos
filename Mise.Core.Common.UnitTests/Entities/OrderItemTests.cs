
using System;
using System.Collections.Generic;
using System.Linq;
using Mise.Core.Common.Entities;
using Mise.Core.Common.Entities.MenuItems;
using Mise.Core.Common.UnitTests.Tools;
using Mise.Core.Entities;
using Mise.Core.Entities.Check;
using Mise.Core.ValueItems;
using Mise.Core.Entities.Menu;

using NUnit.Framework;

namespace Mise.Core.Common.UnitTests.Entities
{
	[TestFixture]
	public class OrderItemTests
	{
		[Test]
		public void TestConstructorMakesDestinationsList(){
			var oi = new OrderItem ();

			Assert.IsNotNull (oi.Destinations);
		}

		[Test]
		public void TestModifiersMultiplyThenAdd(){
			var oi = new OrderItem{
				MenuItem = new MenuItem{
					Price = new Money(10.0M)
				}
			};

			var addMod = new MenuItemModifier { PriceChange = new Money(1.0M) };
			var multMod = new MenuItemModifier { PriceMultiplier = 2.0M };
			var add2Mod = new MenuItemModifier { PriceChange = new Money(2.0M) };

			var modList = new List<MenuItemModifier> { addMod, multMod, add2Mod};
			oi.AddModifiers (modList);

			var total = oi.Total;
			Assert.IsTrue (total.HasValue);
			Assert.AreEqual (23.0M, total.Dollars);
		}

	    [Test]
	    public void TestClone()
	    {
	        var id = Guid.NewGuid();
	        var restid = Guid.NewGuid();
	        var empID = Guid.NewGuid();

	        var createDate = DateTime.UtcNow.AddDays(-1);
	        var updateDate = DateTime.UtcNow;
	        var oi = new OrderItem
	        {
	            Id = id,
	            CreatedDate = createDate,
	            LastUpdatedDate = updateDate,
	            EmployeeWhoComped = empID,
	            IsOrdering = true,
	            Memo = "testing a memo",
	            PlacedByID = empID,
	            RestaurantID = restid,
	            Revision = new EventID{OrderingID = 100},
	            Status = OrderItemStatus.Made,
	            Modifiers = new List<MenuItemModifier>
	            {
	                new MenuItemModifier
	                {
	                    Name = "testMod"
	                }
	            }
	        };

            oi.AddDestination(new OrderDestination
            {
                Name = "testDest"
            });

            Assert.AreEqual(1, oi.Destinations.Count());

            //ACT 
	        var res = oi.Clone();

            //ASSERT
            Assert.IsNotNull(res);
            Assert.AreEqual(id, res.Id);
            Assert.AreEqual(restid, res.RestaurantID);
            Assert.IsTrue(res.IsComped);
            Assert.IsTrue(res.IsOrdering);
            Assert.AreEqual("testing a memo", res.Memo);
	        Assert.AreEqual(empID, res.PlacedByID);
            Assert.AreEqual(OrderItemStatus.Made, res.Status);

            Assert.IsNotNull(res.Destinations, "Destinations is null");
            Assert.IsNotNull(res.Modifiers, "Modifiers is null");

            Assert.AreEqual(1, res.Modifiers.Count());
            Assert.AreEqual("testMod", res.Modifiers.First().Name);

            Assert.AreEqual(1, res.Destinations.Count());
            Assert.AreEqual("testDest", res.Destinations.First().Name);
	    }
	}
}

