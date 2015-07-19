using System;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Common;
using Mise.Core.Common.Entities;
using Mise.Core.Common.Entities.Vendors;
using Mise.Core.Entities;
using Mise.Core.Server;
using Mise.Core.Server.Services.DAL;
using Mise.Core.ValueItems;
using NUnit.Framework;

namespace Mise.Database.Neo4J.IntegrationTests
{
    public static class TestUtilities
    {
        private const string TEST_DB =
          "http://miseunittest:22Q80ht30EU6mPxJ3DVM@miseunittest.sb02.stations.graphenedb.com:24789/db/data/";

       // private const string TEST_DB =
           // "http://miseinventoryprod:x3MPvwDUdKpd9aPU1EfN@miseinventoryprod.sb07.stations.graphenedb.com:24789/db/data/";
        public static int LastOrderingID =10;

        public static IConfig GetConfig()
        {
            return new TestConfig();
        }

        public class TestConfig : IConfig
        {
            public Uri Neo4JConnectionDBUri
            {
                get
                {
                    return new Uri(TEST_DB);
                }
            }
        }
        public static EventID GetEventID()
        {
            return new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = LastOrderingID++};
        }

        public static Task LoadCategories(IEntityDAL dal)
        {
            var cats = new CategoriesService().GetIABIngredientCategories();
            return dal.CreateCategories(cats.ToList());
        }

        public static Restaurant CreateRestaurant()
        {
            var rest = new Restaurant
            {
                ID = Guid.NewGuid(),
                AccountID = null,
                Revision = new EventID { AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 1 },
                Name = RestaurantName.TestName,
            };
            return rest;
        }

        public static Employee CreateEmployee()
        {
            return new Employee
            {
                ID = Guid.NewGuid(),
                Revision = new EventID { AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 2},
                Name = PersonName.TestName
            };
        }

        public static Vendor CreateVendor()
        {
            return new Vendor
            {
                ID = Guid.NewGuid(),
                Revision = new EventID {AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 3}
            };
        }
        public static void AssertAddressesEqual(StreetAddress good, StreetAddress toCompare)
        {
            Assert.AreEqual(good.StreetAddressNumber.Direction, toCompare.StreetAddressNumber.Direction, "direction");
            Assert.AreEqual(good.StreetAddressNumber.Latitude, toCompare.StreetAddressNumber.Latitude, "lat");
            Assert.AreEqual(good.StreetAddressNumber.Longitude, toCompare.StreetAddressNumber.Longitude, "long");
            Assert.AreEqual(good.StreetAddressNumber.Number, toCompare.StreetAddressNumber.Number, "number");
            Assert.IsTrue(good.StreetAddressNumber.Equals(toCompare.StreetAddressNumber), "SAN");

            Assert.IsTrue(good.Zip.Equals(toCompare.Zip), "Zip");
            Assert.IsTrue(good.Equals(toCompare), "Equality on ValueItem");
        }
    }
}
