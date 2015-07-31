using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Common.Entities.Vendors;
using Mise.Core.Entities.Vendors;
using Mise.Core.Repositories;
using Mise.Core.ValueItems;
using Mise.InventoryWebService.ServiceInterface;
using Mise.InventoryWebService.ServiceModelPortable.Responses;
using Moq;
using NUnit.Framework;
using Vendor = Mise.InventoryWebService.ServiceModelPortable.Responses.Vendor;

namespace Mise.InventoryService.Tests
{
    /*
    public class TestVendorRepos : IVendorRepository
    {
        private IEnumerable<IVendor> _vendors; 
        public TestVendorRepos(IEnumerable<IVendor> vendors)
        {
            _vendors = vendors;

        }
        public EventID GetLastEventID()
        {
            throw new NotImplementedException();
        }

        public Task Load(Guid? restaurantID)
        {
            throw new NotImplementedException();
        }

        public IVendor ApplyEvent(IVendorEvent empEvent)
        {
            throw new NotImplementedException();
        }

        public IVendor ApplyEvents(IEnumerable<IVendorEvent> events)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IVendor> GetAll()
        {
            throw new NotImplementedException();
        }

        public IVendor GetByID(Guid entityID)
        {
            throw new NotImplementedException();
        }

        public Guid GetEntityID(IVendorEvent ev)
        {
            throw new NotImplementedException();
        }

        public Task<CommitResult> Commit(Guid entityID)
        {
            throw new NotImplementedException();
        }

        public Task<CommitResult> CommitOnlyImmediately(Guid entityID)
        {
            throw new NotImplementedException();
        }

        public Task<bool> CommitAll()
        {
            throw new NotImplementedException();
        }

        public void CancelTransaction(Guid entityID)
        {
            throw new NotImplementedException();
        }

        public bool StartTransaction(Guid entityID)
        {
            throw new NotImplementedException();
        }

        public bool Dirty { get; private set; }
        public Task<IEnumerable<IVendor>> GetVendorsWithinRadius(Distance radius, Location deviceLocation)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<IVendor>> GetVendorsAssociatedWithRestaurant(Guid restaurantID)
        {
            return Task.FromResult(_vendors);
        }

        public Task<IEnumerable<IVendor>> GetVendorsNotAssociatedWithRestaurantWithinRadius(Guid restaurantID, Location deviceLocation)
        {
            throw new NotImplementedException();
        }

        public Distance CurrentMaxRadius { get; private set; }
    }*/

    [TestFixture]
    public class VendorsServiceTests
    {
        [Test]
        public async Task GetWithVendorIDButNoRestaurantIDShouldNotHavePrivatePrices()
        {
            var restID = Guid.NewGuid();
            var otherRestID = Guid.NewGuid();
            var vendorID = Guid.NewGuid();
            var vendorRes = new List<IVendor>
            {
                new Core.Common.Entities.Vendors.Vendor
                {
                    ID = vendorID,
                    VendorBeverageLineItems = new List<VendorBeverageLineItem>
                    {
                        new VendorBeverageLineItem
                        {
                            PricePerUnitForRestaurant = new Dictionary<Guid, Money>
                            {
                                {restID, new Money(100.0M)},
                                {otherRestID, new Money(10.0M)}
                            }
                        }
                    }
                }
            };
            var moqRepos = new Mock<IVendorRepository>();
            moqRepos.Setup(vr => vr.GetByID(It.IsAny<Guid>()))
                .Returns(vendorRes.First());

            var underTest = new VendorService(moqRepos.Object);

            var request = new Vendor
            {
                VendorID = vendorID
            };
            //ACT
            var res = await underTest.Get(request);

            //ASSERT
            Assert.NotNull(res);
            var realRes = res as VendorResponse;
            Assert.NotNull(realRes);
            var vendors = realRes.Results.ToList();

            Assert.AreEqual(1, vendors.Count);
            var lineItems = vendors.First().GetItemsVendorSells().ToList();

            Assert.AreEqual(1, lineItems.Count);

            var li = lineItems.First() as VendorBeverageLineItem;
            Assert.NotNull(li);
            var prices = li.PricePerUnitForRestaurant;
            Assert.AreEqual(0, prices.Count);
        }

        [Test]
        public async Task GetWithVendorIDAndRestaurantIDShouldHAvePrivatePricesOnlyForThatRestaurant()
        {
            var restID = Guid.NewGuid();
            var otherRestID = Guid.NewGuid();
            var vendorID = Guid.NewGuid();
            var vendorRes = new List<IVendor>
            {
                new Core.Common.Entities.Vendors.Vendor
                {
                    ID = vendorID,
                    VendorBeverageLineItems = new List<VendorBeverageLineItem>
                    {
                        new VendorBeverageLineItem
                        {
                            PricePerUnitForRestaurant = new Dictionary<Guid, Money>
                            {
                                {restID, new Money(100.0M)},
                                {otherRestID, new Money(10.0M)}
                            }
                        }
                    }
                }
            };
            var moqRepos = new Mock<IVendorRepository>();
            moqRepos.Setup(vr => vr.GetByID(It.IsAny<Guid>()))
                .Returns(vendorRes.First());

            var underTest = new VendorService(moqRepos.Object);

            var request = new Vendor
            {
                RestaurantID = restID,
                VendorID = vendorID
            };
            //ACT
            var res = await underTest.Get(request);

            //ASSERT
            Assert.NotNull(res);
            var realRes = res as VendorResponse;
            Assert.NotNull(realRes);
            var vendors = realRes.Results.ToList();

            Assert.AreEqual(1, vendors.Count);
            var lineItems = vendors.First().GetItemsVendorSells().ToList();

            Assert.AreEqual(1, lineItems.Count);

            var li = lineItems.First() as VendorBeverageLineItem;
            Assert.NotNull(li);
            var prices = li.PricePerUnitForRestaurant;
            Assert.AreEqual(1, prices.Count);
            Assert.True(prices.Keys.Contains(restID));
            Assert.False(prices.Keys.Contains(otherRestID));
            Assert.AreEqual(prices[restID].Dollars, 100.0M);
        }
    }
}
