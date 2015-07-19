using System;
using System.Linq;
using Mise.InventoryWebService.ServiceInterface;
using Mise.InventoryWebService.ServiceModelPortable.Responses;
using NUnit.Framework;
using ServiceStack;
using ServiceStack.Testing;

namespace Mise.InventoryService.Tests
{
    /// <summary>
    /// TODO - actually use this, and make it work for our services!
    /// </summary>
    [TestFixture]
    public class UnitTests
    {
        private readonly ServiceStackHost _appHost;

        public UnitTests()
        {
            _appHost = new BasicAppHost(typeof(RestaurantService).Assembly)
            {
                ConfigureContainer = container =>
                {
                    //Add your IoC dependencies here
                }
            }
            .Init();
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            _appHost.Dispose();
        }

        [Ignore("NEeds to be actually written properly")]
        [Test]
        public void TestMethod1()
        {
            var service = _appHost.Container.Resolve<RestaurantService>();

            var response = (RestaurantResponse)service.Get(new Restaurant { RestaurantID = Guid.Empty });

            Assert.That(response.Results.Count(), Is.EqualTo(1));
        }
    }
}
