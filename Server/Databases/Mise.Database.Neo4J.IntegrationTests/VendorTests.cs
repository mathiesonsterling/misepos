using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Common.Entities.Vendors;
using Mise.Core.Entities;
using Mise.Core.Entities.Vendors;
using Mise.Core.Services;
using Mise.Core.Services.UtilityServices;
using Mise.Core.ValueItems;
using Mise.Core.ValueItems.Inventory;
using Mise.Neo4J;
using Mise.Neo4J.Neo4JDAL;
using Moq;
using NUnit.Framework;

namespace Mise.Database.Neo4J.IntegrationTests
{
    [TestFixture]
    public class VendorTests
    {
        private Neo4JEntityDAL _underTest;

        [SetUp]
        public void Setup()
        {
            var logger = new Mock<ILogger>();
            _underTest = new Neo4JEntityDAL(TestUtilities.GetConfig(), logger.Object);

            _underTest.ResetDatabase();
        }

        [Test]
        public async Task VendorShouldDoBasicCRUD()
        {
            var rest = TestUtilities.CreateRestaurant();
            await _underTest.AddRestaurantAsync(rest);

            var emp = TestUtilities.CreateEmployee();
            emp.RestaurantsAndAppsAllowed.Add(rest.ID, new List<MiseAppTypes> { MiseAppTypes.UnitTests });
            await _underTest.AddEmployeeAsync(emp);

            var lineItemID = Guid.NewGuid();
            var vendorID = Guid.NewGuid();

            var vendor = new Vendor
            {
                CreatedByEmployeeID = emp.ID,
                CreatedDate = DateTime.Now,
                ID = vendorID,
                LastUpdatedDate = DateTime.Now,
                Name = "testVendor",
                PhoneNumber = new PhoneNumber { AreaCode = "718", Number = "222-1111" },
                RestaurantsAssociatedIDs = new List<Guid> { rest.ID },
                Revision = new EventID { AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 1919 },
                EmailToOrderFrom = new EmailAddress("orders@boozer.com"),
                StreetAddress = new StreetAddress
                {
                    StreetAddressNumber = new StreetAddressNumber { Direction = "N", Number = "6021", Latitude = 40.64, Longitude = -73.99939392 },
                    City = new City { Name = "Brooklyn" },
                    Country = new Country { Name = "United States" },
                    State = new State { Name = "New York" },
                    Street = new Street { Name = "Ocean Ave" },
                    Zip = new ZipCode { Value = "11226" }
                },
                VendorBeverageLineItems = new List<VendorBeverageLineItem>
                {
                    new VendorBeverageLineItem
                    {
                        Container =
                            new LiquidContainer
                            {
                                AmountContained = new LiquidAmount {Milliliters = 750, SpecificGravity = 211}
                            },
                        CreatedDate = DateTime.Now,
                        ID = lineItemID,
                        LastUpdatedDate = DateTime.Now,
                        MiseName = "testLin",
                        NameInVendor = "superstuff",
                        PublicPricePerUnit= new Money(101.0M),
                        Revision = new EventID {AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
                        UPC = "1111",
                        VendorID = vendorID
                    }
                }
            };

            await _underTest.AddVendorAsync(vendor);

            var getRes = (await _underTest.GetVendorsAsync()).ToList();
            Assert.AreEqual(1, getRes.Count());

            var foundVendor = getRes.First();

            //test all fields
            Assert.AreEqual(vendor.CreatedByEmployeeID, foundVendor.CreatedByEmployeeID, "EmployeeID");
            Assert.AreEqual(vendor.CreatedDate, foundVendor.CreatedDate, "CreatedDate");
            Assert.AreEqual(vendor.ID, foundVendor.ID);
            Assert.AreEqual(vendor.LastUpdatedDate, foundVendor.LastUpdatedDate, "lastUpdated");
            Assert.AreEqual(vendor.Name, foundVendor.Name);
            Assert.AreEqual(vendor.Verified, foundVendor.Verified);
            Assert.True(vendor.PhoneNumber.Equals(foundVendor.PhoneNumber), "Phone Number");

            //test email
            Assert.NotNull(foundVendor.EmailToOrderFrom);
            Assert.True(vendor.EmailToOrderFrom.Equals(foundVendor.EmailToOrderFrom));

            //test the street
            Assert.NotNull(foundVendor.StreetAddress);
            TestUtilities.AssertAddressesEqual(vendor.StreetAddress, foundVendor.StreetAddress);

            //test line items
            var lineItems = foundVendor.GetItemsVendorSells().ToList();
            Assert.AreEqual(1, lineItems.Count());
            var origLineItem = vendor.GetItemsVendorSells().First();
            var foundLineItem = lineItems.First();
            Assert.AreEqual("1111", foundLineItem.UPC);
            Assert.AreEqual(origLineItem.CreatedDate, foundLineItem.CreatedDate);

            Assert.NotNull(foundLineItem.Container);
            Assert.NotNull(foundLineItem.Container.AmountContained);
            Assert.AreEqual(origLineItem.Container.AmountContained.Milliliters, foundLineItem.Container.AmountContained.Milliliters, "milliliters");

            //test restaurants
            var restIDs = foundVendor.GetRestaurantIDsAssociatedWithVendor().ToList();
            Assert.AreEqual(1, restIDs.Count());
            Assert.AreEqual(rest.ID, restIDs.First());

            //UPDATE
            var secondRest = TestUtilities.CreateRestaurant();
            await _underTest.AddRestaurantAsync(secondRest);

            var updatedVendor = new Vendor
            {
                CreatedByEmployeeID = emp.ID,
                CreatedDate = DateTime.Now,
                ID = vendorID,
                LastUpdatedDate = DateTime.Now,
                Name = "testVendorRenamed",
                PhoneNumber = new PhoneNumber { AreaCode = "718", Number = "555-5555" },
                RestaurantsAssociatedIDs = new List<Guid> { rest.ID, secondRest.ID },
                Revision = new EventID { AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 1919 },
                StreetAddress = new StreetAddress
                {
                    StreetAddressNumber = new StreetAddressNumber { Direction = "N", Number = "6021", Latitude = 40.64, Longitude = -73.99939392 },
                    City = new City { Name = "Brooklyn" },
                    Country = new Country { Name = "United States" },
                    State = new State { Name = "New York" },
                    Street = new Street { Name = "Ocean Ave" },
                    Zip = new ZipCode { Value = "11226" }
                },
                VendorBeverageLineItems = new List<VendorBeverageLineItem>
                {
                    new VendorBeverageLineItem
                    {
                        Container =
                            new LiquidContainer
                            {
                                AmountContained = new LiquidAmount {Milliliters = 375}
                            },
                        CreatedDate = DateTime.Now,
                        ID = lineItemID,
                        LastUpdatedDate = DateTime.Now,
                        MiseName = "testLin",
                        NameInVendor = "superstuff",
                        PublicPricePerUnit = new Money(101.0M),
                        Revision = new EventID {AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
                        UPC = "1111",
                        VendorID = vendorID
                    }
                }
            };

            await _underTest.UpdateVendorAsync(updatedVendor);
            var afterUpdate = (await _underTest.GetVendorsAsync()).ToList();

            Assert.AreEqual(1, afterUpdate.Count(), "num after update");

            var auVendor = afterUpdate.First();
            Assert.NotNull(auVendor);
            Assert.IsTrue(updatedVendor.PhoneNumber.Equals(auVendor.PhoneNumber));
            Assert.AreEqual(updatedVendor.Name, auVendor.Name);

            var aulineItems = auVendor.GetItemsVendorSells().ToList();
            Assert.NotNull(aulineItems);
            Assert.AreEqual(1, aulineItems.Count(), "num line items");

            var auLineItem = aulineItems.First();
            Assert.NotNull(auLineItem);
            Assert.NotNull(auLineItem.Container, "Container is null after update");
            Assert.AreEqual(375, auLineItem.Container.AmountContained.Milliliters, "Updated Amount Contained ml");
        }

        [Test]
        public async Task VendorShouldAddPricesPerRestaurant()
        {
            var rest = TestUtilities.CreateRestaurant();
            await _underTest.AddRestaurantAsync(rest);

            var emp = TestUtilities.CreateEmployee();
            emp.RestaurantsAndAppsAllowed.Add(rest.ID, new List<MiseAppTypes> { MiseAppTypes.UnitTests });
            await _underTest.AddEmployeeAsync(emp);

            var lineItemID = Guid.NewGuid();
            var vendorID = Guid.NewGuid();

            var vendor = new Vendor
            {
                CreatedByEmployeeID = emp.ID,
                CreatedDate = DateTime.Now,
                ID = vendorID,
                LastUpdatedDate = DateTime.Now,
                Name = "testVendor",
                PhoneNumber = new PhoneNumber { AreaCode = "718", Number = "222-1111" },
                RestaurantsAssociatedIDs = new List<Guid> { rest.ID },
                Revision = new EventID { AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 1919 },
                StreetAddress = new StreetAddress
                {
                    StreetAddressNumber = new StreetAddressNumber { Direction = "N", Number = "6021", Latitude = 40.64, Longitude = -73.99939392 },
                    City = new City { Name = "Brooklyn" },
                    Country = new Country { Name = "United States" },
                    State = new State { Name = "New York" },
                    Street = new Street { Name = "Ocean Ave" },
                    Zip = new ZipCode { Value = "11226" }
                },
                VendorBeverageLineItems = new List<VendorBeverageLineItem>
                {
                    new VendorBeverageLineItem
                    {
                        Container =
                            new LiquidContainer
                            {
                                AmountContained = new LiquidAmount {Milliliters = 750, SpecificGravity = 211}
                            },
                        CreatedDate = DateTime.Now,
                        ID = lineItemID,
                        LastUpdatedDate = DateTime.Now,
                        MiseName = "testLin",
                        NameInVendor = "superstuff",
                        PublicPricePerUnit= new Money(101.0M),
                        Revision = new EventID {AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
                        UPC = "1111",
                        VendorID = vendorID,
                        PricePerUnitForRestaurant = new Dictionary<Guid, Money>
                        {
                            {rest.ID, new Money(10.0M)}
                        }
                    }
                }
            };

            await _underTest.AddVendorAsync(vendor);

            var getRes = (await _underTest.GetVendorsAsync()).ToList();
            Assert.AreEqual(1, getRes.Count());

            var foundVendor = getRes.First();

            //test line items
            var lineItems = foundVendor.GetItemsVendorSells().ToList();
            Assert.AreEqual(1, lineItems.Count());
            var origLineItem = vendor.GetItemsVendorSells().First();
            var foundLineItem = lineItems.First();
            Assert.AreEqual("1111", foundLineItem.UPC);
            Assert.AreEqual(origLineItem.CreatedDate, foundLineItem.CreatedDate);
            Assert.AreEqual(origLineItem.GetLastPricePaidByRestaurantPerUnit(rest.ID), foundLineItem.GetLastPricePaidByRestaurantPerUnit(rest.ID), "price paid per unit");

            //test restaurants
            var restIDs = foundVendor.GetRestaurantIDsAssociatedWithVendor().ToList();
            Assert.AreEqual(1, restIDs.Count());
            Assert.AreEqual(rest.ID, restIDs.First());

            //UPDATE
            var secondRest = TestUtilities.CreateRestaurant();
            await _underTest.AddRestaurantAsync(secondRest);

            var updatedVendor = new Vendor
            {
                CreatedByEmployeeID = emp.ID,
                CreatedDate = DateTime.Now,
                ID = vendorID,
                LastUpdatedDate = DateTime.Now,
                Name = "testVendorRenamed",
                PhoneNumber = new PhoneNumber { AreaCode = "718", Number = "555-5555" },
                RestaurantsAssociatedIDs = new List<Guid> { rest.ID, secondRest.ID },
                Revision = new EventID { AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 1919 },
                StreetAddress = new StreetAddress
                {
                    StreetAddressNumber = new StreetAddressNumber { Direction = "N", Number = "6021", Latitude = 40.64, Longitude = -73.99939392 },
                    City = new City { Name = "Brooklyn" },
                    Country = new Country { Name = "United States" },
                    State = new State { Name = "New York" },
                    Street = new Street { Name = "Ocean Ave" },
                    Zip = new ZipCode { Value = "11226" }
                },
                VendorBeverageLineItems = new List<VendorBeverageLineItem>
                {
                    new VendorBeverageLineItem
                    {
                        Container =
                            new LiquidContainer
                            {
                                AmountContained = new LiquidAmount {Milliliters = 375}
                            },
                        CreatedDate = DateTime.Now,
                        ID = lineItemID,
                        LastUpdatedDate = DateTime.Now,
                        MiseName = "testLin",
                        NameInVendor = "superstuff",
                        PublicPricePerUnit = new Money(101.0M),
                        Revision = new EventID {AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
                        UPC = "1111",
                        VendorID = vendorID,
                        PricePerUnitForRestaurant = new Dictionary<Guid, Money>
                        {
                            {secondRest.ID, new Money(99.99M)}
                        }
                    }
                }
            };

            await _underTest.UpdateVendorAsync(updatedVendor);
            var afterUpdate = (await _underTest.GetVendorsAsync()).ToList();

            Assert.AreEqual(1, afterUpdate.Count(), "num after update");

            var auVendor = afterUpdate.First();
            Assert.NotNull(auVendor);

            var aulineItems = auVendor.GetItemsVendorSells().ToList();
            Assert.NotNull(aulineItems);
            Assert.AreEqual(1, aulineItems.Count(), "num line items");

            var auLineItem = aulineItems.First();
            Assert.NotNull(auLineItem);
            Assert.AreEqual(auLineItem.GetLastPricePaidByRestaurantPerUnit(secondRest.ID).Dollars, 99.99M);
        }

        [Test]
        public async Task VendorCanCreateWithoutStreetAddress()
        {
            var vendor = new Vendor
            {
                ID = Guid.NewGuid(),
                Revision = new EventID { AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 10 },
                StreetAddress = null,
            };

            //ACT
            await _underTest.AddVendorAsync(vendor);
            var res = await _underTest.GetVendorsAsync();

            //ASSERT
            var vendorList = res as IList<IVendor> ?? res.ToList();
            Assert.AreEqual(1, vendorList.Count());
            Assert.IsNull(vendorList.First().StreetAddress);
        }

        [Test]
        public async Task MultipleCopiesOfVendorOnSystemDontCrash()
        {
            var rest = TestUtilities.CreateRestaurant();
            await _underTest.AddRestaurantAsync(rest);

            var emp = TestUtilities.CreateEmployee();
            var vendor1ID = Guid.NewGuid();
            var vendor2ID = Guid.NewGuid();

            emp.RestaurantsAndAppsAllowed.Add(rest.ID, new List<MiseAppTypes> {MiseAppTypes.UnitTests});
            await _underTest.AddEmployeeAsync(emp);
            var vendors = new List<IVendor>
            {
                new Vendor
                {
                    CreatedByEmployeeID = emp.ID,
                    CreatedDate = DateTime.Now,
                    ID = vendor1ID,
                    LastUpdatedDate = DateTime.Now,
                    Name = "testVendor",
                    PhoneNumber = new PhoneNumber {AreaCode = "718", Number = "222-1111"},
                    RestaurantsAssociatedIDs = new List<Guid> {rest.ID},
                    Revision = new EventID {AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 1919},
                    StreetAddress = new StreetAddress
                    {
                        StreetAddressNumber =
                            new StreetAddressNumber
                            {
                                Direction = "N",
                                Number = "6021",
                                Latitude = 40.64,
                                Longitude = -73.99939392
                            },
                        City = new City {Name = "Brooklyn"},
                        Country = new Country {Name = "United States"},
                        State = new State {Name = "New York"},
                        Street = new Street {Name = "Ocean Ave"},
                        Zip = new ZipCode {Value = "11226"}
                    },
                    VendorBeverageLineItems = new List<VendorBeverageLineItem>
                    {
                        new VendorBeverageLineItem
                        {
                            Container =
                                new LiquidContainer
                                {
                                    AmountContained = new LiquidAmount {Milliliters = 750, SpecificGravity = 211}
                                },
                            CreatedDate = DateTime.Now,
                            ID = Guid.NewGuid(),
                            LastUpdatedDate = DateTime.Now,
                            MiseName = "testLin",
                            NameInVendor = "superstuff",
                            PublicPricePerUnit = new Money(101.0M),
                            Revision = new EventID {AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
                            UPC = "1111",
                            VendorID = vendor1ID,
                            PricePerUnitForRestaurant = new Dictionary<Guid, Money>
                            {
                                {rest.ID, new Money(10.0M)}
                            }
                        }
                    }
                },
                                new Vendor
                {
                    CreatedByEmployeeID = emp.ID,
                    CreatedDate = DateTime.Now,
                    ID = vendor2ID,
                    LastUpdatedDate = DateTime.Now,
                    Name = "testVendor",
                    PhoneNumber = new PhoneNumber {AreaCode = "718", Number = "222-1111"},
                    RestaurantsAssociatedIDs = new List<Guid> {rest.ID},
                    Revision = new EventID {AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 1919},
                    StreetAddress = new StreetAddress
                    {
                        StreetAddressNumber =
                            new StreetAddressNumber
                            {
                                Direction = "N",
                                Number = "6021",
                                Latitude = 40.64,
                                Longitude = -73.99939392
                            },
                        City = new City {Name = "Brooklyn"},
                        Country = new Country {Name = "United States"},
                        State = new State {Name = "New York"},
                        Street = new Street {Name = "Ocean Ave"},
                        Zip = new ZipCode {Value = "11226"}
                    },
                    VendorBeverageLineItems = new List<VendorBeverageLineItem>
                    {
                        new VendorBeverageLineItem
                        {
                            Container =
                                new LiquidContainer
                                {
                                    AmountContained = new LiquidAmount {Milliliters = 750, SpecificGravity = 211}
                                },
                            CreatedDate = DateTime.Now,
                            ID = Guid.NewGuid(),
                            LastUpdatedDate = DateTime.Now,
                            MiseName = "testLin",
                            NameInVendor = "superstuff",
                            PublicPricePerUnit = new Money(101.0M),
                            Revision = new EventID {AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 101},
                            UPC = "1111",
                            VendorID = vendor2ID,
                            PricePerUnitForRestaurant = new Dictionary<Guid, Money>
                            {
                                {rest.ID, new Money(10.0M)}
                            }
                        }
                    }
                }
            };

            //ACT
            foreach (var v in vendors)
            {
                await _underTest.AddVendorAsync(v);
            }
            Assert.True(true, "Added both");

            var res = (await _underTest.GetVendorsAsync()).ToList();

            //ASSERT
            Assert.NotNull(res);
            Assert.AreEqual(2, res.Count);

            var vendIDs = res.Select(v => v.ID).ToList();
            Assert.True(vendIDs.Contains(vendor1ID));
            Assert.True(vendIDs.Contains(vendor2ID));
        }
    }
}
