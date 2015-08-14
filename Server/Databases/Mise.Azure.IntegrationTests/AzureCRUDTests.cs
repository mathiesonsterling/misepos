using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Mise.Core.Common.Events.Employee;
using Mise.Core.Common.Events.Inventory;
using Mise.Core.Common.Events.Vendors;
using Mise.Core.Common.Services.Implementation.Serialization;
using Mise.Core.Entities;
using Mise.Core.Entities.Base;
using Mise.Core.Entities.Menu;
using Mise.Core.Services;
using Mise.Core.Services.UtilityServices;
using Mise.Core.ValueItems;
using Moq;
using NUnit.Framework;

namespace Mise.Azure.IntegrationTests
{
    [TestFixture]
    public class AzureCRUDTests
    {
        public const string DEV_CONNECTION_STRING = "DefaultEndpointsProtocol=https;AccountName=misedevstorage;AccountKey=kYWGD3lFjju57+kIO25phdOpdwn2z53WbDxsno07MjAfK7INPd/BCgFvdD7eafHYheUTDZtGOrtrulRBj1KtwA==";
       
        [TearDown]
        public void TearDown()
        {
            var serial = new JsonNetSerializer();
            var logger = new Mock<ILogger>();
            var azDAL = new AzureStorageDAL(serial, DEV_CONNECTION_STRING, logger.Object);
            azDAL.Clear();
        }


        /// <summary>
        /// Test that our Azure DAL can do all CRUD operations on a menu
        /// </summary>
        [Ignore]
        [Test]
        public void TestAzureCRUDOnMenu()
        {
            var serial = new JsonNetSerializer();
            var logger = new Mock<ILogger>();
            var azDAL = new AzureStorageDAL(serial, DEV_CONNECTION_STRING, logger.Object);

            var menuID = Guid.NewGuid();
            var restID = Guid.NewGuid();
            var menu = new Menu
            {
                ID = menuID,
                Categories = new List<MenuItemCategory>(),
                CreatedDate = DateTime.Now,
                DisplayName = "testMenu",
                Name = "test",
                RestaurantID = restID,
				Revision = new EventID{AppInstanceCode=MiseAppTypes.UnitTests, OrderingID=100}
            };

            //CREATE
            var res = azDAL.UpsertEntities(new[] {menu});
            Assert.NotNull(res);

            //RETRIEVE
            /*
            var got = azDAL.GetEntities<Menu>();
            Assert.NotNull(got);
            Assert.AreEqual(1, got.Count()); */

            var gotByRest = azDAL.GetEntities<Menu>(restID);
            Assert.NotNull(gotByRest);
            Assert.AreEqual(1, gotByRest.Count());

            var gotByID = azDAL.GetEntityByID<Menu>(restID, menuID);
            Assert.NotNull(gotByID);

            var reMenu = gotByID;
            Assert.NotNull(reMenu);
            reMenu.DisplayName = "updated";
            var updateRes = azDAL.UpsertEntities(new[] {reMenu});
            Assert.NotNull(updateRes);

            var reGet = azDAL.GetEntityByID<Menu>(restID, menuID);
            Assert.NotNull(reGet);
            Assert.AreEqual("updated", reGet.DisplayName);

            var delRes = azDAL.Delete(reGet);
            Assert.IsTrue(delRes);

            var getDel = azDAL.GetEntityByID<Menu>(restID, menuID);
            Assert.Null(getDel);
        }

        [Ignore]
        [Test]
        public void TestAzureCRUDGetByIDWith2()
        {
            var serial = new JsonNetSerializer();
            var logger = new Mock<ILogger>();
            var azDAL = new AzureStorageDAL(serial, DEV_CONNECTION_STRING, logger.Object);

            var menuID = Guid.NewGuid();
            var restID = Guid.NewGuid();
            var menu = new Menu
            {
                ID = menuID,
                Categories = new List<MenuItemCategory>(),
                CreatedDate = DateTime.Now,
                DisplayName = "testMenu",
                Name = "test",
                RestaurantID = restID,
				Revision = new EventID{OrderingID = 100, AppInstanceCode=MiseAppTypes.UnitTests}
            };

            var menuTwo = new Menu
            {
                ID = Guid.NewGuid(),
                Categories = new List<MenuItemCategory>(),
                CreatedDate = DateTime.Now,
                DisplayName = "other",
                Name = "other",
                RestaurantID = restID
            };

            //CREATE
            var res = azDAL.UpsertEntities(new[] { menu , menuTwo});
            Assert.NotNull(res);


            var gotByRest = azDAL.GetEntities<Menu>(restID);
            Assert.NotNull(gotByRest);
            Assert.AreEqual(2, gotByRest.Count());

            var gotByID = azDAL.GetEntityByID<Menu>(restID, menuID);
            Assert.NotNull(gotByID);
            Assert.AreEqual("testMenu", gotByID.DisplayName);
            Assert.AreEqual(menuID, gotByID.ID);


            var delRes = azDAL.Delete(menu);
            Assert.IsTrue(delRes);
            var del2 = azDAL.Delete(menuTwo);
            Assert.IsTrue(del2);

            var getDel = azDAL.GetEntityByID<Menu>(restID, menuID);
            Assert.Null(getDel);
        }

        [Ignore]
        [Test]
        public void TestGetWithoutRestaurantID()
        {
            var serial = new JsonNetSerializer();
            var logger = new Mock<ILogger>();
            var azDAL = new AzureStorageDAL(serial, DEV_CONNECTION_STRING, logger.Object);

            var menuID = Guid.NewGuid();
            var restID = Guid.NewGuid();
            var menu = new Menu
            {
                ID = menuID,
                Categories = new List<MenuItemCategory>(),
                CreatedDate = DateTime.Now,
                DisplayName = "testMenu",
                Name = "test",
                RestaurantID = restID,
				Revision = new EventID{OrderingID = 100, AppInstanceCode=MiseAppTypes.UnitTests}
            };

            //CREATE
            var res = azDAL.UpsertEntities(new[] {menu});
            Assert.NotNull(res);

            //RETRIEVE
            
            var got = azDAL.GetEntities<Menu>();
            Assert.NotNull(got);
            Assert.AreEqual(1, got.Count());

            var gotByID = azDAL.GetEntityByID<Menu>(menuID);
            Assert.NotNull(gotByID);
            Assert.AreEqual(menuID, gotByID.ID);

            var delRes = azDAL.Delete(gotByID);
            Assert.IsTrue(delRes);
        }

        [Ignore]
        [Test]
        public void TestAzureCRUDOnMenuAsync()
        {
            var serial = new JsonNetSerializer();
            var logger = new Mock<ILogger>();
            var azDAL = new AzureStorageDAL(serial, DEV_CONNECTION_STRING, logger.Object);

            var menuID = Guid.NewGuid();
            var restID = Guid.NewGuid();
            var menu = new Menu
            {
                ID = menuID,
                Categories = new List<MenuItemCategory>(),
                CreatedDate = DateTime.Now,
                DisplayName = "testMenu",
                Name = "test",
                RestaurantID = restID,
				Revision = new EventID {OrderingID = 100, AppInstanceCode=MiseAppTypes.UnitTests}
            };

            //CREATE
            var upsertTask = azDAL.UpsertEntitiesAsync(new[] { menu });
            var res = upsertTask.Result;
            Assert.NotNull(res);
           // azDAL.UpsertEntities(new[] {menu});

            //RETRIEVE
          
            /*var gotTask = azDAL.GetEntitiesAsync<Menu>();
            var got = gotTask.Result;
            Assert.NotNull(got);
            Assert.AreEqual(1, got.Count());*/

            var gotByRestTask = azDAL.GetEntitiesAsync<Menu>(restID);
            var gotByRest = gotByRestTask.Result;
            Assert.NotNull(gotByRest);
            Assert.AreEqual(1, gotByRest.Count());

            var gotByIDTask = azDAL.GetEntityByIDAsync<Menu>(restID, menuID);
            var gotByID = gotByIDTask.Result;
            Assert.NotNull(gotByID);

            var reMenu = gotByID;
            Assert.NotNull(reMenu);
            reMenu.DisplayName = "updated";
            var updateResTask = azDAL.UpsertEntitiesAsync(new[] { reMenu });
            var updateRes = updateResTask.Result;
            Assert.NotNull(updateRes);

            var reGetTask = azDAL.GetEntityByIDAsync<Menu>(restID, menuID);
            var reGet = reGetTask.Result;
            Assert.NotNull(reGet);
            Assert.AreEqual("updated", reGet.DisplayName);

            var delRes = azDAL.Delete(reGet);
            Assert.IsTrue(delRes);

            var getDelTask = azDAL.GetEntityByIDAsync<Menu>(restID, menuID);
            var getDel = getDelTask.Result;
            Assert.Null(getDel);
        }

        /// <summary>
        /// Tests the IEventStoreDAL portion of Azure
        /// </summary>
        [Test]
        public void TestStoreAndRetrieveEventsForInventoryApp()
        {
            Thread.Sleep(100);
            var serial = new JsonNetSerializer();
            var logger = new Mock<ILogger>();
            var azDAL = new AzureStorageDAL(serial, DEV_CONNECTION_STRING, logger.Object);

            //make some events
            var empEvent = new EmployeeLoggedIntoInventoryAppEvent
            {
                ID = Guid.NewGuid(),
                EmployeeID = Guid.NewGuid(),
                EventOrderingID = new EventID { AppInstanceCode=MiseAppTypes.UnitTests, OrderingID = 1 },
                CreatedDate = DateTime.UtcNow,
            };

            var invEvent = new InventoryCreatedEvent
            {
                ID = Guid.NewGuid(),
                InventoryID = Guid.NewGuid(),
                CreatedDate = DateTime.UtcNow,
                EventOrderingID = new EventID { AppInstanceCode=MiseAppTypes.UnitTests, OrderingID = 2 },
            };

            var vendorEvent = new VendorCreatedEvent
            {
                ID = Guid.NewGuid(),
                VendorID = Guid.NewGuid(),
                EventOrderingID = new EventID { AppInstanceCode=MiseAppTypes.UnitTests, OrderingID = 3 },
                CreatedDate = DateTime.UtcNow,
            };

            var poEvent = new PurchaseOrderCreatedEvent
            {
                ID = Guid.NewGuid(),
                CreatedDate = DateTime.UtcNow,
                EventOrderingID = new EventID { AppInstanceCode=MiseAppTypes.UnitTests, OrderingID = 4 }
            };

            var receiveEvent = new ReceivingOrderCreatedEvent
            {
                ID = Guid.NewGuid(),
                CreatedDate = DateTime.UtcNow,
                EventOrderingID = new EventID { AppInstanceCode=MiseAppTypes.UnitTests, OrderingID = 5 }
            };

            var events = new List<IEntityEventBase> {invEvent, vendorEvent, poEvent, receiveEvent, empEvent};

            //ACT
            var task = azDAL.StoreEventsAsync(events);
            Assert.NotNull(task);
           
            var res = task.Result;
            Thread.Sleep(100);

            Assert.IsTrue(res);

            var retrieveTask = azDAL.GetEventsSince(null, DateTime.UtcNow.AddDays(-2));
            var eventsRes = retrieveTask.Result;
            Thread.Sleep(100);
            //ASSERT
            Assert.NotNull(eventsRes);
            Assert.AreEqual(events.Count, eventsRes.Count());
        }
    }
}
