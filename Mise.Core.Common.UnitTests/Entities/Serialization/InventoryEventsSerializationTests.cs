using System;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using Mise.Core.Common.Events.Inventory;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Common.UnitTests.Entities.Serialization;
using Mise.Core.Common.UnitTests.Tools;

namespace Mise.Core.Common.UnitTests
{
	[TestFixture]
	public class InventoryEventsSerializationTests
	{
		[TestCase(SerializationType.JSONDOTNET)]
		[Test]
		public void TestInventoryCreatedEvent(SerializationType type){
			var serializer = SerializerFactory.GetJSONSerializer(type);
			var ev = new InventoryCreatedEvent {
				ID = Guid.NewGuid (),
				RestaurantSectionsAndSectionIDs = new List<Tuple<RestaurantInventorySection, Guid>>
                 {
                     new Tuple<RestaurantInventorySection, Guid>(
					new RestaurantInventorySection {
						ID = Guid.NewGuid (),
						Name = "testSection"
					}, Guid.NewGuid())
				}
			};

			//ACT
			var json = serializer.Serialize (ev);
			Assert.False (string.IsNullOrEmpty (json));

			var res = serializer.Deserialize<InventoryCreatedEvent> (json);
			Assert.NotNull (res);
			Assert.AreEqual (ev.ID, res.ID);
			Assert.NotNull (res.RestaurantSectionsAndSectionIDs);
			Assert.AreEqual (1, res.RestaurantSectionsAndSectionIDs.Count());
		}
	}
}

