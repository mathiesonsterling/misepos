using System;
using System.Collections.Generic;
using System.Linq;
using Mise.Core.Common.Entities;
using Mise.Core.Common.UnitTests.Tools;
using Mise.Core.ValueItems;
using NUnit.Framework;
using Mise.Core.Entities;

namespace Mise.Core.Common.UnitTests.Entities.Serialization
{
    [TestFixture]
    public class EmployeeSerializationTests
    {
        [TestCase(SerializationType.JSONDOTNET)]
        [Test]
        public void TestEmpty(SerializationType type)
        {
            var serializer = SerializerFactory.GetJSONSerializer(type);
            var emp = new Employee();

            var json = serializer.Serialize(emp);
            Assert.IsFalse(string.IsNullOrEmpty(json), "empty json string");

            var res = serializer.Deserialize<Employee>(json);

            Assert.AreEqual(Guid.Empty, res.ID);
            Assert.IsNotNull(res);
        }

        [TestCase(SerializationType.JSONDOTNET)]
        [Test]
        public void TestPopulated(SerializationType type)
        {
            var serializer = SerializerFactory.GetJSONSerializer(type);

            var id = Guid.NewGuid();
			var restID = Guid.NewGuid ();
            var emp = new Employee()
            {
                ID = id,
                CompBudget = new Money(10.0M),
                CreatedDate = DateTime.Now,
                CurrentlyClockedInToPOS = true,
                DisplayName = "Test",
                Emails = new List<EmailAddress>
                {
                    new EmailAddress
                    {
                        Value = "test@test.com"
                    },
                    new EmailAddress
                    {
                        Value = "testagain@test.com"
                    }
                },
                Name = PersonName.TestName,
				RestaurantsAndAppsAllowed = new Dictionary<Guid, IList<MiseAppTypes>>{
					{restID, new List<MiseAppTypes>{MiseAppTypes.UnitTests, MiseAppTypes.DummyData}}
				}
            };

            var json = serializer.Serialize(emp);
            Assert.IsFalse(string.IsNullOrEmpty(json), "empty json string");

            var res = serializer.Deserialize<Employee>(json);

            //assert
            Assert.IsNotNull(res);
            Assert.AreEqual(id, res.ID);
            Assert.AreEqual(2, res.Emails.Count());
            Assert.IsNotNull(res.RestaurantsAndAppsAllowed.First());
            Assert.AreEqual(restID, res.RestaurantsAndAppsAllowed.First().Key);

			var useUT = res.CanUseAppForRestaurant (restID, MiseAppTypes.UnitTests);
			Assert.True (useUT);

			var useDummy = res.CanUseAppForRestaurant (restID, MiseAppTypes.DummyData);
			Assert.True (useDummy);

			var useSB = res.CanUseAppForRestaurant (restID, MiseAppTypes.StockboyMobile);
			Assert.False (useSB);
        }
    }
}
