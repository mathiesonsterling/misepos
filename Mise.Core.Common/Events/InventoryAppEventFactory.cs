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


namespace Mise.Core.Common.Events
{
	public class InventoryAppEventFactory : IInventoryAppEventFactory
	{
		long _lastEventID;

		IRestaurant _restaurant;

		public Guid? RestaurantID {
			get {
				if (_restaurant != null) {
					return _restaurant.Id;
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

		private DateTimeOffset GetDate(){
			return DateTimeOffset.UtcNow;
		}

	    public ReceivingOrderLineItemAddedEvent CreateReceivingOrderLineItemAddedEvent(IEmployee emp,
			IBaseBeverageLineItem source, int quantity, IReceivingOrder ro)
	    {
            return new ReceivingOrderLineItemAddedEvent
            {
				Id = Guid.NewGuid (),
                CausedById = emp.Id,
                CreatedDate = GetDate(),
                RestaurantId = _restaurant.Id,
                EventOrder = GetNextEventID(),
                DeviceId = _deviceID,
                UPC = source.UPC,
                Container = source.Container,
                CaseSize = source.CaseSize,
				MiseName = source.MiseName,
                DisplayName = source.DisplayName,
				Quantity = quantity,
				ReceivingOrderID = ro.Id,
				Categories = source.GetCategories () != null
					? source.GetCategories ().Cast<ItemCategory>()
					: new List<ItemCategory>(),
                LineItemID = Guid.NewGuid()
            };
	    }

		public ReceivingOrderLineItemAddedEvent CreateReceivingOrderLineItemAddedEvent (IEmployee emp, string name, 
			string upc, IEnumerable<ItemCategory> categories, int caseSize, LiquidContainer container, int quantity, IReceivingOrder ro)
		{
			return new ReceivingOrderLineItemAddedEvent {
				Id = Guid.NewGuid (),
				CausedById = emp.Id,
				CreatedDate = GetDate(),
				RestaurantId = _restaurant.Id,
				EventOrder = GetNextEventID (),
				DeviceId = _deviceID,
				UPC = upc,
				Container = container,
				CaseSize = caseSize,
				DisplayName = name,
				Quantity = quantity,
				ReceivingOrderID = ro.Id,
				Categories = categories,
                LineItemID = Guid.NewGuid()
			};
		}			

		public InventoryLineItemAddedEvent CreateInventoryLineItemAddedEvent (IEmployee emp, 
			IBaseBeverageLineItem source, int quantity, Guid? vendorID, IInventorySection section, int inventoryPosition,  
			IInventory inventory)
		{
			return new InventoryLineItemAddedEvent {
				Id = Guid.NewGuid (),
				CausedById = emp.Id,
				CreatedDate = GetDate(),
				RestaurantId = _restaurant.Id,
				EventOrder = GetNextEventID(),
				DeviceId = _deviceID,
				UPC = source.UPC,
				Container = source.Container,
				CaseSize = source.CaseSize,
				DisplayName = source.DisplayName,
				MiseName = source.MiseName,
				Quantity = quantity,
				VendorBoughtFrom = vendorID,
				RestaurantInventorySectionID = section.RestaurantInventorySectionID,
                InventorySectionID = section.Id,
				InventoryID = inventory.Id,
				Categories = source.GetCategories ().Cast<ItemCategory>(),
                InventoryPosition =  inventoryPosition,
                LineItemID = Guid.NewGuid()
			};
		}

		public InventoryLineItemDeletedEvent CreateInventoryLineItemDeletedEvent (IEmployee emp, IInventory inv,
			IInventorySection sec, IInventoryBeverageLineItem li)
		{
			return new InventoryLineItemDeletedEvent {
				Id = Guid.NewGuid (),
				CausedById = emp.Id,
				CreatedDate = GetDate (),
				EventOrder = GetNextEventID (),
				DeviceId = _deviceID,
				RestaurantId = _restaurant.Id,

				InventoryID = inv.Id,
				InventorySectionID = sec.Id,
				InventoryLineItemID = li.Id
			};
		}

		public InventoryLineItemMovedToNewPositionEvent CreateInventoryLineItemMovedToNewPositionEvent (IEmployee emp, IInventory inv, 
			IInventorySection sec, IInventoryBeverageLineItem li, int newPosition)
		{
			return new InventoryLineItemMovedToNewPositionEvent {
				Id = Guid.NewGuid (),
				CausedById = emp.Id,
				CreatedDate = GetDate (),
				EventOrder = GetNextEventID (),
				DeviceId = _deviceID,
				RestaurantId = _restaurant.Id,

				InventoryID = inv.Id,
				InventorySectionID = sec.Id,
				LineItemID = li.Id,
				NewPositionWanted = newPosition
			};
		}

	    public PARCreatedEvent CreatePARCreatedEvent (IEmployee emp)
		{
			return new PARCreatedEvent {
				Id = Guid.NewGuid (),
				ParID = Guid.NewGuid (),
				CausedById = emp.Id,
				CreatedDate = GetDate(),
				DeviceId = _deviceID,
				EventOrder = GetNextEventID (),
				RestaurantId = _restaurant.Id
			};
		}			

		public PARLineItemAddedEvent CreatePARLineItemAddedEvent (IEmployee emp, IBaseBeverageLineItem source, 
			int? quantity, IPar par)
		{
			return new PARLineItemAddedEvent {
				Id = Guid.NewGuid (),
				CausedById = emp.Id,
				CreatedDate = GetDate(),
				RestaurantId = _restaurant.Id,
				EventOrder = GetNextEventID(),
				DeviceId = _deviceID,
				UPC = source.UPC,
				MiseName = source.MiseName,
				Container = source.Container,
				CaseSize = source.CaseSize,
				DisplayName = source.DisplayName,
				Quantity = quantity,
				ParID = par.Id,
				Categories = source.GetCategories () != null 
					? source.GetCategories ().Cast<ItemCategory>() 
					: new List<ItemCategory>(),
                LineItemID = Guid.NewGuid()
			};
		}

		public PARLineItemAddedEvent CreatePARLineItemAddedEvent (IEmployee emp, string name, string upc, 
			IEnumerable<ItemCategory> categories, int caseSize, LiquidContainer container, int quantity, IPar par)
		{
			return new PARLineItemAddedEvent {
				Id = Guid.NewGuid (),
				CausedById = emp.Id,
				CreatedDate = GetDate(),
				RestaurantId = _restaurant.Id,
				EventOrder = GetNextEventID(),
				DeviceId = _deviceID,
				UPC = upc,
				Container = container,
				CaseSize = caseSize,
				DisplayName = name,
				Quantity = quantity,
				ParID = par.Id,
				Categories = categories,
                LineItemID = Guid.NewGuid()
			};
		}

		public ParLineItemDeletedEvent CreateParLineItemDeletedEvent (IEmployee emp, IPar par, IParBeverageLineItem li)
		{
			return new ParLineItemDeletedEvent{
				Id = Guid.NewGuid (),
				CausedById = emp.Id,
				CreatedDate = GetDate (),
				RestaurantId = _restaurant.Id,
				EventOrder = GetNextEventID (),
				DeviceId = _deviceID,
				ParID = par.Id,
				LineItemId = li.Id
			};
		}

		public PARLineItemQuantityUpdatedEvent CreatePARLineItemQuantityUpdatedEvent (IEmployee emp, IPar par, 
			Guid lineItemID, decimal quantity)
		{
			return new PARLineItemQuantityUpdatedEvent {
				Id = Guid.NewGuid (),
				CausedById = emp.Id,
				CreatedDate = GetDate(),
				RestaurantId = _restaurant.Id,
				EventOrder = GetNextEventID (),
				DeviceId = _deviceID,
				ParID = par.Id,
				UpdatedQuantity = quantity,
				LineItemID = lineItemID
			};
		}

		public InventoryLineItemAddedEvent CreateInventoryLineItemAddedEvent (IEmployee emp, string name, string upc,
			IEnumerable<ItemCategory> categories, int caseSize, LiquidContainer container, int quantity,  Guid? vendorID, 
            IInventorySection section, int inventoryPosisiton, IInventory inventory)
		{
			return new InventoryLineItemAddedEvent {
                Id = Guid.NewGuid(),
				CausedById = emp.Id,
				CreatedDate = GetDate(),
				RestaurantId = _restaurant.Id,
				EventOrder = GetNextEventID (),
				DeviceId = _deviceID,
				UPC = upc,
				DisplayName = name,
				Container = container,
				CaseSize = caseSize,
				Quantity = quantity,
				VendorBoughtFrom = vendorID,
				RestaurantInventorySectionID = section.RestaurantInventorySectionID,
                InventorySectionID = section.Id,
				InventoryID = inventory.Id,
				Categories = categories,
                InventoryPosition = inventoryPosisiton,
                LineItemID = Guid.NewGuid()
			};
		}

		public ReceivingOrderCreatedEvent CreateReceivingOrderCreatedEvent(IEmployee emp, IVendor vendor)
	    {
	        return new ReceivingOrderCreatedEvent
	        {
	            CausedById = emp.Id,
	            CreatedDate = GetDate(),
	            DeviceId = _deviceID,
	            ReceivingOrderID = Guid.NewGuid(),
	            EventOrder = GetNextEventID(),
	            Id = Guid.NewGuid(),
	            RestaurantId = _restaurant.Id,
				VendorID = vendor.Id,
	        };
	    }

		public ReceivingOrderAssociatedWithPOEvent CreateReceivingOrderAssociatedWithPOEvent (IEmployee emp, 
			IReceivingOrder ro, IPurchaseOrder po)
		{
			return new ReceivingOrderAssociatedWithPOEvent {
				CausedById = emp.Id,
				CreatedDate = GetDate(),
				DeviceId = _deviceID,
				ReceivingOrderID = ro.Id,
				EventOrder = GetNextEventID (),
				Id = Guid.NewGuid (),
				RestaurantId = _restaurant.Id,
				PurchaseOrderID = po.Id,
				PurchaseOrderSentDate = po.CreatedDate
			};
		}
			
		public ReceivingOrderCompletedEvent CreateReceivingOrderCompletedEvent(IEmployee emp, IReceivingOrder ro, 
			DateTimeOffset dateReceived, string notes, string invoiceID){
			return new ReceivingOrderCompletedEvent {
				CausedById = emp.Id,
				ReceivingOrderID = ro.Id,
				CreatedDate = GetDate(),
				DeviceId = _deviceID,
				EventOrder = GetNextEventID (),
				Id = Guid.NewGuid (),
				RestaurantId = _restaurant.Id,

				Notes = notes,
				InvoiceID = invoiceID,
				DateReceived = dateReceived
			};
		}

		public ReceivingOrderLineItemQuantityUpdatedEvent CreateROLineItemUpdateQuantityEvent (IEmployee emp, 
			IReceivingOrder ro, Guid lineItemID, int quantity, Money pricePerBottle)
		{
			return new ReceivingOrderLineItemQuantityUpdatedEvent {
				CausedById = emp.Id,
				ReceivingOrderID = ro.Id,
				CreatedDate = GetDate(),
				DeviceId = _deviceID,
				EventOrder = GetNextEventID (),
				Id = Guid.NewGuid (),
				RestaurantId = _restaurant.Id,
				LineItemID = lineItemID,
				UpdatedQuantity = quantity,
				LineItemPrice = pricePerBottle
			};
		}

		public ReceivingOrderLineItemZeroedOutEvent CreateReceivingOrderLineItemZeroedOutEvent (IEmployee emp, IReceivingOrder ro, Guid lineItemID)
		{
			return new ReceivingOrderLineItemZeroedOutEvent {
				CausedById = emp.Id,
				ReceivingOrderID = ro.Id,
				CreatedDate = GetDate(),
				DeviceId = _deviceID,
				EventOrder = GetNextEventID (),
				Id = Guid.NewGuid (),
				RestaurantId = _restaurant.Id,
				LineItemID = lineItemID,
			};
		}

		public ReceivingOrderLineItemDeletedEvent CreateReceivingOrderLineItemDeletedEvent (IEmployee emp, IReceivingOrder receivingOrder, IReceivingOrderLineItem lineItem)
		{
			return new ReceivingOrderLineItemDeletedEvent {
				Id = Guid.NewGuid (),
				CreatedDate = GetDate (),
				CausedById = emp.Id,
				EventOrder = GetNextEventID (),
				DeviceId = _deviceID,
				RestaurantId = _restaurant.Id,

				ReceivingOrderID = receivingOrder.Id,
				LineItemId = lineItem.Id
			};
		}

		public EmployeeLoggedIntoInventoryAppEvent CreateEmployeeLoggedIntoInventoryAppEvent(IEmployee emp)
		{
			return new EmployeeLoggedIntoInventoryAppEvent {
				Id = Guid.NewGuid(),
				EmployeeID = emp.Id,
				CreatedDate = GetDate (),
				EventOrder = GetNextEventID(),
				DeviceId = _deviceID,
			};
		}

		public EmployeeRegisteredForInventoryAppEvent CreateEmployeeRegisteredForInventoryAppEvent(IEmployee emp)
		{
			return new EmployeeRegisteredForInventoryAppEvent {
				Id = Guid.NewGuid(),
				EventOrder = GetNextEventID(),
				DeviceId = _deviceID,
                CreatedDate = GetDate (),
				EmployeeID = emp.Id,

			};
		}

		public EmployeeLoggedOutOfInventoryAppEvent CreateEmployeeLoggedOutOfInventoryAppEvent(IEmployee emp)
		{
			return new EmployeeLoggedOutOfInventoryAppEvent {
				Id = Guid.NewGuid(),
				CreatedDate = GetDate(),
				EventOrder = GetNextEventID(),
				DeviceId = _deviceID,
				EmployeeID = emp.Id,
				CausedById = emp.Id,
				RestaurantId = _restaurant.Id
			};
		}

		public InventoryCreatedEvent CreateInventoryCreatedEvent (IEmployee emp)
		{
		    return new InventoryCreatedEvent {
				Id = Guid.NewGuid(),
				CreatedDate = GetDate (),
				DeviceId = _deviceID,
				EventOrder = GetNextEventID (),
				CausedById = emp.Id,
				RestaurantId = _restaurant.Id,
				InventoryID = Guid.NewGuid ()
			};
		}

		public InventoryNewSectionAddedEvent CreateInventoryNewSectionAddedEvent (IEmployee emp, IInventory inventory, IRestaurantInventorySection restSection)
		{
			return new InventoryNewSectionAddedEvent {
				Id = Guid.NewGuid(),
				CreatedDate = GetDate(),
				DeviceId = _deviceID,
				EventOrder = GetNextEventID(),
				CausedById = emp.Id,
				RestaurantId = _restaurant.Id,
				InventoryID = inventory.Id,

				RestaurantSectionId = restSection.Id,
				Name = restSection.Name,
                SectionID = Guid.NewGuid()
			};
		}

		public InventorySectionStartedByEmployeeEvent CreateInventorySectionStartedByEmployeeEvent (IEmployee emp, IInventory inventory, IInventorySection section)
		{
			return new InventorySectionStartedByEmployeeEvent {
				Id = Guid.NewGuid (),
				CreatedDate = GetDate (),
				DeviceId = _deviceID,
				EventOrder = GetNextEventID (),
				CausedById = emp.Id,
				RestaurantId = _restaurant.Id,
				InventoryID = inventory.Id,
				InventorySectionId = section.Id,
			};
		}

		public InventorySectionCompletedEvent CreateInventorySectionCompletedEvent (IEmployee emp, IInventory inventory, IInventorySection section)
		{
			return new InventorySectionCompletedEvent {
				Id = Guid.NewGuid (),
				CreatedDate = GetDate(),
				DeviceId = _deviceID,
				EventOrder = GetNextEventID (),
				CausedById = emp.Id,
				RestaurantId = _restaurant.Id,
				InventoryID = inventory.Id,
                InventorySectionID = section.Id
			};
		}

		public InventorySectionClearedEvent CreateInventorySectionClearedEvent (IEmployee emp, IInventory inventory, IInventorySection sec)
		{
			return new InventorySectionClearedEvent {
				Id = Guid.NewGuid (),
				CreatedDate = GetDate (),
				DeviceId = _deviceID,
				EventOrder = GetNextEventID (),
				CausedById = emp.Id,
				RestaurantId = _restaurant.Id,
				InventoryID = inventory.Id,
				SectionId = sec.Id
			};
		}

		public InventorySectionAddedToRestaurantEvent CreateInventorySectionAddedToRestaurantEvent (IEmployee emp, string newSectionName, bool isDefaultSection, bool allowsPartialBottles)
		{
			return new InventorySectionAddedToRestaurantEvent {
				Id = Guid.NewGuid(),
				CreatedDate = GetDate(),
				DeviceId = _deviceID,
				EventOrder = GetNextEventID(),
				CausedById = emp.Id,
				RestaurantId = _restaurant.Id,

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
				Id = Guid.NewGuid (),
				InventoryID = inventory.Id,
				CreatedDate = GetDate(),
				DeviceId = _deviceID,
				EventOrder = GetNextEventID (),
				CausedById = emp.Id,
				RestaurantId = _restaurant.Id,
				InventorySectionID = section.Id,

				AmountMeasured = amtMeasured,
				NumFullBottlesMeasured = numFullBottles,
				PartialBottles = partialBottlePercentages.ToList(),
				BeverageLineItem = li
			};
		}

		public InventoryMadeCurrentEvent CreateInventoryMadeCurrentEvent(IEmployee emp, IInventory inventory){
			return new InventoryMadeCurrentEvent {
				Id = Guid.NewGuid (),
				InventoryID = inventory.Id,
				CreatedDate = GetDate(),
				DeviceId = _deviceID,
				EventOrder = GetNextEventID (),
				CausedById = emp.Id,
				RestaurantId = _restaurant.Id
			};
		}
		public InventoryCompletedEvent CreateInventoryCompletedEvent (IEmployee emp, IInventory inventory)
		{
			return new InventoryCompletedEvent {
				Id = Guid.NewGuid (),
				InventoryID = inventory.Id,
				CreatedDate = GetDate(),
				DeviceId = _deviceID,
				EventOrder = GetNextEventID (),
				CausedById = emp.Id,
				RestaurantId = _restaurant.Id
			};
		}

		public PurchaseOrderCreatedEvent CreatePurchaseOrderCreatedEvent (IEmployee emp)
		{
			return new PurchaseOrderCreatedEvent {
				Id = Guid.NewGuid (),
				PurchaseOrderID = Guid.NewGuid(),
				CreatedDate = GetDate(),
				DeviceId = _deviceID,
				EventOrder = GetNextEventID (),
				CausedById = emp.Id,
				RestaurantId = _restaurant.Id,

				EmployeeCreatingName = emp.DisplayName,
			};
		}

		public PurchaseOrderLineItemAddedFromInventoryCalculationEvent CreatePOLineItemAddedFromInventoryCalcEvent (IEmployee emp, IPurchaseOrder po, 
			ParBeverageLineItem baseItem, int? numBottles, LiquidAmount amtDesired, Guid? vendorID)
		{
			return new PurchaseOrderLineItemAddedFromInventoryCalculationEvent {
				Id = Guid.NewGuid (),
				PurchaseOrderID = po.Id,
				CreatedDate = GetDate(),
				DeviceId = _deviceID,
				EventOrder = GetNextEventID (),
				CausedById = emp.Id,
				RestaurantId = _restaurant.Id,

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
				Id = Guid.NewGuid (),
				PurchaseOrderID = po.Id,
				CreatedDate = GetDate(),
				DeviceId = _deviceID,
				EventOrder = GetNextEventID (),
				CausedById = emp.Id,
				RestaurantId = _restaurant.Id,
				VendorID = vendorID,
			};
		}

	    public PurchaseOrderRecievedFromVendorEvent CreatePurchaseOrderRecievedFromVendorEvent(IEmployee emp, IPurchaseOrder po,
	        IReceivingOrder ro, PurchaseOrderStatus status)
	    {
			return new PurchaseOrderRecievedFromVendorEvent {
				Id = Guid.NewGuid (),
				CausedById = emp.Id,
				CreatedDate = GetDate(),
				DeviceId = _deviceID,
				EventOrder = GetNextEventID (),
				PurchaseOrderID = po.Id,
				Status = status,
				ReceivingOrderID = ro.Id,
				RestaurantId = _restaurant.Id
			};
	    }

	    public VendorCreatedEvent CreateVendorCreatedEvent (IEmployee emp, string name, StreetAddress address, PhoneNumber phone, EmailAddress email)
		{
			return new VendorCreatedEvent {
				Id = Guid.NewGuid (),
				VendorID = Guid.NewGuid (),
				CreatedDate = GetDate(),
				DeviceId = _deviceID,
				EventOrder = GetNextEventID (),
				CausedById = emp.Id,
				RestaurantId = _restaurant.Id,

				Name = name,
				Address = address,
				PhoneNumber = phone,
                Email = email
			};
		}

		public VendorAddNewLineItemEvent CreateVendorLineItemAddedEvent (IEmployee emp, IBaseBeverageLineItem source, IVendor v)
		{
			return new VendorAddNewLineItemEvent {
				Id = Guid.NewGuid (),
				VendorID = v.Id,
				CreatedDate = GetDate(),
				DeviceId = _deviceID,
				EventOrder = GetNextEventID (),
				CausedById = emp.Id,
				RestaurantId = _restaurant.Id,
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
				Id = Guid.NewGuid(),
				VendorID = v.Id,
				CreatedDate = GetDate(),
				DeviceId = _deviceID,
				EventOrder = GetNextEventID(),
				CausedById = emp.Id,
				RestaurantId = _restaurant.Id,

				VendorLineItemID = li.Id,
				PricePerUnit = pricePerUnit
			};
		}

		public EmployeeInvitedToApplicationEvent CreateEmployeeInvitedToApplicationEvent (IEmployee emp, EmailAddress destEmail, MiseAppTypes app, RestaurantName restName)
		{
			return new EmployeeInvitedToApplicationEvent {
				Id = Guid.NewGuid (),
				InvitationID = Guid.NewGuid (),
				CreatedDate = GetDate(),
				DeviceId = _deviceID,
				EventOrder = GetNextEventID (),
				CausedById = emp.Id,
				RestaurantId = _restaurant.Id,
                RestaurantName = restName,
				EmailToInvite = destEmail,
				Application = app
			};
		}

		public EmployeeAcceptsInvitationEvent CreateEmployeeAcceptsInvitationEvent (IApplicationInvitation invite, IEmployee emp)
		{
			return new EmployeeAcceptsInvitationEvent {
				Id = Guid.NewGuid (),
				InvitationID = invite.Id,
				CreatedDate = GetDate(),
				DeviceId = _deviceID,
				EventOrder = GetNextEventID (),
				CausedById = emp.Id,
				RestaurantId = _restaurant.Id
			};
		}

		public EmployeeRejectsInvitationEvent CreateEmployeeRejectsInvitiationEvent (IApplicationInvitation invite, IEmployee emp)
		{
			return new EmployeeRejectsInvitationEvent {
				Id = Guid.NewGuid (),
				InvitationID = invite.Id,
				CreatedDate = GetDate(),
				DeviceId = _deviceID,
				EventOrder = GetNextEventID (),
				CausedById = emp.Id,
				RestaurantId = _restaurant.Id
			};
		}

		public EmployeeCreatedEvent CreateEmployeeCreatedEvent (EmailAddress email, Password password, PersonName name, MiseAppTypes appType)
		{
		    var empID = Guid.NewGuid();
			return new EmployeeCreatedEvent {
				Id = Guid.NewGuid(),
                CausedById = empID,
				EmployeeID = empID,
				CreatedDate = GetDate(),
				DeviceId = _deviceID,
				EventOrder = GetNextEventID(),
				Email = email,
                Name = name,
				Password = password,
                AppType = appType,
			};
		}

		public PlaceholderRestaurantCreatedEvent CreatePlaceholderRestaurantCreatedEvent (IEmployee emp)
		{
			return new PlaceholderRestaurantCreatedEvent {
				Id = Guid.NewGuid (),
                CreatedDate = GetDate(),
                DeviceId = _deviceID,
                EventOrder = GetNextEventID(),

				CausedById = emp.Id,
                RestaurantId = Guid.NewGuid()
			};
		}

	    public NewRestaurantRegisteredOnAppEvent CreateNewRestaurantRegisteredOnAppEvent(IEmployee emp, RestaurantName name,
	        StreetAddress address, PhoneNumber phone)
	    {
	        return new NewRestaurantRegisteredOnAppEvent
	        {
	            Id = Guid.NewGuid(),
	            CreatedDate = GetDate(),
	            DeviceId = _deviceID,
	            EventOrder = GetNextEventID(),

	            CausedById = emp.Id,
	            RestaurantId = Guid.NewGuid(),
	            Name = name,
	            StreetAddress = address,
	            PhoneNumber = phone,
	        };
	    }

		public EmployeeRegistersRestaurantEvent CreateEmployeeRegistersRestaurantEvent(IEmployee emp, IRestaurant rest){
			return new EmployeeRegistersRestaurantEvent {
				Id = Guid.NewGuid (),
				CreatedDate = GetDate(),
				DeviceId = _deviceID,
				EventOrder = GetNextEventID (),

				CausedById = emp.Id,
				RestaurantId = rest.Id,
				EmployeeID = emp.Id
			};
		}

		public UserSelectedRestaurant CreateUserSelectedRestaurant (IEmployee emp, Guid restaurantID)
		{
			return new UserSelectedRestaurant {
				Id = Guid.NewGuid (),
				CreatedDate = GetDate(),
				DeviceId = _deviceID,
				EventOrder = GetNextEventID (),

				CausedById = emp.Id,
				RestaurantId = restaurantID,
			};
		}

		public AccountRegisteredFromMobileDeviceEvent CreateAccountRegisteredFromMobileDeviceEvent (IEmployee emp, 
			Guid accountID, EmailAddress email, PhoneNumber phone, CreditCard card, ReferralCode code, MiseAppTypes app, PersonName name)
		{
			return new AccountRegisteredFromMobileDeviceEvent {
				Id = Guid.NewGuid (),
				CreatedDate = GetDate(),
				DeviceId = _deviceID,
				EventOrder = GetNextEventID (),

				CausedById = emp.Id,
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

