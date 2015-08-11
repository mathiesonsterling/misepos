using System;
using System.Collections.Generic;

using NUnit.Framework;
using Mise.Core.Entities.Base;
using Mise.Core.Entities.People;
using Mise.Core.Common.Events;
using Mise.Core.Entities;
using Mise.Core.Common.Entities;
using Mise.Core.ValueItems;
using Mise.Core.Entities.Restaurant;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Entities.Inventory;
using Mise.Core.Entities.Inventory.Events;
using Mise.Core.ValueItems.Inventory;

namespace Mise.Core.Common.UnitTests.Events
{
	[TestFixtureAttribute]
	public class InventoryAppEventFactoryTests
	{
		private InventoryAppEventFactory _underTest;
		private IEmployee _emp;
		Guid _restID;
		IInventory _inv;
		IRestaurant _rest;
		IRestaurantInventorySection _restSec;
		IInventorySection _invSection;

		InventoryBeverageLineItem _invLI;

		IPar _par;
		IParBeverageLineItem _parLI;
		[SetUp]
		public void Setup(){
			_restID = Guid.NewGuid ();
			_underTest = new InventoryAppEventFactory ("testDevice", MiseAppTypes.UnitTests);
			_rest = new Restaurant{ ID = _restID };
			_underTest.SetRestaurant (_rest);

			_restSec = new RestaurantInventorySection{ ID = Guid.NewGuid (), RestaurantID = _restID };

			_emp = new Employee{ ID = Guid.NewGuid () };

			_inv = new Inventory{ ID = Guid.NewGuid (), RestaurantID = _restID };
			_invSection = new InventorySection{ ID = Guid.NewGuid (), InventoryID = _inv.ID };
			_invLI = new InventoryBeverageLineItem{ ID = Guid.NewGuid () };
			_parLI = new ParBeverageLineItem {
				ID = Guid.NewGuid (),
				UPC = "119292",
				DisplayName = "testLI",
			};

			_par = new Par{ ID = Guid.NewGuid (), RestaurantID = _restID };
		}

		#region ID and Date tests
		private void TestCommonFieldsWithRest(IEntityEventBase ev){
			TestCommonFields (ev);
			Assert.AreEqual (_restID, ev.RestaurantID);
		}

		private void TestInventoryEvent(IInventoryEvent ev){
			TestCommonFieldsWithRest (ev);
			Assert.AreNotEqual (Guid.Empty, ev.InventoryID);
			Assert.AreEqual (_inv.ID, ev.InventoryID);
		}

		private void TestCommonFields(IEntityEventBase ev){
			Assert.AreNotEqual (Guid.Empty, ev.ID, "ID");
			Assert.AreNotEqual (Guid.Empty, ev.CausedByID, "CausedByID");
			Assert.AreEqual (_emp.ID, ev.CausedByID, "CausedByID");
			Assert.NotNull (ev.CreatedDate, "CreatedDate");
			Assert.GreaterOrEqual (ev.CreatedDate, DateTimeOffset.UtcNow.AddHours (-1));
			Assert.False (string.IsNullOrEmpty (ev.DeviceID), "DeviceID");
			Assert.NotNull (ev.EventOrderingID, "ordering ID");
		}

		private void TestParEvent(IParEvent ev){
			TestCommonFieldsWithRest (ev);
			Assert.AreNotEqual (Guid.Empty, ev.ParID);
		}

		[Test]	
		public void AccountReg(){
			var ev = _underTest.CreateAccountRegisteredFromMobileDeviceEvent (_emp, Guid.NewGuid (), EmailAddress.TestEmail,
				         PhoneNumber.TestPhoneNumber, new CreditCard (), ReferralCode.TestReferralCode, MiseAppTypes.UnitTests, PersonName.TestName);

			TestCommonFields (ev);
		}

		[Test]
		public void EmployeeAcceptsInvitation(){
			var ev = _underTest.CreateEmployeeAcceptsInvitationEvent (new ApplicationInvitation(), _emp);

			TestCommonFieldsWithRest (ev);
		}

		[Test]
		public void CreateEmployee(){
			var ev = _underTest.CreateEmployeeCreatedEvent (EmailAddress.TestEmail, Password.TestPassword, PersonName.TestName, MiseAppTypes.UnitTests);

			TestCommonFields (ev);
		}

		[Test]
		public void EmployeeInvited(){
			var ev = _underTest.CreateEmployeeInvitedToApplicationEvent (_emp, EmailAddress.TestEmail, MiseAppTypes.UnitTests, RestaurantName.TestName);

			TestCommonFieldsWithRest (ev);
		}

		[Test]
		public void Login(){
			var ev = _underTest.CreateEmployeeLoggedIntoInventoryAppEvent (_emp);

			TestCommonFieldsWithRest (ev);
		}

		[Test]
		public void Logout(){
			var ev = _underTest.CreateEmployeeLoggedOutOfInventoryAppEvent (_emp);

			TestCommonFieldsWithRest (ev);
		}

		[Test]
		public void EmpRegistered(){
			var ev = _underTest.CreateEmployeeRegisteredForInventoryAppEvent (_emp);

			TestCommonFields (ev);
		}

		[Test]
		public void RegisterRestaurant(){
			var ev = _underTest.CreateEmployeeRegistersRestaurantEvent (_emp, _rest);

			TestCommonFields (ev);
		}

		[Test]
		public void RejectInvitiation(){
			var ev = _underTest.CreateEmployeeRejectsInvitiationEvent (new ApplicationInvitation(), _emp);

			TestCommonFields (ev);
		}

		[Test]
		public void CompleteInventory(){
			var ev = _underTest.CreateInventoryCompletedEvent (_emp, _inv);
			TestInventoryEvent (ev);
		}

		[Test]
		public void CreateInventory(){
			var ev = _underTest.CreateInventoryCreatedEvent (_emp);
			Assert.AreNotEqual (Guid.Empty, ev.InventoryID);
			TestCommonFieldsWithRest (ev);
		}

		[Test]
		public void CreateInventoryLineItemFromBase(){
			var ev = _underTest.CreateInventoryLineItemAddedEvent (_emp, _parLI, 12, 
				Money.MiseMonthlyFee, null, _invSection, 23, _inv);

			TestInventoryEvent (ev);
			Assert.AreEqual (_invSection.ID, ev.InventorySectionID);
			Assert.AreEqual (23, ev.InventoryPosition);
			Assert.AreEqual (_parLI.DisplayName, ev.DisplayName);
			Assert.AreEqual (_parLI.UPC, ev.UPC);
		}

		[Test]
		public void LineItemMeasured(){
			var ev = _underTest.CreateInventoryLiquidItemMeasuredEvent (_emp, _inv, _invSection, _invLI, 10,
				         new List<decimal>{ 1.0M, 0.2M }, LiquidAmount.Liter);
			TestInventoryEvent (ev);
			Assert.AreEqual (_invLI, ev.BeverageLineItem);
		}

		[Test]
		public void InventoryMadeCurrent(){
			var ev = _underTest.CreateInventoryMadeCurrentEvent (_emp, _inv);
			TestInventoryEvent (ev);
		}

		[Test]
		public void InventoryNewSection(){
			var ev = _underTest.CreateInventoryNewSectionAddedEvent (_emp, _inv, _restSec);
			TestInventoryEvent (ev);
			Assert.AreNotEqual (_restSec.ID, ev.SectionID);
			Assert.AreEqual (_restSec.ID, ev.RestaurantSectionId);
		}

		[Test]
		public void SectionAddedToRestaurant(){
			var ev = _underTest.CreateInventorySectionAddedToRestaurantEvent (_emp, "NewSec", true, true);
			TestCommonFieldsWithRest (ev);
			Assert.AreEqual ("NewSec", ev.SectionName);
			Assert.AreNotEqual (Guid.Empty, ev.SectionID);
		}

		[Test]
		public void CompleteSection(){
			var ev = _underTest.CreateInventorySectionCompletedEvent (_emp, _inv, _invSection);
			TestInventoryEvent (ev);
			Assert.AreEqual (_invSection.ID, ev.InventorySectionID);
			Assert.AreNotEqual (_restSec.ID, ev.InventorySectionID);
		}

		[Test]
		public void NewRestaurant(){
			var ev = _underTest.CreateNewRestaurantRegisteredOnAppEvent (_emp, RestaurantName.TestName, StreetAddress.TestStreetAddress, PhoneNumber.TestPhoneNumber);
			TestCommonFields (ev);
			Assert.AreNotEqual (Guid.Empty, ev.RestaurantID);
		}

		[Test]
		public void CreatePar(){
			var ev = _underTest.CreatePARCreatedEvent (_emp);
			TestParEvent (ev);
		}

		[Test]
		public void ParLineItemAdded(){
			var ev = _underTest.CreatePARLineItemAddedEvent (_emp, _invLI, 12, _par);
			TestParEvent (ev);
			Assert.AreEqual (_invLI.DisplayName, ev.DisplayName);
		}

		[Test]
		public void ParLineItemQuantityUpdated(){
			var ev = _underTest.CreatePARLineItemQuantityUpdatedEvent (_emp, _par, _parLI.ID, 10);
			TestParEvent (ev);
			Assert.AreEqual (10, ev.UpdatedQuantity);
		}

		[Test]
		public void PlaceholderRestaurantCreated(){
			var ev = _underTest.CreatePlaceholderRestaurantCreatedEvent (_emp);
			TestCommonFields (ev);
		}
		#endregion
	}
}

