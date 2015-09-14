using System;
using System.Linq;
using System.Collections.Generic;

using Mise.Core.Common.Entities;
using Mise.Core.Entities.Check;
using Mise.Core.Entities.Menu;
using NUnit.Framework;

namespace Mise.Core.Common.UnitTests.Entities
{
	[TestFixture]
	public class RestaurantCheckTests
	{
		[Test]
		public void TestCreateFromEvents(){
			var checkID = Guid.NewGuid ();
			var tab = new RestaurantCheck {Id = checkID, CreatedByServerID = Guid.Empty, LastTouchedServerID = Guid.Empty};

			Assert.AreEqual (Guid.Empty, tab.CreatedByServerID);
			Assert.AreEqual (checkID, tab.Id);
		}


		[Test]
		public void TestClone()
		{
			var checkID = Guid.NewGuid ();
			var oiFirstID = Guid.NewGuid ();
			var tab = new RestaurantCheck{
				Id = checkID,
				OrderItems = new List<OrderItem >{
					new OrderItem{
						Id = oiFirstID
					},
					new OrderItem{
						Id = Guid.NewGuid (),
						MenuItem = new MenuItem{
							Id = Guid.NewGuid ()
						}
					}
				}
			};
			var res = tab.Clone () as ICheck;

            Assert.IsNotNull(res);
			Assert.AreEqual (checkID, res.Id);
			Assert.AreEqual (2, res.OrderItems.Count ());
			Assert.AreEqual (oiFirstID, res.OrderItems.First ().Id);
		}
	}
}

