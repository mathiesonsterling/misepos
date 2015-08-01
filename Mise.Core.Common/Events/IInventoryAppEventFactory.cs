using System;
using System.Collections.Generic;
using Mise.Core.Common.Events.ApplicationInvitations;
using Mise.Core.Common.Events.Employee;
using Mise.Core.Common.Events.Inventory;
using Mise.Core.Entities.Inventory;
using Mise.Core.Entities.People;
using Mise.Core.Entities.Restaurant;
using Mise.Core.ValueItems;
using Mise.Core.Common.Events.Restaurant;
using Mise.Core.ValueItems.Inventory;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Entities.Vendors;
using Mise.Core.Common.Events.Vendors;
using Mise.Core.Entities;
using Mise.Core.Common.Events.Accounts;

namespace Mise.Core.Common.Events
{
    public interface IInventoryAppEventFactory
    {
		void SetLastEventID(IEnumerable<EventID> last);

		Guid? RestaurantID{get;}

		void SetRestaurant(IRestaurant restaurant);

		#region Event creations
        EmployeeLoggedIntoInventoryAppEvent CreateEmployeeLoggedIntoInventoryAppEvent(IEmployee emp);
        EmployeeRegisteredForInventoryAppEvent CreateEmployeeRegisteredForInventoryAppEvent(IEmployee emp);
        EmployeeLoggedOutOfInventoryAppEvent CreateEmployeeLoggedOutOfInventoryAppEvent(IEmployee emp);
		EmployeeCreatedEvent CreateEmployeeCreatedEvent(EmailAddress email, Password password, PersonName name, MiseAppTypes appType);

		ReceivingOrderCreatedEvent CreateReceivingOrderCreatedEvent(IEmployee emp, IVendor vendor);
		ReceivingOrderAssociatedWithPOEvent CreateReceivingOrderAssociatedWithPOEvent(IEmployee emp, IReceivingOrder ro, IPurchaseOrder po);
		ReceivingOrderLineItemAddedEvent CreateReceivingOrderLineItemAddedEvent(IEmployee emp, 
			IBaseBeverageLineItem source, int quantity, IReceivingOrder ro);
		ReceivingOrderLineItemAddedEvent CreateReceivingOrderLineItemAddedEvent(IEmployee emp, string name, 
			string upc, IEnumerable<ItemCategory> category, int caseSize, LiquidContainer container, int quantity, IReceivingOrder ro);
		ReceivingOrderCompletedEvent CreateReceivingOrderCompletedEvent(IEmployee emp, IReceivingOrder ro, DateTimeOffset dateReceived, string notes, string invoiceID);
		ReceivingOrderLineItemQuantityUpdatedEvent CreateROLineItemUpdateQuantityEvent(IEmployee emp, 
			IReceivingOrder ro, Guid lineItemID, int quantity, Money pricePerBottle);
		ReceivingOrderLineItemZeroedOutEvent CreateReceivingOrderLineItemZeroedOutEvent(IEmployee emp, IReceivingOrder ro, Guid lineItemID);

		InventoryCreatedEvent CreateInventoryCreatedEvent(IEmployee emp);
		InventoryMadeCurrentEvent CreateInventoryMadeCurrentEvent(IEmployee emp, IInventory inventory);
		InventoryLineItemAddedEvent CreateInventoryLineItemAddedEvent(IEmployee emp, IBaseBeverageLineItem source, int quantity, Money pricePaid, Guid? vendorID, IInventorySection section, 
           int inventoryPosition, IInventory inventory);

		InventoryLineItemAddedEvent CreateInventoryLineItemAddedEvent(IEmployee emp, string name, string upc, 
			IEnumerable<ItemCategory> category, int caseSize, LiquidContainer container, int quantity, Money pricePaid, 
			Guid? vendorID, IInventorySection section, int inventoryPosition, IInventory inventory);
		InventorySectionCompletedEvent CreateInventorySectionCompletedEvent(IEmployee emp, IInventory inventory, IInventorySection section);
		InventoryLiquidItemMeasuredEvent CreateInventoryLiquidItemMeasuredEvent(IEmployee emp, IInventory inventory, 
			IInventorySection section, InventoryBeverageLineItem li, int numFullBottles, IEnumerable<decimal> partialBottlePercentages, LiquidAmount amtMeasured);
		InventoryCompletedEvent CreateInventoryCompletedEvent(IEmployee emp, IInventory inventory);
		InventoryNewSectionAddedEvent CreateInventoryNewSectionAddedEvent(IEmployee emp, IInventory inventory, IRestaurantInventorySection restSection);

		PARCreatedEvent CreatePARCreatedEvent(IEmployee emp);
		PARLineItemAddedEvent CreatePARLineItemAddedEvent(IEmployee emp, IBaseBeverageLineItem source, int? quantity, IPAR par);
		PARLineItemAddedEvent CreatePARLineItemAddedEvent(IEmployee emp, string name, string upc, IEnumerable<ItemCategory> category, 
			int caseSize, LiquidContainer container, int quantity, IPAR par);
		PARLineItemQuantityUpdatedEvent CreatePARLineItemQuantityUpdatedEvent(IEmployee emp, IPAR par, Guid lineItemID, decimal quantity);

		InventorySectionAddedToRestaurantEvent CreateInventorySectionAddedToRestaurantEvent(IEmployee emp, string newSectionName, bool isDefaultSection, bool allowsPartialBottles);

		PurchaseOrderCreatedEvent CreatePurchaseOrderCreatedEvent(IEmployee emp);
		PurchaseOrderLineItemAddedFromInventoryCalculationEvent CreatePOLineItemAddedFromInventoryCalcEvent(IEmployee emp, IPurchaseOrder po, PARBeverageLineItem baseItem, int? numBottles, 
            LiquidAmount amtDesired, Guid? vendorID);
		/// <summary>
		/// Marks when we send items to be purchased form vendor
		/// </summary>
		PurchaseOrderSentToVendorEvent CreatePurchaseOrderSentToVendorEvent(IEmployee emp, IPurchaseOrder po, Guid vendorID);

        /// <summary>
        /// Marks when a vendor has shipped our items to us
        /// </summary>
        /// <param name="emp">Employee taking in the goods</param>
        /// <param name="po">Purchase order we're</param>
        /// <param name="ro"></param>
        /// <param name="status">The status the Purchase order should be given</param>
        /// <returns></returns>
        PurchaseOrderRecievedFromVendorEvent CreatePurchaseOrderRecievedFromVendorEvent(IEmployee emp, IPurchaseOrder po, IReceivingOrder ro, PurchaseOrderStatus status);

		VendorCreatedEvent CreateVendorCreatedEvent(IEmployee emp, string name, StreetAddress address, PhoneNumber phone, EmailAddress email);
		VendorAddNewLineItemEvent CreateVendorLineItemAddedEvent(IEmployee emp, IBaseBeverageLineItem li, IVendor v);
		VendorRestaurantSetsPriceForReceivedItemEvent CreateRestaurantSetPriceEvent(IEmployee emp, IVendorBeverageLineItem li, IVendor v, Money pricePerUnit);

		EmployeeInvitedToApplicationEvent CreateEmployeeInvitedToApplicationEvent(IEmployee emp, EmailAddress destEmail, MiseAppTypes appType, RestaurantName restName);
		EmployeeAcceptsInvitationEvent CreateEmployeeAcceptsInvitationEvent(IApplicationInvitation invite, IEmployee emp);
		EmployeeRejectsInvitationEvent CreateEmployeeRejectsInvitiationEvent(IApplicationInvitation invite, IEmployee emp);

		PlaceholderRestaurantCreatedEvent CreatePlaceholderRestaurantCreatedEvent(IEmployee emp);
        NewRestaurantRegisteredOnAppEvent CreateNewRestaurantRegisteredOnAppEvent(IEmployee emp, RestaurantName name,
            StreetAddress address, PhoneNumber phone);
		EmployeeRegistersRestaurantEvent CreateEmployeeRegistersRestaurantEvent (IEmployee emp, IRestaurant rest);

		UserSelectedRestaurant CreateUserSelectedRestaurant (IEmployee emp, Guid restaurantID);

		AccountRegisteredFromMobileDeviceEvent CreateAccountRegisteredFromMobileDeviceEvent (IEmployee emp, Guid accountID, EmailAddress email, 
			PhoneNumber phone, CreditCard card, ReferralCode code, MiseAppTypes app, PersonName name);
        #endregion

    }
}