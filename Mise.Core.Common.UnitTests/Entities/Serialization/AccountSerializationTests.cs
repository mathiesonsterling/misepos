using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Common.Entities.Accounts;
using Mise.Core.Common.UnitTests.Tools;
using NUnit.Framework;

namespace Mise.Core.Common.UnitTests.Entities.Serialization
{
    [TestFixture]
    public class AccountSerializationTests
    {
        [TestCase(SerializationType.JSONDOTNET)]
        [Test]
        public void EmptyShouldSerialize(SerializationType type)
        {
            var serializer = SerializerFactory.GetJSONSerializer(type);
            var account = new RestaurantAccount();

            var json = serializer.Serialize(account);
            Assert.IsFalse(string.IsNullOrEmpty(json), "empty json");

            var res = serializer.Deserialize<RestaurantAccount>(json);
            Assert.IsNotNull(res);
            Assert.AreEqual(Guid.Empty, res.ID);
            Assert.AreEqual(0, res.GetPayments().Count());
        }
    }
}
