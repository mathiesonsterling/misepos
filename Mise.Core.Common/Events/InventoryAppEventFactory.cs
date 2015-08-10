using System;
using System.Collections.Generic;
using System.Linq;
using Mise.Core.Common.Events.ApplicationInvitations;
using Mise.Core.Entities.Inventory;
using Mise.Core.Entities.People;
using Mise.Core.Entities.Restaurant;
using Mise.Core.ValueItems;
using Mise.Core.Entities;

using Mise.Core.Common.Events.Employee;
using Mise.Core.Common.Events.Inventory;
using Mise.Core.Common.Events.Restaurant;
using Mise.Core.Common.Entities.Inventory;

using Mise.Core.ValueItems.Inventory;
using Mise.Core.Entities.Vendors;
using Mise.Core.Common.Events.Vendors;
using Mise.Core.Common.Events.Accounts;
using Mise.Core.Common.Services;


namespace Mise.Core.Common.Events
{
	public class InventoryAppEventFactory : IInventoryAppEventFactory
	{
		long _lastEventID;

		IRestaurant _restaurant;

		public Guid? RestaurantID {
			get {
				if (_restaurant != null) {
					return _restaurant.ID;
				}
				return null;
			}
		}

		string _deviceID;
		readonly MiseAppTypes _appCode;

		public InventoryAppEventFactory(string deviceID, MiseAppTypes appCode)
		{
			_lastEventID = 0;
			_deviceID = deviceID;
			_appCode = appCode;
		}

		public void SetRestaurant(IRestaurant restaurant){
			_restaurant = restaurant;
		}

		public void SetDeviceID(string id){
			_deviceID = id;
		}
		/// <summary>
		/// When loading from the repository, lets us tell our device to skip up to the last digit given
		/// </summary>
		/// <param name="last"></param>
		public void SetLastEventID(IEnumerable<EventID> last)
		{
			var lastForThisDevice = last.Where(id => id != null && id.AppInstanceCode == _appCode)
				.OrderByDescending(id => id.OrderingID)
				.FirstOrDefault();

			if (lastForThisDevice != null) {
				_lastEventID = lastForThisDevice.OrderingID;
			}
		}

		EventID GetNextEventID()
		{
			var orderingID = (_lastEventID++);
			if (orderingID == long.MaxValue) {
				orderingID = 1;
			}
			return new EventID { AppInstanceCode = _appCode, OrderingID = orderingID };
		}

	

	    public ReceivingOrderLineItemAddedEvent CreateReceivingOrderLineItemAddedEvent(IEmployee emp,
			IBaseBeverageLineItem source, int quantity, IReceivingOrder ro)
	    {
            return new ReceivingOrderLineItemAddedEvent
            {
                CausedByID = emp.ID,
                CreatedDate = DateTimeOffset.UtcNow,
                RestaurantID = _restaurant.ID,
                EventOrderingID = GetNextEventID(),
                DeviceID = _deviceID,
                UPC = source.UPC,
                Container = source.Container,
                CaseSize = source.CaseSize,
				MiseName = source.MiseName,
                DisplayName = source.DisplayName,
				Quantity = quantity,
				ReceivingOrderID = ro.ID,
				Categories = source.GetCategories ().Cast<ItemCategory>(),
                LineItemID = Guid.NewGuid()
            };
	    }

		public ReceivingOrderLineItemAddedEvent CreateReceivingOrderLineItemAddedEvent (IEmployee emp, string name, 
			string upc, IEnumerable<ItemCategory> categories, int caseSize, LiquidContainer container, int quantity, IReceivingOrder ro)
		{
			return new ReceivingOrderLineItemAddedEvent {
				CausedByID = emp.ID,
				CreatedDate = DateTimeOffset.UtcNow,
				RestaurantID = _restaurant.ID,
				EventOrderingID = GetNextEventID (),
				DeviceID = _deviceID,
				UPC = upc,
				Container = container,
				CaseSize = caseSize,
				DisplayName = name,
				Quantity = quantity,
				ReceivingOrderID = ro.ID,
				Categories = categories,
                LineItemID = Guid.NewGuid()
			};
		}			

		public InventoryLineItemAddedEvent CreateInventoryLineItemAddedEvent (IEmployee emp, 
			IBaseBeverageLineItem source, int quantity, Money pricePaid, Guid? vendorID, IInventorySection section, int inventoryPosition,  
			IInventory inventory)
		{
			return new InventoryLineItemAddedEvent {
				CausedByID = emp.ID,
				CreatedDate = DateTimeOffset.UtcNow,
				RestaurantID = _restaurant.ID,
				EventOrderingID = GetNextEventID(),
				DeviceID = _deviceID,
				UPC = source.UPC,
				Container = source.Container,
				CaseSize = source.CaseSize,
				DisplayName = source.DisplayName,
				MiseName = source.MiseName,
				Quantity = quantity,
				PricePaid = pricePaid,
				VendorBoughtFrom = vendorID,
				RestaurantInventorySectionID = section.RestaurantInventorySectionID,
                InventorySectionID = section.ID,
				InventoryID = inventory.ID,
				Categories = source.GetCategories ().Cast<ItemCategory>(),
                InventoryPosition =  inventoryPosition,
                LineItemID = Guid.NewGuid()
			};
		}


	    public PARCreatedEvent CreatePARCreatedEvent (IEmployee emp)
		{
			return new PARCreatedEvent {
				ID = Guid.NewGuid (),
				ParID = Guid.NewGuid (),
				CausedByID = emp.ID,
				CreatedDate = DateTimeOffset.UtcNow,
				DeviceID = _deviceID,
				EventOrderingID = GetNextEventID (),
				RestaurantID = _restaurant.ID
			};
		}			

		public PARLineItemAddedEvent CreatePARLineItemAddedEvent (IEmployee emp, IBaseBeverageLineItem source, 
			int? quantity, IPar par)
		{
			return new PARLineItemAddedEvent {
				CausedByID = emp.ID,
				CreatedDate = DateTimeOffset.UtcNow,
				RestaurantID = _restaurant.ID,
				EventOrderingID = GetNextEventID(),
				DeviceID = _deviceID,
				UPC = source.UPC,
				MiseName = source.MiseName,
				Container = source.Container,
				CaseSize = source.CaseSize,
				DisplayName = source.DisplayName,
				Quantity = quantity,
				ParID = par.ID,
				Categories = source.GetCategories ().Cast<ItemCategory>(),
                LineItemID = Guid.NewGuid()
			};
		}

		public PARLineItemAddedEvent CreatePARLineItemAddedEvent (IEmployee emp, string name, string upc, 
			IEnumerable<ItemCategory> categories, int caseSize, LiquidContainer container, int quantity, IPar par)
		{
			return new PARLineItemAddedEvent {
				CausedByID = emp.ID,
				CreatedDate = DateTimeOffset.UtcNow,
				RestaurantID = _restaurant.ID,
				EventOrderingID = GetNextEventID(),
				DeviceID = _deviceID,
				UPC = upc,
				Container = container,
				CaseSize = caseSize,
				DisplayName = name,
				Quantity = quantity,
				ParID = par.ID,
				Categories = categories,
                LineItemID = Guid.NewGuid()
			};
		}

		public PARLineItemQuantityUpdatedEvent CreatePARLineItemQuantityUpdatedEvent (IEmployee emp, IPar par, 
			Guid lineItemID, decimal quantity)
		{
			return new PARLineItemQuantityUpdatedEvent {
				CausedByID = emp.ID,
				CreatedDate = DateTimeOffset.UtcNow,
				RestaurantID = _restaurant.ID,
				EventOrderingID = GetNextEventID (),
				DeviceID = _deviceID,
				ParID = par.ID,
				UpdatedQuantity = quantity,
				LineItemID = lineItemID
			};
		}

		public InventoryLineItemAddedEvent CreateInventoryLineItemAddedEvent (IEmployee emp, string name, string upc,
			IEnumerable<ItemCategory> categories, int caseSize, LiquidContainer container, int quantity, Money pricePaid, Guid? vendorID, IInventorySection section,
			int inventoryPosisiton, IInventory inventory)
		{
			return new InventoryLineItemAddedEvent {
                ID = Guid.NewGuid(),
				CausedByID = emp.ID,
				CreatedDate = DateTimeOffset.UtcNow,
				RestaurantID = _restaurant.ID,
				EventOrderingID = GetNextEventID (),
				DeviceID = _deviceID,
				UPC = upc,
				DisplayName = name,
				Container = container,
				CaseSize = caseSize,
				Quantity = quantity,
				PricePaid = pricePaid,
				VendorBoughtFrom = vendorID,
				RestaurantInventorySectionID = section.RestaurantInventorySectionID,
                InventorySectionID = section.ID,
				InventoryID = inventory.ID,
				Categories = categories,
                InventoryPosition = inventoryPosisiton,
                LineItemID = Guid.NewGuid()
			};
		}

		public ReceivingOrderCreatedEvent CreateReceivingOrderCreatedEvent(IEmployee emp, IVendor vendor)
	    {
	        return new ReceivingOrderCreatedEvent
	        {
	            CausedByID = emp.ID,
	            CreatedDate = DateTimeOffset.UtcNow,
	            DeviceID = _deviceID,
	            ReceivingOrderID = Guid.NewGuid(),
	            EventOrderingID = GetNextEventID(),
	            ID = Guid.NewGuid(),
	            RestaurantID = _restaurant.ID,
				VendorID = vendor.ID,
	        };
	    }

		public ReceivingOrderAssociatedWithPOEvent CreateReceivingOrderAssociatedWithPOEvent (IEmployee emp, 
			IReceivingOrder ro, IPurchaseOrder po)
		{
			return new ReceivingOrderAssociatedWithPOEvent {
				CausedByID = emp.ID,
				CreatedDate = DateTimeOffset.UtcNow,
				DeviceID = _deviceID,
				ReceivingOrderID = ro.ID,
				EventOrderingID = GetNextEventID (),
				ID = Guid.NewGuid (),
				RestaurantID = _restaurant.ID,
				PurchaseOrderID = po.ID,
				PurchaseOrderSentDate = po.CreatedDate
			};
		}
			
		public ReceivingOrderCompletedEvent CreateReceivingOrderCompletedEvent(IEmployee emp, IReceivingOrder ro, 
			DateTimeOffset dateReceived, string notes, string invoiceID){
			return new ReceivingOrderCompletedEvent {
				CausedByID = emp.ID,
				ReceivingOrderID = ro.ID,
				CreatedDate = DateTimeOffset.UtcNow,
				DeviceID = _deviceID,
				EventOrderingID = GetNextEventID (),
				ID = Guid.NewGuid (),
				RestaurantID = _restaurant.ID,

				Notes = notes,
				InvoiceID = invoiceID,
				DateReceived = dateReceived
			};
		}

		public ReceivingOrderLineItemQuantityUpdatedEvent CreateROLineItemUpdateQuantityEvent (IEmployee emp, 
			IReceivingOrder ro, Guid lineItemID, int quantity, Money pricePerBottle)
		{
			return new ReceivingOrderLineItemQuantityUpdatedEvent {
				CausedByID = emp.ID,
				ReceivingOrderID = ro.ID,
				CreatedDate = DateTimeOffset.UtcNow,
				DeviceID = _deviceID,
				EventOrderingID = GetNextEventID (),
				ID = Guid.NewGuid (),
				RestaurantID = _restaurant.ID,
				LineItemID = lineItemID,
				UpdatedQuantity = quantity,
				LineItemPrice = pricePerBottle
			};
		}

		public ReceivingOrderLineItemZeroedOutEvent CreateReceivingOrderLineItemZeroedOutEvent (IEmployee emp, IReceivingOrder ro, Guid lineItemID)
		{
			return new ReceivingOrderLineItemZeroedOutEvent {
				CausedByID = emp.ID,
				ReceivingOrderID = ro.ID,
				CreatedDate = DateTimeOffset.UtcNow,
				DeviceID = _deviceID,
				EventOrderingID = GetNextEventID (),
				ID = Guid.NewGuid (),
				RestaurantID = _restaurant.ID,
				LineItemID = lineItemID,
			};
		}

		public EmployeeLoggedIntoInventoryAppEvent CreateEmployeeLoggedIntoInventoryAppEvent(IEmployee emp)
		{
			return new EmployeeLoggedIntoInventoryAppEvent {
				ID = Guid.NewGuid(),
				EmployeeID = emp.ID,
				EventOrderingID = GetNextEventID(),
				DeviceID = _deviceID,
			};
		}

		public EmployeeRegisteredForInventoryAppEvent CreateEmployeeRegisteredForInventoryAppEvent(IEmployee emp)
		{
			return new EmployeeRegisteredForInventoryAppEvent {
				ID = Guid.NewGuid(),
				EventOrderingID = GetNextEventID(),
				DeviceID = _deviceID,
                CreatedDate = DateTime.UtcNow,
				EmployeeID = emp.ID
			};
		}

		public EmployeeLoggedOutOfInventoryAppEvent CreateEmployeeLoggedOutOfInventoryAppEvent(IEmployee emp)
		{
			return new EmployeeLoggedOutOfInventoryAppEvent {
				ID = Guid.NewGuid(),
				CreatedDate = DateTimeOffset.UtcNow,
				EventOrderingID = GetNextEventID(),
				DeviceID = _deviceID,
				EmployeeID = emp.ID,
				CausedByID = emp.ID
			};
		}

		public InventoryCreatedEvent CreateInventoryCreatedEvent (IEmployee emp)
		{
			var date = DateTimeOffset.UtcNow;
			var revision = GetNextEventID ();

		    return new InventoryCreatedEvent {
				ID = Guid.NewGuid(),
				CreatedDate = date,
				DeviceID = _deviceID,
				EventOrderingID = revision,
				CausedByID = emp.ID,
				RestaurantID = _restaurant.ID,
				InventoryID = Guid.NewGuid ()
			};
		}

		public InventoryNewSectionAddedEvent CreateInventoryNewSectionAddedEvent (IEmployee emp, IInventory inventory, IRestaurantInventorySection restSection)
		{
			return new InventoryNewSectionAddedEvent {
				ID = Guid.NewGuid(),
				CreatedDate = DateTimeOffset.UtcNow,
				DeviceID = _deviceID,
				EventOrderingID = GetNextEventID(),
				CausedByID = emp.ID,
				RestaurantID = _restaurant.ID,
				InventoryID = inventory.ID,

				RestaurantSectionId = restSection.ID,
				Name = restSection.Name,
                SectionID = Guid.NewGuid()
			};
		}

		public InventorySectionCompletedEvent CreateInventorySectionCompletedEvent (IEmployee emp, IInventory inventory, IInventorySection section)
		{
			return new InventorySectionCompletedEvent {
				ID = Guid.NewGuid (),
				CreatedDate = DateTimeOffset.UtcNow,
				DeviceID = _deviceID,
				EventOrderingID = GetNextEventID (),
				CausedByID = emp.ID,
				RestaurantID = _restaurant.ID,
				InventoryID = inventory.ID,
                InventorySectionID = section.ID
			};
		}

		public InventorySectionAddedToRestaurantEvent CreateInventorySectionAddedToRestaurantEvent (IEmployee emp, string newSectionName, bool isDefaultSection, bool allowsPartialBottles)
		{
			return new InventorySectionAddedToRestaurantEvent {
				ID = Guid.NewGuid(),
				CreatedDate = DateTimeOffset.UtcNow,
				DeviceID = _deviceID,
				EventOrderingID = GetNextEventID(),
				CausedByID = emp.ID,
				RestaurantID = _restaurant.ID,

				SectionName = newSectionName,
				SectionID = Guid.NewGuid(),
				IsDefaultInventorySection = isDefaultSection,
				AllowsPartialBottles = allowsPartialBottles
			};
		}

		public InventoryLiquidItemMeasuredEvent CreateInventoryLiquidItemMeasuredEvent (IEmployee emp, IInventory inventory, 
			IInventorySection section, InventoryBeverageLineItem li, int numFullBottles, IEnumerable<decimal> partialBottlePercentages, LiquidAmount amtMeasured)
		{
			return new InventoryLiquidItemMeasuredEvent {
				ID = Guid.NewGuid (),
				InventoryID = inventory.ID,
				CreatedDate = DateTimeOffset.UtcNow,
				DeviceID = _deviceID,
				EventOrderingID = GetNextEventID (),
				CausedByID = emp.ID,
				RestaurantID = _restaurant.ID,
				InventorySectionID = section.ID,

				AmountMeasured = amtMeasured,
				NumFullBottlesMeasured = numFullBottles,
				PartialBottles = partialBottlePercentages.ToList(),
				BeverageLineItem = li
			};
		}

		public InventoryMadeCurrentEvent CreateInventoryMadeCurrentEvent(IEmployee emp, IInventory inventory){
			return new InventoryMadeCurrentEvent {
				ID = Guid.NewGuid (),
				InventoryID = inventory.ID,
				CreatedDate = DateTimeOffset.UtcNow,
				DeviceID = _deviceID,
				EventOrderingID = GetNextEventID (),
				CausedByID = emp.ID,
				RestaurantID = _restaurant.ID
			};
		}
		public InventoryCompletedEvent CreateInventoryCompletedEvent (IEmployee emp, IInventory inventory)
		{
			return new InventoryCompletedEvent {
				ID = Guid.NewGuid (),
				InventoryID = inventory.ID,
				CreatedDate = DateTimeOffset.UtcNow,
				DeviceID = _deviceID,
				EventOrderingID = GetNextEventID (),
				CausedByID = emp.ID,
				RestaurantID = _restaurant.ID
			};
		}

		public PurchaseOrderCreatedEvent CreatePurchaseOrderCreatedEvent (IEmployee emp)
		{
			return new PurchaseOrderCreatedEvent {
				ID = Guid.NewGuid (),
				PurchaseOrderID = Guid.NewGuid(),
				CreatedDate = DateTimeOffset.UtcNow,
				DeviceID = _deviceID,
				EventOrderingID = GetNextEventID (),
				CausedByID = emp.ID,
				RestaurantID = _restaurant.ID,

				EmployeeCreatingName = emp.DisplayName,
			};
		}

		public PurchaseOrderLineItemAddedFromInventoryCalculationEvent CreatePOLineItemAddedFromInventoryCalcEvent (IEmployee emp, IPurchaseOrder po, 
			ParBeverageLineItem baseItem, int? numBottles, LiquidAmount amtDesired, Guid? vendorID)
		{
			return new PurchaseOrderLineItemAddedFromInventoryCalculationEvent {
				ID = Guid.NewGuid (),
				PurchaseOrderID = po.ID,
				CreatedDate = DateTimeOffset.UtcNow,
				DeviceID = _deviceID,
				EventOrderingID = GetNextEventID (),
				CausedByID = emp.ID,
				RestaurantID = _restaurant.ID,

				AmountNeeded = amtDesired,
				NumBottlesNeeded = numBottles,
				PARLineItem = baseItem,
				VendorWithBestPriceID = vendorID,
				Categories = baseItem.Categories,
                LineItemID = Guid.NewGuid()
			};
		}

		public PurchaseOrderSentToVendorEvent CreatePurchaseOrderSentToVendorEvent (IEmployee emp, IPurchaseOrder po, Guid vendorID)
		{
			return new PurchaseOrderSentToVendorEvent {
				ID = Guid.NewGuid (),
				PurchaseOrderID = po.ID,
				CreatedDate = DateTimeOffset.UtcNow,
				DeviceID = _deviceID,
				EventOrderingID = GetNextEventID (),
				CausedByID = emp.ID,
				RestaurantID = _restaurant.ID,
				VendorID = vendorID,
			};
		}

	    public PurchaseOrderRecievedFromVendorEvent CreatePurchaseOrderRecievedFromVendorEvent(IEmployee emp, IPurchaseOrder po,
	        IReceivingOrder ro, PurchaseOrderStatus status)
	    {
			return new PurchaseOrderRecievedFromVendorEvent {
				ID = Guid.NewGuid (),
				CausedByID = emp.ID,
				CreatedDate = DateTimeOffset.UtcNow,
				DeviceID = _deviceID,
				EventOrderingID = GetNextEventID (),
				PurchaseOrderID = po.ID,
				Status = status,
				ReceivingOrderID = ro.ID
			};
	    }

	    public VendorCreatedEvent CreateVendorCreatedEvent (IEmployee emp, string name, StreetAddress address, PhoneNumber phone, EmailAddress email)
		{
			return new VendorCreatedEvent {
				ID = Guid.NewGuid (),
				VendorID = Guid.NewGuid (),
				CreatedDate = DateTimeOffset.UtcNow,
				DeviceID = _deviceID,
				EventOrderingID = GetNextEventID (),
				CausedByID = emp.ID,
				RestaurantID = _restaurant.ID,

				Name = name,
				Address = address,
				PhoneNumber = phone,
                Email = email
			};
		}

		public VendorAddNewLineItemEvent CreateVendorLineItemAddedEvent (IEmployee emp, IBaseBeverageLineItem source, IVendor v)
		{
			return new VendorAddNewLineItemEvent {
				ID = Guid.NewGuid (),
				VendorID = v.ID,
				CreatedDate = DateTimeOffset.UtcNow,
				DeviceID = _deviceID,
				EventOrderingID = GetNextEventID (),
				CausedByID = emp.ID,
				RestaurantID = _restaurant.ID,
				UPC = source.UPC,
				DisplayName = source.DisplayName,
				MiseName = source.MiseName,
				Container = source.Container,
				CaseSize = source.CaseSize,
                LineItemID = Guid.NewGuid()
			};
		}
			
		public VendorRestaurantSetsPriceForReceivedItemEvent CreateRestaurantSetPriceEvent (IEmployee emp, IVendorBeverageLineItem li, IVendor v, Money pricePerUnit)
		{
			return new VendorRestaurantSetsPriceForReceivedItemEvent {
				ID = Guid.NewGuid(),
				VendorID = v.ID,
				CreatedDate = DateTimeOffset.UtcNow,
				DeviceID = _deviceID,
				EventOrderingID = GetNextEventID(),
				CausedByID = emp.ID,
				RestaurantID = _restaurant.ID,

				VendorLineItemID = li.ID,
				PricePerUnit = pricePerUnit
			};
		}

		public EmployeeInvitedToApplicationEvent CreateEmployeeInvitedToApplicationEvent (IEmployee emp, EmailAddress destEmail, MiseAppTypes app, RestaurantName restName)
		{
			return new EmployeeInvitedToApplicationEvent {
				ID = Guid.NewGuid (),
				InvitationID = Guid.NewGuid (),
				CreatedDate = DateTimeOffset.UtcNow,
				DeviceID = _deviceID,
				EventOrderingID = GetNextEventID (),
				CausedByID = emp.ID,
				RestaurantID = _restaurant.ID,
                RestaurantName = restName,
				EmailToInvite = destEmail,
				Application = app
			};
		}

		public EmployeeAcceptsInvitationEvent CreateEmployeeAcceptsInvitationEvent (IApplicationInvitation invite, IEmployee emp)
		{
			return new EmployeeAcceptsInvitationEvent {
				ID = Guid.NewGuid (),
				InvitationID = invite.ID,
				CreatedDate = DateTimeOffset.UtcNow,
				DeviceID = _deviceID,
				EventOrderingID = GetNextEventID (),
				CausedByID = emp.ID,
				RestaurantID = _restaurant.ID
			};
		}

		public EmployeeRejectsInvitationEvent CreateEmployeeRejectsInvitiationEvent (IApplicationInvitation invite, IEmployee emp)
		{
			return new EmployeeRejectsInvitationEvent {
				ID = Guid.NewGuid (),
				InvitationID = invite.ID,
				CreatedDate = DateTimeOffset.UtcNow,
				DeviceID = _deviceID,
				EventOrderingID = GetNextEventID (),
				CausedByID = emp.ID,
				RestaurantID = _restaurant.ID
			};
		}

		public EmployeeCreatedEvent CreateEmployeeCreatedEvent (EmailAddress email, Password password, PersonName name, MiseAppTypes appType)
		{
			return new EmployeeCreatedEvent {
				ID = Guid.NewGuid(),
				EmployeeID = Guid.NewGuid(),
				CreatedDate = DateTimeOffset.UtcNow,
				DeviceID = _deviceID,
				EventOrderingID = GetNextEventID(),
				Email = email,
                Name = name,
				Password = password,
                AppType = appType,
			};
		}

		public PlaceholderRestaurantCreatedEvent CreatePlaceholderRestaurantCreatedEvent (IEmployee emp)
		{
			return new PlaceholderRestaurantCreatedEvent {
				ID = Guid.NewGuid (),
                CreatedDate = DateTimeOffset.UtcNow,
                DeviceID = _deviceID,
                EventOrderingID = GetNextEventID(),

				CausedByID = emp.ID,
                RestaurantID = Guid.NewGuid()
			};
		}

	    public NewRestaurantRegisteredOnAppEvent CreateNewRestaurantRegisteredOnAppEvent(IEmployee emp, RestaurantName name,
	        StreetAddress address, PhoneNumber phone)
	    {
	        return new NewRestaurantRegisteredOnAppEvent
	        {
	            ID = Guid.NewGuid(),
	            CreatedDate = DateTimeOffset.UtcNow,
	            DeviceID = _deviceID,
	            EventOrderingID = GetNextEventID(),

	            CausedByID = emp.ID,
	            RestaurantID = Guid.NewGuid(),
	            Name = name,
	            StreetAddress = address,
	            PhoneNumber = phone,
	        };
	    }

		public EmployeeRegistersRestaurantEvent CreateEmployeeRegistersRestaurantEvent(IEmployee emp, IRestaurant rest){
			return new EmployeeRegistersRestaurantEvent {
				ID = Guid.NewGuid (),
				CreatedDate = DateTimeOffset.Now,
				DeviceID = _deviceID,
				EventOrderingID = GetNextEventID (),

				CausedByID = emp.ID,
				RestaurantID = rest.ID,
				EmployeeID = emp.ID
			};
		}

		public UserSelectedRestaurant CreateUserSelectedRestaurant (IEmployee emp, Guid restaurantID)
		{
			return new UserSelectedRestaurant {
				ID = Guid.NewGuid (),
				CreatedDate = DateTimeOffset.UtcNow,
				DeviceID = _deviceID,
				EventOrderingID = GetNextEventID (),

				CausedByID = emp.ID,
				RestaurantID = restaurantID,
			};
		}

		public AccountRegisteredFromMobileDeviceEvent CreateAccountRegisteredFromMobileDeviceEvent (IEmployee emp, 
			Guid accountID, EmailAddress email, PhoneNumber phone, CreditCard card, ReferralCode code, MiseAppTypes app, PersonName name)
		{
			return new AccountRegisteredFromMobileDeviceEvent {
				ID = Guid.NewGuid (),
				CreatedDate = DateTimeOffset.UtcNow,
				DeviceID = _deviceID,
				EventOrderingID = GetNextEventID (),

				CausedByID = emp.ID,
				AccountID = accountID,
				Email = email,
				ReferralCode = code,
				PhoneNumber = phone,
				AccountType = MiseAccountTypes.Restaurant,
				AccountHolderName = name,
				AppType = app
			};
		}
	}
}

