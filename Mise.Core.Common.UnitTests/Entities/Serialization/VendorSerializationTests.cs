using System;
using System.Collections.Generic;
using System.Linq;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Common.Entities.Vendors;
using Mise.Core.Common.UnitTests.Tools;
using Mise.Core.ValueItems;
using Mise.Core.ValueItems.Inventory;
using NUnit.Framework;

namespace Mise.Core.Common.UnitTests.Entities.Serialization
{
    [TestFixture]
    public class VendorSerializationTests
    {
        [TestCase(SerializationType.JSONDOTNET)]
        [Test]
        public void EmptyShouldSerialize(SerializationType type)
        {
            var serializer = SerializerFactory.GetJSONSerializer(type);
            var vendor = new Vendor();

            var json = serializer.Serialize(vendor);
            Assert.IsFalse(string.IsNullOrEmpty(json), "empty json string");

            var res = serializer.Deserialize<Vendor>(json);

            Assert.AreEqual(Guid.Empty, res.Id);
            Assert.IsNotNull(res);
        }

        [TestCase(SerializationType.JSONDOTNET)]
        [Test]
        public void TestPopulated(SerializationType type)
        {
            var serializer = SerializerFactory.GetJSONSerializer(type);

            var date = DateTime.UtcNow;
            var created = date.AddDays(-1);

            var id = Guid.NewGuid();

            var emp = new Vendor
            {
                Id = id,
                CreatedDate = created,
                LastUpdatedDate = date,
                Name = new BusinessName("testVendor").FullName,
                PhoneNumber = new PhoneNumber
                {
                    AreaCode = "718",
                    Number = "715 2945"
                },
                Revision = new EventID{OrderingID = 100},
                StreetAddress = new StreetAddress
                {
                    City = new City { Name = "Brooklyn"}
                },
                RestaurantsAssociatedIDs = new List<Guid> { Guid.Empty}

            };

            var json = serializer.Serialize(emp);
            Assert.IsFalse(string.IsNullOrEmpty(json), "empty json string");

            var res = serializer.Deserialize<Vendor>(json);

            //assert
            Assert.IsNotNull(res);
            Assert.AreEqual(id, res.Id);
            Assert.IsNotNull(res.CreatedDate);
            Assert.AreEqual(created.DayOfYear, res.CreatedDate.DayOfYear, "Created Date");

            Assert.NotNull(res.StreetAddress);
            Assert.NotNull(res.StreetAddress.City);
            Assert.AreEqual("Brooklyn", res.StreetAddress.City.Name);

            Assert.AreEqual(1, res.RestaurantsAssociatedIDs.Count());
        }
    }
}
