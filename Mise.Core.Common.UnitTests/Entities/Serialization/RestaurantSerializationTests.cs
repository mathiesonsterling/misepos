﻿using System;
using System.Collections.Generic;
using System.Linq;
using Mise.Core.Common.Entities;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Common.UnitTests.Tools;
using Mise.Core.Entities.Payments;
using Mise.Core.ValueItems;
using Mise.Core.Entities;
using NUnit.Framework;

namespace Mise.Core.Common.UnitTests.Entities.Serialization
{
    [TestFixture]
    public class RestaurantSerializationTests
    {
        [TestCase(SerializationType.JSONDOTNET)]
        [Test]
        public void TestEmpty(SerializationType type)
        {
            var serializer = SerializerFactory.GetJSONSerializer(type);

            var rest = new Restaurant
            {
                Id = Guid.NewGuid()
            };

            var json = serializer.Serialize(rest);
            Assert.IsFalse(string.IsNullOrEmpty(json), "empty json string");

            var res = serializer.Deserialize<Restaurant>(json);

            Assert.AreNotEqual(Guid.Empty, res.Id);
            Assert.IsNotNull(res);
        }

        [TestCase(SerializationType.JSONDOTNET)]
        [Test]
        public void TestPopulated(SerializationType type)
        {
            var serializer = SerializerFactory.GetJSONSerializer(type);

            var id = Guid.NewGuid();
            var actId = Guid.NewGuid();
            var createdDate = DateTimeOffset.UtcNow.AddDays(-2);
            var updateDate = DateTimeOffset.UtcNow;
            var rest = new Restaurant
            {
                Id = id,
                AccountID = actId,
                CreatedDate = createdDate,
                LastUpdatedDate = updateDate,
                Name = new BusinessName("testRest"),
                PhoneNumber = new PhoneNumber
                {
                    AreaCode = "718",
                    Number = "1112222"
                },
                Revision = new EventID { OrderingID = 100 },
                StreetAddress = new StreetAddress
                {
                    City = new City { Name = "Brooklyn" },
                    Country = new Country { Name = "United States of America" },
                    State = new State { Name = "NY" },
                    StreetAddressNumber = new StreetAddressNumber
                    {
                        Number = "699",
                        Latitude = 100.0,
                        Longitude = 100.0,
						ApartmentNumber = "5A",
                    },
                    Street = new Street { Name = "Ocean Ave" },
                    Zip = new ZipCode { Value = "11226" }
                },
                InventorySections = new List<RestaurantInventorySection>
                {
                    new RestaurantInventorySection
                    {
                        Id = Guid.NewGuid(),
                        Name = "mainBar",
                        Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
                    },
                    new RestaurantInventorySection
                    {
                        Id = Guid.NewGuid(),
                        Name = "stockRoom"
                    }
                }
            };

            var json = serializer.Serialize(rest);
            Assert.IsFalse(string.IsNullOrEmpty(json), "empty json string");

            var res = serializer.Deserialize<Restaurant>(json);

            Assert.AreNotEqual(Guid.Empty, res.Id);
            Assert.IsNotNull(res);

            Assert.AreEqual(id, res.Id);
            Assert.AreEqual(actId, res.AccountID);
            Assert.AreEqual(createdDate, res.CreatedDate);
            Assert.AreEqual(updateDate, res.LastUpdatedDate);
            Assert.AreEqual("testRest", res.Name.FullName);
            Assert.AreEqual(MiseAppTypes.UnitTests, res.Revision.AppInstanceCode);
            Assert.IsNotNull(res.PhoneNumber);
            Assert.AreEqual("718", res.PhoneNumber.AreaCode);
            Assert.AreEqual("1112222", res.PhoneNumber.Number);
            Assert.IsNotNull(res.StreetAddress);
            Assert.AreEqual("5A", res.StreetAddress.StreetAddressNumber.ApartmentNumber);
            Assert.AreEqual("Brooklyn", res.StreetAddress.City.Name);
            Assert.AreEqual(100.0, res.StreetAddress.StreetAddressNumber.Latitude);
            Assert.AreEqual(100.0, res.StreetAddress.StreetAddressNumber.Longitude);
            Assert.AreEqual("NY", res.StreetAddress.State.Name);
            Assert.AreEqual("699", res.StreetAddress.StreetAddressNumber.Number);
            Assert.AreEqual("Ocean Ave", res.StreetAddress.Street.Name);
            Assert.AreEqual("11226", res.StreetAddress.Zip.Value);

            var inventorySections = res.GetInventorySections().ToList();
            Assert.NotNull(inventorySections);
            Assert.AreEqual(2, inventorySections.Count);
        }

        [TestCase(SerializationType.JSONDOTNET)]
        [Test]
        public void TestWithDiscounts(SerializationType type)
        {
            var serializer = SerializerFactory.GetJSONSerializer(type);


            var rest = new Restaurant
            {
                Id = Guid.NewGuid(),

            };


            var json = serializer.Serialize(rest);
            Assert.IsFalse(string.IsNullOrEmpty(json), "empty json string");

            var res = serializer.Deserialize<Restaurant>(json);

            Assert.AreNotEqual(Guid.Empty, res.Id);
            Assert.IsNotNull(res);
        }
    }
}
