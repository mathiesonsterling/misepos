using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Common.Events.ApplicationInvitations;
using Mise.Core.Common.Events.DTOs;
using Mise.Core.Common.Events.Employee;
using Mise.Core.Common.Events.Inventory;
using Mise.Core.Common.Events.Restaurant;
using Mise.Core.Common.Events.Vendors;
using Mise.Core.Entities;
using Mise.Core.Entities.Accounts;
using Mise.Core.Entities.Base;
using Mise.Core.Entities.Inventory;
using Mise.Core.Entities.Inventory.Events;
using Mise.Core.Entities.People;
using Mise.Core.Entities.People.Events;
using Mise.Core.Entities.Restaurant;
using Mise.Core.Entities.Restaurant.Events;
using Mise.Core.Entities.Vendors;
using Mise.Core.Entities.Vendors.Events;
using Mise.Core.Repositories;
using Mise.Core.Server.Services.DAL;
using Mise.Core.Services.UtilityServices;
using Mise.Core.ValueItems;
using Mise.InventoryWebService.ServiceInterface;
using Mise.InventoryWebService.ServiceModelPortable.Responses;
using NUnit.Framework;
using Moq;


namespace Mise.InventoryService.Tests
{
    [TestFixture]
    public class EventsServiceTests
    {
        [Test]
        public async Task EventsShouldBeGivenToCorrectRepositories()
        {
            Mock<IInventoryRepository> inventory;
            Mock<IEmployeeRepository> emp;
            Mock<IVendorRepository> vendor;
            Mock<IPurchaseOrderRepository> po;
            Mock<IReceivingOrderRepository> receive;
            Mock<IApplicationInvitationRepository> appInvite;
            Mock<IRestaurantRepository> rest;
            Mock<IAccountRepository> acct;
            Mock<IPARRepository> par;
            EventDataTransportObjectFactory dtoFactory;
            var underTest = CreateEventsServiceAndMocks(out inventory, out emp, out vendor, out po, out par, out receive, out appInvite, out rest, out acct, out dtoFactory);

            var empEvent = new EmployeeLoggedIntoInventoryAppEvent
            {
                EmployeeID = Guid.NewGuid(),
                EventOrderingID = new EventID { AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 1 },
                CreatedDate = DateTime.UtcNow,
            };
         
            var invEvent = new InventoryCreatedEvent
            {
                InventoryID = Guid.NewGuid(),
                CreatedDate = DateTime.UtcNow,
                EventOrderingID = new EventID { AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 2 },
            };

            var vendorEvent = new VendorCreatedEvent
            {
                VendorID = Guid.NewGuid(),
                EventOrderingID = new EventID { AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 3 }
            };

            var poEvent = new PurchaseOrderCreatedEvent
            {
                EventOrderingID = new EventID { AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 4 }
            };

            var receiveEvent = new ReceivingOrderCreatedEvent
            {
                EventOrderingID = new EventID { AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 5 }
            };

            var restEvent = new InventorySectionAddedToRestaurantEvent
            {
                EventOrderingID = new EventID {AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 6}
            };

            var appInviteEvent = new EmployeeInvitedToApplicationEvent
            {
                EventOrderingID = new EventID {AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 7}
            };

            var events = new List<EventDataTransportObject>
            {
                dtoFactory.ToDataTransportObject(empEvent),
                dtoFactory.ToDataTransportObject(vendorEvent),
                dtoFactory.ToDataTransportObject(poEvent),
                dtoFactory.ToDataTransportObject(receiveEvent),
                dtoFactory.ToDataTransportObject(invEvent),
                dtoFactory.ToDataTransportObject(restEvent),
                dtoFactory.ToDataTransportObject(appInviteEvent),
            };

            var request = new EventSubmission
            {
                DeviceID = "testDevice",
                Events = events
            };


            //ACT
            var res = await underTest.Post(request);

            //ASSERT
            Assert.NotNull(res);

            //check our repositories all got fired
            inventory.Verify(i => i.ApplyEvents(It.Is<IEnumerable<IInventoryEvent>>(e => e.Count() == 1)), Times.Once, "InventoryRepos");
            vendor.Verify(v => v.ApplyEvents(It.Is<IEnumerable<IVendorEvent>>(e => e.Count() == 1)), Times.Once);
            po.Verify(p => p.ApplyEvents(It.Is<IEnumerable<IPurchaseOrderEvent>>(e => e.Count() == 1)), Times.Once);
            emp.Verify(p => p.ApplyEvents(It.Is<IEnumerable<IEmployeeEvent>>(e => e.Count() == 1)), Times.Once, "Employee");
            receive.Verify(r => r.ApplyEvents(It.Is<IEnumerable<IReceivingOrderEvent>>(e => e.Count() == 1)), Times.Once, "ReceivingOrder");
            rest.Verify(r => r.ApplyEvents(It.Is<IEnumerable<IRestaurantEvent>>(e => e.Count() == 1)), Times.Once, "Restaurant");
            appInvite.Verify(r => r.ApplyEvents(It.Is<IEnumerable<IApplicationInvitationEvent>>(e => e.Count() == 1)), Times.Once, "AppInvitation");
        }


        [Test]
        public void EventsShouldBeGivenToCorrectRepositoriesAndMultipleEntitiesShouldGetSeparateApplies()
        {
            //ASSEMBLE
            Mock<IInventoryRepository> inventory;
            Mock<IEmployeeRepository> emp;
            Mock<IVendorRepository> vendor;
            Mock<IPurchaseOrderRepository> po;
            Mock<IReceivingOrderRepository> receive;
            Mock<IPARRepository> par;
            Mock<IApplicationInvitationRepository> appInvite;
            Mock<IRestaurantRepository> rest;
            Mock<IAccountRepository> acct;
            EventDataTransportObjectFactory dtoFactory;
            var underTest = CreateEventsServiceAndMocks(out inventory, out emp, out vendor, out po, out par, out receive, out appInvite, out rest, out acct, out dtoFactory);

            var empEvent = new EmployeeLoggedIntoInventoryAppEvent
            {
                EmployeeID = Guid.NewGuid(),
                EventOrderingID = new EventID { AppInstanceCode=MiseAppTypes.UnitTests, OrderingID = 1 },
                CreatedDate = DateTime.UtcNow,
            };
            var empEvent2 = new EmployeeLoggedIntoInventoryAppEvent
            {
                EmployeeID = Guid.NewGuid(),
                EventOrderingID = new EventID { AppInstanceCode=MiseAppTypes.UnitTests, OrderingID = 11 },
                CreatedDate = DateTime.UtcNow,
            };

            var invEvent = new InventoryCreatedEvent
            {
                InventoryID = Guid.NewGuid(),
                CreatedDate = DateTime.UtcNow,
                EventOrderingID = new EventID { AppInstanceCode=MiseAppTypes.UnitTests, OrderingID = 2 },
            };
            var invEvent2 = new InventoryCreatedEvent
            {
                InventoryID = Guid.NewGuid(),
                CreatedDate = DateTime.UtcNow,
                EventOrderingID = new EventID { AppInstanceCode=MiseAppTypes.UnitTests, OrderingID = 22 },
            };

            var vendorEvent = new VendorCreatedEvent
            {
                VendorID = Guid.NewGuid(),
                EventOrderingID = new EventID { AppInstanceCode=MiseAppTypes.UnitTests, OrderingID = 3 },
            };
            var vendorEvent2 = new VendorCreatedEvent
            {
                VendorID = Guid.NewGuid(),
                EventOrderingID = new EventID { AppInstanceCode=MiseAppTypes.UnitTests, OrderingID = 33 },
            };

            var poEvent = new PurchaseOrderCreatedEvent
            {
                EventOrderingID = new EventID { AppInstanceCode=MiseAppTypes.UnitTests, OrderingID = 4 },
                PurchaseOrderID = Guid.NewGuid()
            };
            var poEvent2 = new PurchaseOrderCreatedEvent
            {
                EventOrderingID = new EventID { AppInstanceCode=MiseAppTypes.UnitTests, OrderingID = 44 },
                PurchaseOrderID = Guid.NewGuid()
            };

            var receiveEvent = new ReceivingOrderCreatedEvent
            {
                EventOrderingID = new EventID { AppInstanceCode=MiseAppTypes.UnitTests, OrderingID = 5 },
                ReceivingOrderID = Guid.NewGuid()
            };
            var receiveEvent2 = new ReceivingOrderCreatedEvent
            {
                EventOrderingID = new EventID { AppInstanceCode=MiseAppTypes.UnitTests, OrderingID = 55 },
                ReceivingOrderID = Guid.NewGuid()
            };

            var events = new List<EventDataTransportObject>
            {
                dtoFactory.ToDataTransportObject(empEvent),
                dtoFactory.ToDataTransportObject(vendorEvent),
                dtoFactory.ToDataTransportObject(poEvent),
                dtoFactory.ToDataTransportObject(receiveEvent),
                dtoFactory.ToDataTransportObject(invEvent),
                dtoFactory.ToDataTransportObject(empEvent2),
                dtoFactory.ToDataTransportObject(vendorEvent2),
                dtoFactory.ToDataTransportObject(poEvent2),
                dtoFactory.ToDataTransportObject(receiveEvent2),
                dtoFactory.ToDataTransportObject(invEvent2)
            };

            var request = new EventSubmission
            {
                DeviceID = "testDevice",
                Events = events
            };

            //ACT
            var res = underTest.Post(request);

            //ASSERT
            Assert.NotNull(res);

            //check our repositories all got fired
            inventory.Verify(i => i.ApplyEvents(It.Is<IEnumerable<IInventoryEvent>>(e => e.Count() == 1)), Times.Exactly(2), "InventoryRepos");
            vendor.Verify(v => v.ApplyEvents(It.Is<IEnumerable<IVendorEvent>>(e => e.Count() == 1)), Times.Exactly(2));
            po.Verify(p => p.ApplyEvents(It.Is<IEnumerable<IPurchaseOrderEvent>>(e => e.Count() == 1)), Times.Exactly(2));
            emp.Verify(p => p.ApplyEvents(It.Is<IEnumerable<IEmployeeEvent>>(e => e.Count() == 1)), Times.Exactly(2), "Employee");
            receive.Verify(r => r.ApplyEvents(It.Is<IEnumerable<IReceivingOrderEvent>>(e => e.Count() == 1)), Times.Exactly(2), "ReceivingOrder");
        }


        [Test]
        public async Task DifferentEventsOnSameEntityShouldBeDoneInSameCommit()
        {
            //ASSEMBLE
            var moqDAL = new Mock<IEventStorageDAL>();
            var serial = new Core.Common.Services.Implementation.Serialization.JsonNetSerializer();

            var inventory = CreateMockRepository<IInventoryRepository, IInventory, IInventoryEvent>();
            var emp = CreateMockRepository<IEmployeeRepository, IEmployee, IEmployeeEvent>();
            var vendor = CreateMockRepository<IVendorRepository, IVendor, IVendorEvent>();
            var po = CreateMockRepository<IPurchaseOrderRepository, IPurchaseOrder, IPurchaseOrderEvent>();
            var par = CreateMockRepository<IPARRepository, IPAR, IPAREvent>();
            var receive = CreateMockRepository<IReceivingOrderRepository, IReceivingOrder, IReceivingOrderEvent>();
            var rest = CreateMockRepository<IRestaurantRepository, IRestaurant, IRestaurantEvent>();
            var acct = CreateMockRepository<IAccountRepository, IAccount, IAccountEvent>();
            var appInvite = CreateMockRepository<IApplicationInvitationRepository, IApplicationInvitation, IApplicationInvitationEvent>();
            var logger = new Mock<ILogger>();

            var errorTracker = new Mock<IErrorTrackingService>();

            var underTest = new EventsService(moqDAL.Object, serial, inventory.Object, vendor.Object, po.Object,
                emp.Object, par.Object, receive.Object, appInvite.Object, rest.Object, acct.Object, logger.Object, errorTracker.Object);
            var dtoFactory = new EventDataTransportObjectFactory(serial);

            var entID = Guid.NewGuid();

            var invEvent = new InventoryCreatedEvent
            {
                InventoryID = entID,
                CreatedDate = DateTime.UtcNow,
                EventOrderingID = new EventID { AppInstanceCode=MiseAppTypes.UnitTests, OrderingID = 2 },
            };
            var invEvent2 = new InventoryCreatedEvent
            {
                InventoryID = entID,
                CreatedDate = DateTime.UtcNow,
                EventOrderingID = new EventID { AppInstanceCode=MiseAppTypes.UnitTests, OrderingID = 22 },
            };

            var events = new List<EventDataTransportObject>
            {
                dtoFactory.ToDataTransportObject(invEvent),
                dtoFactory.ToDataTransportObject(invEvent2)
            };

            var request = new EventSubmission
            {
                DeviceID = "testDevice",
                Events = events
            };


            //ACT
            var res = await underTest.Post(request);

            //ASSERT
            Assert.NotNull(res);

            //check our repositories all got fired
            inventory.Verify(i => i.ApplyEvents(It.Is<IEnumerable<IInventoryEvent>>(e => e.Count() == 2)), Times.Once, "InventoryRepos");
        }

        [Test]
        public async Task EventsWhichPertainToMultipleEntitesShouldBeGivenToEach()
        {
            Mock<IInventoryRepository> inventory;
            Mock<IEmployeeRepository> emp;
            Mock<IVendorRepository> vendor;
            Mock<IPurchaseOrderRepository> po;
            Mock<IPARRepository> par;
            Mock<IReceivingOrderRepository> receive;
            Mock<IApplicationInvitationRepository> appInvite;
            Mock<IRestaurantRepository> rest;
            Mock<IAccountRepository> acct;
            EventDataTransportObjectFactory dtoFactory;
            var underTest = CreateEventsServiceAndMocks(out inventory, out emp, out vendor, out po, out par, out receive, out appInvite, out rest, out acct, out dtoFactory);

            var empID = Guid.NewGuid();
            var inviteID = Guid.NewGuid();

            var appInviteEvent = new EmployeeAcceptsInvitationEvent
            {
                EmployeeID = empID,
                InvitationID = inviteID,
                EventOrderingID = new EventID { AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 7 },
                
            };

            var events = new List<EventDataTransportObject>
            {
                dtoFactory.ToDataTransportObject(appInviteEvent as IApplicationInvitationEvent)
            };

            var request = new EventSubmission
            {
                DeviceID = "testDevice",
                Events = events
            };



            //ACT
            var res = await underTest.Post(request);

            //ASSERT
            Assert.NotNull(res);
            emp.Verify(p => p.ApplyEvents(It.Is<IEnumerable<IEmployeeEvent>>(e => e.Count() == 1)), Times.Once, "Employee");
           
            appInvite.Verify(r => r.ApplyEvents(It.Is<IEnumerable<IApplicationInvitationEvent>>(e => e.Count() == 1)), Times.Once, "AppInvitation");
        }

        private static Mock<TRepos> CreateMockRepository<TRepos, TEntityType, TEventType>()
            where TRepos : class, IEventSourcedEntityRepository<TEntityType, TEventType>
            where TEntityType : class, IEventStoreEntityBase<TEventType>
            where TEventType : IEntityEventBase
        {
            var mockEnt = new Mock<TEntityType>();
            var repos = new Mock<TRepos>();
            repos.Setup(r => r.ApplyEvents(It.IsAny<IEnumerable<TEventType>>())).Returns(mockEnt.Object);
           // repos.Setup(r => r.Commit(It.IsAny<Guid>())).Returns(Task.FromResult(CommitResult.StoredInDB));
            repos.Setup(r => r.StartTransaction(It.IsAny<Guid>())).Returns(true);

            return repos;
        }

        private static EventsService CreateEventsServiceAndMocks(out Mock<IInventoryRepository> inventory, out Mock<IEmployeeRepository> emp, 
            out Mock<IVendorRepository> vendor, out Mock<IPurchaseOrderRepository> po, out Mock<IPARRepository> par, 
            out Mock<IReceivingOrderRepository> receive, out Mock<IApplicationInvitationRepository> appInvite, out Mock<IRestaurantRepository> rest,
            out Mock<IAccountRepository> acct, out EventDataTransportObjectFactory dtoFactory)
        {
            var moqDAL = new Mock<IEventStorageDAL>();
            var serial = new Core.Common.Services.Implementation.Serialization.JsonNetSerializer();

            inventory = CreateMockRepository<IInventoryRepository, IInventory, IInventoryEvent>();
            inventory.Setup(r => r.GetEntityID(It.IsAny<IInventoryEvent>()))
                .Returns((IInventoryEvent ev) => ev.InventoryID);

            emp = CreateMockRepository<IEmployeeRepository, IEmployee, IEmployeeEvent>();
            emp.Setup(r => r.GetEntityID(It.IsAny<IEmployeeEvent>())).Returns((IEmployeeEvent e) => e.EmployeeID);

            vendor = CreateMockRepository<IVendorRepository, IVendor, IVendorEvent>();
            vendor.Setup(r => r.GetEntityID(It.IsAny<IVendorEvent>())).Returns((IVendorEvent v) => v.VendorID);

            po = CreateMockRepository<IPurchaseOrderRepository, IPurchaseOrder, IPurchaseOrderEvent>();
            po.Setup(r => r.GetEntityID(It.IsAny<IPurchaseOrderEvent>()))
                .Returns((IPurchaseOrderEvent e) => e.PurchaseOrderID);

            par = CreateMockRepository<IPARRepository, IPAR, IPAREvent>();
            par.Setup(r => r.GetEntityID(It.IsAny<IPAREvent>()))
                .Returns((IPAREvent e) => e.ParID);

            receive = CreateMockRepository<IReceivingOrderRepository, IReceivingOrder, IReceivingOrderEvent>();
            receive.Setup(r => r.GetEntityID(It.IsAny<IReceivingOrderEvent>()))
                .Returns((IReceivingOrderEvent ro) => ro.ReceivingOrderID);

            rest = CreateMockRepository<IRestaurantRepository, IRestaurant, IRestaurantEvent>();
            rest.Setup(r => r.GetEntityID(It.IsAny<IRestaurantEvent>())).Returns((IRestaurantEvent e) => e.RestaurantID);

            acct = CreateMockRepository<IAccountRepository, IAccount, IAccountEvent>();
            acct.Setup(r => r.GetEntityID(It.IsAny<IAccountEvent>())).Returns((IAccountEvent e) => e.AccountID);

            appInvite = CreateMockRepository<IApplicationInvitationRepository, IApplicationInvitation, IApplicationInvitationEvent>();
            appInvite.Setup(r => r.GetEntityID(It.IsAny<IApplicationInvitationEvent>()))
                .Returns((IApplicationInvitationEvent e) => e.InvitationID);

            dtoFactory = new EventDataTransportObjectFactory(serial);
            var logger = new Mock<ILogger>();
            var errorTracker = new Mock<IErrorTrackingService>();
            var underTest = new EventsService(moqDAL.Object, serial, inventory.Object, vendor.Object, po.Object,
                emp.Object, par.Object, receive.Object, appInvite.Object, rest.Object, acct.Object, logger.Object, errorTracker.Object);
            return underTest;
        }
    }
}
