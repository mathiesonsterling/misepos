using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Mise.Core.Common.Entities.Vendors;
using Mise.Core.ValueItems;
using Mise.Core.Common.Events.Vendors;

namespace Mise.Core.Common.UnitTests.Entities.Vendors
{
    [TestFixture]
    public class VendorTests
    {
		[Test]
		public void CreationEventShouldPopulateFields(){

			var createdEvent = new VendorCreatedEvent {
				VendorID = Guid.NewGuid (),
				CausedByID = Guid.NewGuid (),
				CreatedDate = DateTime.UtcNow
			};

			var underTest = new Vendor ();

			//ACT
			underTest.When (createdEvent);

			//ASSERT
			Assert.AreEqual (createdEvent.VendorID, underTest.ID, "ID");
			Assert.AreEqual (createdEvent.CreatedDate, underTest.CreatedDate, "Created Date");
			Assert.AreEqual (createdEvent.CreatedDate, underTest.LastUpdatedDate, "last updated");
            Assert.AreEqual(createdEvent.CausedByID, underTest.CreatedByEmployeeID, "EmployeeID");
            Assert.AreEqual(createdEvent.CreatedDate, underTest.LastUpdatedDate);
            Assert.AreEqual(createdEvent.EventOrderingID, underTest.Revision);
		}

        [Test]
        public void SetAddressEventShouldSet()
        {
            var underTest = new Vendor();
            var addressEvent = new VendorAddressUpdatedEvent
            {
                StreetAddress = new StreetAddress
                {
                    StreetAddressNumber = new StreetAddressNumber {Number = "699", ApartmentNumber = "5A"},
                    Street = new Street {Name = "Ocean Avenue"},
                    City = new City {Name = "Brooklyn"},
                    State = new State {Name = "New York"},
                    Zip = new ZipCode {Value = "11226"},
                    Country = new Country {Name = "United States of America"}
                }
            };

            //ACT
            underTest.When(addressEvent);

            //ASSERT
            Assert.NotNull(underTest.StreetAddress);
            Assert.AreEqual(addressEvent.StreetAddress, underTest.StreetAddress, "StreetAddress object equality");

            Assert.AreEqual("5A", underTest.StreetAddress.StreetAddressNumber.ApartmentNumber);
            Assert.AreEqual("699", underTest.StreetAddress.StreetAddressNumber.Number);
            Assert.AreEqual("Ocean Avenue", underTest.StreetAddress.Street.Name);
            Assert.AreEqual("Brooklyn", underTest.StreetAddress.City.Name);

            Assert.AreEqual(addressEvent.CreatedDate, underTest.LastUpdatedDate);
            Assert.AreEqual(addressEvent.EventOrderingID, underTest.Revision);
        }

        [Test]
        public void PhoneNumberShouldSet()
        {
            var underTest = new Vendor();
            var phoneEvent = new VendorPhoneNumberUpdatedEvent
            {
                PhoneNumber = new PhoneNumber
                {
                    AreaCode = "718",
                    Number = "715-2945"
                }
            };

            //ACT
            underTest.When(phoneEvent);

            //ASSERT
            Assert.NotNull(underTest.PhoneNumber);
            Assert.AreEqual(phoneEvent.PhoneNumber, underTest.PhoneNumber);
        }

        [Test]
        public void RestaurantAssociationEventShouldAddRestaurant()
        {
            var underTest = new Vendor();

            var restID = Guid.NewGuid();
            var associateRestaurantEvent = new RestaurantAssociatedWithVendorEvent
            {
                RestaurantID = restID
            };

            //ACT
            underTest.When(associateRestaurantEvent);
            underTest.When(associateRestaurantEvent);

            var res = underTest.GetRestaurantIDsAssociatedWithVendor().ToList();

            //ASSERT
            Assert.NotNull(res);
            Assert.AreEqual(1, res.Count());
            Assert.AreEqual(restID, res.First());
        }

        [Test]
        public void SettingPriceShouldSetForRestaurant()
        {
            var liId = Guid.NewGuid();
            var restID = Guid.NewGuid();

            var underTest = new Vendor
            {
                VendorBeverageLineItems = new List<VendorBeverageLineItem>
                {
                    new VendorBeverageLineItem
                    {
                        ID = Guid.NewGuid()
                    },
                    new VendorBeverageLineItem
                    {
                        ID = liId,
                    },
                    new VendorBeverageLineItem
                    {
                        ID = Guid.NewGuid(),
                    }
                }
            };

            var costReported = new Money(10.99M);
            var ev = new VendorRestaurantSetsPriceForReceivedItemEvent
            {
                VendorLineItemID = liId,
                PricePerUnit = costReported,
                RestaurantID = restID
            };

            //ACT
            underTest.When(ev);
            var li = underTest.VendorBeverageLineItems.FirstOrDefault(l => liId == l.ID);

            //ASSERT
            Assert.NotNull(li);
            Assert.AreEqual(costReported, li.GetLastPricePaidByRestaurantPerUnit(restID));
            Assert.NotNull(li.LastTimePriceSet);
            Assert.Null(li.PublicPricePerUnit);
        }

        [Test]
        public void SettingPriceShouldSetOverrideForRestaurant()
        {
            var liId = Guid.NewGuid();
            var restID = Guid.NewGuid();
            var origDate = DateTimeOffset.UtcNow.AddYears(-2);
            var underTest = new Vendor
            {
                VendorBeverageLineItems = new List<VendorBeverageLineItem>
                {
                    new VendorBeverageLineItem
                    {
                        ID = liId,
                        PricePerUnitForRestaurant = new Dictionary<Guid, Money>{
                            {restID, new Money(100)}
                        },
                        LastTimePriceSet = origDate
                    },

                }
            };

            var costReported = new Money(10.99M);
            var ev = new VendorRestaurantSetsPriceForReceivedItemEvent
            {
                VendorLineItemID = liId,
                PricePerUnit = costReported,
                RestaurantID = restID
            };

            //ACT
            underTest.When(ev);
            var li = underTest.VendorBeverageLineItems.FirstOrDefault();

            //ASSERT
            Assert.NotNull(li);
            Assert.AreEqual(costReported.Dollars, li.GetLastPricePaidByRestaurantPerUnit(restID).Dollars);
            Assert.Greater(li.LastTimePriceSet, origDate);
        }
    }
}
