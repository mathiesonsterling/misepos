using System;
using Mise.Core.Common.Entities;
using Mise.Core.Common.UnitTests.Tools;
using NUnit.Framework;
using Mise.Core.ValueItems;

namespace Mise.Core.Common.UnitTests.Entities.Serialization
{
    [TestFixture]
    public class MiseTerminalDeviceSerializationTests
    {
        [TestCase(SerializationType.JSONDOTNET)]
        [Test]
        public void TestEmpty(SerializationType type)
        {
            var serializer = SerializerFactory.GetJSONSerializer(type);

            var mtd = new MiseTerminalDevice();

            var json = serializer.Serialize(mtd);

            Assert.IsFalse(string.IsNullOrEmpty(json), "empty json string");

            var res = serializer.Deserialize<MiseTerminalDevice>(json);

            Assert.IsNotNull(res);
        }

        [TestCase(SerializationType.JSONDOTNET)]
        [Test]
        public void TestSetupVals(SerializationType type)
        {
            var serializer = SerializerFactory.GetJSONSerializer(type);

            var id = Guid.NewGuid();
            var mtd = new MiseTerminalDevice
            {
                ID = id,

            };

            var json = serializer.Serialize(mtd);

            Assert.IsFalse(string.IsNullOrEmpty(json), "empty json string");

            var res = serializer.Deserialize<MiseTerminalDevice>(json);

            Assert.IsNotNull(res);
            Assert.AreEqual(id, res.ID);

        }

        [TestCase(SerializationType.JSONDOTNET)]
        [Test]
        public void TestPopulated(SerializationType type)
        {
            var serializer = SerializerFactory.GetJSONSerializer(type);

            var id = Guid.NewGuid();
            var mtd = new MiseTerminalDevice
            {
                TopLevelCategoryID = Guid.Empty,
                CreatedDate = DateTime.Now,
                ID = id,
                RequireEmployeeSignIn = false,
                TableDropChecks = false,
                CreditCardReaderType = CreditCardReaderType.CameraReader,
                HasCashDrawer = true,
                RestaurantID = Guid.NewGuid()
            };

            var json = serializer.Serialize(mtd);

            Assert.IsFalse(string.IsNullOrEmpty(json), "empty json string");

            var res = serializer.Deserialize<MiseTerminalDevice>(json);

            Assert.IsNotNull(res);
			Assert.AreEqual (CreditCardReaderType.CameraReader, res.CreditCardReaderType);
        }
    }
}
