using System;
using System.Collections.Generic;
using System.Linq;

using Moq;
using NUnit.Framework;

using Mise.Core.Services;
using Mise.Core.Entities.People.Events;
using Mise.Core.Common.Events.Employee;
using Mise.Core.Common.Services.Implementation.Serialization;
using Mise.Core.ValueItems;
namespace Mise.Inventory.UnitTests.Services
{
	/*
	[TestFixture]
	public class ServiceStackWebServiceClientIntegrationTests
	{
		Guid TEST_RESTAURANT_ID = Guid.Empty;

		[Test]
		public async void GetRestaurantShouldGetForTestID(){
			var client = CreateClient();
			var results = await client.GetRestaurant(TEST_RESTAURANT_ID);

			Assert.NotNull (results);
			Assert.AreEqual (TEST_RESTAURANT_ID, results.RestaurantID);

			var sections = results.GetInventorySections ().ToList ();
			Assert.NotNull (sections);
			Assert.True (sections.Any ());
		}

		[Test]
		public async void GetEmployeesForRestaurantShouldHaveAtLeastTwoAndNoPasswords(){
			var client = CreateClient();
			var results = await client.GetEmployeesForRestaurant(TEST_RESTAURANT_ID);
			Assert.NotNull (results);

			var emps = results.ToList ();
			Assert.GreaterOrEqual (emps.Count (), 2);

			Assert.True (emps.All (emp => emp.GetRestaurantIDs ().Contains (TEST_RESTAURANT_ID)));

			foreach (var emp in emps) {
				Assert.Null (emp.Password.HashValue, "password hash value is not null!");
			}
		}

		[Test]
		public async void GetEmployeeByEmailAndPasswordShouldReturnAndHaveHash(){
			var client = CreateClient();
			var email = new EmailAddress{Value="mathieson@misepos.com"};
			var password = new Password{ HashValue = "esimavE4esiM" };

			var results = await client.GetEmployeeByPrimaryEmailAndPassword (email, password);

			Assert.NotNull (results);
			Assert.NotNull (results.Password);
			Assert.AreEqual ("esimavE4esiM", results.Password.HashValue);
			Assert.True (results.GetEmailAddresses ().Any ());
		}

		[Test]
		public async void GetVendorsByRadiusShouldReturnVendors(){
			var client = CreateClient();
			var emptyLoc = new Location();
			var bigRadius = new Distance{ Kilometers = 100000 };
			var results = await client.GetVendorsWithinSearchRadius (emptyLoc, bigRadius);

			Assert.NotNull (results);
			var vendors = results.ToList ();
			Assert.True (vendors.Any ());
		}

		[Test]
		public async void GetVendorsByRestaurantShouldReturnVendorsSellingToRestaurant(){
			var client = CreateClient();
			var results = await client.GetVendorsAssociatedWithRestaurant(TEST_RESTAURANT_ID);

			Assert.NotNull (results);
			var vendors = results.ToList ();
			Assert.True (vendors.Any ());

			Assert.True (vendors.All (v => v.GetRestaurantIDsAssociatedWithVendor ().Contains (TEST_RESTAURANT_ID)));
		}

		[Test]
		public async void GetParsByRestaurantShouldReturn(){
			var client = CreateClient();

			var results = await client.GetPARsForRestaurant(TEST_RESTAURANT_ID);

			Assert.True (results.Any ());
		}

		[Test]
		public async void GetInventoriesByRestaurantShouldReturn(){
			var client = CreateClient();

			var results = await client.GetInventoriesForRestaurant (TEST_RESTAURANT_ID);

			Assert.True (results.Any ());
		}

		[Test]
		public async void GetReceivingOrdersByRestaurantShouldReturn(){
			var client = CreateClient();

			var results = await client.GetReceivingOrdersForRestaurant (TEST_RESTAURANT_ID);

			Assert.True (results.Any ());
		}

		/// <summary>
		/// Tests just that events go across the wire, not that they are processed
		/// </summary>
		[Test]
		public async void SendEmployeeEventsShouldNotHaveException(){
			var client = CreateClient();

			//get an employee
			var emps = await client.GetEmployeesForRestaurant (TEST_RESTAURANT_ID);
			var oneEmp = emps.FirstOrDefault ();

			var ev = new EmployeeLoggedIntoInventoryAppEvent{
				RestaurantID = TEST_RESTAURANT_ID,
				EventOrderingID = new EventID{AppInstanceCode = "intTest", OrderingID = 0},
				EmployeeID = oneEmp.ID
			};

			var events = new List<IEmployeeEvent> {ev};
			//ACT
			var res = await client.SendEventsAsync (events);

			//ASSERT
			Assert.IsTrue (res);
		}

		[Test]
		public async void EmployeeLoginAndLogoutViaEventsShouldReflectInGet(){
			var client = CreateClient();

			var emps = await client.GetEmployeesForRestaurant (TEST_RESTAURANT_ID);
			var oneEmp = emps.FirstOrDefault ();

			var ev = new EmployeeLoggedIntoInventoryAppEvent{
				RestaurantID = TEST_RESTAURANT_ID,
				EventOrderingID = new EventID{AppInstanceCode = "intTest", OrderingID = 0},
				EmployeeID = oneEmp.ID
			};

			//send events
			var events = new List<IEmployeeEvent> {ev};
			var res = await client.SendEventsAsync (events);
			Assert.IsTrue (res, "Sent First event");

			//now get again
			var reget = await client.GetEmployeesForRestaurant (TEST_RESTAURANT_ID);
			var updatedEmp = reget.FirstOrDefault (e => e.ID == oneEmp.ID);
			Assert.NotNull (updatedEmp);
			Assert.IsTrue (updatedEmp.CurrentlyLoggedIntoInventoryApp, "currently logged in");

			//log out
			var logoutEv = new EmployeeLoggedOutOfInventoryAppEvent {
				RestaurantID = TEST_RESTAURANT_ID,
				EventOrderingID = new EventID{ AppInstanceCode = "intTest", OrderingID = 1 },
				EmployeeID = updatedEmp.ID
			};

			var logoutEvRes = await client.SendEventsAsync (new []{ logoutEv});
			Assert.IsTrue (logoutEvRes, "sent logout event");

			reget = await client.GetEmployeesForRestaurant (TEST_RESTAURANT_ID);
			updatedEmp = reget.FirstOrDefault (e => e.ID == oneEmp.ID);
			Assert.NotNull (updatedEmp);
			Assert.IsFalse(updatedEmp.CurrentlyLoggedIntoInventoryApp, "currently logged in after logout");
		}

		const string TEST_SERVER_URL = "http://miseinventoryservicedev.azurewebsites.net/";
		//const string TEST_SERVER_URL = "http://localhost:43499/";
		static ServiceStackWebServiceClient CreateClient(){
			var uri = new Uri(TEST_SERVER_URL);

			var logger = new Mock<ILogger> ();
			var client = new ServiceStackWebServiceClient (uri, "integrationtest", new JsonNetSerializer (), logger.Object);

			return client;
		}


	}*/
}

