using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Common.Entities.DTOs;
using NUnit.Framework;

using Mise.Core.Common.Entities;
using Mise.Inventory.Services.Implementation.WebServiceClients.Azure;
namespace Mise.Inventory.UnitTests.Services
{
    [TestFixture]
    public class AzureEntityStorageTests
    {
        [Test]
        public void AzureEntityStorageShouldPickupEntityID()
        {
            var source = new RestaurantEntityDataTransportObject
            {
                Id = Guid.NewGuid(),
                RestaurantID = Guid.NewGuid(),
                CreatedDate = DateTimeOffset.UtcNow,
                LastUpdatedDate = DateTimeOffset.UtcNow,
                SourceType = typeof(Employee)
            };

            //ACT
            var underTest = new AzureEntityStorage(source);

            //ASSERT
            Assert.AreEqual(source.Id, underTest.EntityID);
            Assert.AreEqual(source.RestaurantID, underTest.RestaurantID, "restID");
            Assert.AreEqual(source.LastUpdatedDate, underTest.LastUpdatedDate, "date");
        }
    }
}
