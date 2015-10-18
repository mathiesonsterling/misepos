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
using Mise.Core.Entities.Vendors;
using Mise.Core.Common.Entities.Vendors;
using Mise.Core.Common.Services.Implementation;
using Mise.Core.Entities.Vendors.Events;

namespace Mise.Core.Common.UnitTests.Events
{
	[TestFixtureAttribute]
	public class InventoryAppEventFactoryTests
	{
		private InventoryAppEventFactory _underTest;
		private IEmployee _emp;
		IInventory _inv;
		IRestaurant _rest;
		IRestaurantInventorySection _restSec;
		IInventorySection _invSection;

		InventoryBeverageLineItem _invLI;

		IPar _par;
		ParBeverageLineItem _parLI;

		IPurchaseOrder _po;
		IReceivingOrder _ro;
		ReceivingOrderLineItem _roLI;

		IVendor _vendor;
		IVendorBeverageLineItem _vLI;
		[SetUp]
		public void Setup(){
			var restID = Guid.NewGuid ();
			_underTest = new InventoryAppEventFactory ("testDevice", MiseAppTypes.UnitTests);
			_rest = new Restaurant{ Id = restID };
			_underTest.SetRestaurant (_rest);

			_restSec = new RestaurantInventorySection{ Id = Guid.NewGuid (), RestaurantID = restID };

			_emp = new Employee{ Id = Guid.NewGuid () };

			_inv = new Inventory{ Id = Guid.NewGuid (), RestaurantID = restID };
			_invSection = new InventorySection{ Id = Guid.NewGuid (), InventoryID = _inv.Id };
			_invLI = new InventoryBeverageLineItem{ Id = Guid.NewGuid () };
			_parLI = new ParBeverageLineItem {
				Id = Guid.NewGuid (),
				UPC = "119292",
				DisplayName = "testLI",
			};

			_par = new Par{ Id = Guid.NewGuid (), RestaurantID = restID };

			_po = new PurchaseOrder{ Id = Guid.NewGuid(), RestaurantID = restID };

			_ro = new ReceivingOrder{ Id = Guid.NewGuid (), RestaurantID = restID };
			_roLI = new ReceivingOrderLineItem{ Id = Guid.NewGuid (), RestaurantID = restID };

			_vendor = new Vendor{ Id = Guid.NewGuid (), RestaurantsAssociatedIDs = new List<Guid>{ restID } };
			_vLI = new VendorBeverageLineItem{ Id = Guid.NewGuid (), VendorID = _vendor.Id };
		}

		#region ID and Date tests
		void TestCommonFieldsWithRest(IEntityEventBase ev){
			TestCommonFields (ev);
			Assert.AreEqual (_rest.Id, ev.RestaurantId, "RestaurantID");
		}

		void TestInventoryEvent(IInventoryEvent ev){
			TestCommonFieldsWithRest (ev);
			Assert.AreNotEqual (Guid.Empty, ev.InventoryID);
			Assert.AreEqual (_inv.Id, ev.InventoryID, "InventoryID");
		}

		void TestCommonFields(IEntityEventBase ev){
			Assert.AreNotEqual (Guid.Empty, ev.Id, "ID");
			Assert.AreNotEqual (Guid.Empty, ev.CausedById, "CausedByID");
			Assert.AreEqual (_emp.Id, ev.CausedById, "CausedByID");
			Assert.NotNull (ev.CreatedDate, "CreatedDate");
			Assert.GreaterOrEqual (ev.CreatedDate, DateTimeOffset.UtcNow.AddHours (-1));
			Assert.AreEqual ("testDevice", ev.DeviceId, "DeviceID");
			Assert.NotNull (ev.EventOrder, "ordering ID");
		}

		void TestParEvent(IParEvent ev){
			TestCommonFieldsWithRest (ev);
			Assert.AreNotEqual (Guid.Empty, ev.ParID, "ParID");
		}

		void TestPurchaseOrderEvent(IPurchaseOrderEvent ev){
			TestCommonFieldsWithRest (ev);
			Assert.AreNotEqual (Guid.Empty, ev.PurchaseOrderID, "PurchaseOrderID");
		}

		void TestReceivingOrderEvent(IReceivingOrderEvent ev){
			TestCommonFieldsWithRest (ev);
			Assert.AreNotEqual (Guid.Empty, ev.ReceivingOrderID, "ReceivingOrderID");
		}

		void TestVendorEvent(IVendorEvent ev){
			TestCommonFields (ev);
			Assert.AreNotEqual (Guid.Empty, ev.VendorID, "VendorID");
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

            Assert.AreNotEqual(Guid.Empty, ev.Id, "ID");
            Assert.AreNotEqual(Guid.Empty, ev.CausedById, "CausedByID");
            Assert.AreEqual(ev.EmployeeID, ev.CausedById, "CausedByID");
            Assert.NotNull(ev.CreatedDate, "CreatedDate");
            Assert.GreaterOrEqual(ev.CreatedDate, DateTimeOffset.UtcNow.AddHours(-1));
            Assert.AreEqual("testDevice", ev.DeviceId, "DeviceID");
            Assert.NotNull(ev.EventOrder, "ordering ID");
        }

		[Test]
		public void EmployeeInvited(){
			var ev = _underTest.CreateEmployeeInvitedToApplicationEvent (_emp, EmailAddress.TestEmail, MiseAppTypes.UnitTests, RestaurantName.TestName);

			TestCommonFieldsWithRest (ev);
		}

		[Test]
		public void Login(){
			var ev = _underTest.CreateEmployeeLoggedIntoInventoryAppEvent (_emp);

			TestCommonFields (ev);
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
				null, _invSection, 23, _inv);

			TestInventoryEvent (ev);
			Assert.AreEqual (_invSection.Id, ev.InventorySectionID);
			Assert.AreEqual (23, ev.InventoryPosition);
			Assert.AreEqual (_parLI.DisplayName, ev.DisplayName);
			Assert.AreEqual (_parLI.UPC, ev.UPC);
		}

		[Test]
		public void CreateInventoryLineItemDirect(){
			var ev = _underTest.CreateInventoryLineItemAddedEvent (
				         _emp, 
				         "testItem", 
				         "upc", 
				         new List<ItemCategory>{ CategoriesService.Beer },
				         11,
				         LiquidContainer.Bottle1_75ML,
				         100,
				         null, _invSection, 10, _inv);

			TestInventoryEvent (ev);
		}

		[Test]
		public void LineItemMeasured(){
			var ev = _underTest.CreateInventoryLiquidItemMeasuredEvent (_emp, _inv, _invSection, _invLI, 10,
				         new List<decimal>{ 1.0M, 0.2M }, LiquidAmount.Liter);
			TestInventoryEvent (ev);
			Assert.AreEqual (_invLI, ev.BeverageLineItem);
		}

		[Test]
		public void LineItemDeleted(){
			var ev = _underTest.CreateInventoryLineItemDeletedEvent (_emp, _inv, _invSection, _invLI);
			TestInventoryEvent (ev);
			Assert.AreEqual (_inv.Id, ev.InventoryID);
			Assert.AreEqual (_invSection.Id, ev.InventorySectionID);
			Assert.AreEqual (_invLI.Id, ev.InventoryLineItemID);
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
			Assert.AreNotEqual (_restSec.Id, ev.SectionID);
			Assert.AreEqual (_restSec.Id, ev.RestaurantSectionId);
		}

		[Test]
		public void InventoryLineItemMoved(){
			var ev = _underTest.CreateInventoryLineItemMovedToNewPositionEvent (_emp, _inv, _invSection, _invLI, 5);
			TestInventoryEvent (ev);
			Assert.AreEqual (_invSection.Id, ev.InventorySectionID);
			Assert.AreEqual (5, ev.NewPositionWanted);
		}

		[Test]
		public void InventorySectionCleared(){
			var ev = _underTest.CreateInventorySectionClearedEvent (_emp, _inv, _invSection);
			TestInventoryEvent (ev);
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
			Assert.AreEqual (_invSection.Id, ev.InventorySectionID);
			Assert.AreNotEqual (_restSec.Id, ev.InventorySectionID);
		}

		[Test]
		public void NewRestaurant(){
			var ev = _underTest.CreateNewRestaurantRegisteredOnAppEvent (_emp, RestaurantName.TestName, StreetAddress.TestStreetAddress, PhoneNumber.TestPhoneNumber);
			TestCommonFields (ev);
			Assert.AreNotEqual (Guid.Empty, ev.RestaurantId);
		}

		[Test]
		public void CreatePar(){
			var ev = _underTest.CreatePARCreatedEvent (_emp);
			TestParEvent (ev);
		}

		[Test]
		public void ParLineItemAddedFromBase(){
			var ev = _underTest.CreatePARLineItemAddedEvent (_emp, _invLI, 12, _par);
			TestParEvent (ev);
			Assert.AreEqual (_invLI.DisplayName, ev.DisplayName);
		}

		[Test]
		public void ParLineItemAddedDirect(){
			var ev = _underTest.CreatePARLineItemAddedEvent (_emp, "testITem", "upc",
				new List<ItemCategory>{ CategoriesService.Beer },
				100, LiquidContainer.Bottle330ML, 1000, _par
			);
			TestParEvent (ev);
		}

		[Test]
		public void ParLineItemQuantityUpdated(){
			var ev = _underTest.CreatePARLineItemQuantityUpdatedEvent (_emp, _par, _parLI.Id, 10);
			TestParEvent (ev);
			Assert.AreEqual (10, ev.UpdatedQuantity);
		}

		[Test]
		public void ParLineItemDeleted(){
			var ev = _underTest.CreateParLineItemDeletedEvent (_emp, _par, _parLI);
			TestParEvent (ev);
			Assert.AreEqual (_parLI.Id, ev.LineItemId);
		}

		[Test]
		public void PlaceholderRestaurantCreated(){
			var ev = _underTest.CreatePlaceholderRestaurantCreatedEvent (_emp);
			TestCommonFields (ev);
		}

		[Test]
		public void PurchaseOrderLineItemAdded(){
			var ev = _underTest.CreatePOLineItemAddedFromInventoryCalcEvent (_emp, _po, _parLI, 12, LiquidAmount.SevenFiftyMillilters, Guid.NewGuid ());
			TestPurchaseOrderEvent (ev);
		}

		[Test]
		public void PurchaseOrderCreated(){
			var ev = _underTest.CreatePurchaseOrderCreatedEvent (_emp);
			TestPurchaseOrderEvent (ev);
		}

		[Test]
		public void PurchaseOrderReceived(){
			var ev = _underTest.CreatePurchaseOrderRecievedFromVendorEvent (_emp, _po, _ro, PurchaseOrderStatus.ReceivedWithAlterations);
			TestPurchaseOrderEvent (ev);
		}

		[Test]
		public void PurchaseOrderSentToVendor(){
			var ev = _underTest.CreatePurchaseOrderSentToVendorEvent (_emp, _po, _vendor.Id);
			TestPurchaseOrderEvent (ev);
		}

		[Test]
		public void ReceivingOrderAssociatedWithPO(){
			var ev = _underTest.CreateReceivingOrderAssociatedWithPOEvent (_emp, _ro, _po);
			TestReceivingOrderEvent (ev);
		}

		[Test]
		public void ReceivingOrderCompleted(){
			var ev = _underTest.CreateReceivingOrderCompletedEvent (_emp, _ro, DateTimeOffset.Now, "blerg", "100");
			TestReceivingOrderEvent (ev);
		}

		[Test]
		public void ReceivingOrderCreated(){
			var ev = _underTest.CreateReceivingOrderCreatedEvent (_emp, _vendor);
			TestReceivingOrderEvent (ev);
			Assert.AreEqual (_vendor.Id, ev.VendorID);
		}

		[Test]
		public void ReceivingOrderLineItemAddedFromBase(){
			var ev = _underTest.CreateReceivingOrderLineItemAddedEvent (_emp, _invLI, 10, _ro);
			TestReceivingOrderEvent (ev);
		}

		[Test]
		public void ReceivingOrderLineItemAddedDirect(){
			var ev = _underTest.CreateReceivingOrderLineItemAddedEvent (_emp, 
				"testItem", 
				"upc", 
				new List<ItemCategory>{ CategoriesService.Beer },
				11,
				LiquidContainer.Bottle1_75ML,
				100,
				_ro);
			TestReceivingOrderEvent (ev);
		}

		[Test]
		public void RecevingOrderLineItemZeroedOut(){
			var ev = _underTest.CreateReceivingOrderLineItemZeroedOutEvent (_emp, _ro, _roLI.Id);
			Assert.AreEqual (_roLI.Id, ev.LineItemID);
			TestReceivingOrderEvent (ev);
		}

		[Test]
		public void RestaurantSetPriceForVendor(){
			var ev = _underTest.CreateRestaurantSetPriceEvent (_emp, _vLI, _vendor, Money.MiseMonthlyFee);
			TestVendorEvent (ev);
			Assert.AreEqual (_vLI.Id, ev.VendorLineItemID);
			Assert.AreEqual (_rest.Id, ev.RestaurantId, "rest ID");
		}

		[Test]
		public void ReceivingOrderLineItemUpdated(){
			var ev = _underTest.CreateROLineItemUpdateQuantityEvent (_emp, _ro, _roLI.Id, 100, Money.None);
			TestReceivingOrderEvent (ev);
			Assert.AreEqual (100, ev.UpdatedQuantity);
		}

		[Test]
		public void RestaurantSelected(){
			var ev = _underTest.CreateUserSelectedRestaurant (_emp, _rest.Id);
			TestCommonFieldsWithRest (ev);
		}

		[Test]
		public void VendorCreated(){
			var ev = _underTest.CreateVendorCreatedEvent (_emp, "testVend", StreetAddress.TestStreetAddress, PhoneNumber.TestPhoneNumber, EmailAddress.TestEmail);
			TestVendorEvent (ev);
		}

		[Test]
		public void VendorLineItemAdded(){
			var ev = _underTest.CreateVendorLineItemAddedEvent (_emp, _invLI, _vendor);
			TestVendorEvent (ev);
			Assert.AreEqual (_invLI.DisplayName, ev.DisplayName);
		}

		[Test]
		public void InventorySectionStartedByEmployee(){
			var ev = _underTest.CreateInventorySectionStartedByEmployeeEvent (_emp, _inv, _invSection);
			TestInventoryEvent (ev);
			Assert.AreEqual (_invSection.Id, ev.InventorySectionId);
		}
		#endregion
	}
}

