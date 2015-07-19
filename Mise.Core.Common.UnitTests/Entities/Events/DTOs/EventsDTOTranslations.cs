using System;
using System.Collections.Generic;
using System.Linq;
using Mise.Core.Common.Events.Checks;
using Mise.Core.Common.Events.DTOs;
using Mise.Core.Common.Events.Payments;
using Mise.Core.Common.Events.Payments.CreditCards;
using Mise.Core.Common.Services.Implementation.Serialization;
using Mise.Core.Entities.Check;
using Mise.Core.Entities.Check.Events;
using Mise.Core.Entities.Menu;
using Mise.Core.Entities.Payments;
using Mise.Core.Entities.People;
using Mise.Core.Entities.People.Events;
using Mise.Core.ValueItems;
using Mise.Core.Entities;
using NUnit.Framework;

namespace Mise.Core.Common.UnitTests.Entities.Events.DTOs
{
    [TestFixture]
    public class EventsDtoTranslations
    {
        [Test]
        public void CashPaidOnCheckShouldSerializeAndDeserialize()
        {
            var checkID = Guid.NewGuid();
            var createdDate = DateTime.UtcNow;
            var empID = Guid.NewGuid();
            var origEvent = new CashPaidOnCheckEvent
            {
                EventOrderingID = new EventID
                {
                    AppInstanceCode = MiseAppTypes.UnitTests,
                    OrderingID = 100
                },
                AmountPaid = new Money(10.00M),
                AmountTendered = new Money(20M),
                ChangeGiven = new Money(10M),
                CheckID = checkID,
                CreatedDate = createdDate,
                EmployeeID = empID,
                CausedByID = empID,
                RestaurantID = Guid.NewGuid(),
                ID = Guid.NewGuid(),
                DeviceID = Guid.NewGuid().ToString (),
                VersionGeneratedFrom = 11
            };

            var fact = new EventDataTransportObjectFactory(new JsonNetSerializer());

            //ACT
            var res = fact.ToDataTransportObject(origEvent);
            Assert.IsNotNull(res);
            var deser = fact.ToCheckEvent(res) as CashPaidOnCheckEvent;

            //ASSERT
            AssertCheckEventsAreEqual(deser, origEvent);
            Assert.IsNotNull(deser);
            Assert.AreEqual(origEvent.AmountPaid.Dollars, deser.AmountPaid.Dollars);
            Assert.AreEqual(origEvent.AmountTendered.Dollars, deser.AmountTendered.Dollars);
        }

        [Test]
        public void CheckCreatedShouldSerializeAndDeserialize()
        {
            var checkID = Guid.NewGuid();
            var createdDate = DateTime.UtcNow;
            var empID = Guid.NewGuid();
            var origEvent = new CheckCreatedEvent
            {
                EventOrderingID = new EventID
                {
                    AppInstanceCode = MiseAppTypes.UnitTests,
                    OrderingID = 100
                },
                CheckID = checkID,
                CreatedDate = createdDate,
                EmployeeID = empID,
                CausedByID = empID,
                RestaurantID = Guid.NewGuid(),
                ID = Guid.NewGuid(),
                DeviceID = Guid.NewGuid().ToString (),
                VersionGeneratedFrom = 11,
            };

            var fact = new EventDataTransportObjectFactory(new JsonNetSerializer());

            //ACT
            var res = fact.ToDataTransportObject(origEvent);
            Assert.IsNotNull(res);
            var deser = fact.ToCheckEvent(res) as CheckCreatedEvent;

            //ASSERT
            AssertCheckEventsAreEqual(deser, origEvent);
        }

        [Test]
        public void CustomerAssignedToCheckShouldSerializeAndDeserialize()
        {
            var checkID = Guid.NewGuid();
            var createdDate = DateTime.UtcNow;
            var empID = Guid.NewGuid();
            var origEvent = new CustomerAssignedToCheckEvent
            {
                EventOrderingID = new EventID
                {
                    AppInstanceCode = MiseAppTypes.UnitTests,
                    OrderingID = 100
                },
                CheckID = checkID,
                CreatedDate = createdDate,
                EmployeeID = empID,
                CausedByID = empID,
                RestaurantID = Guid.NewGuid(),
                ID = Guid.NewGuid(),
				DeviceID = Guid.NewGuid().ToString(),
                VersionGeneratedFrom = 11,
                Customer = new Customer { Name = PersonName.TestName, ID = Guid.NewGuid() }
            };

            var fact = new EventDataTransportObjectFactory(new JsonNetSerializer());

            //ACT
            var res = fact.ToDataTransportObject(origEvent);
            Assert.IsNotNull(res);
            var deser = fact.ToCheckEvent(res) as CustomerAssignedToCheckEvent;

            //ASSERT
            AssertCheckEventsAreEqual(deser, origEvent);
            Assert.IsNotNull(deser);
            Assert.IsNotNull(deser.Customer);
            Assert.AreEqual(origEvent.Customer.Name, deser.Customer.Name);
            Assert.AreEqual(origEvent.Customer.ID, deser.Customer.ID);
        }

        [Test]
        public void CheckReopenedShouldSerializeAndDeserialize()
        {
            var checkID = Guid.NewGuid();
            var createdDate = DateTime.UtcNow;
            var empID = Guid.NewGuid();
            var origEvent = new CheckReopenedEvent
            {
                EventOrderingID = new EventID
                {
                    AppInstanceCode = MiseAppTypes.UnitTests,
                    OrderingID = 100
                },
                CheckID = checkID,
                CreatedDate = createdDate,
                EmployeeID = empID,
                CausedByID = empID,
                RestaurantID = Guid.NewGuid(),
                ID = Guid.NewGuid(),
                DeviceID = Guid.NewGuid().ToString (),
                VersionGeneratedFrom = 11,
                
            };

            var fact = new EventDataTransportObjectFactory(new JsonNetSerializer());

            //ACT
            var res = fact.ToDataTransportObject(origEvent);
            Assert.IsNotNull(res);
            var deser = fact.ToCheckEvent(res) as CheckReopenedEvent;

            //ASSERT
            AssertCheckEventsAreEqual(deser, origEvent);
        }


        [Test]
        public void CheckSentShouldSerializeAndDeserialize()
        {
            var checkID = Guid.NewGuid();
            var createdDate = DateTime.UtcNow;
            var empID = Guid.NewGuid();
            var origEvent = new CheckSentEvent
            {
                EventOrderingID = new EventID
                {
					AppInstanceCode = MiseAppTypes.UnitTests,
                    OrderingID = 100
                },
                CheckID = checkID,
                CreatedDate = createdDate,
                EmployeeID = empID,
                CausedByID = empID,
                RestaurantID = Guid.NewGuid(),
                ID = Guid.NewGuid(),
                DeviceID = Guid.NewGuid().ToString (),
                VersionGeneratedFrom = 11,

            };

            var fact = new EventDataTransportObjectFactory(new JsonNetSerializer());

            //ACT
            var res = fact.ToDataTransportObject(origEvent);
            Assert.IsNotNull(res);
            var deser = fact.ToCheckEvent(res) as CheckSentEvent;

            //ASSERT
            AssertCheckEventsAreEqual(deser, origEvent);
        }

        [Test]
        public void CompPaidDirectloyOnCheckShouldSerializeAndDeserialize()
        {
            var checkID = Guid.NewGuid();
            var createdDate = DateTime.UtcNow;
            var empID = Guid.NewGuid();
            var origEvent = new CompPaidDirectlyOnCheckEvent
            {
                EventOrderingID = new EventID
                {
					AppInstanceCode = MiseAppTypes.UnitTests,
                    OrderingID = 100
                },
                CheckID = checkID,
                CreatedDate = createdDate,
                EmployeeID = empID,
                CausedByID = empID,
                RestaurantID = Guid.NewGuid(),
                ID = Guid.NewGuid(),
                DeviceID = Guid.NewGuid().ToString (),
                VersionGeneratedFrom = 11,
                Amount = new Money(10M)
            };

            var fact = new EventDataTransportObjectFactory(new JsonNetSerializer());

            //ACT
			var res = fact.ToDataTransportObject (origEvent as ICheckEvent);
            Assert.IsNotNull(res);
            var checkDeser = fact.ToCheckEvent(res) as CompPaidDirectlyOnCheckEvent;

			var empDto = fact.ToDataTransportObject (origEvent as IEmployeeEvent);
            Assert.IsNotNull(empDto);
            var empDeser = fact.ToEmployeeEvent(empDto) as CompPaidDirectlyOnCheckEvent;

            //ASSERT
            AssertCheckEventsAreEqual(checkDeser, origEvent);
            Assert.IsNotNull(checkDeser);
            Assert.AreEqual(origEvent.Amount.Dollars, checkDeser.Amount.Dollars);
            Assert.IsNotNull(empDeser);
            Assert.AreEqual(origEvent.Amount.Dollars, empDeser.Amount.Dollars);
        }

        [Test]
        public void CreditCardAddedToPaymentShouldSerializeAndDeserialize()
        {
            var checkID = Guid.NewGuid();
            var createdDate = DateTime.UtcNow;
            var empID = Guid.NewGuid();
            var origEvent = new CreditCardAddedForPaymentEvent
            {
                EventOrderingID = new EventID
                {
					AppInstanceCode = MiseAppTypes.UnitTests,
                    OrderingID = 100
                },
                CheckID = checkID,
                CreatedDate = createdDate,
                EmployeeID = empID,
                CausedByID = empID,
                RestaurantID = Guid.NewGuid(),
                ID = Guid.NewGuid(),
                DeviceID = Guid.NewGuid().ToString (),
                VersionGeneratedFrom = 11,
                Amount = new Money(10M),
                CreditCard = GetCreditCard(),
                PaymentID = Guid.NewGuid(),
            };

            var fact = new EventDataTransportObjectFactory(new JsonNetSerializer());

            //ACT
            var res = fact.ToDataTransportObject(origEvent);
            Assert.IsNotNull(res);
            var deser = fact.ToCheckEvent(res) as CreditCardAddedForPaymentEvent;

            //ASSERT
            AssertCheckEventsAreEqual(deser, origEvent);
            Assert.IsNotNull(deser);
            Assert.AreEqual(origEvent.Amount, deser.Amount);
            Assert.AreEqual(origEvent.Amount.Dollars, deser.Amount.Dollars);
            Assert.AreEqual(origEvent.PaymentID, deser.PaymentID);
        }

        [Test]
        public void CreditCardAuthorizationStartedShouldSerializeAndDeserialize()
        {
            var checkID = Guid.NewGuid();
            var createdDate = DateTime.UtcNow;
            var empID = Guid.NewGuid();
            var origEvent = new CreditCardAuthorizationStartedEvent
            {
                EventOrderingID = new EventID
                {
					AppInstanceCode = MiseAppTypes.UnitTests,
                    OrderingID = 100
                },
                CheckID = checkID,
                CreatedDate = createdDate,
                EmployeeID = empID,
                CausedByID = empID,
                RestaurantID = Guid.NewGuid(),
                ID = Guid.NewGuid(),
                DeviceID = Guid.NewGuid().ToString (),
                VersionGeneratedFrom = 11,
                CreditCard = GetCreditCard(),
                PaymentID = Guid.NewGuid()
            };

            var fact = new EventDataTransportObjectFactory(new JsonNetSerializer());

            //ACT
            var res = fact.ToDataTransportObject(origEvent);
            Assert.IsNotNull(res);
            var deser = fact.ToCheckEvent(res) as CreditCardAuthorizationStartedEvent;

            //ASSERT
            AssertCheckEventsAreEqual(deser, origEvent);
            Assert.IsNotNull(deser);
            Assert.AreEqual(origEvent.PaymentID, deser.PaymentID);
        }

        [Test]
        public void CreditCardAuthorizedShouldSerializeAndDeserialize()
        {
            var checkID = Guid.NewGuid();
            var createdDate = DateTime.UtcNow;
            var empID = Guid.NewGuid();
            var origEvent = new CreditCardAuthorizedEvent
            {
                EventOrderingID = new EventID
                {
					AppInstanceCode = MiseAppTypes.UnitTests,
                    OrderingID = 100
                },
                CheckID = checkID,
                CreatedDate = createdDate,
                EmployeeID = empID,
                CausedByID = empID,
                RestaurantID = Guid.NewGuid(),
                ID = Guid.NewGuid(),
                DeviceID = Guid.NewGuid().ToString (),
                VersionGeneratedFrom = 11,
                CreditCard = GetCreditCard(),
                PaymentID = Guid.NewGuid(),
                AuthorizationCode = new CreditCardAuthorizationCode { AuthorizationKey = "1111", IsAuthorized = true, PaymentProviderName = "tester"},
                Amount = new Money(10M),
                WasAuthorized = true

            };

            var fact = new EventDataTransportObjectFactory(new JsonNetSerializer());

            //ACT
            var res = fact.ToDataTransportObject(origEvent);
            Assert.IsNotNull(res);
            var deser = fact.ToCheckEvent(res) as CreditCardAuthorizedEvent;

            //ASSERT
            AssertCheckEventsAreEqual(deser, origEvent);
            Assert.IsNotNull(deser);
            Assert.AreEqual(origEvent.PaymentID, deser.PaymentID);
            Assert.AreEqual(origEvent.Amount, deser.Amount);
            Assert.AreEqual(origEvent.Amount.Dollars, deser.Amount.Dollars);
            AssertAuthorizationCodesAreEqual(origEvent.AuthorizationCode, deser.AuthorizationCode);
        }


        [Test]
        public void CreditCardAuthorizationCancelledShouldSerializeAndDeserialize()
        {
            var checkID = Guid.NewGuid();
            var createdDate = DateTime.UtcNow;
            var empID = Guid.NewGuid();
            var origEvent = new CreditCardAuthorizationCancelledEvent
            {
                EventOrderingID = new EventID
                {
					AppInstanceCode = MiseAppTypes.UnitTests,
                    OrderingID = 100
                },
                CheckID = checkID,
                CreatedDate = createdDate,
                EmployeeID = empID,
                CausedByID = empID,
                RestaurantID = Guid.NewGuid(),
                ID = Guid.NewGuid(),
                DeviceID = Guid.NewGuid().ToString (),
                VersionGeneratedFrom = 11,
                CreditCard = GetCreditCard(),
                PaymentID = Guid.NewGuid(),
                WasRolledBack = true,
                AuthorizationCode = new CreditCardAuthorizationCode { AuthorizationKey = "1111", IsAuthorized = true, PaymentProviderName = "tester" },
            };

            var fact = new EventDataTransportObjectFactory(new JsonNetSerializer());

            //ACT
            var res = fact.ToDataTransportObject(origEvent);
            Assert.IsNotNull(res);
            var deser = fact.ToCheckEvent(res) as CreditCardAuthorizationCancelledEvent;

            //ASSERT
            AssertCheckEventsAreEqual(deser, origEvent);
            Assert.IsNotNull(deser);
            Assert.AreEqual(origEvent.PaymentID, deser.PaymentID);
            Assert.AreEqual(origEvent.WasRolledBack, deser.WasRolledBack);
            AssertAuthorizationCodesAreEqual(origEvent.AuthorizationCode, deser.AuthorizationCode);
        }

        [Test]
        public void CreditCardChareCompletedShouldSerializeAndDeserialize()
        {
            var checkID = Guid.NewGuid();
            var createdDate = DateTime.UtcNow;
            var empID = Guid.NewGuid();
            var origEvent = new CreditCardChargeCompletedEvent
            {
                EventOrderingID = new EventID
                {
					AppInstanceCode = MiseAppTypes.UnitTests,
                    OrderingID = 100
                },
                CheckID = checkID,
                CreatedDate = createdDate,
                EmployeeID = empID,
                CausedByID = empID,
                RestaurantID = Guid.NewGuid(),
                ID = Guid.NewGuid(),
                DeviceID = Guid.NewGuid().ToString (),
                VersionGeneratedFrom = 11,
                CreditCard = GetCreditCard(),
                PaymentID = Guid.NewGuid(),
                AuthorizationCode = new CreditCardAuthorizationCode { AuthorizationKey = "1111", IsAuthorized = true, PaymentProviderName = "tester" },
            };

            var fact = new EventDataTransportObjectFactory(new JsonNetSerializer());

            //ACT
            var res = fact.ToDataTransportObject(origEvent);
            Assert.IsNotNull(res);
            var deser = fact.ToCheckEvent(res) as CreditCardChargeCompletedEvent;

            //ASSERT
            AssertCheckEventsAreEqual(deser, origEvent);
            Assert.IsNotNull(deser);
            Assert.AreEqual(origEvent.PaymentID, deser.PaymentID);
            AssertAuthorizationCodesAreEqual(origEvent.AuthorizationCode, deser.AuthorizationCode);
        }

        [Test]
        public void CreditCardCloseRequestedShouldSerializeAndDeserialize()
        {
            var checkID = Guid.NewGuid();
            var createdDate = DateTime.UtcNow;
            var empID = Guid.NewGuid();
            var origEvent = new CreditCardCloseRequestedEvent
            {
                EventOrderingID = new EventID
                {
					AppInstanceCode = MiseAppTypes.UnitTests,
                    OrderingID = 100
                },
                CheckID = checkID,
                CreatedDate = createdDate,
                EmployeeID = empID,
                CausedByID = empID,
                RestaurantID = Guid.NewGuid(),
                ID = Guid.NewGuid(),
                DeviceID = Guid.NewGuid().ToString (),
                VersionGeneratedFrom = 11,
                CreditCard = GetCreditCard(),
                PaymentID = Guid.NewGuid(),
                CodeFromAuthorization = new CreditCardAuthorizationCode { AuthorizationKey = "1111", IsAuthorized = true, PaymentProviderName = "tester" },
                TipAmount = new Money(11.11M),
                AmountPaid = new Money(6.66M)
            };

            var fact = new EventDataTransportObjectFactory(new JsonNetSerializer());

            //ACT
            var res = fact.ToDataTransportObject(origEvent);
            Assert.IsNotNull(res);
            var deser = fact.ToCheckEvent(res) as CreditCardCloseRequestedEvent;

            //ASSERT
            AssertCheckEventsAreEqual(deser, origEvent);
            Assert.IsNotNull(deser);
            Assert.AreEqual(origEvent.PaymentID, deser.PaymentID);
            AssertAuthorizationCodesAreEqual(origEvent.CodeFromAuthorization, deser.CodeFromAuthorization);
            Assert.AreEqual(origEvent.TipAmount.Dollars, deser.TipAmount.Dollars);
            Assert.AreEqual(origEvent.AmountPaid.Dollars, deser.AmountPaid.Dollars);
        }

        [Test]
        public void CreditCardFailedAuthorizationShouldSerializeAndDeserialize()
        {
            var checkID = Guid.NewGuid();
            var createdDate = DateTime.UtcNow;
            var empID = Guid.NewGuid();
            var origEvent = new CreditCardFailedAuthorizationEvent
            {
                EventOrderingID = new EventID
                {
                    AppInstanceCode = MiseAppTypes.UnitTests,
                    OrderingID = 100
                },
                CheckID = checkID,
                CreatedDate = createdDate,
                EmployeeID = empID,
                CausedByID = empID,
                RestaurantID = Guid.NewGuid(),
                ID = Guid.NewGuid(),
                DeviceID = Guid.NewGuid().ToString (),
                VersionGeneratedFrom = 11,
                CreditCard = GetCreditCard(),
                PaymentID = Guid.NewGuid(),
                AuthorizationCode = new CreditCardAuthorizationCode { AuthorizationKey = "1111", IsAuthorized = true, PaymentProviderName = "tester" },
                Amount = new Money(11.11M)
            };

            var fact = new EventDataTransportObjectFactory(new JsonNetSerializer());

            //ACT
            var res = fact.ToDataTransportObject(origEvent);
            Assert.IsNotNull(res);
            var deser = fact.ToCheckEvent(res) as CreditCardFailedAuthorizationEvent;

            //ASSERT
            AssertCheckEventsAreEqual(deser, origEvent);
            Assert.IsNotNull(deser);
            Assert.AreEqual(origEvent.PaymentID, deser.PaymentID);
            AssertAuthorizationCodesAreEqual(origEvent.AuthorizationCode, deser.AuthorizationCode);
            Assert.AreEqual(origEvent.Amount.Dollars, deser.Amount.Dollars);
        }

        [Test]
        public void CreditCardTipAddedToChargeShouldSerializeAndDeserialize()
        {
            var checkID = Guid.NewGuid();
            var createdDate = DateTime.UtcNow;
            var empID = Guid.NewGuid();
            var origEvent = new CreditCardTipAddedToChargeEvent
            {
                EventOrderingID = new EventID
                {
                    AppInstanceCode = MiseAppTypes.UnitTests,
                    OrderingID = 100
                },
                CheckID = checkID,
                CreatedDate = createdDate,
                EmployeeID = empID,
                CausedByID = empID,
                RestaurantID = Guid.NewGuid(),
                ID = Guid.NewGuid(),
                DeviceID = Guid.NewGuid().ToString (),
                VersionGeneratedFrom = 11,
                CreditCard = GetCreditCard(),
                PaymentID = Guid.NewGuid(),
                AuthorizationCode = new CreditCardAuthorizationCode { AuthorizationKey = "1111", IsAuthorized = true, PaymentProviderName = "tester" },
                TipAmount = new Money(11.11M)
            };

            var fact = new EventDataTransportObjectFactory(new JsonNetSerializer());

            //ACT
            var res = fact.ToDataTransportObject(origEvent);
            Assert.IsNotNull(res);
            var deser = fact.ToCheckEvent(res) as CreditCardTipAddedToChargeEvent;

            //ASSERT
            AssertCheckEventsAreEqual(deser, origEvent);
            Assert.IsNotNull(deser);
            Assert.AreEqual(origEvent.PaymentID, deser.PaymentID);
            AssertAuthorizationCodesAreEqual(origEvent.AuthorizationCode, deser.AuthorizationCode);
            Assert.AreEqual(origEvent.TipAmount.Dollars, deser.TipAmount.Dollars);
        }

        [Test]
        public void DiscountAppliedToCheckShouldSerializeAndDeserialize()
        {
            var checkID = Guid.NewGuid();
            var createdDate = DateTime.UtcNow;
            var empID = Guid.NewGuid();
            var origEvent = new DiscountAppliedToCheckEvent
            {
                EventOrderingID = new EventID
                {
                    AppInstanceCode = MiseAppTypes.UnitTests,
                    OrderingID = 100
                },
                CheckID = checkID,
                CreatedDate = createdDate,
                EmployeeID = empID,
                CausedByID = empID,
                RestaurantID = Guid.NewGuid(),
                ID = Guid.NewGuid(),
                DeviceID = Guid.NewGuid().ToString (),
                VersionGeneratedFrom = 11,
                DiscountPercentage = new DiscountPercentage
                {
                    ID = Guid.NewGuid(),
                    Percentage = .1M,
                    Name ="testCop"
                }
            };

            var fact = new EventDataTransportObjectFactory(new JsonNetSerializer());

            //ACT
            var res = fact.ToDataTransportObject(origEvent);
            Assert.IsNotNull(res);
            var deser = fact.ToCheckEvent(res) as DiscountAppliedToCheckEvent;

            //ASSERT
            AssertCheckEventsAreEqual(deser, origEvent);
            Assert.IsNotNull(deser);
            Assert.IsNotNull(deser.Discount);
            Assert.AreEqual(origEvent.Discount.ID, deser.Discount.ID);
            Assert.AreEqual(origEvent.DiscountPercentage.Percentage, deser.DiscountPercentage.Percentage);
        }

        [Test]
        public void DiscountRemovedFromCheckShouldSerializeAndDeserialize()
        {
            var checkID = Guid.NewGuid();
            var createdDate = DateTime.UtcNow;
            var empID = Guid.NewGuid();
            var origEvent = new DiscountRemovedFromCheckEvent
            {
                EventOrderingID = new EventID
                {
                    AppInstanceCode = MiseAppTypes.UnitTests,
                    OrderingID = 100
                },
                CheckID = checkID,
                CreatedDate = createdDate,
                EmployeeID = empID,
                CausedByID = empID,
                RestaurantID = Guid.NewGuid(),
                ID = Guid.NewGuid(),
                DeviceID = Guid.NewGuid().ToString (),
                VersionGeneratedFrom = 11,
                DiscountID = Guid.NewGuid() 
            };

            var fact = new EventDataTransportObjectFactory(new JsonNetSerializer());

            //ACT
            var res = fact.ToDataTransportObject(origEvent);
            Assert.IsNotNull(res);
            var deser = fact.ToCheckEvent(res) as DiscountRemovedFromCheckEvent;

            //ASSERT
            AssertCheckEventsAreEqual(deser, origEvent);
            Assert.IsNotNull(deser);
            Assert.IsNotNull(deser.DiscountID);
            Assert.AreEqual(origEvent.DiscountID, deser.DiscountID);
        }

        [Test]
        public void ItemCompedGeneralShouldSerializeAndDeserialize()
        {
            var checkID = Guid.NewGuid();
            var createdDate = DateTime.UtcNow;
            var empID = Guid.NewGuid();
            var origEvent = new ItemCompedGeneralEvent
            {
                EventOrderingID = new EventID
                {
                    AppInstanceCode = MiseAppTypes.UnitTests,
                    OrderingID = 100
                },
                CheckID = checkID,
                CreatedDate = createdDate,
                EmployeeID = empID,
                CausedByID = empID,
                RestaurantID = Guid.NewGuid(),
                ID = Guid.NewGuid(),
                DeviceID = Guid.NewGuid().ToString (),
                VersionGeneratedFrom = 11,
                Amount = new Money(1M),
                OrderItemID = Guid.NewGuid(),
                Reason = "because"
            };

            var fact = new EventDataTransportObjectFactory(new JsonNetSerializer());

            //ACT
            var res = fact.ToDataTransportObject(origEvent as ICheckEvent);
            Assert.IsNotNull(res);
            var checkDeser = fact.ToCheckEvent(res) as ItemCompedGeneralEvent;

            var empDto = fact.ToDataTransportObject(origEvent as IEmployeeEvent);
            Assert.IsNotNull(empDto);
            var empDeser = fact.ToEmployeeEvent(empDto) as ItemCompedGeneralEvent;

            //ASSERT
            AssertCheckEventsAreEqual(checkDeser, origEvent);
            Assert.IsNotNull(checkDeser);
            Assert.AreEqual(origEvent.Amount.Dollars, checkDeser.Amount.Dollars);
            Assert.AreEqual(origEvent.OrderItemID, checkDeser.OrderItemID);
            Assert.AreEqual(origEvent.Reason, checkDeser.Reason);

            Assert.IsNotNull(empDeser);
            Assert.AreEqual(origEvent.Amount.Dollars, empDeser.Amount.Dollars);
            Assert.AreEqual(origEvent.OrderItemID, empDeser.OrderItemID);
            Assert.AreEqual(origEvent.Reason, empDeser.Reason);
        }

        [Test]
        public void ItemUnCompedShouldSerializeAndDeserialize()
        {
            var checkID = Guid.NewGuid();
            var createdDate = DateTime.UtcNow;
            var empID = Guid.NewGuid();
            var origEvent = new ItemUncompedEvent
            {
                EventOrderingID = new EventID
                {
                    AppInstanceCode = MiseAppTypes.UnitTests,
                    OrderingID = 100
                },
                CheckID = checkID,
                CreatedDate = createdDate,
                EmployeeID = empID,
                CausedByID = empID,
                RestaurantID = Guid.NewGuid(),
                ID = Guid.NewGuid(),
                DeviceID = Guid.NewGuid().ToString (),
                VersionGeneratedFrom = 11,
                Amount = new Money(1M),
                OrderItemID = Guid.NewGuid(),
            };

            var fact = new EventDataTransportObjectFactory(new JsonNetSerializer());

            //ACT
            var res = fact.ToDataTransportObject(origEvent as ICheckEvent);
            Assert.IsNotNull(res);
            var checkDeser = fact.ToCheckEvent(res) as ItemUncompedEvent;

            var empDto = fact.ToDataTransportObject(origEvent as IEmployeeEvent);
            Assert.IsNotNull(empDto);
            var empDeser = fact.ToEmployeeEvent(empDto) as ItemUncompedEvent;

            //ASSERT
            AssertCheckEventsAreEqual(checkDeser, origEvent);
            Assert.IsNotNull(checkDeser);
            Assert.AreEqual(origEvent.Amount.Dollars, checkDeser.Amount.Dollars);
            Assert.AreEqual(origEvent.OrderItemID, checkDeser.OrderItemID);

            Assert.IsNotNull(empDeser);
            Assert.AreEqual(origEvent.Amount.Dollars, empDeser.Amount.Dollars);
            Assert.AreEqual(origEvent.OrderItemID, empDeser.OrderItemID);
        }

        [Test]
        public void MarkCheckAsPaidShouldSerializeAndDeserialize()
        {
            var checkID = Guid.NewGuid();
            var createdDate = DateTime.UtcNow;
            var empID = Guid.NewGuid();
            var origEvent = new MarkCheckAsPaidEvent
            {
                EventOrderingID = new EventID
                {
                    AppInstanceCode = MiseAppTypes.UnitTests,
                    OrderingID = 100
                },
                CheckID = checkID,
                CreatedDate = createdDate,
                EmployeeID = empID,
                CausedByID = empID,
                RestaurantID = Guid.NewGuid(),
                ID = Guid.NewGuid(),
                DeviceID = Guid.NewGuid().ToString (),
                VersionGeneratedFrom = 11,
                IsSplitPayment = true
            };

            var fact = new EventDataTransportObjectFactory(new JsonNetSerializer());

            //ACT
            var res = fact.ToDataTransportObject(origEvent);
            Assert.IsNotNull(res);
            var deser = fact.ToCheckEvent(res) as MarkCheckAsPaidEvent;

            //ASSERT
            AssertCheckEventsAreEqual(deser, origEvent);
            Assert.IsNotNull(deser);
            Assert.AreEqual(origEvent.IsSplitPayment, deser.IsSplitPayment);
        }

        [Test]
        public void OrderItemDeletedShouldSerializeAndDeserialize()
        {
            var checkID = Guid.NewGuid();
            var createdDate = DateTime.UtcNow;
            var empID = Guid.NewGuid();
            var origEvent = new OrderItemDeletedEvent
            {
                EventOrderingID = new EventID
                {
                    AppInstanceCode = MiseAppTypes.UnitTests,
                    OrderingID = 100
                },
                CheckID = checkID,
                CreatedDate = createdDate,
                EmployeeID = empID,
                CausedByID = empID,
                RestaurantID = Guid.NewGuid(),
                ID = Guid.NewGuid(),
                DeviceID = Guid.NewGuid().ToString (),
                VersionGeneratedFrom = 11,
                OrderItemID = Guid.NewGuid()
            };

            var fact = new EventDataTransportObjectFactory(new JsonNetSerializer());

            //ACT
            var res = fact.ToDataTransportObject(origEvent);
            Assert.IsNotNull(res);
            var deser = fact.ToCheckEvent(res) as OrderItemDeletedEvent;

            //ASSERT
            AssertCheckEventsAreEqual(deser, origEvent);
            Assert.IsNotNull(deser);
            Assert.AreEqual(origEvent.OrderItemID, deser.OrderItemID);
        }

        [Test]
        public void OrderItemModifiedShouldSerializeAndDeserialize()
        {
            var checkID = Guid.NewGuid();
            var createdDate = DateTime.UtcNow;
            var empID = Guid.NewGuid();
            var origEvent = new OrderItemModifiedEvent
            {
                EventOrderingID = new EventID
                {
                    AppInstanceCode = MiseAppTypes.UnitTests,
                    OrderingID = 100
                },
                CheckID = checkID,
                CreatedDate = createdDate,
                EmployeeID = empID,
                CausedByID = empID,
                RestaurantID = Guid.NewGuid(),
                ID = Guid.NewGuid(),
                DeviceID = Guid.NewGuid().ToString (),
                VersionGeneratedFrom = 11,
                OrderItemID = Guid.NewGuid(),
                Modifiers = new List<MenuItemModifier>
                {
                    new MenuItemModifier
                    {
                        ID = Guid.NewGuid()
                    },
                    new MenuItemModifier
                    {
                        ID = Guid.NewGuid()
                    }
                }
            };

            var fact = new EventDataTransportObjectFactory(new JsonNetSerializer());

            //ACT
            var res = fact.ToDataTransportObject(origEvent);
            Assert.IsNotNull(res);
            var deser = fact.ToCheckEvent(res) as OrderItemModifiedEvent;

            //ASSERT
            AssertCheckEventsAreEqual(deser, origEvent);
            Assert.IsNotNull(deser);
            Assert.AreEqual(origEvent.OrderItemID, deser.OrderItemID);
            Assert.AreEqual(origEvent.Modifiers.Count(), deser.Modifiers.Count());
        }

        [Test]
        public void OrderItemSetMemoShouldSerializeAndDeserialize()
        {
            var checkID = Guid.NewGuid();
            var createdDate = DateTime.UtcNow;
            var empID = Guid.NewGuid();
            var origEvent = new OrderItemSetMemoEvent
            {
                EventOrderingID = new EventID
                {
                    AppInstanceCode = MiseAppTypes.UnitTests,
                    OrderingID = 100
                },
                CheckID = checkID,
                CreatedDate = createdDate,
                EmployeeID = empID,
                CausedByID = empID,
                RestaurantID = Guid.NewGuid(),
                ID = Guid.NewGuid(),
                DeviceID = Guid.NewGuid().ToString (),
                VersionGeneratedFrom = 11,
                OrderItemID = Guid.NewGuid(),
                Memo = "testy"
            };

            var fact = new EventDataTransportObjectFactory(new JsonNetSerializer());

            //ACT
            var res = fact.ToDataTransportObject(origEvent);
            Assert.IsNotNull(res);
            var deser = fact.ToCheckEvent(res) as OrderItemSetMemoEvent;

            //ASSERT
            AssertCheckEventsAreEqual(deser, origEvent);
            Assert.IsNotNull(deser);
            Assert.AreEqual(origEvent.OrderItemID, deser.OrderItemID);
            Assert.AreEqual(origEvent.Memo, deser.Memo);
        }

        [Test]
        public void OrderedOnCheckShouldSerializeAndDeserialize()
        {
            var checkID = Guid.NewGuid();
            var createdDate = DateTime.UtcNow;
            var empID = Guid.NewGuid();
            var origEvent = new OrderedOnCheckEvent
            {
                EventOrderingID = new EventID
                {
                    AppInstanceCode = MiseAppTypes.UnitTests,
                    OrderingID = 100
                },
                CheckID = checkID,
                CreatedDate = createdDate,
                EmployeeID = empID,
                CausedByID = empID,
                RestaurantID = Guid.NewGuid(),
                ID = Guid.NewGuid(),
                DeviceID = Guid.NewGuid().ToString (),
                VersionGeneratedFrom = 11,
                OrderItem = new OrderItem
                {
                    ID = Guid.NewGuid(),
                    MenuItem = new MenuItem
                    {
                        Name = "testMenuItem"
                    }
                }
            };

            var fact = new EventDataTransportObjectFactory(new JsonNetSerializer());

            //ACT
            var res = fact.ToDataTransportObject(origEvent);
            Assert.IsNotNull(res);
            var deser = fact.ToCheckEvent(res) as OrderedOnCheckEvent;

            //ASSERT
            AssertCheckEventsAreEqual(deser, origEvent);
            Assert.IsNotNull(deser);
            Assert.AreEqual(origEvent.OrderItem.ID, deser.OrderItem.ID);
            Assert.AreEqual(origEvent.OrderItem.MenuItem.ID, deser.OrderItem.MenuItem.ID);

        }
        #region Helper Methods
        /// <summary>
        /// Compares all fields present on the ICheckEventInterface
        /// </summary>
        /// <param name="deser"></param>
        /// <param name="origEvent"></param>
        private static void AssertCheckEventsAreEqual(ICheckEvent deser, ICheckEvent origEvent)
        {
            Assert.IsNotNull(deser);
            Assert.AreEqual(origEvent.EventOrderingID.OrderingID, deser.EventOrderingID.OrderingID);
            Assert.AreEqual(origEvent.CheckID, deser.CheckID, "CheckID");
            Assert.AreEqual(origEvent.CreatedDate, deser.CreatedDate);
            Assert.AreEqual(origEvent.EmployeeID, deser.EmployeeID, "EmployeeID");
            Assert.AreEqual(origEvent.CausedByID, deser.CausedByID, "CausedBy");
            Assert.AreEqual(origEvent.EventType, deser.EventType);
        }

        private static void AssertAuthorizationCodesAreEqual(CreditCardAuthorizationCode original, CreditCardAuthorizationCode deser)
        {
            Assert.AreEqual(original.AuthorizationKey, deser.AuthorizationKey);
            Assert.AreEqual(original.IsAuthorized, deser.IsAuthorized);
            Assert.AreEqual(original.PaymentProviderName, deser.PaymentProviderName);
        }

        private static CreditCard GetCreditCard()
        {
            return new CreditCard
            {
				ProcessorToken = new CreditCardProcessorToken{
					Processor = CreditCardProcessors.FakeProcessor, 
					Token = "blerg"
				},
                ExpMonth = 101,
                ExpYear = 2014
            };
        }
        #endregion
    }
}
