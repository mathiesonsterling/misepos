using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Common.Entities.DTOs;
using Mise.Core.Common.Entities.DTOs.AzureTypes;
using NUnit.Framework;

namespace Mise.Core.Common.UnitTests.Entities.Serialization
{
    [TestFixture]
    public class AzureEntityStorageTests
    {
        [Test]
        public void AzureEntityStorageShouldPickupEntityID()
        {
            var source = new RestaurantEntityDataTransportObject
            {
                ID = Guid.NewGuid(),
                RestaurantID = Guid.NewGuid(),
                CreatedDate = DateTimeOffset.UtcNow,
                LastUpdatedDate = DateTimeOffset.UtcNow,
                SourceType = typeof(Common.Entities.Employee)
            };

            //ACT
            var underTest = new AzureEntityStorage(source);

            //ASSERT
            Assert.AreEqual(source.ID, underTest.EntityID);
            Assert.AreEqual(source.RestaurantID, underTest.RestaurantID, "restID");
            Assert.AreEqual(source.LastUpdatedDate, underTest.LastUpdatedDate, "date");
        }
    }
}
